using GadgetHubAPI.Models;
using GadgetHubAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotationsController : ControllerBase
    {
        private readonly QuotationService _service;

        public QuotationsController(QuotationService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var results = await _service.GetByProductIdAsync(productId);
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Quotation quotation) => Ok(await _service.AddAsync(quotation));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? Ok() : NotFound();
        }
    }
}
