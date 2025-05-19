using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Keystone.Cli.Bootstrap;
using Keystone.Cli.Configuration;
using Microsoft.Extensions.Configuration;


var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configurationBuilder => configurationBuilder
        .AddJsonFile("appsettings.json", optional: false)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production"}.json", optional: true)
        .AddEnvironmentVariables()
    ).ConfigureServices((context, services) => services
        .AddSingleton(context.Configuration)
        .AddDependencies()
        .AddCommands()
        .AddSingleton<CliEntryPoint>()
    ).Build();

var cli = host.Services.GetRequiredService<CliEntryPoint>();
await cli.RunAsync(args);
