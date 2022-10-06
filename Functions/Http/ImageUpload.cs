using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerSideProgrammingAssignment.Model;
using ServerSideProgrammingAssignment.Service;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using SixLabors.Fonts;
using System.Data.Common;

namespace ServerSideProgrammingAssignment.Functions.Http
{
    public static class ImageUpload
    {
        private static QueueClient apiqueue;
        [FunctionName("Upload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            string Connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");


            apiqueue = new(Connection, "color-queue", new QueueClientOptions()
            {
                MessageEncoding = QueueMessageEncoding.Base64,
            });



            Stream myBlob = new MemoryStream();
            var file = req.Form.Files["File"];
            myBlob = file.OpenReadStream();
            var blobClient = new BlobContainerClient(Connection, containerName);
            var blob = blobClient.GetBlobClient(file.FileName);

            try
            {
                await blob.UploadAsync(myBlob);

                apiqueue.SendMessage(file.FileName);

                return new OkObjectResult("file uploaded successfylly");

            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
