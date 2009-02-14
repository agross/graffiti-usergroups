using System.Reflection;

using DnugLeipzig.Definitions.Builders;
using DnugLeipzig.Definitions.Plugins.Talks;

using Rhino.Mocks;

namespace DnugLeipzig.ForTesting.Builders
{
	public class StubbedTalkPluginConfigurationBuilder : EntityBuilder<ITalkPluginConfigurationProvider>
	{
		protected override ITalkPluginConfigurationProvider BuildInstance()
		{
			var result = MockRepository.GenerateMock<ITalkPluginConfigurationProvider>();

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