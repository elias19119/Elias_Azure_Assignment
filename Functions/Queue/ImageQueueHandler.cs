using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using ServerSideProgrammingAssignment.Service;

namespace ServerSideProgrammingAssignment.Functions.Queue
{
    public class ImageQueueHandler
    {
        private static BlobContainerClient _uploadImageClient;
        private static BlobContainerClient _editImageClient;
        private static string _connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private static string containerName = Environment.GetEnvironmentVariable("ContainerName");
        [FunctionName("ImageQueueHandler")]
        public async Task Run([QueueTrigger("image-queue", Connection = "AzureWebJobsStorage")]string uuid, ILogger log)
        {
            log.LogInformation($"Processing image of {uuid}");
            _uploadImageClient = new(_connection, containerName);
            _editImageClient = new(_connection, "edited-image-container");

            BlobClient uploadclient = _uploadImageClient.GetBlobClient(uuid);
            var response = await uploadclient.DownloadContentAsync();
            var downloadedImage =  response.Value.Content.ToArray();
            byte[] Image = downloadedImage;
            byte[] modifiedImage;
            try
            {
                modifiedImage =  ImageHelper.AddTextToImage(Image, ("processed ", (2, 2), 20, "0000FF", "FFC0CB"));
            }
            catch
            {
                return;
            }
            BlobClient editclient = _editImageClient.GetBlobClient(uuid);
            await editclient.UploadAsync(new MemoryStream(modifiedImage));
        }
    }
}
