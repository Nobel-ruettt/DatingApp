using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Dtos;
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
        public async Task<IActionResult> register(UserForRegisterDto userforregisterdto)
        {
            userforregisterdto.Username= userforregisterdto.Username.ToLower();

            if(await _context.UserExists(userforregisterdto.Username))
            {
                return BadRequest("Username alreaady exists");
            }

            var user = new User();

            user.UserName = userforregisterdto.Username;

            user =await _context.Register(user, userforregisterdto.Password);

            return StatusCode(201);

        }
    }
}
