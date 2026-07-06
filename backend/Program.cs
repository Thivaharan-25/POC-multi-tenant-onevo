using Microsoft.EntityFrameworkCore;
using OnevoHr.Api.Data;
using OnevoHr.Api.Data.Seed;
using OnevoHr.Api.Middleware;
using OnevoHr.Api.Repositories.Implementations;
using OnevoHr.Api.Repositories.Interfaces;
using OnevoHr.Api.Services.Implementations;
using OnevoHr.Api.Services.Interfaces;
using OnevoHr.Api.Services.Notifications;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("backend.Tests")]

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRlsBypassContext, RlsBypassContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

builder.Services.AddScoped<TenantRlsInterceptor>();
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
               .AddInterceptors(serviceProvider.GetRequiredService<TenantRlsInterceptor>()));
}

// Repositories
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IModuleCatalogRepository, ModuleCatalogRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IDemoProfileRepository, DemoProfileRepository>();
builder.Services.AddScoped<IDemoRequestRepository, DemoRequestRepository>();
builder.Services.AddScoped<ITemplateRepository, TemplateRepository>();
builder.Services.AddScoped<IOrgRepository, OrgRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
// builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddScoped<IPlatformUserRepository, PlatformUserRepository>();

// Services
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<ITenantContextService, TenantContextService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPlatformAuthService, PlatformAuthService>();
builder.Services.AddScoped<IDemoRequestService, DemoRequestService>();
builder.Services.AddScoped<IDemoApprovalService, DemoApprovalService>();
builder.Services.AddScoped<IDemoUpgradeService, DemoUpgradeService>();
builder.Services.AddScoped<IDemoProfileService, DemoProfileService>();
builder.Services.AddScoped<ITenantProvisioningService, TenantProvisioningService>();
builder.Services.AddScoped<ITenantActivationService, TenantActivationService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
builder.Services.AddScoped<IModuleCatalogService, ModuleCatalogService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IFeatureGateService, FeatureGateService>();
builder.Services.AddScoped<IScopeResolverService, ScopeResolverService>();
builder.Services.AddScoped<ITemplateApplicationService, TemplateApplicationService>();
builder.Services.AddScoped<IOrgStructureService, OrgStructureService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
// builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IOutboxService, OutboxService>();
builder.Services.AddHostedService<OutboxPublisherService>();

var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    var rlsBypass = scope.ServiceProvider.GetRequiredService<IRlsBypassContext>();
    if (db.Database.CanConnect() && !db.Database.GetPendingMigrations().Any())
    {
        using (rlsBypass.BeginTrustedRlsBypass())
        {
            await DatabaseSeeder.SeedAsync(db, passwordHasher);
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("CorsPolicy");

app.UseMiddleware<TenantResolutionMiddleware>();
app.UseMiddleware<CurrentUserMiddleware>();
app.UseMiddleware<CsrfMiddleware>();
app.UseMiddleware<PermissionMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
