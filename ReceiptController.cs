using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Web_StoreAPI.Dtos;
using Web_StoreAPI.DataModels;
using Web_StoreAPI.Controllers;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http.Features;

namespace Web_StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ReceiptController(DataContext dataContext)
        {
            _dataContext=dataContext;

            _dataContext.Users.Include("cartItems").Load();
        }

        [HttpGet("Get_All_Receipts")]
        public async Task<ActionResult<List<Receipt>>> Get()
        {
            return Ok(await _dataContext.Receipts.ToListAsync());
        }

        [HttpGet("Get_users_items")]
        public async Task<ActionResult<List<Item>>> Get(string token)
        {
            var receipts = await _dataContext.Receipts.ToListAsync();


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

            if(user == null)
            {
                return BadRequest("Token is not valid");
            }

            var userReceipts = receipts.Where(x => x.UserId == user.Id).ToList();

            List<Item> items = new List<Item>();

            List<Item> itemsDB = await _dataContext.Items.ToListAsync();

            foreach(Item item in itemsDB)
            {
                foreach(Receipt receipt in userReceipts)
                {
                    if(item.ReceiptId == receipt.Id)
                    {
                        items.Add(item);
                    }
                }
            }

            return Ok(items);
        }

        [HttpPost("Post_Receipt")]
        public async Task<ActionResult<Receipt>> Post(string token)
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

            if (user == null)
            {
                return BadRequest("Not found user");
            }

            double sum = 0;

            foreach(Item item in user.cartItems)
            {
                sum+= item.DiscountPrice;
            }

            var newReceipt = new Receipt()
            {
                Sum = sum,
                Date = DateTime.Now,
                ItemsAmount =  user.cartItems.Count,
                UserId = user.Id,
                User = user,
            };

            foreach(Item item in user.cartItems)
            {
                item.Receipt = newReceipt;
                item.ReceiptId = newReceipt.Id;
                item.Status = true;
            }

            _dataContext.Receipts.Add(newReceipt);
            user.cartItems = new List<Item>();
            await _dataContext.SaveChangesAsync();

            return newReceipt;
        }
        
    }
}
