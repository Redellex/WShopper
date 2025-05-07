using HtmlAgilityPack;
using RestSharp;
using WShopper.Models;

namespace WShopper.Shop1;

public class NetShopClient
{
    private string searchEndpoint = "wyszukiwanie";

    private RestClient client;
    private string baseUrl = "";

    public NetShopClient(NetShop netShopConfig)
    {
        this.baseUrl = netShopConfig.BaseUrl;
        this.client = new RestClient(netShopConfig.BaseUrl);
    }

    public async Task<List<AdModel>> QuerySearch(string query)
    {
        var request = new RestRequest(searchEndpoint + $"?query={query}&categories[]=");
        var response = await client.ExecuteAsync(request);
        return ExtractAds(response.Content);
    }

    private List<AdModel> ExtractAds(string content)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(content);
        var adNodes = doc.DocumentNode.SelectNodes("//div[contains(@id, 'ogloszenie-')]");
        List<AdModel> ads = new List<AdModel>();
        foreach (var adNode in adNodes)
        {
 
            var titleNode = adNode.SelectSingleNode(".//div[@class='title']/h3");
            string title = titleNode != null? titleNode.InnerText.Trim() : "";
            
            var priceNode = adNode.SelectSingleNode(".//div[@class='price']/span");
            string price = priceNode != null ? priceNode.InnerText.Trim() : "";

            var descriptionNode = adNode.SelectSingleNode(".//div[@class='description']/div[@class='info']/p");
            string description = descriptionNode != null ? descriptionNode.InnerText.Trim() : "";
            
            var linkNode = adNode.SelectSingleNode(".//a[@title]");
            string link = linkNode != null ? linkNode.GetAttributeValue("href", "") : "";

            var ImageNode = adNode.SelectSingleNode(".//div[@class='thumb']/img");
            var imageUrl  = ImageNode != null ? this.baseUrl.TrimEnd('/') + ImageNode.GetAttributeValue("src", "").Trim() : "";

            var ad = new AdModel
            {
                Title = title,
                Price = price,
                Description = description,
                Link = link,
                Image = imageUrl
            };

            ads.Add(ad);
        }

        return ads;
    }
}