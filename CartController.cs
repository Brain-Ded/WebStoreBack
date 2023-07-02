using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Web_StoreAPI.DataModels;

namespace Web_StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public CartController(DataContext dataContext, IConfiguration configuration)
        {
            _dataContext = dataContext;

            _dataContext.Users.Include("cartItems").Load();           
        }

        [HttpGet("ClearCart")]
        public async Task<ActionResult<List<Item>>> ClearCart(string token)
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

            var user = await _dataContext.Users.FirstAsync(x => x.Email.Equals(email));

            foreach(Item item in user.cartItems)
            {
                item.Status = false;
            }

            user.cartItems = new List<Item>();

            await _dataContext.SaveChangesAsync();

            return Ok(user.cartItems);
        }

        [HttpGet("GetCart")]
        public async Task<ActionResult<List<Item>>> GetCart(string token)
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

            var user = await _dataContext.Users.FirstAsync(x => x.Email.Equals(email));

            return Ok(user.cartItems);
        }

        [HttpPost("Add_To_Cart")]
        public async Task<ActionResult<List<Item>>> AddToCart(string itemName, string token)
        {
            var item = await _dataContext.Items.FirstAsync(x => x.Name.Equals(itemName) && x.Status == false);

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = tokenHandler.ReadJwtToken(token);

            var claims = jwtToken.Claims.ToList();

            DateTimeOffset timeOfsset = DateTimeOffset.FromUnixTimeSeconds(jwtToken.Payload.Exp.Value);

            DateTime time = timeOfsset.LocalDateTime;

            if (time.CompareTo(DateTime.Now) < 1)
            {
                return BadRequest("Token has expired");
            }

            item.Status = true;

            string email = claims[1].Value;

            var user = await _dataContext.Users.FirstAsync(x => x.Email.Equals(email));

            user.cartItems.Add(item);

            await _dataContext.SaveChangesAsync();

            return Ok(user.cartItems);
        }

        //[HttpPost("Remove_From_Cart")]
        //public async Task<ActionResult<List<Item>>> RemoveFromCart(int itemId, string userToken)
        //{
        //    var item = await _dataContext.Items.FindAsync(itemId);

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    var jwtToken = tokenHandler.ReadJwtToken(userToken);

        //    var claims = jwtToken.Claims.ToList();

        //    DateTimeOffset timeOfsset = DateTimeOffset.FromUnixTimeSeconds(jwtToken.Payload.Exp.Value);

        //    DateTime time = timeOfsset.LocalDateTime;

        //    if (time.CompareTo(DateTime.Now) < 1)
        //    {
        //        return BadRequest("Token has expired");
        //    }

        //    string email = claims[1].Value;

        //    var user = await _dataContext.Users.FirstAsync(x => x.Email.Equals(email));

        //    item.Status = false;

        //    user.cartItems.Remove(item);

        //    await _dataContext.SaveChangesAsync();

        //    return Ok();
        //}
    }

}
