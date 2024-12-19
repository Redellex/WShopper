using HtmlAgilityPack;
using RestSharp;
using WShopper.Models;

namespace WShopper.Shop1;

public class NetShopClient
{
    private string searchEndpoint = "wyszukiwanie";

    private RestClient client;

    public NetShopClient(NetShop netShopConfig)
    {
        client = new RestClient(netShopConfig.BaseUrl);
    }

    public async Task<List<NetShopAdModel>> QuerySearch(string query)
    {
        var request = new RestRequest(searchEndpoint + $"?query={query}&categories[]=");
        var response = await client.ExecuteAsync(request);
        return ExtractAds(response.Content);
    }

    private List<NetShopAdModel> ExtractAds(string content)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(content);
        var adNodes = doc.DocumentNode.SelectNodes("//div[contains(@id, 'ogloszenie-')]");
        List<NetShopAdModel> ads = new List<NetShopAdModel>();
        foreach (var adNode in adNodes)
        {
            var titleNode = adNode.SelectSingleNode(".//div[@class='title']/h3");
            string title = titleNode?.InnerText.Trim();

            var priceNode = adNode.SelectSingleNode(".//div[@class='price']/span");
            string price = priceNode?.InnerText.Trim();

            var descriptionNode = adNode.SelectSingleNode(".//div[@class='description']/div[@class='info']/p");
            string description = descriptionNode?.InnerText.Trim();
            
            var linkNode = adNode.SelectSingleNode(".//a[@title]");
            string link = linkNode?.GetAttributeValue("href", "");

            var ad = new NetShopAdModel
            {
                Title = title,
                Price = price,
                Description = description,
                Link = link
            };

            ads.Add(ad);
        }

        return ads;
    }
}