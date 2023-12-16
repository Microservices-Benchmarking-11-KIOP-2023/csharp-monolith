using Pb.ApiGateway.Providers;
using Pb.ApiGateway.Setup;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<IHotelProvider, HotelProvider>();
builder.Services.SetupGatewayRegistry(builder.Configuration);

var app = builder.Build();

app.MapControllers();

app.Run();
