# Troubleshooting Guide

## Package Version Conflicts and Assembly Loading Issues

### Issue 1: System.Diagnostics.DiagnosticSource Version Conflict
**Problem:** Version conflicts with System.Diagnostics.DiagnosticSource across microservices.
```
System.IO.FileLoadException: Could not load file or assembly 'System.Diagnostics.DiagnosticSource, Version=8.0.0.0'
```

**Solution:**
1. Update package versions in both payment and notification services:
   ```xml
   <PackageReference Include="Microsoft.Bcl.AsyncInterfaces" Version="8.0.0" />
   <PackageReference Include="Microsoft.Extensions.DependencyInjection.Abstractions" Version="8.0.2" />
   <PackageReference Include="Microsoft.Extensions.Logging.Abstractions" Version="8.0.3" />
   <PackageReference Include="System.Diagnostics.DiagnosticSource" Version="8.0.0" />
   ```

2. Update Dapr.AspNetCore to consistent version:
   ```xml
   <PackageReference Include="Dapr.AspNetCore" Version="1.12.0" />
   ```

### Issue 2: Azure Key Vault Connection in Development
**Problem:** Services trying to connect to Azure Key Vault in development environment.
```
Unhandled exception. System.AggregateException: Retry failed after 4 tries... (No such host is known. (your-keyvault-dev.vault.azure.net:443))
```

**Solution:**
1. Make Key Vault optional in development:
   ```csharp
   if (!string.IsNullOrEmpty(keyVaultEndpoint) && !builder.Environment.IsDevelopment())
   {
       builder.Configuration.AddAzureKeyVault(/*...*/);
   }
   ```

2. Update appsettings.Development.json:
   ```json
   {
     "KeyVault": {
       "Enabled": false,
       "Endpoint": ""
     }
   }
   ```

### Issue 3: CORS Configuration
**Problem:** CORS errors when accessing services from frontend:
```
Access to XMLHttpRequest at 'http://localhost:5016/product' from origin 'http://localhost:3000' has been blocked by CORS policy
```

**Solution:**
1. Configure CORS in Program.cs:
   ```csharp
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
   ```

2. Add middleware in correct order:
   ```csharp
   // Use CORS before other middleware
   app.UseCors();
   
   app.UseRouting();
   app.UseCloudEvents();
   
   app.UseAuthentication();
   app.UseAuthorization();
   ```

3. Configure allowed origins in appsettings.Development.json:
   ```json
   {
     "CorsPolicy": {
       "Origins": [
         "http://localhost:3000",
         "http://localhost:4200",
         "http://localhost:5000"
       ]
     }
   }
   ```

## React Frontend Issues

### Issue 4: TypeScript Authentication Hook Type Safety
**Problem:** TypeScript errors in orderHooks.ts due to potentially null authentication token:
```typescript
Argument of type '() => Promise<string | null>' is not assignable to parameter of type '() => Promise<string>'.
  Type 'Promise<string | null>' is not assignable to type 'Promise<string>'.
    Type 'string | null' is not assignable to type 'string'.
      Type 'null' is not assignable to type 'string'.
```

**Solution:**
1. Update useAuth.ts to ensure type safety:
```typescript
const getAccessToken = useCallback(async (): Promise<string> => {
  if (!auth.token) {
    throw new Error('No access token available');
  }
  return auth.token;
}, [auth.token]);
```

2. Add proper error handling in orderHooks.ts:
```typescript
const getApiClient = () => {
  const { getAccessToken } = useAuth();
  const nav = useNavigate();

  const handleAuthError = (error: Error) => {
    console.error('Authentication error:', error);
    nav('/login');
    throw error;
  };

  return createAuthenticatedApiClient(Config.baseOrderApiUrl, async () => {
    try {
      return await getAccessToken();
    } catch (error) {
      handleAuthError(error as Error);
      throw error;
    }
  });
};
```

**Key Changes:**
1. Made getAccessToken return type explicit with Promise<string>
2. Added proper error handling for missing tokens
3. Implemented automatic redirection to login page
4. Maintained type safety throughout the authentication flow

**Best Practices:**
1. Always specify explicit return types for authentication-related functions
2. Handle authentication errors gracefully with user redirection
3. Use TypeScript's type system to catch potential null/undefined issues
4. Implement proper error boundaries in React components

## Best Practices Applied

1. **Environment-specific Configuration:**
   - Different settings for development and production
   - Relaxed security in development, strict in production

2. **Middleware Order:**
   - CORS before routing and authentication
   - Proper order of security middleware

3. **Security Headers:**
   - Added security headers for production
   - HTTPS redirection and HSTS configuration

4. **Error Handling:**
   - Graceful error handling for missing services
   - Development-specific error responses

## Running Services Locally

1. Ensure Redis is running for state management
2. Start services with Dapr
3. Frontend will connect to local services through Dapr

## References

- [Dapr Documentation](https://docs.dapr.io/)
- [ASP.NET Core CORS](https://docs.microsoft.com/aspnet/core/security/cors)
- [ASP.NET Core Security](https://docs.microsoft.com/aspnet/core/security/)
