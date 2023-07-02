using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Web_StoreAPI.Dtos;

namespace Web_StoreAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendorCompanyController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public VendorCompanyController(DataContext dataContext)
        {
            _dataContext=dataContext;
        }

        [HttpGet]
        public async Task<ActionResult<List<VendorCompany>>> Get()
        {
            return Ok(await _dataContext.VendorCompanys.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<List<VendorCompany>>> Get(int id)
        {
            var company = await _dataContext.VendorCompanys.FindAsync(id);
            if (company is null)
            {
                return BadRequest("No company found");
            }
            return Ok(company);
        }

        [HttpPost]
        public async Task<ActionResult<List<VendorCompany>>> Post(VendorCompanyDto newCompany)
        {

            var copmany = new VendorCompany()
            {
                Name = newCompany.Name,
                Email = newCompany.Email,
                Password = newCompany.Password,
                Adress = newCompany.Adress
            };

            _dataContext.VendorCompanys.Add(copmany);
            await _dataContext.SaveChangesAsync();

            return await Get(copmany.Id);
        }
    }
}
