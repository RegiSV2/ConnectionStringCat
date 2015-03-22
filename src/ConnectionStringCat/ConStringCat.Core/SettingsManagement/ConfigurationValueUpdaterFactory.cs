using ConStringCat.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConStringCat.Core.SettingsManagement
{
	/// <summary>
	/// Creates updaters
	/// </summary>
	public interface ConfigurationValueUpdaterFactory
	{
		ConfigurationValueUpdater CreateXmlUpdater(string filePath, string xmlPath);

		ConfigurationValueUpdater CreateJsonUpdater(string filePath, string jsonPath);
	}
}
