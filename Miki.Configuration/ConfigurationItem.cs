using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Miki.Configuration
{
	public class ConfigurationItem : IConfigurationItem
	{
		public PropertyInfo Type => type;

		private PropertyInfo type;

		private object parentInstance = null;

		internal ConfigurationItem(PropertyInfo type, object parent)
		{
			this.type = type;
			parentInstance = parent;
		}

		public object GetValue()
		{
			return Type.GetValue(parentInstance);
		}

		public void SetValue(object newValue)
		{
			Type.SetValue(parentInstance, newValue);
		}
	}
}
