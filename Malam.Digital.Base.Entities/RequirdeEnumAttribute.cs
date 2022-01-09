using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Malam.Digital.Base.Entities
{
    public class RequiredEnumAttribute : RequiredAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return false;
            var type = value.GetType();
            return type.IsEnum && Enum.IsDefined(type, value); ;
        }
    }
}
