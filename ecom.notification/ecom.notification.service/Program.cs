using Dapr.Client;
using ecom.notification.application.Notification;
using ecom.notification.infrastructure.Services.Customer;
using ecom.notification.infrastructure.Services.Email;
using ecom.notification.infrastructure.Services.Product;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add Azure Key Vault configuration only in non-development environments
var keyVaultEndpoint = builder.Configuration["KeyVault:Endpoint"];
if (!string.IsNullOrEmpty(keyVaultEndpoint) && !builder.Environment.IsDevelopment())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultEndpoint),
        new DefaultAzureCredential());
}

// Configure JWT Authentication with relaxed settings in development
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
if (builder.Environment.IsDevelopment())
{
    // In development, use minimal authentication setup
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = false,
                ClockSkew = TimeSpan.Zero
            };
        });
}
else
{
    // Production JWT settings
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = jwtSettings["Authority"];
            options.Audience = jwtSettings["Audience"];
            options.RequireHttpsMetadata = true;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
        });
}

// Add services to the container.
builder.Services.AddHttpClient();
builder.Services.AddSingleton<INotificationApplication, NotificationApplication>();
builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddControllers().AddDapr();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Configure HTTPS
builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = 443;
    options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
});

Console.WriteLine($"order Dapr port: {Environment.GetEnvironmentVariable("DAPR_HTTP_PORT")}");
builder.Services.AddDaprClient();
// Using the DAPR SDK to create a DaprClient, in stead of fiddling with URI's our selves
builder.Services.AddSingleton<IProductService>(sc =>
    new ProductService(DaprClient.CreateInvokeHttpClient("product")));

builder.Services.AddSingleton<ICustomerService>(sc =>
    new CustomerService(DaprClient.CreateInvokeHttpClient("customer")));

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
    // Add security headers
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self'; style-src 'self'; img-src 'self' data:; connect-src 'self'");
    
    // Add HSTS header in production
    if (!app.Environment.IsDevelopment())
    {
        context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000; includeSubDomains");
    }
    
    await next();
});

app.UseHttpsRedirection();
app.UseHsts();

app.UseAuthentication();
app.UseAuthorization();

// Use the configured CORS policy
app.UseCors();

app.UseEndpoints(endpoints =>
{
    endpoints.MapSubscribeHandler();
    endpoints.MapControllers();
});

app.Run();
