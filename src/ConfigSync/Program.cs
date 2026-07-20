using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace ConfigSync;

public class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        WebApplication app = builder.Build();

        app.Run();
    }
}