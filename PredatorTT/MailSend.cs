using System.Net;
using System.Net.Mail;

namespace PredatorTT
{
    class MailSend
    {
		public static void SendMail(MailData data, string attachFile = null)
		{
			try
			{
				MailMessage mail = new MailMessage();
				mail.From = new MailAddress(data.from);
				mail.To.Add(new MailAddress(data.To));
				mail.Subject = data.subject;
				mail.Body = data.body;
				if (!string.IsNullOrEmpty(attachFile))
					mail.Attachments.Add(new Attachment(attachFile));
				SmtpClient client = new SmtpClient();
				client.Host = data.smtp;
				client.Port = 587;
				client.EnableSsl = true;
				client.Credentials = new NetworkCredential(data.from.Split('@')[0], data.pass);
				client.DeliveryMethod = SmtpDeliveryMethod.Network;
				client.Send(mail);
				mail.Dispose();
			}
			catch { }
		}

		public static bool CheckForInternetConnection(string url)
		{
			try
			{
				using (var client = new WebClient())
				{
					using (client.OpenRead(url))
					{
						return true;
					}
				}
			}
			catch
			{
				return false;
			}
		}
	}
}
