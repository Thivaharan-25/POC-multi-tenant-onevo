using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OnevoHr.Api.Data;

using Microsoft.AspNetCore.TestHost;

namespace OnevoHr.Api.Tests.TestInfrastructure;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public string DatabaseName { get; } = Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            var descriptors = services.Where(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>) || 
                     d.ServiceType == typeof(DbContextOptions) ||
                     d.ServiceType == typeof(AppDbContext)).ToList();
            
            foreach (var d in descriptors)
            {
                services.Remove(d);
            }

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName);
            });
        });
    }
}
