using System;
using System.Collections.Generic;

namespace Malam.Digital.Base.Entities.Model
{
    public partial class UserStatus
    {
        public UserStatus()
        {
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Value { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
