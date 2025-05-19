using Cocona;
using Keystone.Cli.Application;
using Keystone.Cli.Configuration;
using Microsoft.Extensions.Configuration;


var builder = CoconaApp.CreateBuilder();
var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

builder.Configuration.AddJsonFile("appsettings.json", optional: false);
builder.Configuration.AddJsonFile($"appsettings.{env}.json", optional: true);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddDependencies();

var app = builder.Build();
app.AddCommands<TemplateCommands>();

await app.RunAsync();
