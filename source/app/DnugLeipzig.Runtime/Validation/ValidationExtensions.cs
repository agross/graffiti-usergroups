using DnugLeipzig.Definitions.Specifications;
using DnugLeipzig.Runtime.Specifications;

namespace DnugLeipzig.Runtime.Validation
{
	internal static class ValidationExtensions
	{
		public static bool IsEmail(this string value)
		{
			EmailAddressSpecification spec = new EmailAddressSpecification();
			return spec.IsSatisfiedBy(value);
		}
	}
}