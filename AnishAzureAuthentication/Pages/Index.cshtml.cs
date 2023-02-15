using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Web;
using System.Net.Http.Headers;
using System;
using System.Reflection.Metadata;
using NuGet.Packaging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AnishAzureAuthentication.Models;
using System.Data;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace AnishAzureAuthentication.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITokenAcquisition _tokenAcquisition;
        private string[] scope = new string[] { "api://de7ba255-6d0b-4ba6-b5d7-c47a542e01a9/Inventory.ReadWebAPI", "api://de7ba255-6d0b-4ba6-b5d7-c47a542e01a9/Inventory.WriteWebAPI" };
        private string apiURL = "https://azurewebapiauth.azurewebsites.net/api/";
        public List<Inventory> inventoryList =  new List<Inventory>();
        public IndexModel(ILogger<IndexModel> logger, ITokenAcquisition tokenAcquisition)
        {
            _logger = logger;
            _tokenAcquisition = tokenAcquisition;
        }

        public async Task<List<Inventory>> OnGet()
        {
            try
            {
                string accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(scope);

                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage responseMessage = await client.GetAsync(apiURL + "InventoryDetails");
                string responseContent = await responseMessage.Content.ReadAsStringAsync();
                if (responseContent != null)
                {
                    inventoryList= JsonConvert.DeserializeObject<List<Inventory>>(responseContent);
                    return inventoryList;
                }
                return inventoryList;
            }
            catch(Exception ex)
            {
                _logger.LogError("Failure message : " + ex);
            }
            return inventoryList;
        }

        public async Task<String> Edit(int InventoryID,string InventoryName,string Description, double Price)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("apiURL");
                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("InventoryID", InventoryID.ToString()),
                new KeyValuePair<string, string>("InventoryName",InventoryName.ToString()),
                new KeyValuePair<string, string>("Description", Description.ToString()),
                new KeyValuePair<string, string>("Price", Price.ToString())
            });
                var result = await client.PostAsync("AddInventory", content);
                string resultContent = await result.Content.ReadAsStringAsync();
                return resultContent;
            }
        }
    }
}