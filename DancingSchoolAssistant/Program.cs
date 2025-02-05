using DancingSchoolAssistant;
using Microsoft.Extensions.Configuration;

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddUserSecrets<Program>()
    .Build();

var azureConfig = new AzureConfig();
config.Bind(azureConfig);

await WithAzureOpenAi.Run(azureConfig);
// await WithSemanticKernel.Run(azureConfig);