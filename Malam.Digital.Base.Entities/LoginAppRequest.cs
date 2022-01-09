using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Malam.Digital.Base.Entities
{
   public  class LoginAppRequest
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }
        [Required]
        public string DeviceId { get; set; }
    }
}
