using System;
using System.Collections.Generic;

namespace Malam.Digital.Base.Entities.Model
{
    public partial class Roles
    {
        public Roles()
        {
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Role { get; set; }

        public virtual ICollection<User> User { get; set; }
    }
}
