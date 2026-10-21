using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AuthServices.Infrastructure.Data;

public class AuthDbContextFactory
    : IDesignTimeDbContextFactory<AuthDbContext>
{
    public AuthDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AuthDbContext>();

        options.UseSqlServer(
            @"Server=.\SQLEXPRESS;Database=AutoCareAuthDB;Trusted_Connection=True;TrustServerCertificate=True;");

        return new AuthDbContext(options.Options);
    }
}
