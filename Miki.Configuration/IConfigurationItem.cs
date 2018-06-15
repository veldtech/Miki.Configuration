using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Miki.Configuration
{
    public interface IConfigurationItem
    {
		PropertyInfo Type { get; }

		object GetValue();
		void SetValue(object value);
    }
}
