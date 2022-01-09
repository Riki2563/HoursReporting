using System;
using System.Collections.Generic;
using System.Text;

namespace Malam.Digital.Base.Entities
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public Jwt Jwt { get; set; }
        public InforuUser InforuUser { get; set; }
        public Gmail Gmail { get; set; }
        public string AppLink { get; set; }
    }
    public class InforuUser
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string InforuParam { get; set; }
    }
    public class Gmail
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }

    public class Jwt
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
    }
}
