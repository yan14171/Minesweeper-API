using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using minesweeper_api.Data;
using minesweeper_api.Data.Models;
using minesweeper_api.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace minesweeper_api.Controllers
{
    [ApiController]
    [AuthExceptionFilter]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<Stat> _statRepo;
        private readonly JwtConfig _jwtConfig;

        public AuthController(IRepository<User> userRepo,
                              IRepository<Stat> statRepo,
                              JwtConfig config)
        {
            this._userRepo = userRepo;
            this._statRepo = statRepo;
            this._jwtConfig = config;
        }

        [HttpPost]
        public IActionResult Authorize([FromBody] User request)
        {
            var foundUser = _userRepo.GetAll().FirstOrDefault(n => n.Email == request.Email);
            
            User createdUser;
            if (foundUser is not null && foundUser.Password != request.Password)
                throw new UnauthorizedAccessException("Wrong password specified");
            else if (foundUser?.Password == request.Password)
                createdUser = foundUser;
            else
                createdUser = new User
                {
                    Email = request.Email,
                    Password = request.Password,
                    Name = GenerateName(request.Email)
                };

            _userRepo.Add(createdUser);

            var tokenObj = GenerateJwtToken(createdUser);

            return Ok(
                new {
                IsSuccess = true,
                Messages = new List<string> { "Token created successfully" },
                Token = tokenObj
            });
        }

        private string GenerateName(string email)
        {
            var @index = email.IndexOf('@');
            if (@index == -1)
                return email;

            var cutEmail = email.Substring(0, index);

            return cutEmail;
        }

        private string GenerateJwtToken(User user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = GetUserIdentity(user),
                Expires = DateTime.UtcNow.AddMinutes(30), // needs to be implemented with refreshes 
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            return jwtToken;
        }
        
        private ClaimsIdentity GetUserIdentity(User user)
        {
            var claims = new ClaimsIdentity(
              new[]{
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            });

            return claims;
        }
    }
}
