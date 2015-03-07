using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using ConStringCat.Core.Model;
using ConStringCat.Core.ValueUpdating;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.ValueUpdating
{
	[TestFixture]
	public class XmlFileConnectionStringUpdaterTests
	{
		private readonly string _testDocumentPath = "xmlfile.xml";

		private readonly string _validXPath = "/catalog";

		private readonly string _validDocumentPath = "someFile.xml";

		[SetUp]
		public void SetUp()
		{
			var assembly = Assembly.GetExecutingAssembly();
			var resourceName = "ConStringCat.Core.UnitTests.TestXmlFile.xml";

			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				Debug.Assert(stream != null, "stream != null");
				using (var reader = new StreamReader(stream))
				using (var writer = new StreamWriter(_testDocumentPath))
					writer.Write(reader.ReadToEnd());
			}
		}

		[TearDown]
		public void TearDown()
		{
			if(File.Exists(_testDocumentPath))
				File.Delete(_testDocumentPath);
		}

		[Test]
		public void Create_InvalidConstructorParameters_ShouldFail()
		{
			Assert.That(() => new XmlFileConnectionStringUpdater(null, _validXPath),
				Throws.Exception);
			Assert.That(() => new XmlFileConnectionStringUpdater(_validDocumentPath, null),
				Throws.Exception);
		}

		[Test]
		public void SetNewValue_PathToTag_ShouldUpdateDocument()
		{
			var xPath = "/catalog/book[@id='bk102']/price";
			SetNewValue_TestWithPath(xPath);
		}

		[Test]
		public void SetNewValue_PathToAttribute_ShouldUpdateDocument()
		{
			var xPath = "/catalog/book[last()]/@id";
			SetNewValue_TestWithPath(xPath);
		}

		[Test]
		public void SetNewValue_PathToMultipleNodes_ShouldUpdateDocument()
		{
			var xPath = "/catalog/book";
			SetNewValue_TestWithPath(xPath);
		}

		[Test]
		public void SetNewValue_DocumentDoesNotExist_ShouldThrowConnectionStringUpdatingException()
		{
			File.Delete(_testDocumentPath);
			var updater = CreateUpdater(_testDocumentPath, _validXPath);

			AssertConnectionStringUpdatingExceptionThrown(updater);
		}

		[Test]
		public void SetNewValue_InvalidXPath_ShouldThrowConnectionStringUpdatingException()
		{
			var updater = CreateUpdater(_testDocumentPath, "some invalid xPath");

			AssertConnectionStringUpdatingExceptionThrown(updater);
		}

		[Test]
		public void SetNewValue_PathToZeroNodes_ShouldThrowConnectionStringUpdatingException()
		{
			var updater = CreateUpdater(_testDocumentPath, "/nonExistingRoute");

			AssertConnectionStringUpdatingExceptionThrown(updater);
		}

		private void SetNewValue_TestWithPath(string xPath)
		{
			var updater = CreateUpdater(_testDocumentPath, xPath);

			var newValue = "some new value";
			updater.SetNewValue(newValue);

			var document = new XmlDocument();
			document.Load(_testDocumentPath);
			var nodes = document.SelectNodes(xPath);
			Assert.That(nodes != null && nodes.Count > 0);
			foreach (var node in nodes.Cast<XmlNode>())
			{
				Assert.That(node.InnerXml == newValue);
			}
		}

		private void AssertConnectionStringUpdatingExceptionThrown(XmlFileConnectionStringUpdater updater)
		{
			Assert.That(() => updater.SetNewValue("some value"), Throws.InstanceOf<ConnectionStringUpdatingException>());
		}

		private XmlFileConnectionStringUpdater CreateUpdater(string filePath, string xPath)
		{
			return new XmlFileConnectionStringUpdater(filePath, xPath);
		}
	}
}