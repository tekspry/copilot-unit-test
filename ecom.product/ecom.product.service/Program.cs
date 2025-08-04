using ecom.product.application.ProductApp;
using ecom.product.database.ProductDB;
using ecom.product.infrastructure.Services.Open_AI;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Key Vault conditionally
if (builder.Configuration.GetValue<bool>("KeyVault:Enabled", false))
{
    var keyVaultEndpoint = builder.Configuration["KeyVault:Endpoint"];
    if (!string.IsNullOrEmpty(keyVaultEndpoint))
    {
        try
        {
            builder.Configuration.AddAzureKeyVault(
                new Uri(keyVaultEndpoint),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    ExcludeEnvironmentCredential = false,
                    ExcludeManagedIdentityCredential = false,
                    ExcludeVisualStudioCredential = false,
                    ExcludeAzureCliCredential = false,
                    ExcludeAzurePowerShellCredential = false,
                    ExcludeInteractiveBrowserCredential = false
                }));
        }
        catch (Exception ex)
        {
            // Log the error but don't throw in development
            Console.WriteLine($"Failed to configure Azure Key Vault: {ex.Message}");
            if (!builder.Environment.IsDevelopment())
            {
                throw;
            }
        }
    }
}

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = jwtSettings["Authority"];
        options.Audience = jwtSettings["Audience"];
        options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = !builder.Environment.IsDevelopment(),
            ValidateAudience = !builder.Environment.IsDevelopment(),
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };

        // Add development signing key if configured
        var secretKey = jwtSettings["SecretKey"];
        if (builder.Environment.IsDevelopment() && !string.IsNullOrEmpty(secretKey))
        {
            options.TokenValidationParameters.IssuerSigningKey = 
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }
    });

// Configure CORS
var allowedOrigins = builder.Configuration.GetSection("CorsPolicy:Origins").Get<string[]>() ?? new[] { "http://localhost:3000" };
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add services to the container.
builder.Services.AddSingleton<IProductApplication, ProductApplication>();
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IProductOpenAI, ProductOpenAI>();
builder.Services.AddControllers().AddDapr();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
        c.RoutePrefix = "swagger";
    });
}

// Use CORS before other middleware
app.UseCors();

app.UseRouting();
app.UseCloudEvents();

app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapSubscribeHandler();
app.MapControllers();

app.Run();