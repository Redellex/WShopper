using RestSharp;
using WShopper.Models;

namespace WShopperFrontend;

public class Service
{
    private readonly string _baseUrl = "http://localhost:5195/"; // Podaj adres swojego API

    public async Task<List<AdModel>> GetAds(string searchWord)
    {
        var client = new RestClient(_baseUrl);
        var netShop = new RestRequest("netShop");
        netShop.AddQueryParameter("query", searchWord);
        var bazarShop = new RestRequest("bazarShop"); 
        bazarShop.AddQueryParameter("query", searchWord);
        var DayShop = new RestRequest("dayShop"); 
        DayShop.AddQueryParameter("query", searchWord);
        
        
        var netShopResponse = await client.ExecuteAsync<List<AdModel>>(netShop);
        var bazarShopResponse = await client.ExecuteAsync<List<AdModel>>(bazarShop);
        var dayShopResponse = await client.ExecuteAsync<List<AdModel>>(DayShop);
        
        var ads = new List<AdModel>();
        ads.AddRange(netShopResponse.Data);
        ads.AddRange(bazarShopResponse.Data);
        ads.AddRange(dayShopResponse.Data);

        return netShopResponse.IsSuccessful ? ads : new List<AdModel>();
    }
}