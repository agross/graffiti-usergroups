using System.Collections.Generic;

using DnugLeipzig.Definitions.GraffitiIntegration;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.GraffitiIntegration
{
	public class GraffitiEmailContext : IGraffitiEmailContext
	{
		readonly Dictionary<string, object> _values = new Dictionary<string, object>();

		#region Implementation of IGraffitiEmailContext
		public object Put(string key, object value)
		{
			if (_values.ContainsKey(key))
			{
				_values[key] = value;
			}
			else
			{
				_values.Add(key, value);
			}

			return value;
		}

		public EmailTemplateToolboxContext ToEmailTemplateToolboxContext()
		{
			EmailTemplateToolboxContext result = new EmailTemplateToolboxContext();
			foreach (var keyValuePair in _values)
			{
				result.Put(keyValuePair.Key, keyValuePair.Value);
			}

			return result;
		}
		#endregion
	}
}