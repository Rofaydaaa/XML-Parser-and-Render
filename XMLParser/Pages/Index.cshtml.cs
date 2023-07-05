using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace XMLParser.Pages
{
    public class IndexModel : PageModel
    {
        public List<ItemDetails> ItemsDetails { get; set; } = new List<ItemDetails>();

        public void OnGet()
        {
            XmlDocument xmlDoc = new XmlDocument();
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("http://scripting.com/rss.xml").Result;
                if (response.IsSuccessStatusCode)
                {
                    var xmlContent = response.Content.ReadAsStringAsync().Result;
                    xmlDoc.LoadXml(xmlContent);
                }

                var itemsNode = xmlDoc.SelectNodes("rss/channel/item");

                if (itemsNode != null)
                {
                    foreach (XmlNode itemNode in itemsNode)
                    {
                        var itemDetails = new ItemDetails
                        {
                            Description = itemNode.SelectSingleNode("description")?.InnerText,
                            PubDate = itemNode.SelectSingleNode("pubDate")?.InnerText,
                            Link = itemNode.SelectSingleNode("link")?.InnerText,
                            Guide = itemNode.SelectSingleNode("guid")?.InnerText,
                            Source = itemNode.SelectSingleNode("source")?.InnerText
                        };
                        ItemsDetails.Add(itemDetails);
                    }
                }
            }
        }


    }

    public class ItemDetails
    {
        public string Description { get; set; }
        public string PubDate { get; set; }
        public string Link { get; set; }
        public string Guide { get; set; }
        public String Source { get; set; }
    }
}