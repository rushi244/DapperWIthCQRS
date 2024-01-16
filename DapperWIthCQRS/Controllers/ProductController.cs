using DapperWIthCQRS.Application.Command;
using DapperWIthCQRS.Application.Queries;
using DapperWIthCQRS.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DapperWIthCQRS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _mediator.Send(new GetProductQueries());
            return Ok(products);
        }
        [HttpGet("{id}",Name ="GetProductById")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var products = await _mediator.Send(new GetProductByIdQueries(id));
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> AddProduct([FromBody] Product p)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var createdProduct = await _mediator.Send(new CreateProductCommand(p));
                return CreatedAtRoute("GetProductById", new { id = createdProduct.Id }, createdProduct);
                //return Ok(createdProduct);

            }catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error");
            }
           
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            var dbProduct =await _mediator.Send(new GetProductByIdQueries(id));
            if (dbProduct is null)
                return NotFound();

            await  _mediator.Send(new UpdateProductCommand(id, product));
            return Ok("Update Success...!");

        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var dbProduct = await _mediator.Send(new GetProductByIdQueries(id));
                if(dbProduct is null)
                return NotFound();

            await _mediator.Send(new DeleteProductCommand(id));

            return Ok("Deleted Success...!");
        }
    }
}
