using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Malam.Digital.Base.Entities
{
    public enum UserStatusEnum
    {
        Active = 1,
        NotActive = 2
    }
    public enum Role
    {
        Admin = 1,
        Secretary = 2
    }
    public enum StatusCode
    {
        [Description("Success")]
        Success = 0,
        [Description("Error in server")]
        Error = -1,
        [Description("token Expire")]
        GuideExpire = -2,
        [Description("User Unauthorized")]
        UserUnauthorized = -3,
        [Description("password must be minimum 6 chars")]
        errorPasswordLength = -4,
        [Description("required fields")]
        requiredField = -5,
        [Description("wrong Mail")]
        wrongMail = -6,
        [Description("Mail Exists")]
        MailExists = -7,
        [Description("Wrong Sms")]
        WrongSms = -8,
        [Description("Client send Null Object")]
        NullObject = -9,
        [Description("User not exists")]
        UserNotExists = -10,
        [Description("can not invite exists user")]
        InviteExistsUser = -11,
        [Description("Wrong userName or password")]
        WrongUserNameOrPassword = -15,
        [Description("Error File Type")]
        ErrorFileType = -16,
        [Description("Menu Exist")]
        MenuExist = -17,
        [Description("Categories Not Match Combination")]
        CategoriesNotMatchCombination = -18,
        [Description("Missing Sub Category")]
        MissingSubCategory = -19,
        [Description("Missing Item To SubCategory")]
        MissingItemToSubCategory = -20,
        [Description("Wrong Credit Card")]
        WrongCreditCardNum = 33,
        [Description("Wrong Credit Card")]
        WrongCreditCard = 39,
        [Description("Wrong expire date")]
        WrongExpireDate = 36


    }
    public static class EnumHelper
    {
        /// <summary>
        /// Retrieve the description on the enum, e.g.
        /// [Description("Bright Pink")]
        /// BrightPink = 2,
        /// Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

    }
}
