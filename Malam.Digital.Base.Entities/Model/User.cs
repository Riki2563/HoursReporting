using System;
using System.Collections.Generic;

namespace Malam.Digital.Base.Entities.Model
{
    public partial class User
    {
        public User()
        {
            InverseCreatedByUser = new HashSet<User>();
            InverseLastModifyUser = new HashSet<User>();
        }

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int UserStatusId { get; set; }
        public int RoleId { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreateDate { get; set; }
        public int? LastModifyUserId { get; set; }
        public DateTime? LastModifyDate { get; set; }
        public int SysRowStatus { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string SmsPassword { get; set; }
        public string DeviceId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? LastActivity { get; set; }

        public virtual User CreatedByUser { get; set; }
        public virtual User LastModifyUser { get; set; }
        public virtual Roles Role { get; set; }
        public virtual UserStatus UserStatus { get; set; }
        public virtual ICollection<User> InverseCreatedByUser { get; set; }
        public virtual ICollection<User> InverseLastModifyUser { get; set; }
    }
}
