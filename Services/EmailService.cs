using MailKit.Security;
using MimeKit.Text;
using MimeKit;

using MailKit.Net.Smtp;
using MVCEcommerce.Dto_s;



namespace MVCEcommerce.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration config)
        {
            _config = config;
        }


        public async Task SendEmail(EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:UserName").Value));
            email.To.Add(MailboxAddress.Parse(request.Para));
            email.Subject = request.Asunto;
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = request.Contenido
            };

            using var smtp = new SmtpClient();
           
            smtp.ServerCertificateValidationCallback = (s, certificate, chain, sslPolicyErrors) => true; // Solo para pruebas

            await smtp.ConnectAsync(
                _config.GetSection("Email:Host").Value,
               Convert.ToInt32(_config.GetSection("Email:Port").Value),
               SecureSocketOptions.Auto

                );


           await smtp.AuthenticateAsync(_config.GetSection("Email:UserName").Value, _config.GetSection("Email:PassWord").Value);

           await smtp.SendAsync(email);
           await smtp.DisconnectAsync(true);


        }
    }
}
