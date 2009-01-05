namespace DnugLeipzig.Definitions.Validation
{
	public interface IValidator<T>
	{
		NotificationResult Validate(T instance);
	}
}