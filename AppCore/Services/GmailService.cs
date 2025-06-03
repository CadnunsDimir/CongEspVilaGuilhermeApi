using CongEspVilaGuilhermeApi.Core.Models;
using CongEspVilaGuilhermeApi.Domain.Entities;
using CongEspVilaGuilhermeApi.Domain.Services;
using System.Net;
using System.Net.Mail;

namespace CongEspVilaGuilhermeApi.AppCore.Services
{
    /// <summary>
    /// if you use gmail, create a email only for this app and create a app password in this link:
    /// https://myaccount.google.com/apppasswords
    /// </summary>
    public class GmailService : IEmailService
    {
        private readonly string senderEmail = Settings.EmailAddress;
        private readonly string emailPassword = Settings.EmailPassword;
        private readonly string emailServerHost = Settings.EmailServerHost;

        public void NotifyNewUser(User user)
        {
            SendEmail(new Email
            {
                EmailAddress = user.Email,
                Subject = "Nova Usuario Criado [CongEspVilaGuilherme]",
                MultiLineMessage = $"O usuário '{user.UserName}' foi criado com sucesso. agora você acessar o sistema com acesso basico (Apenas Leitura)"
            });
        }

        public void SendNewPassword(User user, string plainPassword)
        {
            SendEmail(new Email
            {
                EmailAddress = user.Email,
                Subject = "Nova Senha Sistema [CongEspVilaGuilherme]",
                MultiLineMessage = $"Usuário: {user.UserName}\nSenha: {plainPassword}"
            });
        }

        public void SendResetPassordEmail(User user)
        {
            SendEmail(new Email
            {
                EmailAddress = user.Email,
                Subject = "Reset de Senha Solicitado [CongEspVilaGuilherme]",
                HtmlMessage = $"Usuário: {user.UserName}<br>" +
                $"<a href=\"http://{Settings.FrontAppHost}/#/reset-password/{user.ResetPasswordId}\">Clique aqui</a>"
            });
        }

        private void SendEmail(Email email)
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(senderEmail);
            mail.To.Add(email.EmailAddress);
            mail.Bcc.Add(senderEmail);

            mail.Subject = email.Subject;
            mail.Body = email.Body;
            mail.IsBodyHtml = email.IsBodyHtml;

            var client = new SmtpClient(emailServerHost);
            client.EnableSsl = true;
            NetworkCredential cred = new NetworkCredential(senderEmail, emailPassword);
            client.Credentials = cred;
            client.Send(mail);
        }
    }
}
