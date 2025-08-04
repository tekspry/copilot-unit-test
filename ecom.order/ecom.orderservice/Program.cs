using Dapr.Client;
using ecom.order.infrastructure.Services.Customer;
using ecom.order.application.Order;
using ecom.order.database.order;
using ecom.order.infrastructure.Product;
//using ecom.order.infrastructure.Services.Customer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IOrderApplication, OrderApplication>();
builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
builder.Services.AddControllers().AddDapr();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddControllers();

Console.WriteLine($"order Dapr port: {Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")}");
builder.Services.AddDaprClient();
// Using the DAPR SDK to create a DaprClient, in stead of fiddling with URI's our selves
builder.Services.AddSingleton<IProductService>(sc =>
    new ProductService(DaprClient.CreateInvokeHttpClient("product")));
builder.Services.AddSingleton<ICustomerService>(sc =>
    new CustomerService(DaprClient.CreateInvokeHttpClient("customer")));

// Configure CORS
var allowedOrigins = builder.Configuration.GetSection("CorsPolicy:Origins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .WithHeaders("Content-Type", "Authorization")
              .WithMethods("GET", "POST", "PUT", "DELETE");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCloudEvents();

// Add security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; connect-src 'self'");
    await next();
});

app.UseHttpsRedirection();
app.UseHsts();

// Use the configured CORS policy
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();

