using System;
using System.Collections.Generic;
using System.Text;

namespace Miki.Configuration
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ConfigurableAttribute : Attribute
    { }
}
