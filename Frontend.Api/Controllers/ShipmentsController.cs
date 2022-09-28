using Dapr.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts;

namespace Frontend.Api.Controllers
{
    [Route("api/shipments")]
    [ApiController]
    public class ShipmentsController : ControllerBase
    {
     
        private readonly ILogger _logger;
        private readonly DaprClient _daprClient;


        public ShipmentsController( ILogger<ShipmentsController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }


        [HttpGet]
        public async Task<IEnumerable<ShipmentModel?>> Get(int idfrom = 1)
        {

            _logger.LogInformation("Getting shipments starting from id {idfrom}", idfrom);

            var result  = await _daprClient.InvokeMethodAsync<List<ShipmentModel>>(HttpMethod.Get, "shipments-backend-processor", $"api/shipments?idfrom={idfrom}");

            return result;
        }
    }
}
