using System.Diagnostics;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


Console.WriteLine("Azure Blob Storage exercises\n");

string containerName = "";

// case 6, read container metadata
ReadContainerMetadataAsync(containerName).GetAwaiter().GetResult();

//case 5, add container metadata
// AddContainerMetadata(containerName).GetAwaiter().GetResult();

// case 4, get container properties
// GetContainerProperties(containerName).GetAwaiter().GetResult();

// case3, download blobs from the container
// DownloadBlobFromContainer(containerName).GetAwaiter().GetResult();

// case2, update the blobs to the container
// ListBlobsInContainer(containerName).GetAwaiter().GetResult();

// case1 , Run the examples asynchroniously, wait for the results before proceeding
// ProcessAsync().GetAwaiter().GetResult();

Console.WriteLine("Press enter to exit the sample application.");
Console.ReadLine();

static BlobServiceClient GetBlobServiceClient()
{
   string connectionString = "";
   return new BlobServiceClient(connectionString);
}

static async Task ProcessAsync()
{
   // string connectionString = "";
   // BlobServiceClient blobServiceClient = new(connectionString);

   string containerName = "quickstartblobs" + Guid.NewGuid().ToString();
   BlobServiceClient blobServiceClient = GetBlobServiceClient();

   // Create the container and return a container client object
   BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
   Console.WriteLine($"Created container {containerClient.Name}");
}



static async Task UpdateBlobsToContainer(string containerName)
{
   // Create a local file in the ./data/ directory for uploading and downloading
   string localPath = "./data/";
   string fileName = "wtfile" + Guid.NewGuid().ToString() + ".txt";
   string localFilePath = Path.Combine(localPath, fileName);

   // Write text to the file
   await File.WriteAllTextAsync(localFilePath, "Hello, World!");

   // Get a reference to a container named "quickstartblobs"
   BlobServiceClient blobServiceClient = GetBlobServiceClient();
   BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

   // Get a reference to a blob
   BlobClient blobClient = containerClient.GetBlobClient(fileName);

   Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

   // Open the file and upload its data
   using (FileStream uploadFileStream = File.OpenRead(localFilePath))
   {
      await blobClient.UploadAsync(uploadFileStream, true);
      uploadFileStream.Close();
   }

   Console.WriteLine("\nThe file was uploaded. We'll verify by Listing...");

}




static async Task ListBlobsInContainer(string containerName)
{
   Console.WriteLine("Listing blobs...");



   // Get a reference to a container named "quickstartblobs"
   BlobServiceClient blobServiceClient = GetBlobServiceClient();
   BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

   await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
   {
      Console.WriteLine("\t" + blobItem.Name);
   }

   Console.WriteLine("\n You can verify by looking inside the container in Portal.Listing completed.");

}

static async Task DownloadBlobFromContainer(string containerName)
{
   Console.WriteLine("Download blobs...");

   // Create a local file in the ./data/ directory for uploading and downloading
   string localPath = "./data/";
   string fileName = "wtfilea11399c0-f2e1-473d-bd44-d26b9183cc9d.txt";
   string localFilePath = Path.Combine(localPath, fileName);
   string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOAD.txt");

   // Get a reference to a container named "quickstartblobs"
   BlobServiceClient blobServiceClient = GetBlobServiceClient();
   BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

   // Download the blob's contents and save it to a file
   BlobDownloadInfo download = await containerClient.GetBlobClient(fileName).DownloadAsync();

   using (FileStream fs = File.OpenWrite(downloadFilePath))
   {
      await download.Content.CopyToAsync(fs);
   }
   Console.WriteLine("\n locate the local file in the data directory created earlier.");

}






static async Task DeleteContainer(string containerName)
{
   Console.WriteLine("Delete container...");


   // Get a reference to a container named "quickstartblobs"
   BlobServiceClient blobServiceClient = GetBlobServiceClient();
   BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

   // Delete the container
   await containerClient.DeleteAsync();

   Console.WriteLine("Delete the lcoal source and downloaded files.");
   string localPath = "./data/";
   string fileName = "";
   string localFilePath = Path.Combine(localPath, fileName);
   string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOAD.txt");
   File.Delete(localFilePath);
   File.Delete(downloadFilePath);
}


// Retrive container properties
static async Task GetContainerProperties(string containerName)
{
   Console.WriteLine("Get container properties...");

   // Get a reference to a container named "quickstartblobs"
   BlobServiceClient blobServiceClient = GetBlobServiceClient();
   BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

   try
   {
      // Get the container's properties
      BlobContainerProperties properties = await containerClient.GetPropertiesAsync();

      Console.WriteLine($"Properties for container {containerName}");
      Console.WriteLine($"Public access level: {properties.PublicAccess}");
      Console.WriteLine($"Last modified time in UTC: {properties.LastModified}");
      Console.WriteLine($"Lease status: {properties.LeaseStatus}");
      Console.WriteLine($"Lease state: {properties.LeaseState}");
      Console.WriteLine($"Lease duration: {properties.LeaseDuration}");
   }
   catch (Azure.RequestFailedException e)
   {
      Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
      Console.WriteLine(e.Message);
      return;
   }

}


static async Task AddContainerMetadata(string containerName)
{
   Console.WriteLine("Add container metadata...");

   try
   {

      // Get a reference to a container named "quickstartblobs"
      BlobServiceClient blobServiceClient = GetBlobServiceClient();
      BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

      // Define the metadata
      IDictionary<string, string> metadata = new Dictionary<string, string>
   {
       { "docType", "textDocuments" },
       { "category", "guidance" }
   };

      // Set the container's metadata
      await containerClient.SetMetadataAsync(metadata);

      Console.WriteLine("The container's metadata was updated.");
   }
   catch (Azure.RequestFailedException e)
   {
      Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
      Console.WriteLine(e.Message);
      return;
   }

}

static async Task ReadContainerMetadataAsync(string containerName)
{
   Console.WriteLine("Read container metadata...");

   try
   {
      // Get a reference to a container named "quickstartblobs"
      BlobServiceClient blobServiceClient = GetBlobServiceClient();
      BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

      // Get the container's properties and metadata
      BlobContainerProperties properties = await containerClient.GetPropertiesAsync();

      Console.WriteLine($"Metadata for container {containerName}");
      foreach (KeyValuePair<string, string> metadata in properties.Metadata)
      {
         Console.WriteLine($"\tKey: {metadata.Key}");
         Console.WriteLine($"\tValue: {metadata.Value}");
      }
   }
   catch (Azure.RequestFailedException e)
   {
      Console.WriteLine($"HTTP error code {e.Status}: {e.ErrorCode}");
      Console.WriteLine(e.Message);
      return;
   }

}