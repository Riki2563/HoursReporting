using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Malam.Digital.Base.Entities.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        [RequiredEnum]
        public UserStatusEnum UserStatusId { get; set; }
        [Required]
        [RequiredEnum]
        public Role RoleId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
    }
}
