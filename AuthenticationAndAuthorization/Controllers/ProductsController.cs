
using AuthenticationApp.Authorization;
using AuthenticationApp.Data;
using AuthenticationApp.Model;
using AuthenticationApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        public ProductsController(IProductService service) {
            _service = service;

        }

        [HttpGet]
        [Route("")]
        [CheckPermissionAttribute(Permission.ReadProducts)]
        public async Task<IActionResult> GetAll()
        {
            var result=await _service.GetAllProducts();
            return Ok(result);
        }
        [HttpGet]
        [Route("GetProductById")]//From Query String
        //[Authorize(Roles = "Admin,SuperUser")]//Role Based
        //[Authorize(Policy = "SuperUsersOnly")]//Policy Based
        //[Authorize(Policy = "EmployeesOnly")]//Policy Based
        //[Authorize(Policy = "AgeGreaterThan26")]//Custom Policy
        [Authorize(Policy = "AgeGreaterThan25")]//Custom Policy
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetProductById(id);
            if (result == null) { 
                return NotFound();
            }
           else return Ok(result);
        }
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> CreateProduct( Product product)
        {
            var res = await _service.AddProduct(product);
            return Ok(res);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            var res = await _service.EditProduct(product);
            return Ok(res);
        }
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var res = await _service.RemoveProduct(id);
            return Ok(res);
        }
    }
}
