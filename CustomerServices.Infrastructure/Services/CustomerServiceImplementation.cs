using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CustomerServices.Application.DTOs.Customers;
using CustomerServices.Application.Interfaces;
using CustomerServices.Domain.Entities;

namespace CustomerServices.Infrastructure.Services;

public class CustomerServiceImplementation : ICustomerService
{
    private readonly ICustomerRepository _repository;

    public CustomerServiceImplementation(ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CustomerResponse> CreateAsync(
        CreateCustomerRequest request)
    {
        var email = request.Email.Trim().ToLowerInvariant();

        if (await _repository.ExistsByEmailAsync(email))
        {
            throw new InvalidOperationException(
                "A customer with this email already exists.");
        }

        var customer = new Customer
        {
            AuthUserId = request.AuthUserId,
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = email,
            PhoneNumber = request.PhoneNumber.Trim(),
            Address = request.Address.Trim(),
            City = request.City.Trim(),
            State = request.State.Trim(),
            PostalCode = request.PostalCode.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _repository.AddAsync(customer);
        await _repository.SaveChangesAsync();

        return MapToResponse(customer);
    }

    public async Task<CustomerResponse?> GetByIdAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        return customer is null ? null : MapToResponse(customer);
    }

    public async Task<List<CustomerResponse>> GetAllAsync()
    {
        var customers = await _repository.GetAllAsync();

        return customers.Select(MapToResponse).ToList();
    }

    public async Task<CustomerResponse?> UpdateAsync(
        Guid id,
        UpdateCustomerRequest request)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer is null)
            return null;

        customer.FirstName = request.FirstName.Trim();
        customer.LastName = request.LastName.Trim();
        customer.PhoneNumber = request.PhoneNumber.Trim();
        customer.Address = request.Address.Trim();
        customer.City = request.City.Trim();
        customer.State = request.State.Trim();
        customer.PostalCode = request.PostalCode.Trim();
        customer.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(customer);
        await _repository.SaveChangesAsync();

        return MapToResponse(customer);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer is null)
            return false;

        await _repository.DeleteAsync(id);

        return true;
    }

    private static CustomerResponse MapToResponse(Customer customer)
    {
        return new CustomerResponse
        {
            Id = customer.Id,
            AuthUserId = customer.AuthUserId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            City = customer.City,
            State = customer.State,
            PostalCode = customer.PostalCode,
            CreatedAt = customer.CreatedAt,
            IsActive = customer.IsActive
        };
    }
}

