using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Miki.Configuration
{
	public interface ISerializationProvider
	{
		Task ExportAsync(string file, IEnumerable<ConfigurationContainer> containers);
		Task ImportAsync(string file, ConfigurationManager containers);
	}

	public class ConfigurationManager
	{
		public IReadOnlyList<ConfigurationContainer> Containers 
			=> configurationItems;

		private List<ConfigurationContainer> configurationItems = new List<ConfigurationContainer>();

		public ConfigurationManager()
		{
			//var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
			//foreach (Assembly a in allAssemblies)
			//{
			//	var configurables = a.GetTypes()
			//		.Where(x => x.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			//				.Where(y => y.GetCustomAttributes<ConfigurableAttribute>().Count() > 0).Count() > 0)
			//		.ToList();
			//	if (configurables.Count > 0)
			//	{
			//		foreach (var configurable in configurables)
			//		{
			//			ConfigurationContainer container = new ConfigurationContainer(configurable);
			//			var allConfigurables = configurable.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
			//				.Where(y => y.GetCustomAttributes<ConfigurableAttribute>().Count() > 0)
			//				.ToList();

			//			foreach (var c in allConfigurables)
			//			{
			//				container.AddType(c);
			//			}

			//			configurationItems.Add(configurable.GUID, container);
			//		}
			//	}
			//}
		}

		public async Task ExportAsync(ISerializationProvider provider, string filePath)
		{
			await provider.ExportAsync(filePath, configurationItems);
		}

		public ConfigurationContainer GetContainer<T>()
			where T : class
		{
			return configurationItems.FirstOrDefault(x => x.Type.GUID == typeof(T).GUID);
		}

		public async Task ImportAsync(ISerializationProvider provider, string filePath)
		{
			await provider.ImportAsync(filePath, this);
		}

		public void RegisterType(Type type, object instance)
		{
			var configurables = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
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

	public class ConfigurationContainer
	{
		public List<ConfigurationItem> ConfigurableItems { get; private set; } = new List<ConfigurationItem>();

		public Type Type { get; private set; }

		private object instanceOf = null;

		public ConfigurationContainer(Type t, object instance)
		{
			Type = t;
			instanceOf = instance;
		}

		public void AddType(PropertyInfo type)
		{
			ConfigurationItem item = new ConfigurationItem(type, instanceOf);
			ConfigurableItems.Add(item);
		}
	}

	public class ConfigurationItem
	{
		public PropertyInfo Type { get; private set; }

		private object parentInstance = null;

		internal ConfigurationItem(PropertyInfo type, object parent)
		{
			Type = type;
			parentInstance = parent;
		}

		public T GetValue<T>()
		{
			return (T)Type.GetValue(parentInstance);
		}

		public void SetValue(object newValue)
		{
			Type.SetValue(parentInstance, newValue);
		}
	}
}