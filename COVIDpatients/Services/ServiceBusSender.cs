using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace COVIDpatients.Services
{
    public class ServiceBusSender
    {
        private readonly QueueClient _queclient;
        public ServiceBusSender(IConfiguration configuration)
        {
            _queclient = new QueueClient(configuration.GetConnectionString("ServiceBusConnectionString"),"covidpatients");
        }
        public async Task SendMessage(MessagePayload payload)
        {
            string data = JsonConvert.SerializeObject(payload);

            Message message = new Message(Encoding.UTF8.GetBytes(data));
            await _queclient.SendAsync(message);
        }
    }

    public class MessagePayload
    {
        public string EventName { get; set; }
        public string UserEmail { get; set; }
        public string dateStart { get; internal set; }
    }
}
