using System.ClientModel;
using Azure.AI.OpenAI;
using Azure.AI.OpenAI.Chat;
using Azure.Identity;
using OpenAI.Chat;
#pragma warning disable AOAI001

namespace DancingSchoolAssistant;

public static class WithAzureOpenAi
{
    public static async Task Run(AzureConfig azureConfig)
    {
        const bool showCitations = false;
        
        AzureOpenAIClient client = new(
            new Uri(azureConfig.AzureOAIEndpoint),
            new DefaultAzureCredential(true));

        var chatClient = client.GetChatClient(azureConfig.AzureOAIDeploymentName);

        var chatCompletionsOptions = new ChatCompletionOptions
        {
            MaxOutputTokenCount = 600,
            Temperature = 0.9f
        };
        
        chatCompletionsOptions.AddDataSource(new AzureSearchChatDataSource
        {
            Endpoint = new Uri(azureConfig.AzureSearchEndpoint),
            IndexName = azureConfig.AzureSearchIndex,
            Authentication = DataSourceAuthentication.FromApiKey(azureConfig.AzureSearchKey),
        });

        Console.WriteLine("Enter a question:");
        string text = Console.ReadLine() ?? "";

        Console.WriteLine("...Sending the following request to Azure OpenAI endpoint...");
        Console.WriteLine("Request: " + text + "\n");

        AsyncCollectionResult<StreamingChatCompletionUpdate>? completionUpdates = chatClient.CompleteChatStreamingAsync(
        [
            new SystemChatMessage("You are a helpful assistant that helps students learn about dance school schedule."),
            new UserChatMessage(text)
        ], chatCompletionsOptions);

        await foreach (var completionUpdate in completionUpdates)
        {
            foreach (var contentPart in completionUpdate.ContentUpdate)
            {
                Console.Write(contentPart.Text);
            }
        }

// Chat Completion without streaming

// ChatCompletion response = await chatClient.CompleteChatAsync([
//
// ], chatCompletionsOptions);
// string responseMessage = response.Content[0].Text;
//
// // Print response
// Console.WriteLine("Response: " + responseMessage + "\n");
//
// var onYourDataContext = response.GetMessageContext();
//
// if (onYourDataContext?.Intent is not null)
// {
//     Console.WriteLine($"Intent: {onYourDataContext.Intent}");
// }
//
// if (showCitations)
// {
//     foreach (var citation in onYourDataContext?.Citations ?? [])
//     {
//         Console.WriteLine($"Citation: {citation.Content}");
//     }
// }
    }
}