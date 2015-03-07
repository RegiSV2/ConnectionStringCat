using System.Diagnostics.Contracts;

namespace ConStringCat.Core.Model
{
	public sealed class ConnectionStringVariant
	{
		#region Static Stuff

		private static readonly ConnectionStringVariant NullVariant
			= new ConnectionStringVariant("-", "");

		public static ConnectionStringVariant Null
		{
			get { return NullVariant; }
		}

		#endregion

		public ConnectionStringVariant(string name, string value)
		{
			Contract.Requires(!string.IsNullOrEmpty(name));
			Contract.Requires(value != null);
			Name = name;
			Value = value;
		}

		public string Name { get; private set; }

		public string Value { get; private set; }

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(!string.IsNullOrEmpty(Name));
			Contract.Invariant(Value != null);
		}
	}
}