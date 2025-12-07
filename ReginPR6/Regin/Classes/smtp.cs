using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Regin.Classes
{
    public class smtp
    {
        public enum _message
        {
            verify,
            change
        }

        private static string GenToken()
        {
            Random random = new Random();
            string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(Chars, 6).Select(x => x[random.Next(Chars.Length)]).ToArray());
        }

        private static string GetMessage(string code,_message type)
        {
            switch(type)
            {
                case _message.verify:
                    return $@"
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px; }}
                                .code {{ font-size: 24px; font-weight: bold; color: #1a73e8; text-align: center; margin: 20px 0; }}
                                .footer {{ margin-top: 30px; font-size: 12px; color: #5f6368; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h2>Подтверждение вашей Почты</h2>
                                <p>Подтверждения почты или смены пароля используйте следующий код:</p>
                                <div class='code'>{code}</div>
                                <p>Если вы не запрашивали этот код, пожалуйста, проигнорируйте это письмо.</p>
                            </div>
                        </body>
                        </html>
                    ";
                case _message.change:
                    return $@"
                        <html>
                        <head>
                            <style>
                                body {{ font-family: Arial, sans-serif; }}
                                .container {{ max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 8px; }}
                                .code {{ font-size: 24px; font-weight: bold; color: #1a73e8; text-align: center; margin: 20px 0; }}
                                .footer {{ margin-top: 30px; font-size: 12px; color: #5f6368; }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <h2>Смена пароля</h2>
                                <p>Ваш новый пароль:</p>
                                <div class='code'>{code}</div>
                            </div>
                        </body>
                        </html>
                    ";
                default:
                    return "ошибка";
            }
        }

        public static bool send(string email, _message type, out string verificationCode)
        {
            try
            {
                string Sender = "illHlli@yandex.ru";
                string SenderOAUTHTOken = "bxkegwxmrkghpsnl";
                //https://mail.yandex.ru/?r=969&uid=1279282984#setup/client
                //https://id.yandex.ru/security/app-passwords?retpath=https%3A%2F%2Fmail.yandex.ru%2F%3Fr%3D969%26uid%3D1279282984&uid=1279282984&scope=mail
                verificationCode = GenToken();
                //Console.WriteLine(email);
                //Console.WriteLine(verificationCode);
                // Настройка SMTP клиента
                using (SmtpClient smtpClient = new SmtpClient("smtp.yandex.ru", 587))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = new NetworkCredential(Sender, SenderOAUTHTOken);
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Создание письма
                    using (MailMessage mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(Sender, "Forgot");
                        mailMessage.To.Add(email);
                        mailMessage.Subject = "Верификация вашей Почты";
                        mailMessage.IsBodyHtml = true;

                        mailMessage.Body = GetMessage(verificationCode, type);

                        // Отправка письма

                        smtpClient.Send(mailMessage);
                        return true;
                    }
                }
            }
            catch (SmtpException ex)
            {
                Debug.WriteLine(ex.Message);
                verificationCode = string.Empty;
                return false;
            }
        }
    }
}
