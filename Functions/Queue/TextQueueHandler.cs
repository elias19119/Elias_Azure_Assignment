using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
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

    public class TextQueueHandler
    {
        private static QueueClient imageque;
        private static string _connection = Environment.GetEnvironmentVariable("AzureWebJobsStorage");

        [FunctionName("TextQueueHandler")]
        public static async Task Run([QueueTrigger("text-queue", Connection = "AzureWebJobsStorage")] string uuid, ILogger log)
        {
            log.LogInformation($"Processing text of {uuid}");


            imageque = new(_connection, "image-queue", new QueueClientOptions()
            {
                MessageEncoding = QueueMessageEncoding.Base64,
            });

            await imageque.SendMessageAsync(uuid);
        }
    }
}
