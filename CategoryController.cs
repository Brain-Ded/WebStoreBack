using Microsoft.AspNetCore.Mvc;

namespace Web_StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly DataContext _dataContext;
        public CategoryController(DataContext dataContext)
        {
            _dataContext=dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<Category>>> Get()
        {
            return Ok(await _dataContext.Categories.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> Get(int id)
        {
            var category = await _dataContext.Categories.FindAsync(id);
            if (category is null)
            {
                return BadRequest("No category found");
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<List<Category>>> Post()
        {
            return Ok(await _dataContext.Categories.ToListAsync());
        }
    }
}
