using System;
using System.Collections.Generic;
using System.Text;

namespace Malam.Digital.Base.Entities
{
  public  class LoginResponse
    {
        public int UserId { get; set; }
        public Role Role { get; set; }
        public string  Token { get; set; }
        public string RefreshToken { get; set; }

    }
}
