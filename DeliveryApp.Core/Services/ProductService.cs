using Azure.Data.Tables;
using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Models;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;


namespace DeliveryApp.Core.Services;

public class ProductService : IProductService
{
	private readonly TableServiceClient _tableServiceClient;
	private readonly TableClient _tableClient;
	private readonly string _tableName = "products";
	private readonly BlobService _blobService;
	private readonly ILogger<ProductService> _logger;


	public ProductService(string connectionString, ILogger<ProductService> logger, BlobService blobService)
	{
		_tableServiceClient = new TableServiceClient(connectionString);
		_tableClient = _tableServiceClient.GetTableClient(_tableName);
		_tableClient.CreateIfNotExists();
		_blobService = blobService;
		_logger = logger;
	}

	public async Task AddProduct(Product product, IEnumerable<byte> bytes)
	{
		try
		{
			string blobUrl = await _blobService.AddBlob(product.Image, bytes);
			product.Image = blobUrl;
			product.Url = blobUrl;
			await _tableClient.AddEntityAsync(product);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while adding product {ProductName}", product.Name);
			throw;
		}
	}

	public IEnumerable<Product> GetProducts()
	{
		try
		{
			var response = _tableClient.Query<Product>(x => x.PartitionKey.Equals(nameof(Product)));
			return response.ToList();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while retrieving products");
			throw;
		}
	}

	public Task OrderProduct(string rowKey, string email)
	{
		throw new NotImplementedException();
	}
}
