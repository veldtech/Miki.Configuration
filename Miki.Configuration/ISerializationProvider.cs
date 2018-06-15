using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Configuration
{
	public interface ISerializationProvider
	{
		Task ExportAsync(string file, IEnumerable<ConfigurationContainer> containers);
		Task ImportAsync(string file, ConfigurationManager containers);
	}
}
