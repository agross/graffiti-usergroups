using System.Reflection;

using DnugLeipzig.Definitions.Builders;
using DnugLeipzig.Definitions.Plugins.Events;

using Rhino.Mocks;

namespace DnugLeipzig.ForTesting.Builders
{
	public class StubbedEventPluginConfigurationBuilder : EntityBuilder<IEventPluginConfigurationProvider>
	{
		protected override IEventPluginConfigurationProvider BuildInstance()
		{
			var result = MockRepository.GenerateMock<IEventPluginConfigurationProvider>();

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