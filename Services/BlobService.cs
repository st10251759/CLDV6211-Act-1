using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SnackMVCApp.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;

        public BlobService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("BlobConnection");

            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get or create the snack-images container
            _containerClient = blobServiceClient.GetBlobContainerClient("snack-images");

            // Only creates if it doesn't already exist - safe to call every time
            _containerClient.CreateIfNotExists(PublicAccessType.Blob);
        }

        // UPLOAD - returns the public URL of the uploaded image
        public async Task<string> UploadAsync(IFormFile file)
        {
            // Unique file name to avoid overwriting existing blobs
            string blobName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });
            }

            // Return full URL to store in DB
            return blobClient.Uri.ToString();
        }

        // DELETE - removes blob using its URL
        public async Task DeleteAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            // Extract just the blob filename from the full URL
            string blobName = Path.GetFileName(new Uri(imageUrl).LocalPath);

            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
