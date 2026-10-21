
using CustomerServices.Application.DTOs.Customers;
using CustomerServices.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CustomerService.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomersController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request)
    {
        try
        {
            var customer = await _customerService.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = customer.Id },
                customer);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);

        if (customer is null)
            return NotFound(new { message = "Customer not found." });

        return Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateCustomerRequest request)
    {
        var customer = await _customerService.UpdateAsync(id, request);

        if (customer is null)
            return NotFound(new { message = "Customer not found." });

        return Ok(customer);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _customerService.DeleteAsync(id);

        if (!deleted)
            return NotFound(new { message = "Customer not found." });

        return NoContent();
    }
}
