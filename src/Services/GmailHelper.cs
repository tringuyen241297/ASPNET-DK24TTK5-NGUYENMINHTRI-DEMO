using MailKit.Net.Smtp;
using MimeKit;
namespace CakeShop.Services
{
	public static class GmailService
	{
		public static void sendGmail(string fromEmail, string fromName, string toEmail, string subject, string htmlBody)
		{
			using (var client = new SmtpClient())
			{
				client.Connect("Smtp.gmail.com");
				client.Authenticate("thuylieupham256@gmail.com", "vlehbtkhlyausfei");

				var message = new MimeMessage();
				message.From.Add(new MailboxAddress(fromName, fromEmail));
				message.To.Add(new MailboxAddress("", toEmail));
				message.Subject = subject;

				var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
				message.Body = bodyBuilder.ToMessageBody();

				client.Send(message);
				client.Disconnect(true);
			}
		}
	}
}
