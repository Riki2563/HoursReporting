using AutoMapper;
using Malam.Digital.Base.Bll.Helpers;
using Malam.Digital.Base.Entities;
using Malam.Digital.Base.Entities.Dto;
using Malam.Digital.Base.Entities.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Malam.Digital.Base.Bll.Services
{
    public class UserService
    {
        #region Fields
        private IMapper _mapper;
        private readonly BaseExampleContext _context;
        private readonly AppSettings _appSettings;

        private readonly string SMS_HADER = "password to app";
        private readonly string SMS_BODY = "your password is";

        #endregion

        #region Ctor
        public UserService(BaseExampleContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        #endregion

        #region Public Function
        public async Task<IEnumerable<UserDto>> GetAllUsers()
        {
            try
            {
                var list = await _context.User.ToListAsync();
                return _mapper.Map<IList<UserDto>>(list);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public async Task<UserDto> GetUser(int id)
        {
            try
            {
                var user = await _context.User.FindAsync(id);
                user.Password = null;
                return _mapper.Map<UserDto>(user);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<StatusCode> PostUser(UserDto user,int userId)
        {
            try
            {
                StatusCode status = StatusCode.Error;
                var userExist = _context.User.Any(x => x.UserName == user.UserName);
                if (userExist)
                {
                    return StatusCode.MailExists;
                }//todo אולי צריך לבדוק גם שטלפון לא קיים 

                var userDb = _mapper.Map<Entities.Model.User>(user);
                var password = GenerateRandomPassword();
                userDb.Password = EncryptBll.Encrypt(password);
                userDb.CreatedByUserId = userId;
                _context.User.Add(userDb);
                await _context.SaveChangesAsync();
                user.Id = userDb.Id;
                MsgBll ctrl = new MsgBll();
                var mailTo = new List<string>
                {
                    user.UserName
                };
                ctrl.SendMail(SMS_HADER, $"{SMS_BODY} {password  }", mailTo, _appSettings.Gmail);
                status = StatusCode.Success;
                return status;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task PutUser(UserDto user,int userId)
        {
            try
            {
                var userDb = _context.User.Find(user.Id);
                _mapper.Map(user, userDb);
                userDb.LastModifyUserId  = userId;
                userDb.LastModifyDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private Function
        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        private string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = false,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
        "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
        "abcdefghijkmnopqrstuvwxyz",    // lowercase
        "0123456789",                   // digits
        "!@$?_-"                        // non-alphanumeric
    };
            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength
                || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }
        #endregion
    }
}
