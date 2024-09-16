using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace DeliveryAppClient.Pages;

public class OrderModel : PageModel
{
	private readonly ILogger<OrderModel> _logger;
	private readonly IProductService _productService;
	private readonly INavigationService _navigationService;

	public OrderModel(ILogger<OrderModel> logger, IProductService productService, INavigationService navigationService)
	{
		_logger = logger;
		_productService = productService;
		_navigationService = navigationService;
	}


	[BindProperty(SupportsGet = true)]
	public string Name { get; set; }

	[BindProperty(SupportsGet = true)]
	public string Description { get; set; }

	[BindProperty(SupportsGet = true)]
	public decimal Price { get; set; }

	[BindProperty(SupportsGet = true)]
	public string ProductKey { get; set; }



	public async Task<IActionResult> OnPostAsync(string key, string userEmail)
	{

		var product = _productService.GetProducts().FirstOrDefault(p => p.RowKey == key);

		if (product == null)
		{
			_logger.LogError($"Product with key {key} not found.");
			return Page();
		}


		Order order = new Order()
		{
			ProductRowKey = product.RowKey,
			Email = userEmail,
		};

		var orderWithProduct = new
		{
			Order = order,
			Product = new
			{
				product.Name,
				product.Price,
				product.Description,
				product.Url
			}
		};

		string message = JsonSerializer.Serialize(orderWithProduct);

		await _navigationService.SendMessageAsync(message);

		_logger.LogInformation($"Order for product {product.Name} with email {userEmail} was successfully sent.");

		return RedirectToPage("/Index");
	}
}

