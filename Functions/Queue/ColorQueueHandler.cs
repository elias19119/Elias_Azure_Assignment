using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ServerSideProgrammingAssignment.Model;
using ServerSideProgrammingAssignment.Service;

namespace ServerSideProgrammingAssignment.Functions.Queue
{
    public class ColorQueueHandler
    {
        private static BlobContainerClient _uploadImageClient;
        private static QueueClient textque;

        private static string _connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private static string containerName = Environment.GetEnvironmentVariable("ContainerName");

        [FunctionName("ColorQueueHandler")]
        public async static Task Run([QueueTrigger("color-queue", Connection = "AzureWebJobsStorage")] string uuid, ILogger log)
        {
            log.LogInformation($"Processing colors of {uuid}");

            _uploadImageClient = new(_connection, containerName);
            textque = new(_connection, "text-queue", new QueueClientOptions()
            {
                MessageEncoding = QueueMessageEncoding.Base64,
            });



            BlobClient uploadclient = _uploadImageClient.GetBlobClient(uuid);
            var response = await uploadclient.DownloadContentAsync();
            var res = response.Value.Content.ToArray(); 

            byte[] data = res;
            (byte[], string[]) result;

            try
            {
                result = await Task.Run(() => ImageHelper.EditImage(data));
            } 
            catch {
                return; 
            }

            var ColorHex1 = result.Item2[0];
            var ColorHex2 = result.Item2[1];
            var ColorHex3 = result.Item2[2];

            HttpClient _httpClient = new();

            var ColorName1 = await GetColorName(ColorHex1, _httpClient);
            var ColorName2 = await GetColorName(ColorHex2, _httpClient);
            var ColorName3 = await GetColorName(ColorHex3, _httpClient);
            log.LogInformation($"first color of  image  {uuid}  is {ColorName1}");
            log.LogInformation($"second color of image  {uuid}  is {ColorName2} ");
            log.LogInformation($"third  color of image  {uuid}  is {ColorName3}");


            await textque.SendMessageAsync(uuid);


        }

        private async static Task<string> GetColorName(string hexColor, HttpClient client)
        {
            var response = await client.GetAsync($"https://www.thecolorapi.com/id?hex={hexColor}");
            string content = await response.Content.ReadAsStringAsync();
            ColorApi color = JsonConvert.DeserializeObject<ColorApi>(content);
            return color.Name.Value;
        }
    }
}
