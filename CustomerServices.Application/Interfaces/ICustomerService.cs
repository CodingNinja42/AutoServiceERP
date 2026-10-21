using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CustomerServices.Application.DTOs.Customers;

namespace CustomerServices.Application.Interfaces;

public interface ICustomerService
{
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);

    Task<CustomerResponse?> GetByIdAsync(Guid id);

    Task<List<CustomerResponse>> GetAllAsync();

    Task<CustomerResponse?> UpdateAsync(
        Guid id,
        UpdateCustomerRequest request);

    Task<bool> DeleteAsync(Guid id);
}
