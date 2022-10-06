using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerSideProgrammingAssignment.Service;
using Microsoft.AspNetCore.Routing;
using Azure.Storage.Blobs;

namespace ServerSideProgrammingAssignment.Functions.Http
{
    public static class ImageDownload
    {
        private static BlobContainerClient _client;
        private static string _connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private static string containerName = Environment.GetEnvironmentVariable("ContainerName");
        [FunctionName("Download")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "Download/{id}")] HttpRequest req,
            ILogger log, string id)
        {
            _client = new(_connection, containerName);
            BlobClient client = _client.GetBlobClient(id);
            var response = await client.DownloadContentAsync();
            var result = response.Value.Content.ToArray();
            byte[] image = result;

            try
            {
                return new FileContentResult(result, "image/png");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
