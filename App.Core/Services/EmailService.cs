using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace App.Core.Services
{
    public interface IEmailService
    {
        Task<bool> SendOtpEmail(string toEmail, string otp);
        Task<bool> SendNewPassword(string toEmail, string otp);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        #region Send OTP
        public async Task<bool> SendOtpEmail(string toEmail, string otp)
        {
            try
            {
                var fromEmail = configuration.GetValue<string>("EMAIL_CONFIGURATION:EMAIL");
                var fromPassword = configuration.GetValue<string>("EMAIL_CONFIGURATION:PASSWORD");
                var host = configuration.GetValue<string>("EMAIL_CONFIGURATION:HOST");
                var port = configuration.GetValue<int>("EMAIL_CONFIGURATION:PORT");

                var subject = $"YOUR OTP: {otp}";
                var body = GenerateEmailHtml(otp);

                var smtpClient = new SmtpClient(host, port);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);

                var message = new MailMessage(fromEmail!, toEmail, subject, body);
                message.IsBodyHtml = true;
                await smtpClient.SendMailAsync(message);

                return true;
            }
            catch
            {
                return false;
            }
        }
        private string GenerateEmailHtml(string otp)
        {
            return
            $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f2f2f2;
                        padding: 20px;
                    }}
                    .otp-container {{
                        background-color: #fff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 2px 5px rgba(0,0,0,0.1);
                        max-width: 400px;
                        margin: auto;
                        text-align: center;
                    }}
                    .otp-code {{
                        font-size: 32px;
                        color: #007BFF;
                        margin-top: 10px;
                        letter-spacing: 5px;
                    }}
                    .footer {{
                        margin-top: 20px;
                        font-size: 12px;
                        color: #999;
                    }}
                </style>
            </head>
            <body>
                <div class='otp-container'>
                    <h2>Your One-Time Password (OTP)</h2>
                    <p>Use the code below to proceed:</p>
                    <div class='otp-code'>{otp}</div>
                    <div class='footer'>
                        This code is valid for only a short time.
                    </div>
                </div>
            </body>
            </html>";
        }
        #endregion

        #region Send New Password
        public async Task<bool> SendNewPassword(string toEmail, string newPassword)
        {
            try
            {
                var fromEmail = configuration.GetValue<string>("EMAIL_CONFIGURATION:EMAIL");
                var fromPassword = configuration.GetValue<string>("EMAIL_CONFIGURATION:PASSWORD");
                var host = configuration.GetValue<string>("EMAIL_CONFIGURATION:HOST");
                var port = configuration.GetValue<int>("EMAIL_CONFIGURATION:PORT");

                var subject = $"YOUR New Password";
                var body = GenerateNewPasswordHtml(newPassword);

                var smtpClient = new SmtpClient(host, port);
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);

                var message = new MailMessage(fromEmail!, toEmail, subject, body);
                message.IsBodyHtml = true;
                await smtpClient.SendMailAsync(message);

                return true;
            }
            catch
            {
                return false;
            }
        }

        private string GenerateNewPasswordHtml(string newPassword)
        {
            return
            $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: Arial, sans-serif;
                        background-color: #f2f2f2;
                        padding: 20px;
                    }}
                    .email-container {{
                        background-color: #fff;
                        padding: 20px;
                        border-radius: 10px;
                        box-shadow: 0 2px 5px rgba(0,0,0,0.1);
                        max-width: 400px;
                        margin: auto;
                        text-align: center;
                    }}
                    .password-display {{
                        font-size: 28px;
                        color: #007BFF;
                        margin: 15px 0;
                        letter-spacing: 2px;
                        font-weight: bold;
                    }}
                    .footer {{
                        margin-top: 20px;
                        font-size: 12px;
                        color: #999;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h2>Your New Password</h2>
                    <p>Please use the password below to log in:</p>
                    <div class='password-display'>{newPassword}</div>
                    <p>We recommend that you change this password after logging in for security reasons.</p>
                    <div class='footer'>
                        This is an automated message. Please do not reply.
                    </div>
                </div>
            </body>
            </html>";
        }
        #endregion
    }
}