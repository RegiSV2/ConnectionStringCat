using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public string Alias { get; private set; }

		public string ConnectionString { get; private set; }

		public ConnectionStringVariant(string alias, string connectionString)
		{
			Contract.Requires(!string.IsNullOrEmpty(alias));
			Contract.Requires(connectionString != null);
			Alias = alias;
			ConnectionString = connectionString;
		}

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(!string.IsNullOrEmpty(Alias));
			Contract.Invariant(ConnectionString != null);
		}
	}
}
