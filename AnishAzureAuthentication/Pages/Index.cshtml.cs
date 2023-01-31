using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.Reflection.Metadata;

namespace AnishAzureAuthentication.Pages
{
    //[Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        public string accessToken;
        public string blobContent;

        public IndexModel(ILogger<IndexModel> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task OnGet()
        {
            //string[] scope = new string[] { "https://storage.azure.com/user_impersonation" };
            //accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scope);

            TokenAcquisitionTokenCredential tokenAcquisitionTokenCredential = new TokenAcquisitionTokenCredential(_tokenAcquisition);

            Uri blobUri = new Uri("https://anishstorage786.blob.core.windows.net/anishcontainer/AZ-900.txt");
            BlobClient blobClient = new BlobClient(blobUri, tokenAcquisitionTokenCredential);

            MemoryStream ms = new MemoryStream();
            blobClient.DownloadTo(ms);
            ms.Position = 0;

            StreamReader sr = new StreamReader(ms);
            blobContent = sr.ReadToEnd();
        }
    }
}