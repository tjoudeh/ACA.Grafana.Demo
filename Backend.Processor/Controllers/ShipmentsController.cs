using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Backend.Processor.Controllers
{
    [Route("api/shipments")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
        private static string STORE_NAME = "shipmentsstatestore";
        private readonly ILogger _logger;
        private readonly DaprClient _daprClient;


        public ShipmentsController(IConfiguration config, ILogger<ShipmentsController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }


        [HttpGet]
        public async Task<IEnumerable<ShipmentModel?>> Get(int idfrom = 1)
        {
            var result = new List<ShipmentModel?>();

            var keys = new List<string>();

            keys = Enumerable.Range(idfrom, 10).ToList().ConvertAll(x => x.ToString());

            foreach (var key in keys)
            {
                _logger.LogInformation("Reading shipment with id {id} from state store", key);
                var shipmentModel = await _daprClient.GetStateAsync<ShipmentModel>(STORE_NAME, key);

                if (shipmentModel != null)
                {
                    _logger.LogInformation("Adding shipment with id {id} to the final result", key);
                    result.Add(shipmentModel);
                }
            }

            return result;
        }
    }
}
