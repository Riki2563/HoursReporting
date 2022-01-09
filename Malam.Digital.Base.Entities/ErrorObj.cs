using System;
using System.Collections.Generic;
using System.Text;

namespace Malam.Digital.Base.Entities
{
    public class ErrorObj
    {
        public StatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }

        public ErrorObj()
        {
            StatusCode = StatusCode.Success;
            ErrorMessage = EnumHelper.GetDescription(StatusCode.Success);
        }
        public ErrorObj(StatusCode status)
        {
            StatusCode = status;
            ErrorMessage = EnumHelper.GetDescription(status);
        }

        public ErrorObj(StatusCode status, string message)
        {
            StatusCode = status;
            ErrorMessage = message;
        }




    }
}
