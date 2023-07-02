using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Web_StoreAPI.DataModels;
using Web_StoreAPI.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Web_StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _config;

        public AuthController(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;
            _config = configuration;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Register(UserDto request)
        {
            if (await _dataContext.Users.AnyAsync(x => x.Email.Equals(request.Email)))
            {
                return BadRequest("User with this email aldready exists");
            }

            var userType = await _dataContext.UserTypes.FindAsync(request.UserTypeId);

            if (userType == null)
            {
                return NotFound();
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash);

            var user = new User()
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                UserType = userType,
                UserTypeId = request.UserTypeId,
            };

            _dataContext.Users.Add(user);

            await _dataContext.SaveChangesAsync();

            var logUser = new UserLoginDto()
            {
                Email = user.Email,
                Password = request.Password
            };

            string token = CreateToken(user, userType);

            return Ok(token);
        }

        [HttpPost("LogIn")]
        public async Task<ActionResult<string>> Login(UserLoginDto request)
        {
            var user = await _dataContext.Users.FirstAsync(x => x.Email.Equals(request.Email)
            && x.PasswordHash.SequenceEqual(VerifyPasswordHash(request.Password)));

            var userType = await _dataContext.UserTypes.FindAsync(user.UserTypeId);

            if (user == null || userType == null)
            {
                return BadRequest("Either Email or Password is incorrect");
            }

            string token = CreateToken(user, userType);

            return Ok(token);
        }

        [HttpPost("VerifyToken")]

        public async Task<ActionResult<User>> VerifyToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();

            DateTimeOffset timeOfsset = DateTimeOffset.FromUnixTimeSeconds(jwtToken.Payload.Exp.Value);

            DateTime time = timeOfsset.LocalDateTime;

            if (time.CompareTo(DateTime.Now) < 1)
            {
                return BadRequest("Token has expired");
            }

            string email = claims[1].Value;

            var User = await _dataContext.Users.FirstAsync(x => x.Email.Equals(email));

            if(User == null)
            {
                return BadRequest("User with this email does not exist");
            }

            return Ok(User);
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult<string>> Delete(UserLoginDto request)
        {
            var user = await _dataContext.Users.FirstAsync(x => x.Email.Equals(request.Email)
            && x.PasswordHash.SequenceEqual(VerifyPasswordHash(request.Password)));

            if (user == null)
            {
                return BadRequest("Either Email or Password is incorrect");
            }

            _dataContext.Users.Remove(user);

            await _dataContext.SaveChangesAsync();

            return Ok("User was deleted");
        }

        private string CreateToken(User user, UserType type)
        {
            List<Claim> claim = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, type.Type)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken
                (
                    claims: claim,
                    expires: DateTime.Now.AddHours(2),
                    signingCredentials: cred
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash)
        {
            using (var hmac = new HMACSHA512())
            {
                hmac.Key = System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value);
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private byte[] VerifyPasswordHash(string password)
        {
            byte[] passwordHash;
            using (var hmac = new HMACSHA512())
            {
                hmac.Key = System.Text.Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value);
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }

            return passwordHash;
        }
    }
}
