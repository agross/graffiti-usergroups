namespace DnugLeipzig.Definitions.Validation
{
	public interface IValidator<T>
	{
		ValidationReport Validate(T instance);
	}
}