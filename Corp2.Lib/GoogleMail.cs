using Corp2.Lib;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Configuration;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Web;

namespace Corp2.Lib
{
    public class GoogleMail: ClassBase
    {
        static string[] Scopes = { GmailService.Scope.GmailSend };
        static string ApplicationName = "Corporations Demo";
        readonly string sendingUserEmail = ConfigurationManager.AppSettings["SendingEmailUser"];

        //public static bool SendMail(string[] args)
        //{
        //    UserCredential credential;

        //    using (var stream =
        //        new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
        //    {
        //        // The file token.json stores the user's access and refresh tokens, and is created
        //        // automatically when the authorization flow completes for the first time.
        //        string credPath = "token.json";
        //        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
        //            GoogleClientSecrets.Load(stream).Secrets,
        //            Scopes,
        //            "user",
        //            CancellationToken.None,
        //            new FileDataStore(credPath, true)).Result;
        //        Console.WriteLine("Credential file saved to: " + credPath);
        //    }

        //    // Create Gmail API service.
        //    var service = new GmailService(new BaseClientService.Initializer()
        //    {
        //        HttpClientInitializer = credential,
        //        ApplicationName = ApplicationName,
        //    });

        //    // Define parameters of request.
        //    UsersResource.LabelsResource.ListRequest request = service.Users.Labels.List("me");

        //    // List labels.
        //    IList<Label> labels = request.Execute().Labels;
        //    Console.WriteLine("Labels:");
        //    if (labels != null && labels.Count > 0)
        //    {
        //        foreach (var labelItem in labels)
        //        {
        //            Console.WriteLine("{0}", labelItem.Name);
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine("No labels found.");
        //    }
        //    Console.Read();
        //}


        public static GoogleMail Default
        {
            get
            {
                return new GoogleMail();
            }
        }

        public bool SendMail(string sendTo, string messageSubject, string messageBody)
        {
            bool retVal = false;
            try
            {
                var msg = new AE.Net.Mail.MailMessage
                {
                    Subject = messageSubject,
                    Body = messageBody,
                    From = new MailAddress(sendingUserEmail)
                };
                msg.To.Add(new MailAddress(sendTo));
                msg.ReplyTo.Add(msg.From);
                var msgStr = new StringWriter();
                msg.Save(msgStr);

                UserCredential credential;
                string path = HttpContext.Current.Server.MapPath("\\");
                using (var stream = new FileStream($"{path}credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = $"{path}token.json";
                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        sendingUserEmail,
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                // Create Gmail API service.
                var gmailService = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                   
                });

                var result = gmailService.Users.Messages.Send(new Message { Raw = Base64UrlEncode(msgStr.ToString()) }, "me").Execute();
                Console.WriteLine("Message ID {0} sent.", result.Id);
            }
            catch (Exception ex)
            {
                ErrorDescription = ex.Message;
            }
            return retVal;
        }

        private static string Base64UrlEncode(string input)
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
    }
}