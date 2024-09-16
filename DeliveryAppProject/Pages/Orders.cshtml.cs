using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using DeliveryAppProject.DTOs;

namespace DeliveryAppProject.Pages;

public class OrdersModel : PageModel
{
	private readonly ILogger<OrdersModel> _logger;
	private readonly INavigationService _navigationService;
	private readonly IProductService _productService;

	public OrdersModel(ILogger<OrdersModel> logger, INavigationService navigationService, IProductService productService)
	{
		_logger = logger;
		_navigationService = navigationService;
		_productService = productService;
	}

	public List<OrderDTO> Orders { get; set; } = new List<OrderDTO>();

	public async Task OnGet()
	{
		try
		{
			var products = _productService.GetProducts();
			var ordersJson = await _navigationService.ReceiveMessagesAsync();

			var orderWithProducts = ordersJson.Select(x =>
				JsonSerializer.Deserialize<OrderWithProduct>(x)!).ToList();

			Orders = orderWithProducts.Select(x =>
				{
					var product = products.FirstOrDefault(y => y.RowKey.Equals(x.Order.ProductRowKey));

					if (product == null)
					{
						_logger.LogError($"Product with RowKey {x.Order.ProductRowKey} not found.");
						return null;
					}

					return new OrderDTO()
					{
						Name = x.Product.Name,
						Email = x.Order.Email,
						Price = x.Product.Price,
						Image = x.Product.Url,
					};
				})
				.Where(orderDTO => orderDTO != null)
				.ToList();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while loading orders.");
			ModelState.AddModelError(string.Empty, "Unable to load orders. Please try again later.");
		}
	}




    //public async Task OnGet()
    //{
	    // var products = _productService.GetProducts();
	    // var ordersJson = await _navigationService.ReceiveMassagesAsync();
	    // var orders = ordersJson.Select(x => JsonSerializer.Deserialize<Order>(x)!).ToList();
	    // Orders = orders.Select(x => {
		    //  var product = products.First(y => y.RowKey.Equals(x.ProductRowKey));
		    //  return new OrderDTO()
		    //  {
			    //   Name = product.Name,
			    //   Email = x.Email,
			    //   Price = product.Price,
			     //   Image = product.Url,
		      //  };
	       // }).ToList();
        //}



}

