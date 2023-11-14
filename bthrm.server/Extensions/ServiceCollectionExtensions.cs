using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using bthrm.server.Database;
using bthrm.server.Websockets;
using MySql.Data.MySqlClient;

namespace bthrm.server.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSharedServices(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddLogging();
        services.AddSingleton<Program>();

        //db
        services.AddTransient<IDbConnection>(_ => new MySqlConnection(config.GetConnectionString("DB")));
        services.AddTransient<HeartRateRepository>();
    }
}