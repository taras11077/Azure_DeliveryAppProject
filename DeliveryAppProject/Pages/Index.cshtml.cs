using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DeliveryAppProject.Pages;

public class IndexModel : PageModel
{
	private readonly ILogger<IndexModel> _logger;
	private readonly IProductService _productService;

	public IndexModel(ILogger<IndexModel> logger, IProductService productService)
	{
		_logger = logger;
		_productService = productService;
		Products = new List<Product>();
	}

	public List<Product> Products { get; set; }

	public void OnGet()
	{
		try
		{
			var products = _productService.GetProducts();
			Products = new List<Product>(products);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while fetching products.");
			ModelState.AddModelError(string.Empty, "An error occurred while loading the products. Please try again later.");
		}
	}
}


