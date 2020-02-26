using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.Data;
using DatingApp.Dtos;
using DatingApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _context;

        private readonly IConfiguration _config;
        public AuthController(IAuthRepository context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(UserForRegisterDto userforregisterdto)
        {
            userforregisterdto.Username = userforregisterdto.Username.ToLower();

            if (await _context.UserExists(userforregisterdto.Username))
            {
                return BadRequest("Username alreaady exists");
            }

            var user = new User();

            user.UserName = userforregisterdto.Username;

            user = await _context.Register(user, userforregisterdto.Password);

            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> login(UserForLoginDto userforlogindto)
        {
            var UserFromRepo = await _context.Login(userforlogindto.Username, userforlogindto.Password);
            if (UserFromRepo == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, UserFromRepo.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var TokenHandler = new JwtSecurityTokenHandler();

            var token = TokenHandler.CreateToken(TokenDescriptor);

            return Ok(new
            {
                token = TokenHandler.WriteToken(token)
            });
        }
    }   
}

