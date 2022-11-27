using LastKey_Application.Services;
using LastKey_Domain.Interfaces;
using LastKey_Infrastructure.Repositories;

namespace LastKey_Web.Extensions;

public static class ApplicationServicesExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        
        services.AddScoped<IUserService, UserService>();
        
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        return services;
    }
}