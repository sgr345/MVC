using Dapper;
using Npgsql;

namespace MVC6.Models.Providers
{
    public class PostgresProvider : IDBConnection
    {
        private ILogger<PostgresProvider> logger;
        public PostgresProvider(ILogger<PostgresProvider> _logger)
        {
            logger = _logger;
        }
        private NpgsqlConnection conn = new NpgsqlConnection(
            WebApplication.CreateBuilder().Configuration.GetConnectionString(name: "PostgresConnection")
            );

        public int Execute(string sql, object param = null)
        {
            try
            {
                return conn.Execute(sql, param);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }

        }

        public IEnumerable<T> Query<T>(string sql, object param = null)
        {
            IEnumerable<T> result = null;
            try
            {
                result = conn.Query<T>(sql, param);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
            return result;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {
            IEnumerable<T> result = null;
            try
            {
                result = await conn.QueryAsync<T>(sql, param);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
            return result.ToList();
        }

        public IEnumerable<dynamic> Query(string sql, object param = null)
        {
            IEnumerable<dynamic> result = null!;
            try
            {
                result = conn.Query<dynamic>(sql, param);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
            return result;
        }
        public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null)
        {
            IEnumerable<dynamic> result = null!;
            try
            {
                result = await conn.QueryAsync<dynamic>(sql, param);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
            return result.ToList();
        }

        public async Task<T> QueryFirstAsync<T>(string sql, object param =null)
        {
            T result;
            try
            {
                result = await conn.QueryFirstAsync<T>(sql, param);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                throw;
            }
            return result;
        }

    }
}

