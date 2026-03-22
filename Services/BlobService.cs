using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace SnackMVCApp.Services
{
    // Handles all image uploads/deletes to Azurite (local Azure Blob Storage emulator)
    // Later: swap connection string for real Azure Storage!
    public class BlobService
    {
        // Our snack-images container client (like a folder in blob storage)
        private readonly BlobContainerClient _containerClient;

        // Flag to ensure container is created only ONCE per app lifetime
        private bool _containerInitialised = false;

        // Constructor: Gets connection from appsettings.json
        // IMPORTANT: Do NOT create container here - causes startup crash!
        public BlobService(IConfiguration configuration)
        {
            // Read Azurite connection string from appsettings.json
            var connectionString = configuration.GetConnectionString("BlobConnection");

            // Creates the main blob service client
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Gets reference to our "snack-images" container (creates on first use)
            _containerClient = blobServiceClient.GetBlobContainerClient("snack-images");
        }

        // Lazy container creation — called before every upload/delete
        // Creates "snack-images" container ONLY if it doesn't exist
        private async Task EnsureContainerExistsAsync()
        {
            if (!_containerInitialised)
            {
                // PublicAccessType.Blob = images accessible via direct URL
                await _containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
                _containerInitialised = true;
                Console.WriteLine("✅ Snack-images container created/initialized");
            }
        }

        // 🚀 UPLOAD: Takes snack image file → stores in Azurite → returns URL
        // Returns: "http://127.0.0.1:10000/devstoreaccount1/snack-images/abc123.jpg"
        public async Task<string> UploadAsync(IFormFile file)
        {
            // Ensure container exists before uploading
            await EnsureContainerExistsAsync();

            // Unique filename = GUID + original name (prevents overwrites)
            // Example: "abc123_topdeck.jpg"
            string blobName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            // Get blob client for our specific image
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);

            // Upload file stream to Azurite
            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobHttpHeaders
                {
                    // Sets MIME type (image/jpeg, image/png, etc.)
                    ContentType = file.ContentType
                });
            }

            // Return full public URL to save in SQL database
            return blobClient.Uri.ToString();
        }

        // 🗑️ DELETE: Removes image from Azurite using its URL
        public async Task DeleteAsync(string imageUrl)
        {
            // Skip if no image URL
            if (string.IsNullOrEmpty(imageUrl)) return;

            // Ensure container exists (safe to call)
            await EnsureContainerExistsAsync();

            // Extract filename from URL: "/devstoreaccount1/snack-images/abc123.jpg" → "abc123.jpg"
            string blobName = Path.GetFileName(new Uri(imageUrl).LocalPath);

            // Get blob client and delete if exists
            BlobClient blobClient = _containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();

            Console.WriteLine($"🗑️ Deleted blob: {blobName}");
        }
    }
}
