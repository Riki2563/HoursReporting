using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Malam.Digital.Base.Bll.Services;
using log4net;
using Malam.Digital.Base.Entities.Dto;
using Malam.Digital.Base.Entities;
using Microsoft.AspNetCore.Authorization;

namespace Malam.Digital.BaseWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserService _service;
        private ILog _log;
        public UsersController(UserService service,ILog log)
        {
            _service  = service ;
            _log = log;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUser()
        {
            _log.Debug("Start GetUser ");
            try
            {
                return Ok(await _service.GetAllUsers());
            }
            catch (Exception ex)
            {
                _log.Error(typeof(UsersController), ex);
                return BadRequest(new ErrorObj(Malam.Digital.Base.Entities.StatusCode.Error));
            }
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            _log.Debug($"Start GetUser/{id} ");
            UserDto user;
            try
            {
                 user = await _service.GetUser(id);
            }
            catch (Exception ex)
            {
                _log.Error(typeof(UsersController), ex);
                return BadRequest(new ErrorObj(Malam.Digital.Base.Entities.StatusCode.Error));
            }
            if (user == null)
            {
                return NotFound();
            }

            return Ok( user);
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDto user)
        {
            _log.Debug("Start PutUser ");
            if (user == null)
            {
                return BadRequest(new ErrorObj(Malam.Digital.Base.Entities.StatusCode.NullObject));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObj(Malam.Digital.Base.Entities.StatusCode.requiredField, ModelState.ToString()));
            }
            if (id != user.Id)
            {
                return BadRequest(new ErrorObj(Malam.Digital.Base.Entities.StatusCode.Error));
            }
            try
            {
               await _service.PutUser(user, int.Parse(User.Identity.Name));
            }
            catch (Exception ex)
            {
                _log.Error(typeof(UsersController), ex);
                return BadRequest(new ErrorObj(Malam.Digital.Base.Entities.StatusCode.Error));
            }
            return NoContent();
        }

        // POST: api/Users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> PostUser(UserDto user)
        {
            _log.Debug("Start PostUser ");
            if (user == null)
            {
                return BadRequest(new ErrorObj(Malam.Digital.Base.Entities.StatusCode.NullObject));
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(new ErrorObj(Malam.Digital.Base.Entities.StatusCode.requiredField, ModelState.ToString()));
            }
            try
            {
                var status  = await _service.PostUser(user, int.Parse(User.Identity.Name));
                if (status == Base.Entities.StatusCode.Success)
                    return Ok(user);
                else
                    return BadRequest(new ErrorObj(status));

            }

            catch (Exception ex)
            {
                _log.Error(typeof(UsersController), ex);
                return BadRequest(new ErrorObj(Base.Entities.StatusCode.Error));
            }
}

     
        
    }
}
