using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Enums;
using MeetingScheduler.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Bcpg;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common.Services
{
	public class WebexService : IWebexService
	{
		private WebexToken _token;
		private readonly IMemoryCache _memoryCache;
		private readonly IUserProvider _userProvider;
		private readonly WebexSettings _webexSettings;
		private WebexAccessDetails _account;
		private DateTime _dateTime;
		private readonly ILogger<WebexService> _logger;
		public WebexService(IMemoryCache memoryCache, IOptions<WebexSettings> webexSettings, ILogger<WebexService> logger, IUserProvider userProvider)
		{
			_webexSettings = webexSettings.Value;
			_account = _webexSettings.Ceo;
			_userProvider = userProvider;
			_logger = logger;
			_memoryCache = memoryCache;
			_dateTime = DateTime.UtcNow;

		}

		/// <summary>
		/// Builds the Webex authorization URL for the specified account (Main/Ceo).
		/// Use this URL to redirect the user to Webex to obtain the authorization code.
		/// </summary>
		public string GetAuthorizationUrl(WebexUserType webexAccount, string state = null, string scope = "spark:all meeting:schedules_write meeting:schedules_read")
		{
			if (webexAccount == WebexUserType.Main)
				_account = _webexSettings.Main;
			else
				_account = _webexSettings.Ceo;

			var clientId = _account.ClientId;
			var redirectUri = _account.RedirectUri ?? string.Empty;

			// Webex authorization endpoint uses response_type=code
			var uri = $"{_webexSettings.ApiUrl.TrimEnd('/')}/authorize" +
					  $"?response_type=code" +
					  $"&client_id={Uri.EscapeDataString(clientId)}" +
					  $"&redirect_uri={Uri.EscapeDataString(redirectUri)}" +
					  $"&scope={Uri.EscapeDataString(scope)}";

			if (!string.IsNullOrWhiteSpace(state))
			{
				uri += $"&state={Uri.EscapeDataString(state)}";
			}

			return uri;
		}

		private async Task RefreshToken(WebexUserType webexAccount)
		{
			var now = DateTime.UtcNow;
			try
			{
				var userId = _userProvider.CurrentUser.UserId;

				// include provider name and account type in cache key to avoid collisions (supports Zoom later)
				string cacheKey = $"webex_{webexAccount}_{userId}";

				// Try to get the token from memory cache
				if (!_memoryCache.TryGetValue(cacheKey, out WebexToken cachedToken))
				{
					if (webexAccount == WebexUserType.Main)
						_account = _webexSettings.Main;
					else if (webexAccount == WebexUserType.Ceo)
						_account = _webexSettings.Ceo;

					// If there's no refresh token configured on account, we cannot refresh
					if (string.IsNullOrWhiteSpace(_account.RefreshToken))
					{
						_logger.LogWarning("No refresh token available for Webex account {AccountUser}", _account?.AccountUser);
						return;
					}

					// Prepare form data for refresh_token grant
					var requestBody = new Dictionary<string, string>
					{
						{ "grant_type", _webexSettings.GrantTypes?.Refresh?.ToString() ?? "refresh_token" },
						{ "client_id", _account.ClientId },
						{ "client_secret", _account.ClientSecret },
						{ "refresh_token", _account.RefreshToken }
					};

					RestClient restcli = new RestClient($"{_webexSettings.ApiUrl.TrimEnd('/')}/access_token");

					var request = new RestRequest { Method = Method.Post };

					// Add as form URL encoded
					foreach (var kv in requestBody)
					{
						request.AddParameter(kv.Key, kv.Value);
					}
					request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

					var response = await restcli.ExecuteAsync(request).ConfigureAwait(false);

					if (response.StatusCode != HttpStatusCode.OK)
					{
						_logger.LogError("Failed to refresh Webex token for account {AccountUser}. Status: {Status}, Content: {Content}",
							_account?.AccountUser, response.StatusCode, response.Content);
						return;
					}

					var token = JsonSerializer.Deserialize<WebexToken>(response.Content);
					if (token == null)
					{
						_logger.LogError("Failed to deserialize Webex token response for account {AccountUser}. Content: {Content}",
							_account?.AccountUser, response.Content);
						return;
					}

					_token = token;

					// Update configured refresh token so subsequent refreshes use the latest refresh_token (persisting it to config store is recommended)
					_account.RefreshToken = token.refresh_token;

					// Cache the token; use refresh_token_expires_in if available, otherwise expires_in
					var expirySeconds = token.refresh_token_expires_in > 0 ? token.refresh_token_expires_in : token.expires_in;
					if (expirySeconds <= 0) expirySeconds = 3600;

					_memoryCache.Set(cacheKey, token, new MemoryCacheEntryOptions
					{
						AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expirySeconds)
					});
				}
				else
				{
					_token = cachedToken;
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while refreshing Webex token.");
				throw;
			}
		}

		/// <summary>
		/// Exchanges an authorization code (obtained via the authorization URL) for access and refresh tokens.
		/// Stores the token in memory cache and updates the account's RefreshToken property.
		/// Returns true if exchange succeeded.
		/// </summary>
		public async Task<bool> ExchangeAuthorizationCodeAsync(string code, WebexUserType webexAccount)
		{
			if (string.IsNullOrWhiteSpace(code))
			{
				_logger.LogWarning("ExchangeAuthorizationCodeAsync called with empty code.");
				return false;
			}

			try
			{
				if (webexAccount == WebexUserType.Main)
					_account = _webexSettings.Main;
				else
					_account = _webexSettings.Ceo;

				var requestBody = new Dictionary<string, string>
				{
					{ "grant_type", "authorization_code" },
					{ "client_id", _account.ClientId },
					{ "client_secret", _account.ClientSecret },
					{ "code", code },
					{ "redirect_uri", _account.RedirectUri ?? string.Empty }
				};

				RestClient restcli = new RestClient($"{_webexSettings.ApiUrl.TrimEnd('/')}/access_token");
				var request = new RestRequest { Method = Method.Post };

				foreach (var kv in requestBody)
				{
					request.AddParameter(kv.Key, kv.Value);
				}
				request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

				var response = await restcli.ExecuteAsync(request).ConfigureAwait(false);

				if (response.StatusCode != HttpStatusCode.OK)
				{
					_logger.LogError("Failed to exchange Webex authorization code for account {AccountUser}. Status: {Status}, Content: {Content}",
						_account?.AccountUser, response.StatusCode, response.Content);
					return false;
				}

				var token = JsonSerializer.Deserialize<WebexToken>(response.Content);
				if (token == null)
				{
					_logger.LogError("Failed to deserialize Webex token response for account {AccountUser}. Content: {Content}",
						_account?.AccountUser, response.Content);
					return false;
				}

				_token = token;

				// Persist the refresh token into the in-memory account config so subsequent refresh calls can use it.
				_account.RefreshToken = token.refresh_token;

				// Cache token per user and account type. Include provider name to be explicit for future Zoom support.
				var userId = _userProvider.CurrentUser.UserId;
				string cacheKey = $"webex_{webexAccount}_{userId}";

				var expirySeconds = token.refresh_token_expires_in > 0 ? token.refresh_token_expires_in : token.expires_in;
				if (expirySeconds <= 0) expirySeconds = 3600;

				_memoryCache.Set(cacheKey, token, new MemoryCacheEntryOptions
				{
					AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(expirySeconds)
				});

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error exchanging Webex authorization code.");
				return false;
			}
		}


		public async Task<MeetingDetails> CreateMeeting(MeetingDetails details)
		{
			await RefreshToken((WebexUserType)details.ZoomAccount);
			if (_token.access_token == null)
			{
				return null;
			}
			var client = new RestClient($"{_webexSettings.ApiUrl}/meetings/");
			var request = new RestRequest { Method = Method.Post, RequestFormat = DataFormat.Json };

			string password = GeneratePassword(10);

			request
				.AddJsonBody(new
				{
					title = details.Topic,
					agenda = details.Agenda,
					start = details.StartDateTime.ToUniversalTime(),
					end = details.EndDateTime.ToUniversalTime(),
					enabledJoinBeforeHost = true,
					joinBeforeHostMinutes = 15,
					scheduledType = details.ScheduledType,
					enabledAutoRecordMeeting = details.RecordedMeeting,
					password = $"{password}",
				}).AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));

			var restResponse = await client.ExecuteAsync(request);

			HttpStatusCode statusCode = restResponse.StatusCode;
			int numericStatusCode = (int)statusCode;
			var response = JObject.Parse(restResponse.Content);
			details.ResponseStatusCode = numericStatusCode;

			if (numericStatusCode == Convert.ToInt32(WebexMeetingStatus.OK))
			{
				details.Password = (string)response["password"];
				details.JoiningUrl = (string)response["webLink"];
				details.MeetingId = (string)response["id"];
				details.Timezone = (string)response["timezone"];
				details.WebexMeetingNumber = (string)response["meetingNumber"];
			}

			return details;

		}
		public async Task<List<MeetingDetails>> GetUpcomingMeetings(DateTime from, WebexUserType webexAccount)
		{
			await RefreshToken(webexAccount);
			if (_token?.access_token == null)
			{
				return null;
			}
			List<MeetingDetails> upcomingMeetings = new List<MeetingDetails>();
			try
			{
				var client = new RestClient($"{_webexSettings.ApiUrl}/meetings");

				var request = new RestRequest
				{
					RequestFormat = DataFormat.Json,
					Method = Method.Get
				}.AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));

				var restResponse = await client.ExecuteAsync(request);

				if (restResponse.StatusCode == HttpStatusCode.Unauthorized)
				{
					_logger.LogError("Error Authorizing Webex Account" + _account.AccountUser);
					return null;
				}
				var response = JObject.Parse(restResponse.Content);

				var resultDetails = response["items"];

				if (resultDetails != null)
				{
					foreach (var obj in resultDetails)
					{
						DateTime startTime = (DateTime)obj["start"];// DateTime.ParseExact((string)obj["start"], "MM/dd/yyyy HH:mm:ss", CultureInfo.CurrentCulture).ToLocalTime();
						if (startTime.Date.ToLocalTime() >= from.Date.ToLocalTime())
						{
							MeetingDetails detail = new MeetingDetails();
							detail.StartDateTime = startTime.ToLocalTime();
							detail.EndDateTime = ((DateTime)obj["end"]).ToLocalTime();
							detail.Topic = (string)obj["title"];
							upcomingMeetings.Add(detail);
						}
					}
				}

			}
			catch (Exception ex)
			{
				throw;
			}
			return upcomingMeetings;
		}

		public string GeneratePassword(int length)
		{
			var random = new Random();
			string s = string.Empty;
			for (int i = 0; i < length; i++)
				s = String.Concat(s, random.Next(10).ToString());
			return s;
		}


		public async Task<int> CancelMeeting(string webexMeetingId, WebexUserType webexAccount = WebexUserType.Main)
		{
			await RefreshToken(webexAccount);

			if (_token.access_token == null) return -1;

			var client = new RestClient($"{_webexSettings.ApiUrl}/meetings/{webexMeetingId}");

			var request = new RestRequest { Method = Method.Delete, RequestFormat = DataFormat.Json }.AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));


			var restResponse = await client.ExecuteAsync(request);

			return (int)restResponse.StatusCode;
		}



	}
}