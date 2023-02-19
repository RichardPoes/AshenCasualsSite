using ACS.Client.Security;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace ACS.Client;
public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        await AddLocalSettings(builder);
        var baseApiUrl = builder.Configuration["WebApiUrl"] ?? builder.HostEnvironment.BaseAddress;
        var apiUrl = $"{baseApiUrl}/api/";

        builder.Services.AddHttpClient("ACS.ServerAPI", client => client.BaseAddress = new Uri(apiUrl))
            .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

        // Supply HttpClient instances that include access tokens when making requests to the server project
        builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("ACS.ServerAPI"));

        builder.Services.AddMsalAuthentication(options =>
        {
            builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
            options.ProviderOptions.DefaultAccessTokenScopes.Add(builder.Configuration["ServerApi:Scopes"]);
            options.ProviderOptions.LoginMode = "redirect";
        });

        builder.Services.AddApiAuthorization().AddAccountClaimsPrincipalFactory<SplitRoleUserFactory>();

        await builder.Build().RunAsync();
    }

    private static async Task AddLocalSettings(WebAssemblyHostBuilder builder)
    {
        var http = new HttpClient
        {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
        };

        using var response = await http.GetAsync("local.settings.json");

        if (response?.IsSuccessStatusCode == true)
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            builder.Configuration.AddJsonStream(stream);
        }
    }
}
