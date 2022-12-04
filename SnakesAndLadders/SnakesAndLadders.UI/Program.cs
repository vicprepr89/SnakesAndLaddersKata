using Microsoft.Extensions.Configuration;
using SnakesAndLadders.Application.Dto;
using SnakesAndLadders.UI.Helpers;
using SnakesAndLadders.UI.Services;

namespace SnakesAndLadders.UI
{
    internal class Program
    {
        private const string ConfigFilename = "config.json";

        private const string TitleKey = "Title";

        private const string BackendConfigfKey = "BackendConfig";

        private static async Task Main()
        {
            try
            {
                // Load the app configuration from a JSON file.
                IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(ConfigFilename, optional: false);

                IConfiguration config = configBuilder.Build();

                Console.Title = config[TitleKey] ?? string.Empty;

                BackendConfig? backendConfig = config.GetSection(BackendConfigfKey).Get<BackendConfig>();

                if (backendConfig is null)
                {
                    Console.WriteLine($"ERROR: The Backend configuration is missing.");
                    _ = Console.ReadLine();

                    return;
                }

                // Initialize the client to call the backend.
                HttpClientService httpClientService = new(backendConfig!);

                IEnumerable<UserDto> users = await httpClientService.GetUsersAsync();

                Console.WriteLine(@$"Welcome to Snakes And Ladders!
Number of players: {users.Count()} ({string.Join(", ", users.Select(u => $"Player {u.Id}"))})");

                Console.WriteLine("Starting game...");

                int currentPlayerId = -1;
                int currentPlayerPosition;
                bool currentPlayerHasWon;

                do
                {
                    currentPlayerId++;

                    if (currentPlayerId >= users.Count())
                    {
                        currentPlayerId = 0;
                    }

                    currentPlayerPosition = await httpClientService.GetUserPosition(currentPlayerId);

                    Console.WriteLine();
                    Console.WriteLine($"Player {currentPlayerId} - Current Position: {currentPlayerPosition}");
                    Console.Write(">>> Press any key to roll the dice! <<<");
                    _ = Console.ReadLine();

                    currentPlayerPosition = await httpClientService.MoveUserPosition(currentPlayerId);

                    Console.WriteLine($"Player {currentPlayerId} - New Position: {currentPlayerPosition}");

                    currentPlayerHasWon = await httpClientService.UserHasWonAsync(currentPlayerId);
                }
                while (!currentPlayerHasWon);

                Console.WriteLine($"*** Player {currentPlayerId} has won! ***");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
            }
            finally
            {
                _ = Console.ReadLine();
            }
        }
    }
}