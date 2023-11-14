using dotenv.net;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Net;
using bthrm.server.Extensions;
using bthrm.server.Websockets;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace bthrm.server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine($"Starting server!");
            DotEnv.Load(new DotEnvOptions(true, probeForEnv: true, probeLevelsToSearch: 5));
            var config = new ConfigurationBuilder().AddEnvironmentVariables().Build();
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddSharedServices(config);

            services.AddSingleton<HeartRateSessionManager>();

            var provider = services.BuildServiceProvider();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };

            var server = new HeartRateServer(IPAddress.Any, 4001, provider, config);
            server.Start();

            while (true)
            {
                await Task.Delay(1000);
            }
        }
    }
}