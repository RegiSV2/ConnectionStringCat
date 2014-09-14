using System.Diagnostics.Contracts;

namespace ConStringCat.Core.Model
{
	public class ConnectionStringVariant
	{
		#region Static Stuff

		private static readonly ConnectionStringVariant NullVariant
			= new ConnectionStringVariant("-", "");

		public static ConnectionStringVariant Null
		{
			get { return NullVariant; }
		}

		#endregion

		public ConnectionStringVariant(string alias, string connectionString)
		{
			Contract.Requires(!string.IsNullOrEmpty(alias));
			Contract.Requires(connectionString != null);
			Alias = alias;
			ConnectionString = connectionString;
		}

		public string Alias { get; private set; }

		public string ConnectionString { get; private set; }

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(!string.IsNullOrEmpty(Alias));
			Contract.Invariant(ConnectionString != null);
		}
	}
}