using CorePush.Google;
using Malam.Digital.Base.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
//using System.Xml;
//using System.Xml.Serialization;

namespace Malam.Digital.Base.Bll.Helpers
{
   
public class MsgBll
    {
        private readonly string SERVER_KEY = "AIzaSyD2EZMwC8MpKR5TBebSnTRTLBqlJTQABno";
        private readonly string NOTIFICATION_TITLE = "earlyBird notification";
        private readonly string INFORU_URL = "http://api.inforu.co.il/SendMessageXml.ashx";
        private readonly string PARAM_NAME = "InforuXML=";
        private readonly string SUCCESS = "successfully";
        #region Public function 
        public bool SMSSend(Inforu inforu)
        {
          
           XmlSerializer xmlSerializer = new XmlSerializer(inforu.GetType());         
            using (StringWriter textWriter = new StringWriter())
            {
                xmlSerializer.Serialize(textWriter, inforu);
                XElement docX = XElement.Parse(textWriter.ToString());
                foreach (XElement XE in docX.DescendantsAndSelf())
                {
                    // Stripping the namespace by setting the name of the element to it's localname only
                    XE.Name = XE.Name.LocalName;
                    // replacing all attributes with attributes that are not namespaces and their names are set to only the localname
                    XE.ReplaceAttributes((from xattrib in XE.Attributes().Where(xa => !xa.IsNamespaceDeclaration) select new XAttribute(xattrib.Name.LocalName, xattrib.Value)));
                }
              
                string paramValue = HttpUtility.UrlEncode(docX.ToString(), Encoding.UTF8);
                string result = URLPostData(INFORU_URL,PARAM_NAME + paramValue);
                if (result.Contains(SUCCESS))
                    return true;
                return false;
            }
            
        }


        public bool SendMail(string subject, string body, List<string> mailTo, Gmail gmail)

        {
            try
            {
                MailMessage message = new MailMessage
                {
                    From = new MailAddress(gmail.Mail),
                    Subject = subject,
                    IsBodyHtml = true,
                    Body = body
                };
                SmtpClient client = new SmtpClient() { DeliveryMethod = SmtpDeliveryMethod.Network, Host = "smtp.gmail.com", Port = 587, EnableSsl = true, UseDefaultCredentials = false };
                client.Credentials = new System.Net.NetworkCredential(gmail.Mail, gmail.Password);

                foreach (string s in mailTo)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        var mails = s.Split(';');
                        mails.ToList()?.ForEach(x =>
                        {
                            if (new EmailAddressAttribute().IsValid(x))
                                message.To.Add(new MailAddress(x));
                        });
                    }
                }

                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }

        public async void SendNotification(string body, string token,int value=0)
        {
            try
            {
                using (var fcm = new FcmSender(SERVER_KEY, ""))
                {
                    var payload = new
                    {
                        to = "",
                        priority = "high",
                        content_available = true,
                        notification = new
                        {
                            body = body,
                            title = NOTIFICATION_TITLE,
                            badge = 1
                        },
                        data = new
                        {
                            key1 = value //,
                        //    key2 = "value2"
                        }

                    };
                    await fcm.SendAsync(token, payload);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        #endregion

        #region Private Function
        private string URLPostData(string url, string data)
        {
            //Setup the web request
            string szResult = string.Empty;
            WebRequest Request = WebRequest.Create(url);
            Request.Timeout = 30000;
            Request.Method = "POST";
            Request.ContentType = "application/x-www-form-urlencoded";
            //Set the POST data in a buffer
            byte[] PostBuffer;
            try
            {
                // replacing " " with "+" according to Http post RPC
                data = data.Replace(" ", "+");
                //Specify the length of the buffer
                PostBuffer = Encoding.UTF8.GetBytes(data);
                Request.ContentLength = PostBuffer.Length;
                //Open up a request stream
                Stream RequestStream = Request.GetRequestStream();
                //Write the POST data
                RequestStream.Write(PostBuffer, 0, PostBuffer.Length);
                //Close the stream
                RequestStream.Close();
                //Create the Response object
                WebResponse Response;
                Response = Request.GetResponse();
                //Create the reader for the response
                StreamReader sr = new StreamReader(Response.GetResponseStream(), Encoding.UTF8);
                //Read the response
                szResult = sr.ReadToEnd();
                //Close the reader, and response
                sr.Close();
                Response.Close();
                return szResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
    }
}
