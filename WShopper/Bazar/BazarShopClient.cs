using HtmlAgilityPack;
using RestSharp;
using WShopper.Models;

namespace WShopper.Bazar;

public class BazarShopClient
{
    private string url;

    public BazarShopClient(BazarShop bazarShopConfig)
    {
        url = bazarShopConfig.BaseUrl;
    }

    public async Task<List<AdModel>> QuerySearch(string query)
    {
        var contentString = "";
        
        using (var client = new HttpClient())
        {
            using (var content = new MultipartFormDataContent())
            {
                content.Add(new StringContent(query), "search_slovo");
                content.Add(new StringContent("0"), "search_typ");
                content.Add(new StringContent("0"), "search_cena_od");
                content.Add(new StringContent(""), "search_cena_do");
                content.Add(new StringContent("Szukaj"), "search");
                
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:137.0) Gecko/20100101 Firefox/137.0");

                var response = await client.PostAsync(url, content);

                contentString = await response.Content.ReadAsStringAsync();

            }
        }
        
        return this.ExtractAds(contentString); 
    }
    

    private List<AdModel> ExtractAds(string content)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(content);

        var adNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@id, 'hidtop')]/following-sibling::div[@class='inner inzerat']");
        List<AdModel> ads = new List<AdModel>();
        foreach (var adNode in adNodes)
        {

            var linkNode = adNode.SelectSingleNode(".//a[@class='img']");
            var link = linkNode != null ? linkNode.GetAttributeValue("href", string.Empty) : string.Empty;

            var tytulNode = adNode.SelectSingleNode(".//div[@class='top']//h2//a");
            var title = tytulNode != null ? tytulNode.InnerText.Trim() : string.Empty;

            var opisNode = adNode.SelectSingleNode(".//p");
            var desc = opisNode != null ? opisNode.InnerText.Trim() : string.Empty;

            var cenaNode = adNode.SelectSingleNode(".//ul[@class='cendat']//li[@class='cena']//strong");
            var price = cenaNode != null ? cenaNode.InnerText.Trim() : string.Empty;

            var dataNode = adNode.SelectSingleNode(".//ul[@class='cendat']//li[@class='datum']");
            var date = dataNode != null ? dataNode.InnerText.Trim().Replace("\n", " ") : string.Empty;

            var lokalizacjaNode = adNode.SelectSingleNode(".//ul[@class='cendat']//li[@class='lokalita']");
            var location = lokalizacjaNode != null ? lokalizacjaNode.InnerText.Trim() : string.Empty;

            var kategoriaNode = adNode.SelectSingleNode(".//h3//a");
            var category = kategoriaNode != null ? kategoriaNode.InnerText.Trim() : string.Empty;

            var imageNode = adNode.SelectSingleNode(".//a[@class='img']/img");
            var image  = imageNode != null ? imageNode.GetAttributeValue("src", "").Trim() : string.Empty;
            
            ads.Add(new AdModel
            {
                Title = title,
                Description = $"{desc}\nData: {date}\n Miejsce: {location}\n Kategoria: {category}",
                Price = price,
                Link = link,
                Image = image
            });
        }

        return ads;
    }
}