using System;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace eShop.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseKestrel()
                .UseUrls("http://0.0.0.0:5001")
                .UseStartup<Startup>()
                .Build();
    }
}
