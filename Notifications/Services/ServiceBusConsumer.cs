using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Notifications.Services
{
    public class ServiceBusConsumer
    {
        private readonly QueueClient _queclient;
        private readonly ILogger _logger;
        public ServiceBusConsumer(IConfiguration configuration, ILogger logger)
        {
            _logger = logger;
            _queclient = new QueueClient(configuration.GetConnectionString("ServiceBusConnectionString"), "covidpatients");
        }

        public void Register()
        {
            var options = new MessageHandlerOptions((e) => Task.CompletedTask)
            {
                AutoComplete = false
            };
            _queclient.RegisterMessageHandler(ProcessMessage, options);
        }

        private async Task ProcessMessage(Message message, CancellationToken token)
        {

            try
            {
                var payload = JsonConvert.DeserializeObject<MessagePayload>(Encoding.UTF8.GetString(message.Body));

                _logger.Information("Processing message: " + payload.EventName);

                if (payload.EventName == "NewUserRegistered")
                {
                
                    EmailSender sender = new EmailSender();
                    sender.SendNewPatientEmail(payload.UserEmail, payload.dateStart);
                }
                await _queclient.CompleteAsync(message.SystemProperties.LockToken);


            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
                _logger.Error(e, "Thrown exception at sending message");
                throw;
            }




        }
    }
    public class MessagePayload
    {
        public string EventName { get; set; }
        public string UserEmail { get; set; }
        public string dateStart { get; set; }
    }
}
