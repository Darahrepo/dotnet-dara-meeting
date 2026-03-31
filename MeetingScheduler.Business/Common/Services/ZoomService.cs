using AutoMapper.Configuration;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using MeetingScheduler.Domain.Enums;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Extensions.Logging;
using System.Data.SqlTypes;
using Ical.Net.DataTypes;
using Org.BouncyCastle.Asn1;
using Ical.Net.CalendarComponents;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class ZoomService : IZoomService
    {
        private ZoomToken _token;
        private ZoomAccessDetails _account;
        private readonly ZoomSettings _zoomSettings;
        private readonly ILogger<ZoomService> _logger;

        public ZoomService(IOptions<ZoomSettings> zoomSettings, ILogger<ZoomService> logger)
        {
            _zoomSettings = zoomSettings.Value;
            _logger = logger;
            //_tokenString =  SetTokenString();
        }

        private async Task SetTokenString(ZoomUserType zoomAccount = ZoomUserType.Main)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            try
            {
                if (zoomAccount == ZoomUserType.Ceo)
                {
                    _account = _zoomSettings.Ceo;
                }
                else
                {
                    _account = _zoomSettings.Main;
                }
                var accountId = _account.AccountId;
                var clientID = _account.Client;
                var clientSecret = _account.ClientSecret;
                RestClient restcli = new RestClient($"https://zoom.us/oauth/token?grant_type=account_credentials&account_id={accountId}");
                var request = new RestRequest("", Method.Post);
                request.RequestFormat = DataFormat.Json;

                //request.Timeout = -1;
                string clientID_clientSecret = clientID + ":" + clientSecret;
                var plaintextbytes = System.Text.Encoding.UTF8.GetBytes(clientID_clientSecret);
                string base64Value = System.Convert.ToBase64String(plaintextbytes);
                request.AddHeader("Authorization", "Basic" + base64Value);
                var response = await restcli.ExecuteAsync(request);

                _token = JsonConvert.DeserializeObject<ZoomToken>(response.Content);
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }

        public async Task<MeetingDetails> CreateMeeting(MeetingDetails details)
        {
            await SetTokenString(details.ZoomAccount);
            var client = new RestClient($"https://api.zoom.us/v2/users/{_account.AccountUser}/meetings");
            var request = new RestRequest{ Method = Method.Post };
            request.RequestFormat = DataFormat.Json;
            string password = GeneratePassword(10);
            string auto_recording = details.RecordedMeeting == true ? "cloud" : "none" ;
            
            request.AddJsonBody(new
            {
                topic = details.Topic,
                duration = details.DurationInMinutes,
                start_time = details.StartDateTime,
                type = ZoomMeetingType.ScheduledMeeting,
                password = $"{password}",
                settings = new
                {
                    auto_recording = auto_recording,
                    join_before_host = "true",
                    waiting_room = "false"
                }
            });
           
            request.AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));


            var restResponse = await client.ExecuteAsync(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;
            var response = JObject.Parse(restResponse.Content);
            details.ResponseStatusCode = numericStatusCode;

            if(numericStatusCode == Convert.ToInt32(ZoomMeetingStatus.success))
            {
                details.Password = (string)response["password"];
                details.JoiningUrl = (string)response["join_url"];
                details.MeetingId = (string)response["id"];
                details.Timezone = (string)response["timezone"];
            }

            return details;

        }
        public async Task<List<ZoomMeetingDetails>> GetUpcomingMeetings(DateTime from , ZoomUserType zoomAccount = ZoomUserType.Main)
        {
            await SetTokenString(zoomAccount);
            if(_token.access_token == null)
            {
                return null;
            }
            List<ZoomMeetingDetails> upcomingMeetings = new List<ZoomMeetingDetails>();
            try
            {
                var client = new RestClient($"https://api.zoom.us/v2/users/{_account.AccountUser}/meetings/?type=upcoming");
                var request = new RestRequest{ Method = Method.Get };
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));
                var restResponse = await client.ExecuteAsync(request);
                if (restResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("Error Authorizing Zoom Account" + _account.AccountUser);
                    return null;
                }
                var response = JObject.Parse(restResponse.Content);
                var resultDetails = response["meetings"];// = objMeetings.ToObject<List<ZoomMeetingDetails>>();
                if (resultDetails != null)
                {
                    foreach (var obj in resultDetails)
                    {
                        DateTime startTime = DateTime.ParseExact((string)obj["start_time"], "MM/dd/yyyy HH:mm:ss", CultureInfo.CurrentCulture).ToLocalTime();
                        if (startTime.Date >= from.Date)
                        {
                            ZoomMeetingDetails detail = new ZoomMeetingDetails();
                            detail.start_time = startTime;
                            detail.duration = (int)obj["duration"];
                            detail.topic = (string)obj["topic"];
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
        public async Task<List<ZoomMeetingDetails>> GetUpcomingWebinars(DateTime from,  ZoomUserType zoomAccount = ZoomUserType.Main)
        {
            await SetTokenString(zoomAccount);
            List<ZoomMeetingDetails> upcomingWebinars = new List<ZoomMeetingDetails>();
            try
            {
                var client = new RestClient($"https://api.zoom.us/v2/users/{_account.AccountUser}/webinars/?type=upcoming");
                var request = new RestRequest{ Method = Method.Get };
                request.RequestFormat = DataFormat.Json;
                request.AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));
                var restResponse = await client.ExecuteAsync(request);
                HttpStatusCode statusCode = restResponse.StatusCode;
                int numericStatusCode = (int)statusCode;
                if (restResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    _logger.LogError("Error Authorizing Zoom Account" + _account.AccountUser);
                    return null;
                }
                var response = JObject.Parse(restResponse.Content);
                var objWebinars = response["webinars"];
                var resultDetails = response["webinars"];// = objMeetings.ToObject<List<ZoomMeetingDetails>>();
                if (resultDetails != null)
                {
                    foreach (var obj in resultDetails)
                    {
                        DateTime startTime = DateTime.ParseExact((string)obj["start_time"], "MM/dd/yyyy HH:mm:ss" ,CultureInfo.CurrentCulture).ToLocalTime();
                        if (startTime.Date >= from.Date)
                        {
                            ZoomMeetingDetails detail = new ZoomMeetingDetails();
                            detail.start_time = startTime;
                            detail.duration = (int)obj["duration"];
                            detail.topic = (string)obj["topic"];
                            upcomingWebinars.Add(detail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return upcomingWebinars;
        }
        public string GeneratePassword(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            return s;
        }
        public async Task<int> CancelMeeting(string meetingId, ZoomUserType zoomAccount = ZoomUserType.Main)
        {
            await SetTokenString(zoomAccount);
            var client = new RestClient($"https://api.zoom.us/v2/meetings/{meetingId}");
            var request = new RestRequest{ Method = Method.Delete };
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));


            var restResponse = await client.ExecuteAsync(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;
            return numericStatusCode;
        }

       
        public async Task<ZoomWebinarDetails> CreateWebinar(ZoomWebinarDetails details)
        {
            await SetTokenString(details.ZoomAccount);
            var client = new RestClient($"https://api.zoom.us/v2/users/{_account.AccountUser}/webinars");
            var request = new RestRequest{ Method = Method.Delete };
            request.RequestFormat = DataFormat.Json;
            string password = GeneratePassword(10);

            string auto_recording = details.recorded_webinar == true ? "cloud" : "none";
            int approval_type = details.registration_required == true ? (int)WebinarApprovalType.AutomaticApproval : (int)WebinarApprovalType.NoRegistrationRequired;



            details.type = ZoomMeetingType.ScheduledMeeting;
            details.password = $"{password}";
            details.Settings.auto_recording = auto_recording;
            details.Settings.approval_type = approval_type;

            
            var detailsJSON = System.Text.Json.JsonSerializer.Serialize(details);
            request.AddJsonBody(detailsJSON);
            request.AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));


            var restResponse = await client.ExecuteAsync(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;
            var response = JObject.Parse(restResponse.Content);
            details.response_status_code = numericStatusCode;

            if (numericStatusCode == Convert.ToInt32(ZoomMeetingStatus.success))
            {
                details.starting_url = (string)response["start_url"];
                details.webinarId = (string)response["id"];
                details.password = (string)response["password"];
                details.registration_url = details.registration_required == true?(string)response["registration_url"] :null;
            }

            return details;

        }

        public async Task<int> CancelWebinar(string meetingId, ZoomUserType zoomAccount = ZoomUserType.Main)
        {
            await SetTokenString(zoomAccount);
            var client = new RestClient($"https://api.zoom.us/v2/webinars/{meetingId}");
            var request = new RestRequest{ Method = Method.Delete };
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("authorization", String.Format("Bearer {0}", _token.access_token));


            var restResponse = await client.ExecuteAsync(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;

            return numericStatusCode;

        }
        //public async Task SyncZoomWithDB(List<ZoomMeetingDetails> details)
        //{
        //    try
        //    {
        //        var client = new RestClient($"https://api.zoom.us/v2/users/{_zoomSettings.AccountUser}/meetings");
        //        var request = new RestRequest(Method.GET);
        //        request.RequestFormat = DataFormat.Json;


        //        request.AddHeader("authorization", String.Format("Bearer {0}", _tokenString));
        //        IRestResponse restResponse = await client.ExecuteAsync(request);
        //        HttpStatusCode statusCode = restResponse.StatusCode;
        //        int numericStatusCode = (int)statusCode;
        //        var response = JObject.Parse(restResponse.Content);
        //        List<ZoomMeetingDetails> resultDetails = JsonConvert.DeserializeObject< List<ZoomMeetingDetails>>((string)response);

        //        List< ZoomMeetingDetails> upcomingMeetings = resultDetails.FindAll(x=>x.start_time > DateTime.UtcNow);



        //        foreach (var det in details)
        //        {
        //            det.ResponseStatusCode = numericStatusCode;
        //            if (numericStatusCode == Convert.ToInt32(ZoomMeetingStatus.success))
        //            {
        //                det.JoiningUrl = (string)response["join_url"];
        //                det.MeetingId = (string)response["id"];
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}

        //public async Task<ZoomWebinarDetails> SendWebinarRegistrationEmails(string webinarId)
        //{
        //    var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        //    var now = DateTime.UtcNow;
        //    var apiSecret = _zoomSettings.APISecret;
        //    byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);

        //    var tokenDescriptor = new SecurityTokenDescriptor
        //    {
        //        Issuer = _zoomSettings.Issuer,
        //        Expires = now.AddMinutes(_zoomSettings.AccessTokenLifeTimeMinutes),
        //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
        //    };

        //    var token = tokenHandler.CreateToken(tokenDescriptor);
        //    var tokenString = tokenHandler.WriteToken(token);
        //    var client = new RestClient($"https://api.zoom.us/v2/users/{_zoomSettings.AccountUser}/meetings");
        //    var request = new RestRequest(Method.POST);
        //    request.RequestFormat = DataFormat.Json;
        //    string password = GeneratePassword(10);

        //    string auto_recording = details.RecordedWebinar == true ? "cloud" : "none";
        //    int approval_type = details.RegistrationRequired == true ? (int)WebinarApprovalType.AutomaticApproval : (int)WebinarApprovalType.NoRegistrationRequired;

        //    request.AddJsonBody(new
        //    {
        //        topic = details.Topic,
        //        duration = details.DurationInMinutes,
        //        start_time = details.StartDateTime,
        //        type = ZoomMeetingType.ScheduledMeeting,
        //        password = $"{password}",
        //        agenda = details.Agenda,
        //        settings = new
        //        {
        //            auto_recording = auto_recording,
        //            join_before_host = "true",
        //            waiting_room = "false",
        //            approval_type = approval_type,
        //        }
        //    });

        //    request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));


        //    IRestResponse restResponse = await client.ExecuteAsync(request);
        //    HttpStatusCode statusCode = restResponse.StatusCode;
        //    int numericStatusCode = (int)statusCode;
        //    var response = JObject.Parse(restResponse.Content);
        //    details.ResponseStatusCode = numericStatusCode;

        //    if (numericStatusCode == Convert.ToInt32(ZoomMeetingStatus.success))
        //    {
        //        details.JoiningUrl = (string)response["join_url"];
        //        details.WebinarId = (string)response["id"];
        //    }

        //    return details;

        //}

    }
}
