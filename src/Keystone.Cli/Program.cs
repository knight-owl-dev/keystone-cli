using Cocona;
using Keystone.Cli.Configuration;
using Keystone.Cli.Presentation;
using Microsoft.Extensions.Configuration;


var builder = CoconaApp.CreateBuilder();
var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, "appsettings.json"), optional: false);
builder.Configuration.AddJsonFile(Path.Combine(AppContext.BaseDirectory, $"appsettings.{env}.json"), optional: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDependencies();

var app = builder.Build();

app.AddCommands<BrowseCommandController>();
app.AddCommands<InfoCommandController>();
app.AddCommands<NewCommandController>();

await app.RunAsync();
