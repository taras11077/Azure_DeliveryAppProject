using DeliveryApp.Core.Interfaces;
using DeliveryApp.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<INavigationService>(sp =>
{
	var connectionString = builder.Configuration.GetConnectionString("ServiceBus")!;
	var logger = sp.GetRequiredService<ILogger<ServiceBusQueue>>();
	return new ServiceBusQueue(connectionString, logger);
});

builder.Services.AddSingleton<BlobService>(sp =>
{
	var connectionString = builder.Configuration.GetConnectionString("Storage")!;
	var logger = sp.GetRequiredService<ILogger<BlobService>>();
	return new BlobService(connectionString, logger);
});

builder.Services.AddTransient<IProductService>(sp =>
{
	var connectionString = builder.Configuration.GetConnectionString("Storage")!;
	var logger = sp.GetRequiredService<ILogger<ProductService>>();
	var blobService = sp.GetRequiredService<BlobService>();
	return new ProductService(connectionString, logger, blobService);
});

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddSession(opt =>
{
    opt.IdleTimeout = TimeSpan.FromSeconds(60);
    opt.Cookie.IsEssential = true;
    opt.Cookie.HttpOnly = true;
});

//builder.Services.AddTransient<ServiceBusQueue>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
