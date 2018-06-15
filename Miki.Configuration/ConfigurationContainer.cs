using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Miki.Configuration
{
	public class ConfigurationContainer
	{
		public List<IConfigurationItem> ConfigurableItems { get; private set; } = new List<IConfigurationItem>();

		public Type Type { get; private set; }

		private object instanceOf = null;

		public ConfigurationContainer(Type t, object instance)
		{
			Type = t;
			instanceOf = instance;
		}

		public void AddType(PropertyInfo type)
		{
			IConfigurationItem item = new ConfigurationItem(type, instanceOf);
			ConfigurableItems.Add(item);
		}
	}
}
