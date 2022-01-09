using AutoMapper;
using CryptoHelper;
using Malam.Digital.Base.Bll.Helpers;
using Malam.Digital.Base.Entities;
using Malam.Digital.Base.Entities.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Malam.Digital.Base.Bll.Services
{
    public class AuthService
    {
        #region Fields
        private IMapper _mapper;
        private readonly BaseExampleContext _context;
        private readonly AppSettings _appSettings;

        private readonly string MAIL_SUBJECT = "password";
        private readonly string MAIL_BODY = "your password is:";
        
        #endregion

        #region Ctor
        public AuthService(BaseExampleContext context, IMapper mapper, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }
        #endregion

        #region Public Function
        public async Task<LoginResponse> SmsVerification(SmsVerificationRequest smsVerificationRequest)
        {
            var user =await  _context.User.Include(x => x.Role).Where(x => x.Phone == smsVerificationRequest.Phone && smsVerificationRequest.SmsPassword == x.SmsPassword).FirstOrDefaultAsync();
            if (user != null)
            {
                user.DeviceId = smsVerificationRequest.DeviceId;
                try
                {
                    var tokenString = this.GenerateToken(user);
                    user.RefreshToken = GenerateRefreshToken();
                    await _context.SaveChangesAsync();
                    //    // return basic user info (without password) and token to store client side
                    return new LoginResponse
                    {
                        UserId = user.Id,
                        Role = (Role)user.RoleId,
                        Token = tokenString,
                        RefreshToken = user.RefreshToken
                    };

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return null;
        }

        public async Task<bool> Registration(RegistrationRequest userApp)
        {
            var smsPassword = new Random().Next(100000, 999999);
            try
            {
                var user = await _context.User.Where(x => x.Phone == userApp.Phone).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.SmsPassword = smsPassword.ToString();
                }
                await _context.SaveChangesAsync();
                MsgBll msg = new MsgBll();
                var inforu = new Inforu(_appSettings.InforuUser) { Content = { Message = $"{MAIL_BODY} {smsPassword}" }, Recipients = { PhoneNumber = userApp.Phone } };
                return msg.SMSSend(inforu);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<LoginResponse> LoginApp(LoginAppRequest loginAppRequest)
        {
            try
            {
                var user =await _context.User.Include(x => x.Role).Where(x => x.Id == loginAppRequest.UserId && loginAppRequest.DeviceId == x.DeviceId).FirstOrDefaultAsync();

                if (user != null)
                {
                    var tokenString = this.GenerateToken(user);
                    user.RefreshToken = GenerateRefreshToken();
                    await _context.SaveChangesAsync();
                    // return basic user info (without password) and token to store client side
                    return new LoginResponse
                    {
                        UserId = user.Id,
                        Role = (Role)user.RoleId,
                        Token = tokenString,
                        RefreshToken = user.RefreshToken
                    };
                  
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return null;
        }

        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            var user =await _context.User.Include(x => x.Role).Where(x => x.UserName == loginRequest.UserName).FirstOrDefaultAsync();

            if (user != null && loginRequest.Password == EncryptBll.Decrypt(user.Password))
            {
                try
                {
                    var tokenString = GenerateToken(user);
                    user.RefreshToken = GenerateRefreshToken();
                   await  _context.SaveChangesAsync();
                    //    // return basic user info (without password) and token to store client side
                    return new LoginResponse
                    {
                        UserId = user.Id,
                        Role = (Role)user.RoleId,
                        Token = tokenString,
                        RefreshToken= user.RefreshToken
                    };
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return null;
        }

        public async Task<StatusCode> ForgetPassword(string userName)
        {
            try
            {
                var user =await  _context.User.Where(x => x.UserName == userName).FirstOrDefaultAsync();

                if (user != null)
                {
                    var password = EncryptBll.Decrypt(user.Password);
                    MsgBll msgBll = new MsgBll();
                    List<string> mails = new List<string>
                {
                    userName
                };
                    msgBll.SendMail(MAIL_SUBJECT , $"{ MAIL_BODY } {password}", mails, _appSettings.Gmail);
                    return StatusCode.Success;
                }
                return StatusCode.UserNotExists;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<LoginResponse> RefreshToken(RefreshTokenRequest refreshToken)
        {
            int session = 20;
            try
            {
                var cp = CheckToken(refreshToken.Token, out var securityToken);
                if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");
                if (cp != null)
                {
                    var id = cp.Claims.First(c => c.Type == ClaimTypes.Name);
                    var user = await _context.User.FindAsync(int.Parse(id.Value));

                    if (user.RefreshToken == refreshToken.RefreshToken && user.LastActivity?.AddMinutes(session) >= DateTime.Now)
                    {
                        var tokenString = GenerateToken(user);
                        user.RefreshToken = GenerateRefreshToken();
                        await _context.SaveChangesAsync();
                        return new LoginResponse
                        {
                            UserId = user.Id,
                            Role = (Role)user.RoleId,
                            Token = tokenString,
                            RefreshToken = user.RefreshToken
                        };
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Private Function

        private string GenerateRefreshToken(int size = 32)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private string GenerateToken(Entities.Model.User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role,Enum.GetName(typeof(Role),  user.RoleId))
                }),
                Audience = _appSettings.Jwt.Audience,
                Issuer = _appSettings.Jwt.Issuer,
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private ClaimsPrincipal CheckToken(string token,out SecurityToken securityToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
          return  tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey =new SymmetricSecurityKey(key),
                ValidateLifetime = false // we check expired tokens here
            }, out securityToken);

            

        }

        // Method for hashing the password
        private string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        // Method to verify the password hash against the given password
        private bool VerifyPassword(string hash, string password)
        {
            return Crypto.VerifyHashedPassword(hash, password);
        }
        #endregion
    }
}
