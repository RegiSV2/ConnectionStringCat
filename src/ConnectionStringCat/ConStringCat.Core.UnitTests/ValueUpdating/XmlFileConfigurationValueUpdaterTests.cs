using System.IO;
using System.Linq;
using System.Xml;
using ConStringCat.Core.Model;
using ConStringCat.Core.UnitTests.Utils;
using ConStringCat.Core.ValueUpdating;
using NUnit.Framework;

namespace ConStringCat.Core.UnitTests.ValueUpdating
{
	[TestFixture]
	public class XmlFileConfigurationValueUpdaterTests
	{
		private const string TestDocumentPath = "xmlfile.xml";
		private const string ValidXPath = "/catalog";
		private const string ValidDocumentPath = "someFile.xml";

		[SetUp]
		public void SetUp()
		{
			EmbeddedResourceInterop.WriteEmbeddedResourceToFile(
				"ConStringCat.Core.UnitTests.TestXmlFile.xml", TestDocumentPath);
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(TestDocumentPath))
				File.Delete(TestDocumentPath);
		}

		[Test]
		public void Create_InvalidConstructorParameters_ShouldFail()
		{
			Assert.That(() => new XmlFileConfigurationValueUpdater(null, ValidXPath),
				Throws.Exception);
			Assert.That(() => new XmlFileConfigurationValueUpdater(ValidDocumentPath, null),
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
			File.Delete(TestDocumentPath);
			var updater = CreateUpdater(TestDocumentPath, ValidXPath);

			AssertConfigurationValueUpdatingExceptionThrown(updater);
		}

		[Test]
		public void SetNewValue_InvalidXPath_ShouldThrowConnectionStringUpdatingException()
		{
			var updater = CreateUpdater(TestDocumentPath, "some invalid xPath");

			AssertConfigurationValueUpdatingExceptionThrown(updater);
		}

		[Test]
		public void SetNewValue_PathToZeroNodes_ShouldThrowConnectionStringUpdatingException()
		{
			var updater = CreateUpdater(TestDocumentPath, "/nonExistingRoute");

			AssertConfigurationValueUpdatingExceptionThrown(updater);
		}

		private void SetNewValue_TestWithPath(string xPath)
		{
			var updater = CreateUpdater(TestDocumentPath, xPath);

			var newValue = "some new value";
			updater.SetNewValue(newValue);

			var document = new XmlDocument();
			document.Load(TestDocumentPath);
			var nodes = document.SelectNodes(xPath);
			Assert.That(nodes != null && nodes.Count > 0);
			foreach (var node in nodes.Cast<XmlNode>())
			{
				Assert.That(node.InnerXml == newValue);
			}
		}

		private void AssertConfigurationValueUpdatingExceptionThrown(XmlFileConfigurationValueUpdater updater)
		{
			Assert.That(() => updater.SetNewValue("some value"), Throws.InstanceOf<ConfigurationValueUpdatingException>());
		}

		private XmlFileConfigurationValueUpdater CreateUpdater(string filePath, string xPath)
		{
			return new XmlFileConfigurationValueUpdater(filePath, xPath);
		}
	}
}