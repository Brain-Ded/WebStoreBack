using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Web_StoreAPI.Dtos;
using System.Security.Cryptography.X509Certificates;
using Web_StoreAPI.DataModels;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace Web_StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public UserController(DataContext dataContext)
        {
            _dataContext=dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> Get()
        {
            return Ok(await _dataContext.Users.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<User>>> Get(int id)
        {
            var user = await _dataContext.Users.FindAsync(id);
            if (user is null)
            {
                return BadRequest("No user found");
            }
            return Ok(user);
        }
    }
}
