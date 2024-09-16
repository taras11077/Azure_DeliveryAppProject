using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.Extensions.Logging;

namespace DeliveryApp.Core.Services;

public class BlobService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _containerClient;
    private readonly string _blobContainerName = "images";
    private readonly ILogger<BlobService> _logger;

	public BlobService(string connectionString, ILogger<BlobService> logger)
    {
        _blobServiceClient = new BlobServiceClient(connectionString);
        _containerClient = _blobServiceClient.GetBlobContainerClient(_blobContainerName);
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);  // відкрити публічний доступ для читання до контейнера
        _logger = logger;
	}

    //public async Task<(string name, string sas)> AddBlob(string blobName, IEnumerable<byte> data)
    public async Task<string> AddBlob(string blobName, IEnumerable<byte> data)
	{
		try
		{
			var extension = Path.GetExtension(blobName);    // розширення файлу
			var name = Path.GetFileNameWithoutExtension(blobName); // ім'я без розширення
																   
			var uniqueBlobName = $"{name}_{Guid.NewGuid()}{extension}"; // нове унікальне ім'я

			var blobClient = _containerClient.GetBlobClient(uniqueBlobName);
			using var ms = new MemoryStream(data.ToArray());

			//var sas = GenerateSAS(blob);
			await blobClient.UploadAsync(ms);
			//return (uniqueName, sas);
			return GetBlobUrl(uniqueBlobName);  // повертаємо постійну URL
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while adding blob {BlobName}", blobName);
			throw;
		}
        
	}

	public string GetBlobUrl(string blobName)
	{
		try
		{
			return _containerClient.GetBlobClient(blobName).Uri.ToString();
		}
		catch (RequestFailedException ex)
		{
			_logger.LogError($"Request to Azure Blob Storage failed: {ex.Message}");
			throw;
		}
		catch (Exception ex)
		{
			_logger.LogError($"An error occurred while getting the Blob URL: {ex.Message}");
			throw;
		}
	}


	public async Task<IEnumerable<byte>> GetBlob(string blobName)
    {
	    try
	    {
		    var blob = _containerClient.GetBlobClient(blobName);
		    using var stream = await blob.OpenReadAsync();
		    using var ms = new MemoryStream();
		    stream.CopyTo(ms);
		    return ms.ToArray();
		}
	    catch (Exception ex)
	    {
			_logger.LogError(ex, "Error occurred while retrieving blob {BlobName}", blobName);
			throw;
		}
    }

    private string GenerateSAS(BlobClient blob)
    {
	    try
	    {
		    if (!blob.CanGenerateSasUri)
		    {
			    return string.Empty;
		    }

		    var sasBuilder = new BlobSasBuilder()
		    {
			    BlobContainerName = _blobContainerName,
			    BlobName = blob.Name,
			    ExpiresOn = DateTime.UtcNow.AddMinutes(30),
			    Resource = "b"
		    };
		    sasBuilder.SetPermissions(BlobAccountSasPermissions.All);
		    return blob.GenerateSasUri(sasBuilder).ToString();
	    }
	    catch (Exception ex)
	    {
		    _logger.LogError(ex, "Error occurred while generating SAS for blob {BlobName}", blob.Name);
		    throw;
	    }
    }


}
