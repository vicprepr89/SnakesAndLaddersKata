namespace SnakesAndLadders.UI
{
    using System.Linq;
    using Microsoft.Extensions.Configuration;
    using SnakesAndLadders.Application.Dto;
    using SnakesAndLadders.UI.Helpers;
    using SnakesAndLadders.UI.Services;
    using SnakesAndLadders.WebApi.Helpers;

    internal static class Program
    {
        private const string ConfigFilename = "config.json";

        private const string TitleKey = "Title";

        private const string BackendConfigfKey = "BackendConfig";

        private static async Task Main()
        {
            try
            {
                IConfigurationRoot config = LoadAppConfigFromJsonFile();

                SetAppTitle(config);

                BackendConfig? backendConfig = GetBackendConfig(config);

                if (backendConfig is null)
                {
                    Console.WriteLine($"ERROR: The Backend configuration is missing.");
                    _ = Console.ReadLine();

                    return;
                }

                HttpClientService httpClientService = new(backendConfig!);

                UserDto[] users = await httpClientService.GetUsersAsync();

                Console.WriteLine(@$"Welcome to Snakes And Ladders!
Number of players: {users.Length} ({string.Join(", ", users.Select(u => $"Player {u.Id}"))})");

                Console.WriteLine("Randomly determining the first user...");

                int currentPlayerId = await httpClientService.GenerateFirstUserIdAsync();

                Console.WriteLine($"First user will be Player {currentPlayerId}");

                Console.WriteLine("Starting game...");

                int currentPlayerIndex = GetCurrentPlayerIndex(users, currentPlayerId);

                UserDto currentPlayer;
                int currentPlayerPosition;
                bool currentPlayerHasWon;

                do
                {
                    if (currentPlayerIndex >= users.Length)
                    {
                        currentPlayerIndex = 0;
                    }

                    currentPlayer = users[currentPlayerIndex];

                    currentPlayerPosition = await httpClientService.GetUserPosition(currentPlayer.Id);

                    Console.WriteLine();
                    Console.WriteLine($"Player {currentPlayer.Id} - Current Position: {currentPlayerPosition}");
                    Console.Write(">>> Press any key to roll the dice! <<<");
                    _ = Console.ReadLine();

                    MoveUserPositionResult moveUserPositionResult = await httpClientService.MoveUserPosition(currentPlayer.Id);

                    Console.WriteLine($"Player {currentPlayer.Id} has rolled the dice and has rolled a {moveUserPositionResult.DiceRollValue}");

                    currentPlayerPosition = moveUserPositionResult.NewUserPosition;

                    Console.WriteLine($"Player {currentPlayer.Id} - New Position: {currentPlayerPosition}");

                    currentPlayerHasWon = await httpClientService.UserHasWonAsync(currentPlayer.Id);

                    currentPlayerIndex++;
                }
                while (!currentPlayerHasWon);

                Console.WriteLine($"*** Player {currentPlayer.Id} has won! ***");
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

        private static IConfigurationRoot LoadAppConfigFromJsonFile()
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(ConfigFilename, optional: false);

            return configBuilder.Build();
        }

        private static void SetAppTitle(IConfigurationRoot config)
        {
            Console.Title = config[TitleKey] ?? string.Empty;
        }

        private static BackendConfig? GetBackendConfig(IConfigurationRoot config)
        {
            return config.GetSection(BackendConfigfKey).Get<BackendConfig>();
        }
        private static int GetCurrentPlayerIndex(UserDto[] users, int currentPlayerId)
        {
            for (int i = 0; i < users.Length; i++)
            {
                if (users[i].Id == currentPlayerId)
                {
                    return i;
                }
            }

            throw new InvalidOperationException($"The user with the identifier {currentPlayerId} does not exist in the specified collection.");
        }
    }
}