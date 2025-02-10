using Azure.AI.OpenAI.Chat;
using Azure.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.AzureOpenAI;

#pragma warning disable AOAI001
#pragma warning disable SKEXP0010

namespace DancingSchoolAssistant;

public static class WithSemanticKernel
{
    public static async Task Run(AzureConfig azureConfig)
    {
        var azureOpenAiPromptExecutionSettings = new AzureOpenAIPromptExecutionSettings
        {
            AzureChatDataSource = new AzureSearchChatDataSource
            {
                Endpoint = new Uri(azureConfig.AzureSearchEndpoint),
                IndexName = azureConfig.AzureSearchIndex,
                Authentication = DataSourceAuthentication.FromApiKey(azureConfig.AzureSearchKey),
            }
        };
        
        var kernel = Kernel
            .CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: azureConfig.AzureOAIDeploymentName,
                endpoint: azureConfig.AzureOAIEndpoint,
                new DefaultAzureCredential(),
                apiVersion: "V2024_06_01")
            .Build();

        var chatCompletion = kernel.Services.GetRequiredService<IChatCompletionService>();

        Console.WriteLine("Enter a question:");
        string userQuestion = Console.ReadLine() ?? "";

        var chatHistory = new ChatHistory(
            [
                new ChatMessageContent(AuthorRole.System, "You are a helpful assistant that helps students learn about dance school schedule"),
                new ChatMessageContent(AuthorRole.User, userQuestion)
            ]
        );
        
        var contentsAsync = chatCompletion.GetStreamingChatMessageContentsAsync(chatHistory, azureOpenAiPromptExecutionSettings);
        
        await foreach (var contentPart in contentsAsync)
        {
            Console.Write(contentPart);
        }

        Console.WriteLine("\nResponse complete.");
    }
}