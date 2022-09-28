using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace Backend.Processor.Controllers
{
    [Route("api/shipmentsprocessor")]
    [ApiController]
    public class ShipmentsProcessorController : ControllerBase
    {

        private static string STORE_NAME = "shipmentsstatestore";
        private readonly IConfiguration _config;
        private readonly ILogger _logger;
        private readonly DaprClient _daprClient;
        public ShipmentsProcessorController(IConfiguration config, ILogger<ShipmentsProcessorController> logger, DaprClient daprClient)
        {
            _config = config;
            _logger = logger;
            _daprClient = daprClient;
        }

        [Dapr.Topic("pubsub-servicebus", "shipmentstopic")]
        [HttpPost("shipmentupdate")]
        public async Task<IActionResult> ShipmentUpdateReceived([FromBody] ShipmentModel shipmentModel)
        {
            _logger.LogInformation("Started processing message with shipment WaybillNo '{0}'", shipmentModel.WaybillNo);

            shipmentModel.TimeStamp = DateTime.UtcNow;

            await SaveShipment(shipmentModel);

            var delayForSconds = _config.GetValue<int>("DelayForSeconds", 5);

            if (delayForSconds > 0)
            {
                Thread.Sleep(delayForSconds * 1000);
            }

            return Ok();
           
        }

        private async Task SaveShipment(ShipmentModel shipmentModel)
        {
           

            await _daprClient.SaveStateAsync(STORE_NAME,
                                                    shipmentModel.Id.ToString(),
                                                    shipmentModel);

            _logger.LogInformation("Shipment with WaybillNo '{0}' saved successfuly", shipmentModel.WaybillNo);
        }
    }
}
