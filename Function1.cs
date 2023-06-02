using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.Services.AppAuthentication;
using System.Net.Http;

namespace acr_access_func
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            
            // ...
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            string accessToken = await azureServiceTokenProvider.GetAccessTokenAsync("https://management.azure.com/");
            
            // OR

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Put, "https://management.azure.com/subscriptions/<YOUR SUBSCRIPTION ID>/resourceGroups/<YOUR RESOURCE GROUP>/providers/Microsoft.ContainerRegistry/registries/<YOUR REGISTRY NAME>/tokens/<YOUR NEW TOKEN NAME>?api-version=2023-01-01-preview");
            var token = "Bearer " + accessToken;
            request.Headers.Add("Authorization", token);
            var content = new StringContent("{\r\n  \"properties\": {\r\n    \"scopeMapId\": \"<YOUR SCOPE ID FROM - az acr scope-map list -r MYREGISTRY\",\r\n    \"status\": \"enabled\",\r\n    \"credentials\": {\r\n      \"passwords\": {\r\n        \"expiry\": \"5/24/2023 6:00PM\",\r\n        \"value\": \"testcredentials47052\",\r\n        \"name\": {\r\n            \"password1\": \"password1name\",\r\n            \"password2\": \"password2name\"\r\n        }\r\n    }\r\n    }\r\n  }\r\n}\r\n", null, "application/json");
            request.Content = content;
           
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            log.LogInformation(await response.Content.ReadAsStringAsync());


            return new OkObjectResult(response);
        }
    }
}
