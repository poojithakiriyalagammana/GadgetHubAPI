using GadgetHubAPI.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _service;
    public OrdersController(OrderService service) => _service = service;

    [HttpGet] public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());
    [HttpPost] public async Task<IActionResult> Create(Order order) => Ok(await _service.AddAsync(order));
}
