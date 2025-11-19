using Microsoft.Extensions.Configuration;

namespace EventPlatform.Shared.Utils;

public class UrlHelper
{
    public static string GetBackendUrl(IConfiguration configuration)
    {
        var backendUrl = Environment.GetEnvironmentVariable("BACKEND_URL");
        if (string.IsNullOrWhiteSpace(backendUrl))
        {
            backendUrl = configuration["URLs:BackendURL"];
            if (string.IsNullOrWhiteSpace(backendUrl))
            {
                throw new Exception("Backend URL not configured");
            }
        }
        return backendUrl;
    }

    public static string GetFrontendUrl(IConfiguration configuration)
    {
        var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL");
        if (string.IsNullOrWhiteSpace(frontendUrl))
        {
            frontendUrl = configuration["URLs:FrontendURL"];
            if (string.IsNullOrWhiteSpace(frontendUrl))
            {
                throw new Exception("Frontend URL not configured");
            }
        }
        return frontendUrl;
    }

    public static string GetFastAPIUrl(IConfiguration configuration)
    {
        var fastapiUrl = Environment.GetEnvironmentVariable("FASTAPI_URL");
        if (string.IsNullOrWhiteSpace(fastapiUrl))
        {
            fastapiUrl = configuration["URLs:FastAPIURL"];
            if (string.IsNullOrWhiteSpace(fastapiUrl))
            {
                throw new Exception("fastapi URL not configured");
            }
        }
        return fastapiUrl;
    }
}