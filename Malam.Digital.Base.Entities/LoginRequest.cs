using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Malam.Digital.Base.Entities
{
   public  class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }
        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
   
   
       
    
}
