using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using ConStringCat.Core.Model;

namespace ConStringCat.Core.ValueUpdating
{
	/// <summary>
	///     Updates values in xml files
	/// </summary>
	public sealed class XmlFileConfigurationValueUpdater : ConfigurationValueUpdater
	{
		private const string FileNotFoundMsg = "File \"{0}\" not found";
		private const string InvalidSelectionMsg = "Invalid xml node selection specified";
		private const string NodesNotFoundMsg = "Couldn't find xml nodes to update";

		public XmlFileConfigurationValueUpdater(string documentPath, string xPath)
		{
			Contract.Requires(!string.IsNullOrEmpty(documentPath));
			Contract.Requires(!string.IsNullOrEmpty(xPath));

			DocumentPath = documentPath;
			XPath = xPath;
		}

		public string DocumentPath { get; private set; }
		public string XPath { get; private set; }

		public void SetNewValue(string newValue)
		{
			try
			{
				TrySetValue(newValue);
			}
			catch (FileNotFoundException ex)
			{
				ThrowConnectionStringUpdatingException(string.Format(FileNotFoundMsg, DocumentPath), ex);
			}
			catch (XPathException ex)
			{
				ThrowConnectionStringUpdatingException(InvalidSelectionMsg, ex);
			}
		}

		private void TrySetValue(string newValue)
		{
			var document = new XmlDocument();
			document.PreserveWhitespace = true;
			document.Load(DocumentPath);

			var nodes = document.SelectNodes(XPath);
			if (nodes == null || nodes.Count == 0)
				ThrowConnectionStringUpdatingException(NodesNotFoundMsg);

			Debug.Assert(nodes != null, "nodes != null");
			foreach (var node in nodes.Cast<XmlNode>())
				node.InnerXml = newValue;

			document.Save(DocumentPath);
		}

		private void ThrowConnectionStringUpdatingException(string message)
		{
			throw new ConfigurationValueUpdatingException(message);
		}

		private void ThrowConnectionStringUpdatingException(string message, Exception innerEx)
		{
			throw new ConfigurationValueUpdatingException(message, innerEx);
		}
	}
}