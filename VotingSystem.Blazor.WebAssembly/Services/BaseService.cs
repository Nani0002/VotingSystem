namespace VotingSystem.Blazor.WebAssembly.Services
{
    public class BaseService
    {
        protected async Task HandleError(HttpResponseMessage response)
        {
            await Console.Out.WriteLineAsync(await response.Content.ReadAsStringAsync());
        }
    }
}
