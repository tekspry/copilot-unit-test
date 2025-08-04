dapr run `
    --app-id customer `
    --app-port 5295 `
    --dapr-http-port 3505 `
    --components-path ../dapr/components `
    dotnet run