using UsingMinimalApiDiscovery.Data;
using WilderMinds.MinimalApiDiscovery;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<CustomerRepository>();
builder.Services.AddTransient<StateCollection>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapApis();

app.Run();
