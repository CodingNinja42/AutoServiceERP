using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using CustomerServices.Domain.Entities;

namespace CustomerServices.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id);

    Task<Customer?> GetByEmailAsync(string email);

    Task<List<Customer>> GetAllAsync();

    Task<bool> ExistsByEmailAsync(string email);

    Task AddAsync(Customer customer);

    Task UpdateAsync(Customer customer);

    Task SaveChangesAsync();
}

