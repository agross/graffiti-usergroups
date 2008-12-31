using System;
using System.Reflection;

using DnugLeipzig.Definitions.Builders;
using DnugLeipzig.Definitions.Configuration.Plugins;

using Rhino.Mocks;

namespace DnugLeipzig.ForTesting.Builders
{
	public class StubbedEventPluginConfigurationBuilder : EntityBuilder<IEventPluginConfiguration>
	{
		protected override IEventPluginConfiguration BuildInstance()
		{
			var result = MockRepository.GenerateMock<IEventPluginConfiguration>();

			foreach (var property in result.GetType().GetProperties(BindingFlags.Public |
			                                                        BindingFlags.GetProperty |
			                                                        BindingFlags.Instance))
			{
				if (property.PropertyType != typeof(string))
				{
					continue;
				}

				result.Stub(x => property.GetValue(x, null)).Return(property.Name);
			}
		
			return result;
		}
	}
}