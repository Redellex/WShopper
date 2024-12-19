
using Newtonsoft.Json;

namespace WShopper;

public static class ConfigHandler
{
    public static ConfigModel GetConfig()
    {
        string jsonConfig = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "Config.json"));
        var config = JsonConvert.DeserializeObject<ConfigModel>(jsonConfig);
        if (config.NetShop.BaseUrl == null)
        {
            throw new Exception("You need to fill with config.json with adresses.");
        }

        return config;
    }
}