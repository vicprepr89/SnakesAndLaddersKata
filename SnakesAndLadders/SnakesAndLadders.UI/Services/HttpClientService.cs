namespace SnakesAndLadders.UI.Services
{
    using System.Net;
    using Newtonsoft.Json;
    using Polly;
    using SnakesAndLadders.Application.Dto;
    using SnakesAndLadders.UI.Helpers;
    using SnakesAndLadders.WebApi.Helpers;

    public class HttpClientService
    {
        private const string AcceptHeaderKey = "Accept";

        private const string JsonMediaTypeHeaderValue = "application/json";

        private const string UsersEndpoint = "users";

        private const int RetryCount = 3;

        private readonly BackendConfig _backendConfig;

        public HttpClientService(BackendConfig backendConfig)
        {
            _backendConfig = backendConfig;
        }

        public async Task<UserDto[]> GetUsersAsync()
        {
            IEnumerable<UserDto>? users = await CallBackendAsync<IEnumerable<UserDto>>(HttpMethod.Get, UsersEndpoint);

            return users?.ToArray() ?? Array.Empty<UserDto>();
        }

        public async Task<int> GenerateFirstUserIdAsync()
        {
            int firstUserId = await CallBackendAsync<int>(HttpMethod.Get, $"{UsersEndpoint}/generateFirst");

            return firstUserId;
        }

        public async Task<int> GetUserPosition(int id)
        {
            int userPosition = await CallBackendAsync<int>(HttpMethod.Get, $"{UsersEndpoint}/{id}/position");

            return userPosition;
        }

        /// <summary>
        /// Moves the user position accord to generated dice roll and returns the new user position.
        /// </summary>
        /// <param name="id">The user identifier.</param>
        /// <returns>The dice roll value and the new user position.</returns>
        public async Task<MoveUserPositionResult> MoveUserPosition(int id)
        {
            MoveUserPositionResult? result = await CallBackendAsync<MoveUserPositionResult>(HttpMethod.Post, $"{UsersEndpoint}/{id}/move");

            return result ?? throw new Exception($"The positon of the user '{id}' could not be moved.");
        }

        public async Task<bool> UserHasWonAsync(int id)
        {
            bool userHasWon = await CallBackendAsync<bool>(HttpMethod.Get, $"{UsersEndpoint}/{id}/hasWon");

            return userHasWon;
        }

        private async Task<T?> CallBackendAsync<T>(HttpMethod httpMethod, string requestUri)
        {
            HttpClient client = new() { BaseAddress = new Uri(_backendConfig.Url) };

            using HttpRequestMessage request = new(httpMethod, requestUri);

            request.Headers.Add(AcceptHeaderKey, JsonMediaTypeHeaderValue);

            HttpResponseMessage httpResponseMessage = await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => client.SendAsync(request));

            string responseContent = await httpResponseMessage.Content.ReadAsStringAsync();

            return httpResponseMessage.StatusCode switch
            {
                HttpStatusCode.OK => JsonConvert.DeserializeObject<T>(responseContent),
                HttpStatusCode.NotFound => throw new Exception(responseContent),
                HttpStatusCode.InternalServerError => throw new Exception(responseContent),
                _ => default
            };
        }
    }
}