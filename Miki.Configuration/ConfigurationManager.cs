using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Miki.Configuration
{
	public class ConfigurationManager
	{
		public IReadOnlyList<ConfigurationContainer> Containers 
			=> configurationItems;

		private List<ConfigurationContainer> configurationItems = new List<ConfigurationContainer>();

		public ConfigurationManager()
		{

		}

		public async Task ExportAsync(ISerializationProvider provider, string filePath)
		{
			await provider.ExportAsync(filePath, configurationItems);
		}

		public ConfigurationContainer GetContainer<T>()
			where T : class
		{
			return configurationItems.FirstOrDefault(x => x.Type.GetHashCode() == typeof(T).GetHashCode());
		}

		public async Task ImportAsync(ISerializationProvider provider, string filePath)
		{
			await provider.ImportAsync(filePath, this);
		}

		public void RegisterType(Type type, object instance)
		{
			var configurables = type.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				.Where(y => y.GetCustomAttributes<ConfigurableAttribute>().Count() > 0).ToList();

			if (configurables.Count == 0)
			{
				return;
			}

			ConfigurationContainer container = new ConfigurationContainer(type, instance);

			foreach (var configurable in configurables)
			{
				container.AddType(configurable);
			}

			configurationItems.Add(container);
		}
		public void RegisterType<T>(T instance)
		{
			RegisterType(typeof(T), instance);
		}
	}
}