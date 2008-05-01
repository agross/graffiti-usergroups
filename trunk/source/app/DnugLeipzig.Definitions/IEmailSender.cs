using System.Net.Mail;

using Graffiti.Core;

namespace DnugLeipzig.Definitions
{
	public interface IEmailSender
	{
		void Send(EmailTemplate template);
		void SendMailMessage(MailMessage mailMessage);
	}
}