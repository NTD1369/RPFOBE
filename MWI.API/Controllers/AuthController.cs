using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MWI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IUserService _userService;
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config, IUserService userService)
        {
            _config = config;
            _userService = userService;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> GetByUsername(LoginViewModel model)
        {
            try
            {
                var userFromRepo = await _userService.LoginMwi(model.UserName, model.Password);
                if (userFromRepo.Success == false || userFromRepo.Data == null)
                    return Unauthorized();
                var Model = userFromRepo.Data as MUser;
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, Model.UserId.ToString()),
                    new Claim(ClaimTypes.Name, Model.Username)
                };
                string KeyConf = _config.GetSection("AppSettings:Token").Value;
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KeyConf));

                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddMinutes(30),
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new
                {
                    token = tokenHandler.WriteToken(token),
                    user = userFromRepo
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
