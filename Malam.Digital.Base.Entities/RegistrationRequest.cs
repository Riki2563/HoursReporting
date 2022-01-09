using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Malam.Digital.Base.Entities
{
   public  class RegistrationRequest
    {
        [Phone]
        [Required]
        public string Phone { get; set; }
    }
}
