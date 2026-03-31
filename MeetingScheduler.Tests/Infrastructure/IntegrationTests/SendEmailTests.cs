using MailKit.Security;
using MeetingScheduler.Domain.Common.Models;
using MimeKit;
using NUnit.Framework;

namespace MeetingScheduler.Tests.Infrastructure.IntegrationTests;

[TestFixture]
public class SendEmailTests
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public async Task TestSendTestMail()
	{
		// Arrange

		// Act
		await SendTestMail();

		// Assert
	}

	public async Task SendTestMail()
	{
		var mail = new MimeMessage
		{
			Sender = MailboxAddress.Parse("no_reply@darah.org.sa")
		};

		mail.From.Add(MailboxAddress.Parse("no_reply@darah.org.sa"));

		mail.To.Add(MailboxAddress.Parse("merna@darah.org.sa"));

		mail.Subject = "Test Darah Mail Subject Async";

		var builder = new BodyBuilder();

		MailRequest mailRequest = new MailRequest();

		builder.HtmlBody = mailRequest.HtmlBody;

		mail.Body = builder.ToMessageBody();

		mail.Priority = MessagePriority.Urgent;

		using var client = new MailKit.Net.Smtp.SmtpClient();

		client.Connect("esmtp-ss.deem.sa", 587, SecureSocketOptions.Auto);

		await client.AuthenticateAsync("no_reply@darah.org.sa", "d_N@511#").ConfigureAwait(false);

		await client.SendAsync(mail);

		client.Disconnect(true);
	}
}
