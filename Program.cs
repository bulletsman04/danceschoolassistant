using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DancingSchoolAssistant;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>();

builder.Services.Configure<AzureConfig>(builder.Configuration);

var host = builder.Build();
var azureConfig = host.Services.GetRequiredService<IOptions<AzureConfig>>().Value;

await WithAzureOpenAi.Run(azureConfig);
// await WithSemanticKernel.Run(azureConfig);
