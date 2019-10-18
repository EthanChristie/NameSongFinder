using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NameSongFinder.WebApi
{
    public class Program
    {
        public static IContainer Container { get; set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
            var builder = new ContainerBuilder();

            BuildContainer(builder);

            Container = builder.Build();
        }

        private static void BuildContainer(ContainerBuilder builder)
        {
            builder.RegisterType<NameSongFinder>().As<INameSongFinder>();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
