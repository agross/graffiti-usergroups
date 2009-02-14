using System;
using System.Net;
using System.Net.Mail;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.GraffitiIntegration;
using DnugLeipzig.Definitions.Services;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Services
{
	public class GraffitiEmailSender : Service, IEmailSender
	{
		readonly IGraffitiSiteSettings _settings;

		public GraffitiEmailSender(IGraffitiSiteSettings settings)
		{
			_settings = settings;
		}

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
				using (MailMessage message = new MailMessage(template.From ?? _settings.EmailFrom, template.To))
				{
					message.Subject = template.Subject;
					message.IsBodyHtml = template.IsHTML;
					message.Body = body;
					SendMailMessage(message);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(Create.New.LogMessage().WithTitle("Could not send e-mail"), ex);
				throw;
			}
		}
		#endregion

		public void SendMailMessage(MailMessage message)
		{
			SmtpClient client = new SmtpClient
			                    {
			                    	Host = _settings.EmailServer,
			                    	EnableSsl = _settings.EmailRequiresSsl
			                    };

			if (_settings.EmailServerRequiresAuthentication)
			{
				client.Credentials = new NetworkCredential(_settings.EmailUser, _settings.EmailPassword);
			}

			if (_settings.EmailPort > 0)
			{
				client.Port = _settings.EmailPort;
			}

			client.Send(message);
		}
	}
}