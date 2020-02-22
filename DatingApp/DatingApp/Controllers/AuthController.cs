using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController:ControllerBase
    {
        private readonly IAuthRepository _context;
        public AuthController(IAuthRepository context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(string username,string password)
        {
            username = username.ToLower();

            if(await _context.UserExists(username))
            {
                return BadRequest("Username alreaady exists");
            }

            var user = new User();

            user.UserName = username;

            user =await _context.Register(user, password);

            return StatusCode(201);

        }
    }
}
