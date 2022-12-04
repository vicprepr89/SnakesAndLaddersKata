using Microsoft.Extensions.DependencyInjection;
using SnakesAndLadders.Application.Dto;
using SnakesAndLadders.Application.Interfaces;
using SnakesAndLadders.Application.Services;
using SnakesAndLadders.Data.DataContext;
using SnakesAndLadders.Data.Interfaces;

namespace SnakesAndLadders.Application.Startup
{
    public static class SnakesAndLaddersServiceCollectionExtensions
    {
        public static void AddSnakesAndLadders(this IServiceCollection services)
        {
            // Register services.          
            _ = services.AddSingleton<ISnakesAndLaddersDataSeed, SnakesAndLaddersDataSeed>();
            _ = services.AddSingleton<SnakesAndLaddersDataContext>();
            _ = services.AddScoped<IUsersService, UsersService>();
            _ = services.AddScoped<IDiceRollService, DiceRollService>();

            // Register & configure automapper.
            _ = services.AddAutoMapper(typeof(DtoMappingProfile));
        }
    }
}