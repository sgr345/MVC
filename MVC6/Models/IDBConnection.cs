using System.Data;

namespace MVC6.Models
{
	public interface IDBConnection
	{
		int Execute(string sql, object param = null);
		IEnumerable<T> Query<T>(string sql , object param = null);
		Task<IEnumerable<T>> QueryAsync<T>(string sql , object param = null);
		IEnumerable<dynamic> Query(string sql, object param = null);
		Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null);
		Task<T> QueryFirstAsync<T>(string sql, object param = null);
	}
}

