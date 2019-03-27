using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace KinExplorerCreateAccount
{
    public class Program
    {
        private static readonly Dictionary<string, string> DefaultConfiguration = new Dictionary<string, string>
        {
            {"Horizon_Url", "https://horizon.kinfederation.com/"},
            {"HorizonNetwork_Id", "Kin Mainnet ; December 2018" },
            {"Secret_Seed", "your seed yo" },
            {"App_Id", "kin_explorer" },
            {"Swagger_Enabled", "True" },
        };

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(DefaultConfiguration).AddEnvironmentVariables();
            IConfiguration configuration = builder.Build();

            var host = CreateWebHostBuilder(args).UseConfiguration(configuration).Build();
            host.Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
