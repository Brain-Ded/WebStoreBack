using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Web_StoreAPI.DataModels;
using Web_StoreAPI.Dtos;
using System.Numerics;

namespace Web_StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public ItemController(DataContext dataContext)
        {
            _dataContext=dataContext;
        }     

        [HttpGet]
        public async Task<ActionResult<List<Item>>> Get()
        {
            var items = await _dataContext.Items.ToListAsync();

            var filtered = items.DistinctBy(x => x.Name);

            return Ok(filtered);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<Item>>> Get(int id)
        {
            var item = await _dataContext.Items.FindAsync(id);
            if (item is null)
            {
                return BadRequest("No item found");
            }
            item.Views += 1;
            return Ok(item);
        }

        [HttpGet("Get_By_Name")]
        public async Task<ActionResult<List<Item>>> GetByName(string search)
        {
            var items = await _dataContext.Items.ToListAsync();

            string lSearch = search.ToLower();

            if (items is null)
            {
                return Ok(new List<Item>());
            }

            List<Item> filteredItems = new List<Item>();

            foreach(Item item in items)
            {
                if (item.Name.ToLower().Contains(lSearch))
                {
                    filteredItems.Add(item);
                }
            }

            return Ok(filteredItems.DistinctBy(x => x.Name));
        }

        [HttpGet("Get_By_Category")]
        public async Task<ActionResult<List<Item>>> GetByCategory(string categoryName)
        {
            var category = await _dataContext.Categories.FirstAsync(x=> x.Name.Equals(categoryName));

            var items = await _dataContext.Items.ToListAsync();

            var filteredItems = items.Where(x => x.CategoryId == category.Id);

            return Ok(filteredItems.DistinctBy(x => x.Name));

        }

        [HttpPost]
        public async Task<ActionResult<List<Item>>> Post(ItemDto newItem)
        {
            var category = await _dataContext.Categories.FindAsync(newItem.CategoryId);
            var vendor = await _dataContext.VendorCompanys.FindAsync(newItem.VendorCompanyId);

            if (category == null || vendor == null)
            {
                return NotFound();
            }

            for (int i = 0; i<newItem.Amount; ++i)
            {
                _dataContext.Items.Add(new Item()
                {
                    Name = newItem.Name,
                    ImageURL = newItem.ImageURL,
                    ActualPrice = newItem.ActualPrice,
                    DiscountPrice = newItem.DiscountPrice,
                    Description = newItem.Description,
                    Status = false,
                    Tags = newItem.Tags,
                    CategoryId = newItem.CategoryId,
                    Category = category,
                    VendorCompany = vendor,
                    VendorCompanyId = newItem.VendorCompanyId
                });


                await _dataContext.SaveChangesAsync();
                await Task.Delay(100);
            }

            return await Get(_dataContext.Items.Max(x=>x.Id));
        }

        [HttpPut("UpdateAdvertise")]
        public async Task<ActionResult<List<Item>>> UpdateAdvertise(string itemName, int adLevel)
        {
            if (adLevel < 0 || adLevel > 9)
            {
                return BadRequest("Incorrect level of advertising(Must be between 0 and 9)");
            }
                var item = await _dataContext.Items.FirstAsync(x => x.Name.Equals(itemName));
            if(item == null)
            {
                return NotFound("Item not found");
            }
            
            item.Advertise = adLevel;

            await _dataContext.SaveChangesAsync();

            return Ok(item);
        }

        [HttpPut("AddItems")]
        public async Task<ActionResult<List<Item>>> AddItems(string itemName, int amount)
        {
            var item = await _dataContext.Items.FirstAsync(x => x.Name.Equals(itemName));
            if (item == null)
            {
                return NotFound("Item not found");
            }

            List<Item> items = new List<Item>();

            for(int i=0; i<amount; ++i)
            {
                _dataContext.Items.Add(new Item()
                {
                    Name = item.Name,
                    ImageURL = item.ImageURL,
                    ActualPrice = item.ActualPrice,
                    DiscountPrice = item.DiscountPrice,
                    Description = item.Description,
                    Status = false,
                    Tags = item.Tags,
                    CategoryId = item.CategoryId,
                    VendorCompanyId = item.VendorCompanyId
                });

                await _dataContext.SaveChangesAsync();
                await Task.Delay(100);

            }               

            return Ok(item);
        }


        [HttpPut("RemoveItems")]
        public async Task<ActionResult<List<Item>>> TakeItems(string itemName, int amount)
        {
            var items = await _dataContext.Items.ToListAsync();

            var item = items.Where(x => x.Name.Equals(itemName)).ToList().MaxBy(x=> x.Id);

            if (item == null)
            {
                return NotFound("Item not found");
            }

            _dataContext.Items.Remove(item);

            await _dataContext.SaveChangesAsync();

            return Ok(item);
        }
    }
}
