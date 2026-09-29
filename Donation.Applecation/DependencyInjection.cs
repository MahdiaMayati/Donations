using Donation.Application.Abstractions.Services;
using Donation.Application.Common.Behaviors;
using Donation.Application.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Donation.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<ICityDependencyChecker, CityDependencyChecker>();
        services.AddScoped<IAreaDependencyChecker, AreaDependencyChecker>();
        services.AddScoped<IAddressDependencyChecker, AddressDependencyChecker>();
        services.AddScoped<IOrganizationDependencyChecker, OrganizationDependencyChecker>();

        return services;
    }
}
