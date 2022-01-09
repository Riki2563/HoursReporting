using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Malam.Digital.Base.Entities
{
  public   class SmsVerificationRequest
    {
        [Phone]
        [Required]
        public string Phone { get; set; }
        [Required]
        public string SmsPassword { get; set; }
        [Required]
        public string DeviceId { get; set; }
    }
}
