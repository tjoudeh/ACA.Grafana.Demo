using Azure.Messaging.ServiceBus;
using Bogus;
using Shared.Contracts;
using System.Text.Json;

namespace Client.Publisher
{
    internal class Program
    {
        private const string topicName = "shipmentstopic";
        private const string connString = "<conn string>";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Simulating shipments tracking updates, how many updates needed?");

            var requestedAmount = DetermineMsgsCount();
            await QueueShipments(requestedAmount);

            Console.WriteLine("Simulation completed, thank you.");
        }

        private static async Task QueueShipments(int messagesCount)
        {
            var serviceBusClient = new ServiceBusClient(connString);
            var serviceBusSender = serviceBusClient.CreateSender(topicName);
            var messagesBatch = new List<ServiceBusMessage>();

            var lastIndex = ReadLastIndex();

            for (int currentCount = 1; currentCount < messagesCount + 1; currentCount++)
            {
                lastIndex++;

                var shipment = GenerateShipment(lastIndex);
                var rawShipment = JsonSerializer.Serialize(new { data = shipment });
                var shipmentMessage = new ServiceBusMessage(rawShipment);

                Console.WriteLine($"{currentCount}. Adding Shipment id '{shipment.ShipmentId}' and Waybill '{shipment.WaybillNo}' to batch");

                messagesBatch.Add(shipmentMessage);

                if  (currentCount % 25 == 0){

                    Console.WriteLine($"Sending batch of {messagesBatch.Count} messages to the topic");
                    await serviceBusSender.SendMessagesAsync(messagesBatch);
                    messagesBatch = new List<ServiceBusMessage>();
                }
            }

            WriteLastIndex(lastIndex);
        }

        private static ShipmentModel GenerateShipment(int index)
        {
      
            var shipmentGenerator = new Faker<ShipmentModel>()
                .RuleFor(u => u.Id, index)
                .RuleFor(u => u.RecipientName, f => f.Name.FullName())
                .RuleFor(u => u.ShipmentId, f => f.Random.Uuid())
                .RuleFor(u => u.WaybillNo, f => f.Random.ULong(1000000, 9999999).ToString())
                .RuleFor(u => u.CreatedOn, f => f.Date.Between(DateTime.UtcNow.AddDays(-5).Date, DateTime.UtcNow.AddDays(-1).Date));

            return shipmentGenerator.Generate();
        }

        private static int ReadLastIndex()
        {
            var lastIndex = 0;
            try
            {
                var dirPath = AppDomain.CurrentDomain.BaseDirectory;

                var filePath = Path.Combine(dirPath, "index.txt");

                var text =  File.ReadAllText(filePath);

                if (!string.IsNullOrWhiteSpace(text))
                {
                    lastIndex = Convert.ToInt32(text);
                }
            }
            catch (Exception)
            {

                return 0;
            }

            return lastIndex;
        }

        private static void WriteLastIndex(int index)
        {
            // Create a string with a line of text.
            var text = index.ToString();

            var dirPath = AppDomain.CurrentDomain.BaseDirectory;

            // Write the text to a new file named "WriteFile.txt".
            File.WriteAllText(Path.Combine(dirPath, "index.txt"), text);
        }

        private static int DetermineMsgsCount()
        {
            var rawAmount = Console.ReadLine();
            if (int.TryParse(rawAmount, out int amount))
            {
                return amount;
            }

            Console.WriteLine("Not a valid number, please try again");
            return DetermineMsgsCount();
        }
    }
}