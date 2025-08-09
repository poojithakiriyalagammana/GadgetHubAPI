using GadgetHubAPI.Models;
using GadgetHubAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GadgetHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DistributorsController : ControllerBase
    {
        private readonly DistributorService _service;

        public DistributorsController(DistributorService service)
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

        [HttpPost]
        public async Task<IActionResult> Create(Distributor distributor) => Ok(await _service.AddAsync(distributor));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            return result ? Ok() : NotFound();
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Distributor distributor)
        {
            if (id != distributor.DistributorId)
                return BadRequest("ID mismatch");

            var updated = await _service.UpdateAsync(distributor);
            return updated == null ? NotFound() : Ok(updated);
        }
    }
}
