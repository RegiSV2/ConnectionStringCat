using System.IO;
using System.Reflection;
using System.Resources;

namespace ConStringCat.Core.Utils
{
	/// <summary>
	///     Provides simple API for access to embedded resources
	/// </summary>
	public static class EmbeddedResourcesHelper
	{
		private const string ResourceNotFoundMsg = "EmbeddedrResource with name {0} not found";

		public static string ReadEmbeddedResourceFile(string fileName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream(fileName))
			{
				if (stream == null)
					throw new MissingManifestResourceException(string.Format(ResourceNotFoundMsg, fileName));
				using (var reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
		}
	}
}