using HtmlAgilityPack;
using RestSharp;
using WShopper.Models;

namespace WShopper.Day;

public class DayShopClient
{
    private string searchEndpoint = "ogl-listowanie.php";

    private RestClient client;

    public DayShopClient(DayShop netShopConfig)
    {
        client = new RestClient(netShopConfig.BaseUrl);
    }

    public async Task<List<AdModel>> QuerySearch(string query)
    {
        var request = new RestRequest(searchEndpoint + $"?q={query}");
        var response = await client.ExecuteAsync(request);
        return ExtractAds(response.Content);
    }

    private List<AdModel> ExtractAds(string content)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(content);
        var adNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'product-simple')]");
        List<AdModel> ads = new List<AdModel>();
        foreach (var adNode in adNodes)
        {
            var titleNode = adNode.SelectSingleNode(".//h3[@class='product-name overflow-4']/a");;
            string title = titleNode != null ? titleNode.InnerText.Trim(): "";

            var priceNode = adNode.SelectSingleNode(".//div[@class='col-6 pricenumber']/a");
            string price = priceNode != null ? priceNode.InnerText.Trim() : "";

            var countdownNode = adNode.SelectSingleNode(".//div[contains(@class, 'product-countdown')]");
            string countDown = countdownNode!= null ? countdownNode.InnerText.Trim() :"";
            
            var linkNode = adNode.SelectSingleNode(".//figure[@class='product-media']/a");
            string link = linkNode != null ? linkNode.GetAttributeValue("href", "") : "";
            
            var imgDivNode = adNode.SelectSingleNode(".//figure//div[contains(@style, 'background-image')]");
            var image = "";
            if (imgDivNode != null)
            {
                var style = imgDivNode.GetAttributeValue("style", "");
                var start = style.IndexOf("url('") + 5;
                var end = style.IndexOf("')", start);
                if (start > 4 && end > start)
                {
                     image = style.Substring(start, end - start);
                }
            }

            var ad = new AdModel()
            {
                Title = title,
                Price = price,
                Description =  $"Pozostalo: {countDown}",
                Link = link,
                Image = image
            };

            ads.Add(ad);
        }

        return ads;
    }
}