using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using BlazorWPBlog.UI.Services;
using WordPressPCL;

namespace BlazorWPBlog.UI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var wpApiEndpoint = builder.Configuration["WP_Endpoint"];
            var client = new WordPressClient(wpApiEndpoint);
            builder.Services.AddSingleton(client);

            builder.Services.AddSingleton<ITagService, TagService>();
            builder.Services.AddSingleton<ICategoryService, CategoryService>();

            await builder.Build().RunAsync();
        }
    }
}
