using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Simple.Core.Email
{
    public static class EmailAgent
    {
        static bool mailSent = false;
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                Console.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                Console.WriteLine("[{0}] {1}", token, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message sent.");
            }
            mailSent = true;
        }
        public static void Send(string host, string fromAddress, string toAddress)
        {
            //AccessKey ID：
            //LTAI5tSMWkL24UwvJhkmZdaq
            //AccessKey Secret：
            //0f5izQepshIMyaCDAC0kmvNvtdRPtd

            //// Command-line argument must be the SMTP host.
            //SmtpClient client = new SmtpClient(host, 25);
            //client.UseDefaultCredentials = true;
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.Credentials = new NetworkCredential("bot@bq8.com", "OcnjtTa5tUXf8sfQ");

            //// Specify the email sender.
            //// Create a mailing address that includes a UTF8 character
            //// in the display name.
            //MailAddress from = new MailAddress(fromAddress, "Bot", Encoding.UTF8);
            //// Set destinations for the email message.
            //MailAddress to = new MailAddress(toAddress);
            //// Specify the message content.
            //MailMessage message = new MailMessage(from, to);
            //message.Body = "This is a test email message sent by an application. ";
            //// Include some non-ASCII characters in body and subject.
            //string someArrows = new string(new char[] { '\u2190', '\u2191', '\u2192', '\u2193' });
            //message.Body += Environment.NewLine + someArrows;
            //message.BodyEncoding = Encoding.UTF8;
            //message.Subject = "test message 1" + someArrows;
            //message.SubjectEncoding = System.Text.Encoding.UTF8;
            //// Set the method that is called back when the send operation ends.
            //client.SendCompleted += new
            //SendCompletedEventHandler(SendCompletedCallback);
            //// The userState can be any object that allows your callback
            //// method to identify this send operation.
            //// For this example, the userToken is a string constant.
            //string userState = "test message1";
            //client.SendAsync(message, userState);
            //// Clean up.
            //message.Dispose();
            //Console.WriteLine("Goodbye.");


            SmtpClient client = new SmtpClient();
            client.Host = host;//使用163的SMTP服务器发送邮件
            client.Port = 25;
            client.UseDefaultCredentials = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(fromAddress, "OcnjtTa5tUXf8sfQ");//163的SMTP服务器需要用163邮箱的用户名和密码作认证，如果没有需要去163申请个,
                                                                                        //这里假定你已经拥有了一个163邮箱的账户，用户名为abc，密码为*******
            MailMessage Message = new MailMessage();
            Message.From = new MailAddress(fromAddress);//这里需要注意，163似乎有规定发信人的邮箱地址必须是163的，而且发信人的邮箱用户名必须和上面SMTP服务器认证时的用户名相同
                                                        //因为上面用的用户名abc作SMTP服务器认证，所以这里发信人的邮箱地址也应该写为abc@163.com
                                                        //Message.To.Add("123456@gmail.com");//将邮件发送给Gmail
            Message.To.Add("1053123198@qq.com");//将邮件发送给QQ邮箱
            Message.Subject = "测试标体";
            Message.Body = "测试邮件体";
            Message.SubjectEncoding = Encoding.UTF8;
            Message.BodyEncoding = Encoding.UTF8;
            Message.Priority = MailPriority.High;
            Message.IsBodyHtml = true;
            client.Send(Message);

        }
    }
}
