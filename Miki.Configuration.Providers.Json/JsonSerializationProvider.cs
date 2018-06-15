using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Miki.Configuration
{
	public class JsonSerializationProvider : ISerializationProvider
	{
		public async Task ExportAsync(string file, IEnumerable<ConfigurationContainer> manager)
		{
			StreamWriter sw = new StreamWriter(file);

			Dictionary<string, object> ok = new Dictionary<string, object>();

			foreach(var container in manager)
			{
				string containerId = container.Type.Name;

				List<KeyValuePair<string, object>> values = container.ConfigurableItems.Select(x => new KeyValuePair<string, object>(x.Type.Name, x.GetValue())).ToList();

				ExpandoObject value = new ExpandoObject();

				foreach(var v in values)
				{
					((IDictionary<string, object>)value).Add(v.Key, v.Value);
				}

				ok.Add(containerId, value);
			}

			await sw.WriteAsync(JsonConvert.SerializeObject(ok, Formatting.Indented));
			sw.Flush();
			sw.Close();
		}

		public async Task ImportAsync(string file, ConfigurationManager manager)
		{
			List<ConfigurationContainer> containers = new List<ConfigurationContainer>();

			StreamReader sr = new StreamReader(file);
			string fileContent = await sr.ReadToEndAsync();
			object o = JsonConvert.DeserializeObject(fileContent);

			if (o is JObject ob)
			{
				foreach (JToken container in ob.Children())
				{
					if (container is JProperty property)
					{
						ConfigurationContainer newContainer = manager.Containers.FirstOrDefault(x => x.Type.Name == property.Name);
						if (newContainer != null)
						{
							foreach (JObject value in property.Children())
							{
								foreach (JToken obj in value.Children())
								{
									if (obj is JProperty valueProperty)
									{
										IConfigurationItem item = newContainer.ConfigurableItems.FirstOrDefault(x => x.Type.Name == valueProperty.Name);
										item.SetValue(valueProperty.Value.ToObject(item.Type.PropertyType));
									}
								}
							}
						}
					}
				}
			}

			sr.Close();
		}
	}
}
