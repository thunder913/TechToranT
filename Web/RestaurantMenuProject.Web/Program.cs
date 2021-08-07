namespace RestaurantMenuProject.Web
{
    using Azure.Core;
    using Azure.Extensions.AspNetCore.Configuration.Secrets;
    using Azure.Identity;
    using Azure.Security.KeyVault.Secrets;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Azure.KeyVault;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Hosting;
    using System;

    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseStartup<Startup>();
                    })
            .ConfigureAppConfiguration((context, config) => 
            {
                var buildConfiguration = config.Build();

                string kvURL = buildConfiguration["KeyVaultConfig:KVUrl"];
                string tenantId = buildConfiguration["KeyVaultConfig:TenantId"];
                string clientId = buildConfiguration["KeyVaultConfig:ClientId"];
                string clientSecret = buildConfiguration["KeyVaultConfig:ClientSecretId"];

                var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                var client = new SecretClient(new Uri(kvURL), credential);
                config.AddAzureKeyVault(client, new AzureKeyVaultConfigurationOptions());
            });
    }
}
