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
		JsonSerializerSettings serializationSettings;

		public JsonSerializationProvider()
		{
			serializationSettings = new JsonSerializerSettings();
			serializationSettings.Formatting = Formatting.Indented;
		}
		public JsonSerializationProvider(JsonSerializerSettings settings)
		{
			serializationSettings = settings;
		}

		public async Task ExportAsync(string file, IEnumerable<ConfigurationContainer> manager)
		{
			Dictionary<string, object> allObjects = new Dictionary<string, object>();

			foreach (var container in manager)
			{
				string containerId = container.Type.Name;

				List<KeyValuePair<string, object>> values = container.ConfigurableItems
					.Select(x => new KeyValuePair<string, object>(x.Type.Name, x.GetValue())).ToList();

				ExpandoObject value = new ExpandoObject();

				foreach (var v in values)
				{
					((IDictionary<string, object>)value).Add(v.Key, v.Value);
				}

				allObjects.Add(containerId, value);
			}

			string json = JsonConvert.SerializeObject(allObjects, serializationSettings);

			using (StreamWriter sw = new StreamWriter(new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
			{
				await sw.WriteAsync(json);
			}
		}

		public async Task ImportAsync(string file, ConfigurationManager manager)
		{
			List<ConfigurationContainer> containers = new List<ConfigurationContainer>();
			string json = "";

			using (StreamReader sr = new StreamReader(new FileStream(file, FileMode.Open, FileAccess.Read)))
			{
				json = await sr.ReadToEndAsync();
			}

			if (string.IsNullOrWhiteSpace(json))
			{
				throw new ArgumentNullException("File contains no data");
			}

			object o = JsonConvert.DeserializeObject(json);

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
		}
	}
}