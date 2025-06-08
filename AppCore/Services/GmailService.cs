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
    public class GmailService : IEmailService, IDisposable
    {
        private readonly string senderEmail = Settings.EmailAddress;
        private readonly string emailPassword = Settings.EmailPassword;
        private readonly string emailServerHost = Settings.EmailServerHost;
        private readonly ILoggerService logger;
        private readonly SmtpClient emailClient;
        private bool disposed = false;

        public GmailService(ILoggerService logger)
        {
            this.logger = logger;
            emailClient =  new SmtpClient(emailServerHost)
            {
                Port = 587,
                EnableSsl = true,
                Credentials = new NetworkCredential(senderEmail, emailPassword),
            };
        }

        public async Task NotifyNewUserAsync(User user)
        {
            await SendEmailAsync(new Email
            {
                EmailAddress = user.Email,
                Subject = "Nova Usuario Criado [CongEspVilaGuilherme]",
                MultiLineMessage = $"O usuário '{user.UserName}' foi criado com sucesso. agora você acessar o sistema com acesso basico (Apenas Leitura)",
                SendCopyToAdmin = true
            });
        }

        public async Task SendNewPasswordAsync(User user, string plainPassword)
        {
            await SendEmailAsync(new Email
            {
                EmailAddress = user.Email,
                Subject = "Nova Senha Sistema [CongEspVilaGuilherme]",
                MultiLineMessage = $"Usuário: {user.UserName}\nSenha: {plainPassword}"
            });
        }

        public async Task SendResetPassordEmailAsync(User user)
        {
            logger.Log($"[GmailService] Enviando e-mail para: {user.Email} ({user.UserName})");
            await SendEmailAsync(new Email
            {
                EmailAddress = user.Email,
                Subject = "Reset de Senha Solicitado [CongEspVilaGuilherme]",
                HtmlMessage = $"Usuário: {user.UserName}<br>" +
                $"<a href=\"http://{Settings.FrontAppHost}/reset-password/{user.ResetPasswordId}\">Clique aqui</a>"
            });
        }

        internal async Task CheckConnectionAsync()
        {
            try
            {
                await SendEmailToAdminAsync("CongEspVilaGuilhermeApi is Starting...");
                logger.Log("[GmailService.CheckConnection] Gmail Conected Sucessfully!");
            }
            catch (Exception ex)
            {
                logger.Log("[GmailService.CheckConnection] Error: " + ex.Message);
            }
        }

        private async Task SendEmailToAdminAsync(string message)
        {
            await SendEmailAsync(new Email
            {
                EmailAddress = senderEmail,
                Subject = "Admin Event [CongEspVilaGuilherme]",
                HtmlMessage = message
            });
        }

        private async Task SendEmailAsync(Email email)
        {
            var mail = new MailMessage(senderEmail, email.EmailAddress)
            {
                Subject = email.Subject,
                Body = email.Body,
                IsBodyHtml = email.IsBodyHtml
            };

            if (email.SendCopyToAdmin)
            {                
                mail.Bcc.Add(senderEmail);
            }

            try
            {
                await emailClient.SendMailAsync(mail);
            }
            catch (SmtpException ex)
            {
                logger.Log($"[GmailService.SendEmailAsync] Error sending email to {email.EmailAddress}: {ex.Message}");
                logger.Log(ex.ToString());
                throw;
            }
                
        }

        protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                emailClient?.Dispose();
            }
            
            disposed = true;
        }
    }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
