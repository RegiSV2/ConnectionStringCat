using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConStringCat.Core.UnitTests.Utils
{
	/// <summary>
	/// Provides operations for work with embedded resources
	/// </summary>
	public static class EmbeddedResourceInterop
	{
		/// <summary>
		/// Writes embedded resource to external file
		/// </summary>
		public static void WriteEmbeddedResourceToFile(string embeddedResourceName, string fileName)
		{
			var assembly = Assembly.GetExecutingAssembly();

			using (var stream = assembly.GetManifestResourceStream(embeddedResourceName))
			{
				Debug.Assert(stream != null, "stream != null");
				using (var reader = new StreamReader(stream))
				using (var writer = new StreamWriter(fileName))
					writer.Write(reader.ReadToEnd());
			}
		}
	}
}
