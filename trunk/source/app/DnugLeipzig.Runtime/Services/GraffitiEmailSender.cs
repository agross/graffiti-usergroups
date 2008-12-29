using System;
using System.Net;
using System.Net.Mail;
using System.Web;

using DnugLeipzig.Definitions.Services;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Services
{
	public class GraffitiEmailSender : Service, IEmailSender
	{
		#region IEmailSender Members
		public void Send(EmailTemplate template)
		{
			try
			{
				string templatePath =
					VirtualPathUtility.ToAbsolute(String.Format("~/files/themes/{0}/templates/{1}",
					                                            GraffitiContext.Current.Theme,
					                                            template.TemplateName));

				templatePath = HttpContext.Current.Server.MapPath(templatePath);

				string body = TemplateEngine.Evaluate(Util.GetFileText(templatePath), template.Context);
				using (MailMessage message = new MailMessage(template.From ?? SiteSettings.Get().EmailFrom, template.To))
				{
					message.Subject = template.Subject;
					message.IsBodyHtml = template.IsHTML;
					message.Body = body;
					SendMailMessage(message);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Could not send registration message", ex.ToString());
				throw;
			}
		}

		public void SendMailMessage(MailMessage mailMessage)
		{
			try
			{
				using (mailMessage)
				{
					SiteSettings settings = SiteSettings.Get();
					SmtpClient client = new SmtpClient
					                    {
					                    	Host = settings.EmailServer
					                    };

					if (settings.EmailServerRequiresAuthentication)
					{
						client.Credentials = new NetworkCredential(settings.EmailUser, settings.EmailPassword);
					}

					client.EnableSsl = settings.EmailRequiresSSL;

					if (settings.EmailPort > 0)
					{
						client.Port = settings.EmailPort;
					}

					client.Send(mailMessage);
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Could not send registration message", ex.ToString());
				throw;
			}
		}
		#endregion
	}
}