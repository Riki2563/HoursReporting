using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Malam.Digital.Base.Entities;
using log4net;
using Malam.Digital.Base.Bll.Services;

namespace Malam.Digital.BaseWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AuthService  _service;
        private ILog _log;

        public AuthController(AuthService service, ILog log)
        {
            _service = service;
            _log = log;
        }
        [HttpPost, Route("LoginApp")]
        public async Task<ActionResult<LoginResponse>> LoginApp([FromBody]LoginAppRequest  loginAppRequest )
        {
            if (loginAppRequest == null)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.NullObject));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.requiredField, ModelState.ToString()));
            }

            try
            {
               var token =await  _service.LoginApp(loginAppRequest);

                if (token != null)
                {
                    return Ok (token);
                }
                else
                {
                    return Unauthorized(new ErrorObj(Base.Entities.StatusCode.UserUnauthorized));
                }
            }
            catch (Exception ex)
            {
                _log.Error(typeof(AuthController), ex);
                return Unauthorized(new ErrorObj(Base.Entities.StatusCode.Error));
            }

        }

        [HttpPost, Route("Login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody]LoginRequest loginRequest )
        {
            _log.Debug("Start Login ");
            if (loginRequest == null)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.NullObject));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.requiredField, ModelState.ToString()));
            }

            try
            {
              var  token =await  _service.Login(loginRequest);

                if (token != null)
                {
                    return Ok( token);
                }
                else
                {
                    return Unauthorized(new ErrorObj(Base.Entities.StatusCode.UserUnauthorized));
                }
            }
            catch (Exception ex)
            {
                _log.Error(typeof(AuthController), ex);
                return Unauthorized(new ErrorObj(Base.Entities.StatusCode.Error));
            }

        }

        [HttpGet, Route("forgetPassword/{userName}")]
        public async Task<IActionResult> ForgetPassword(string userName)
        {
            _log.Debug("Start ForgetPassword ");
            try
            {
                var result =await  _service.ForgetPassword(userName);
                if (result == (int)Base.Entities.StatusCode.Success)
                    return Ok();
                else
                    return BadRequest(Base.Entities.StatusCode.Error);
            }
            catch (Exception ex)
            {
                _log.Error(typeof(AuthController), ex);
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.Error));
            }
        }

        [HttpPost, Route("Registration")]
        public async Task<IActionResult> Registration([FromBody] RegistrationRequest registrationRequest)
        {
            _log.Debug("Start Registration ");
            if (registrationRequest == null)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.NullObject));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.requiredField, ModelState.ToString()));
            }


            try
            {
                var sms =await _service.Registration(registrationRequest);
                if (!sms)
                {
                    return BadRequest(new ErrorObj(Base.Entities.StatusCode.WrongSms));
                }
                return Ok();

            }
            catch (Exception ex)
            {
                _log.Error(typeof(AuthController), ex);
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.Error));
            }
        }

        [HttpPost, Route("SmsVerification")]
        public async Task<ActionResult<LoginResponse>> SmsVerification([FromBody] SmsVerificationRequest  smsVerificationRequest )
        {
            if (smsVerificationRequest  == null)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.NullObject));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.requiredField, ModelState.ToString()));
            }

            try
            {
               var token =await _service.SmsVerification(smsVerificationRequest);
                if (token != null)
                {
                    return Ok(token);
                }
                else
                {
                    return BadRequest(new ErrorObj(Base.Entities.StatusCode.WrongSms));
                }
            }
            catch (Exception ex)
            {
                _log.Error(typeof(AuthController), ex);
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.Error));
            }
           
        }

        [HttpPost, Route("RefreshToken")]
        public async Task<ActionResult<LoginResponse>> RefreshToken(RefreshTokenRequest refreshToken)
        {
            _log.Debug("Start Login ");
            if (refreshToken == null)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.NullObject));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.requiredField, ModelState.ToString()));
            }

            try
            {
                var token = await _service.RefreshToken(refreshToken);

                if (token != null)
                {
                    return Ok(token);
                }
                else
                {
                    return Unauthorized(new ErrorObj(Base.Entities.StatusCode.UserUnauthorized));
                }
            }
            catch (Exception ex)
            {
                _log.Error(typeof(AuthController), ex);
                return Unauthorized(new ErrorObj(Base.Entities.StatusCode.Error));
            }

        }
    }
}
