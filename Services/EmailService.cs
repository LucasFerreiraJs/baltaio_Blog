using System.Net;
using System.Net.Mail;

namespace Blog.Services
{
    public class EmailService
    {

        public bool Send(
            string toName,
            string toEmail,
            string subject,
            string body,
            string fromName = "equipe teste",
            string fromEmail = "casferreiraa@gmail.com"

            )
        {

            var smtpClient = new SmtpClient(Configuration.Smtp.Host, Configuration.Smtp.Port);

            smtpClient.Credentials = new NetworkCredential(Configuration.Smtp.UserName, Configuration.Smtp.Password);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.EnableSsl = true;

            var mail = new MailMessage();


            mail.From = new MailAddress(fromEmail, fromName);
            mail.To.Add(new MailAddress(toEmail, toName));
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true;

            try
            {

                //smtpClient.Send(mail); //disparar email
                return true;

            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                return false;
            }

        }
    }
}
