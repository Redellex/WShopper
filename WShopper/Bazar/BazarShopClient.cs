using HtmlAgilityPack;
using RestSharp;
using WShopper.Models;

namespace WShopper.Bazar;

public class BazarShopClient
{
    private string searchEndpoint = "wyszukiwanie";

    private RestClient client;

    public BazarShopClient(BazarShop netShopConfig)
    {
        client = new RestClient(netShopConfig.BaseUrl);
    }

    public async Task<List<BazarShopAdModel>> QuerySearch(string query)
    {
        var request = new RestRequest(searchEndpoint);
        request.Method = Method.Post;
        request.AddParameter("search_slovo", query, ParameterType.GetOrPost);
        request.AddParameter("search_typ", "0", ParameterType.GetOrPost);
        request.AddParameter("search_cena_od", "0", ParameterType.GetOrPost);
        request.AddParameter("search_cena_do", "", ParameterType.GetOrPost);
        request.AddParameter("search", "Szukaj", ParameterType.GetOrPost);
        var response = await client.ExecuteAsync(request);
        return ExtractAds(response.Content);
    }

    private List<BazarShopAdModel> ExtractAds(string content)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(content);

        // Parsujemy ogłoszenia
        var adNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'hidtop')]/following-sibling::div[@class='inner inzerat']");
        List<BazarShopAdModel> ads = new List<BazarShopAdModel>();
        foreach (var adNode in adNodes)
        {
            var ad = new BazarShopAdModel();

            var linkNode = adNode.SelectSingleNode(".//a[@class='img']");
            ad.Link = linkNode != null ? linkNode.GetAttributeValue("href", string.Empty) : string.Empty;

            var tytulNode = adNode.SelectSingleNode(".//div[@class='top']//h2//a");
            ad.Title = tytulNode != null ? tytulNode.InnerText.Trim() : string.Empty;

            var opisNode = adNode.SelectSingleNode(".//p");
            ad.Description = opisNode != null ? opisNode.InnerText.Trim() : string.Empty;

            var cenaNode = adNode.SelectSingleNode(".//ul[@class='cendat']//li[@class='cena']//strong");
            ad.Price = cenaNode != null ? cenaNode.InnerText.Trim() : string.Empty;

            var dataNode = adNode.SelectSingleNode(".//ul[@class='cendat']//li[@class='datum']");
            ad.Date = dataNode != null ? dataNode.InnerText.Trim().Replace("\n", " ") : string.Empty;

            var lokalizacjaNode = adNode.SelectSingleNode(".//ul[@class='cendat']//li[@class='lokalita']");
            ad.Location = lokalizacjaNode != null ? lokalizacjaNode.InnerText.Trim() : string.Empty;

            var kategoriaNode = adNode.SelectSingleNode(".//h3//a");
            ad.Category = kategoriaNode != null ? kategoriaNode.InnerText.Trim() : string.Empty;

            ads.Add(ad);
        }

        return ads;
    }
}