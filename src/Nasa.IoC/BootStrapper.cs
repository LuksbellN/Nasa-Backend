using Nasa.Domain.Repositories;
using Nasa.Domain.Services;
using Nasa.Domain.Services.Interfaces;
using Nasa.Repository;
using Nasa.Repository.Repositories;
using Nasa.Repository.Repositories.UoW;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nasa.IoC;

public static class BootStrapper
    {
        public static void Inject(IServiceCollection services, IConfiguration configuration)
        {
            DbConnectionFactory(services, configuration);

            Base(services);
        }

        /// <summary>
        /// Configura a conexão com a base de dados na injecao de dependencia
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        private static void DbConnectionFactory(IServiceCollection services, IConfiguration configuration)
        {
            // Dicionario de conexoes
            var connectionDict = new Dictionary<DatabaseConnectionName, string>
            {
                { DatabaseConnectionName.NpgsqlDbConnection, configuration.GetConnectionString("NpgsqlDbConnection") },
            };

            // Injeta o Dicionario de conexoes
            services.AddSingleton<IDictionary<DatabaseConnectionName, string>>(connectionDict);

            // Injeta a fabrica de conexoes
            services.AddTransient<IDbConnectionFactory, DapperDbConnectionFactory>();
        }

        private static void Base(IServiceCollection services)
        {
            services.AddScoped<ILogsRepository, LogsRepository>();
            services.AddScoped<ILogsService, LogsService>();
            
            services.AddScoped<IRastreamentoTubaroesRepository, RastreamentoTubaroesRepository>();
            services.AddScoped<IRastreamentoTubaroesService, RastreamentoTubaroesService>();
            
            services.AddScoped<IHistoricoAgregadoRepository, HistoricoAgregadoRepository>();
            services.AddScoped<IHistoricoAgregadoService, HistoricoAgregadoService>();
        }

    }