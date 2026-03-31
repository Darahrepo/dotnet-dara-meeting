using MailKit.Net.Smtp;
using MailKit.Security;
using MeetingScheduler.Domain.Common.Interfaces;
using MeetingScheduler.Domain.Common.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace MeetingScheduler.Infrastructure.Common.Services
{
    public class EmailServices : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileServices _fileService;
        private readonly ILogger<EmailServices> _logger;
        public EmailServices(IOptions<MailSettings> mailSettings, 
            IFileServices fileService,
            ILogger<EmailServices> logger, 
            IWebHostEnvironment webHostEnvironment)
        {
            _mailSettings = mailSettings.Value;
            _fileService = fileService;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        public async Task SendEmail(MailRequest mailRequest)
        {
            try
            {
                var email = new MimeMessage
                {

                    Sender = MailboxAddress.Parse(_mailSettings.Mail)
                };

                email.From.Add(MailboxAddress.Parse(_mailSettings.Mail.ToString()));

                foreach (var to in mailRequest.ToEmailAddresses)
                {
                    if (!string.IsNullOrWhiteSpace(to))
                    {
                        email.To.Add(MailboxAddress.Parse(to));
                    }
                }

                if (mailRequest.CcEmailAddresses != null)
                {
                    foreach (var cc in mailRequest.CcEmailAddresses)
                    {
                        if (!string.IsNullOrWhiteSpace(cc))
                        {
                            email.Cc.Add(MailboxAddress.Parse(cc));
                        }
                    }
                }

                if (mailRequest.BccEmailAddresses != null)
                {
                    foreach (var bcc in mailRequest.BccEmailAddresses)
                    {
                        if (!string.IsNullOrWhiteSpace(bcc))
                        {
                            email.Bcc.Add(MailboxAddress.Parse(bcc));
                        }
                    }
                }

                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();


                mailRequest.LinkedResources = await GetLinkedResources();


                if (mailRequest.LinkedResources != null && mailRequest.LinkedResources.Any())
                {
                    var emailBodyBuilder = new StringBuilder(mailRequest.HtmlBody);
                    foreach (var linkedResource in mailRequest.LinkedResources)
                    {
                        var image = builder.LinkedResources.Add(linkedResource.FileName, linkedResource.Stream);
                        image.ContentId = MimeUtils.GenerateMessageId();
                        emailBodyBuilder = emailBodyBuilder.Replace($"[[{linkedResource.FileName}]]", image.ContentId);
                    }
                    mailRequest.HtmlBody = emailBodyBuilder.ToString();
                }


                if (mailRequest.Attachments != null)
                {
                    foreach (var attachment in mailRequest.Attachments)
                    {
                        builder.Attachments.Add(attachment.FileName, attachment.Stream);
                    }
                    //if (mailRequest.Calendar != null) // CalendarInvitation service
                    //{
                    //    byte[] byteArray = Encoding.ASCII.GetBytes(mailRequest.Calendar);
                    //    MemoryStream stream = new MemoryStream(byteArray);
                    //    builder.Attachments.Add( "outlook invitation.ics" , stream);
                    //}
                } 

                builder.HtmlBody = mailRequest.HtmlBody;
                email.Body = builder.ToMessageBody();
                //mailRequest.MessagePriority = MessagePriority.Urgent
                //_ = Enum.TryParse(mailRequest.MessagePriority, out MessagePriority messagePriority);
                email.Priority = MessagePriority.Urgent;

                using var client = new MailKit.Net.Smtp.SmtpClient();

                //if (_webHostEnvironment.IsDevelopment())
                //{
                //    //client.CheckCertificateRevocation = false;
                //    client.Connect(_mailSettings.Host, _mailSettings.Port,SecureSocketOptions.Auto);
                //}
                //else
                //{
                    client.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.Auto);
                //}


                await client.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password).ConfigureAwait(false);
                
                await client.SendAsync(email);
                client.Disconnect(true);
            }
            catch (Exception Ex)
            {
                _logger.LogError(Ex.InnerException.Message);
            }
        }

        public async Task<List<MailAttachment>> GetLinkedResources()
        {
            List<MailAttachment> mailAttachments = new List<MailAttachment>();

            var darahLogoPath = await _fileService.GetFileUrl("dist/img/darahLogo-white.png");
            FileStream darahLogo = new FileStream(darahLogoPath, FileMode.Open, FileAccess.Read);
            MailAttachment darahLogoAttachment = new MailAttachment(darahLogo, "darahLogo-white.png");
            mailAttachments.Add(darahLogoAttachment);
            return mailAttachments;
        }
    }
}