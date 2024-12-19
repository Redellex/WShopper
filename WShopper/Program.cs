using WShopper;
using WShopper.Shop1;

var builder = WebApplication.CreateBuilder(args);
var config = ConfigHandler.GetConfig();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.MapGet("/netShop",  async (string query = "") =>
    {
        var netShop = new NetShopClient(config.NetShop);
        var ads = await netShop.QuerySearch(query);
        return ads;
    })
    .WithName("netShop")
    .WithOpenApi();

app.Run();
