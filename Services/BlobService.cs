using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SnackMVCApp.Services
{
    public class BlobService
    {
        private readonly BlobContainerClient _containerClient;
        private bool _containerInitialised = false;

        public BlobService(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("BlobConnection");
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Only get the client reference — do NOT call CreateIfNotExists here
            _containerClient = blobServiceClient.GetBlobContainerClient("snack-images");
        }

        // Call this before any operation — creates container only once
        private async Task EnsureContainerExistsAsync()
        {
            if (!_containerInitialised)
            {
                await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                _containerInitialised = true;
            }
        }

        // UPLOAD
        public async Task<string> UploadAsync(IFormFile file)
        {
            await EnsureContainerExistsAsync();

            string blobName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    ContentType = file.ContentType
                });
            }

            return blobClient.Uri.ToString();
        }

        // DELETE
        public async Task DeleteAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;

            await EnsureContainerExistsAsync();

            string blobName = Path.GetFileName(new Uri(imageUrl).LocalPath);
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
        }
    }
}
