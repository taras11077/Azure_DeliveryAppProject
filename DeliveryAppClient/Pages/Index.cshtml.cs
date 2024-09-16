using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using DeliveryApp.Core.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace DeliveryAppClient.Pages;

public class IndexModel : PageModel
{
	private readonly ILogger<IndexModel> _logger;
	private readonly INavigationService _navigationService;
	private readonly IProductService _productService;

	public IndexModel(ILogger<IndexModel> logger, INavigationService navigationService, IProductService productService)
	{
		_logger = logger;
		_navigationService = navigationService;
		_productService = productService;
		Products = new List<Product>();
	}

	public List<Product> Products { get; set; }

	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			Products = new List<Product>(_productService.GetProducts());
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while fetching products.");
			ModelState.AddModelError(string.Empty, "An error occurred while loading the products. Please try again later.");
		}

		try
		{
			var messages = await _navigationService.ReceiveMessagesAsync();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while receiving messages from the queue.");
			ModelState.AddModelError(string.Empty, "An error occurred while receiving messages. Please try again later.");
		}

		return Page();
	}



	//public async Task OnPostAsync(string key)
	//{
	//    Order order = new Order()
	//    {
	//        ProductRowKey = key,
	//        Email = ""
	//    };

	//    await _navigationService.SendMessageAsync(JsonSerializer.Serialize(order));
	//}

}

