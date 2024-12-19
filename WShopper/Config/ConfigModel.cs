namespace WShopper;

public class ConfigModel
{
    public NetShop NetShop { get; set; }
    public BazarShop BazarShop { get; set; }
}

public class NetShop
{
    public string BaseUrl { get; set; }
}

public class BazarShop
{
    public string BaseUrl { get; set; }
}