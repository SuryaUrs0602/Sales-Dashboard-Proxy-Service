using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SalesDashBoardApplication;

namespace SalesDashBoardApplicationProxyService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly SalesDashBoardClient _salesDashBoardClient;
        private readonly ILogger<OrdersController> _logger;


        public OrdersController(SalesDashBoardClient salesDashBoardClient, ILogger<OrdersController> logger)
        {
            _salesDashBoardClient = salesDashBoardClient;
            _logger = logger;
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> AllOrdersData()
        {
            try
            {
                _logger.LogInformation("Fetching All orders data");
                var orders = await _salesDashBoardClient.OrdersAllAsync();
                return Ok(orders);
            }

            catch (ApiException ex)
            {
                _logger.LogError(ex, "Error occured while Fetching orders data");
                return StatusCode(500, new { error = "Could not process due to some error" });
            }
        }



        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> UsersOrderData(int userId)
        {
            try
            {
                _logger.LogInformation("Fetching all orders of a user");
                var orders = await _salesDashBoardClient.UserAsync(userId);
                return Ok(orders);
            }

            catch (ApiException ex)
            {
                _logger.LogError(ex, "Error occured while Fetching orders data of a user");
                return StatusCode(500, new { error = "Could not process due to some error" });
            }
        }

                

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderAddDto orderAddDto)
        {
            try
            {
                _logger.LogInformation("Creating a new order");
                await _salesDashBoardClient.OrdersAsync(orderAddDto);
                return Ok();
            }

            catch (ApiException ex)
            {
                _logger.LogError(ex, "Error occured while creating a new orders for a user");
                return StatusCode(500, new { error = "Could not process due to some error" });
            }
        }
    }
}
