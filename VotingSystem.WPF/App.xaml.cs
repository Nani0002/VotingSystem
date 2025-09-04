using System.Configuration;
using System.Data;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VotingSystem.WPF.Infrastructure;
using VotingSystem.WPF.Services;
using VotingSystem.WPF.View;
using VotingSystem.WPF.ViewModel;
using VotingSystem.WPF.Services.HubServices;

namespace VotingSystem.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IHost? AppHost { get; private set; }
        public static IConfiguration? Configuration { get; private set; }

        public App()
        {
            AppHost = Host.CreateDefaultBuilder().ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                Configuration = context.Configuration;

                var apiBaseUrl = Configuration["ApiBaseUrl"]
                                 ?? throw new InvalidOperationException("Missing ApiBaseUrl");

                services.AddTransient<AuthenticatedHttpClientHandler>();

                services.AddHttpClient("AuthenticatedClient")
                    .ConfigureHttpClient(client =>
                    {
                        client.BaseAddress = new Uri(apiBaseUrl);
                    })
                    .AddHttpMessageHandler<AuthenticatedHttpClientHandler>();

                services.AddSingleton(sp =>
                {
                    var factory = sp.GetRequiredService<IHttpClientFactory>();
                    return factory.CreateClient("AuthenticatedClient");
                });

                services.AddSingleton<IAuthenticationService, AuthenticationService>();
                services.AddSingleton<ITokenStoreService, TokenStoreService>();
                services.AddSingleton<IHttpRequestUtility, HttpRequestUtility>();
                services.AddSingleton<ITopicsService, TopicsService>();
                services.AddSingleton<ITopicHubService, TopicHubService>();
                services.AddSingleton<NavigationService>();

                services.AddSingleton<JsonSerializerOptions>(_ =>
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        NumberHandling = JsonNumberHandling.AllowReadingFromString
                    };
                    options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                    return options;
                });

                services.AddTransient<LoginPage>();
                services.AddTransient<RegisterPage>();
                services.AddTransient<TopicsPage>();
                services.AddTransient<TopicShowPage>();

                services.AddTransient<LoginViewModel>();
                services.AddTransient<RegisterViewModel>();
                services.AddTransient<TopicsListViewModel>();
                services.AddTransient<TopicShowViewModel>();

                services.AddTransient<MainWindow>();

                services.AddAutoMapper(typeof(WPFMappingProfile));
            }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            await AppHost!.StartAsync();

            var mainWindow = AppHost.Services.GetRequiredService<MainWindow>();
            mainWindow.Show();

            base.OnStartup(e);
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            await AppHost!.StopAsync();
            base.OnExit(e);
        }
    }
}
