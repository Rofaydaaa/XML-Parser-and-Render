using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualBasic;
using System;
using System.Xml;

namespace XMLParser.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        public List<ItemDetails> ItemsDetails { get; set; } = new List<ItemDetails>();

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IActionResult> OnGet()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync("http://scripting.com/rss.xml");
                XmlDocument xmlDoc = new XmlDocument();

                if (response.IsSuccessStatusCode)
                {
                    var xmlContent = await response.Content.ReadAsStringAsync();
                    xmlDoc.LoadXml(xmlContent);
                    foreach (XmlNode itemNode in xmlDoc.SelectNodes("rss/channel/item"))
                    {
                        var itemDetails = new ItemDetails();
                        itemDetails.Description = itemNode.SelectSingleNode("description")?.InnerText;
                        itemDetails.PubDate = itemNode.SelectSingleNode("pubDate")?.InnerText;
                        itemDetails.Link = itemNode.SelectSingleNode("link")?.InnerText;
                        itemDetails.Guide = itemNode.SelectSingleNode("guid")?.InnerText;
                        ItemsDetails.Add(itemDetails);
                    }
                    return Page();
                }
                else
                {
                    _logger.LogError("Unsuccessful status code");
                    return RedirectToPage("/Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing the request.");
                return RedirectToPage("/Error");
            }
        }
    }

    public class ItemDetails
    {
        public string Description { get; set; }
        public string PubDate { get; set; }
        public string Link { get; set; }
        public string Guide { get; set; }
    }
}
