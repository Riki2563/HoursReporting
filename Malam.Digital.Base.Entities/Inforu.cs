using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Malam.Digital.Base.Entities
{
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class Content
    {
        [XmlAttribute]
        public string Type { get; set; }

        public string Message { get; set; }
    }
    public class Recipients
    {
        public string PhoneNumber { get; set; }
    }
    public class Settings
    {
        public int MessageInterval
        {
            get
            {
                return 0;
            }
        }
        [XmlElement(IsNullable = false)]
        public string SenderNumber { get; set; }
        [XmlElement(IsNullable = false)]
        public string Sender { get; set; }
        public string CustomerParameter { get; set; }
    }
    public class Inforu
    {
        public User User { get; set; }
        public Content Content { get; set; }
        public Recipients Recipients { get; set; }
        public Settings Settings { get; set; }

        public Inforu(InforuUser inforuUser)
        {
            User = new User() { Username = inforuUser.UserName, Password = inforuUser.Password };
            Content = new Content() { Type = "sms" };
            Recipients = new Recipients();
            Settings = new Settings() { CustomerParameter = inforuUser.InforuParam };
        }
        public Inforu() { }
    }
}
