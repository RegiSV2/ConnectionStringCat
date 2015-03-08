using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EnvDTE80;
using Newtonsoft.Json.Schema;

namespace ConStringCat.Core.Utils
{
	/// <summary>
	/// Provides simple API for access to embedded resources
	/// </summary>
	public static class EmbeddedResourcesHelper
	{
		public static string ReadEmbeddedResourceFile(string fileName)
		{
			var assembly = Assembly.GetExecutingAssembly();
			using (var stream = assembly.GetManifestResourceStream(fileName))
			{
				Contract.Assert(stream != null);
				using (var reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
		}
	}
}
