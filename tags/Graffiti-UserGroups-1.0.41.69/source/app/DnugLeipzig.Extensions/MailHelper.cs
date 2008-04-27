using System;
using System.Net;
using System.Net.Mail;
using System.Web;

using Graffiti.Core;

namespace DnugLeipzig.Extensions
{
	internal class MailHelper
	{
		public static void Send(EmailTemplate template)
		{
			OnBeforeEmailSent(template);

			string templatePath =
				VirtualPathUtility.ToAbsolute(String.Format("~/files/themes/{0}/templates/{1}",
															GraffitiContext.Current.Theme,
															template.TemplateName));

			templatePath = HttpContext.Current.Server.MapPath(templatePath);

			string body = TemplateEngine.Evaluate(Graffiti.Core.Util.GetFileText(templatePath), template.Context);
			using (MailMessage message = new MailMessage(template.From ?? SiteSettings.Get().EmailFrom, template.To))
			{
				message.Subject = template.Subject;
				message.IsBodyHtml = template.IsHTML;
				message.Body = body;
				SendMailMessage(message);
			}

			OnAfterEmailSent(template);
		}

		public static void SendMailMessage(MailMessage mailMessage)
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

				if (settings.EmailRequiresSSL)
				{
					client.EnableSsl = true;
				}

				if (settings.EmailPort > 0)
				{
					client.Port = settings.EmailPort;
				}

				client.Send(mailMessage);
			}
		}

		protected static void OnBeforeEmailSent(EmailTemplate template)
		{
			// TODO: This does not work as expected.
			//EmailTemplateHandler temp = Events.Instance().BeforeEmailSent;
			//if (temp != null)
			//{
			//    temp.Invoke(template, EventArgs.Empty);
			//}
		}

		protected static void OnAfterEmailSent(EmailTemplate template)
		{
			// TODO: This does not work as expected.
			//EmailTemplateHandler temp = Events.Instance().AfterEmailSent;
			//if (temp != null)
			//{
			//    temp.Invoke(template, EventArgs.Empty);
			//}
		}
	}
}