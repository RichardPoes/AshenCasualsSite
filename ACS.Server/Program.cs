using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ACS.Server;

internal class Program
{
    private static void Main(string[] args)
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureAppConfiguration(ConfigureAppConfiguration)
            .ConfigureServices((hostBuilderContext, services) =>
                ConfigureServices(services, hostBuilderContext.Configuration))
            .Build();

        host.Run();
    }

    private static void ConfigureAppConfiguration(IConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("local.settings.json", optional: true)
            .AddEnvironmentVariables();
    }


    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {

    }
}