using GadgetHubAPI.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly CustomerService _service;
    public CustomersController(CustomerService service) => _service = service;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
        => (await _service.GetByIdAsync(id)) is Customer c ? Ok(c) : NotFound();
    [HttpPost] public async Task<IActionResult> Create(Customer customer) => Ok(await _service.AddAsync(customer));

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Customer customer)
    {
        if (id != customer.CustomerId)
            return BadRequest("ID mismatch");

        var updated = await _service.UpdateAsync(customer);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        return result ? Ok() : NotFound();
    }
}
