using System.Collections.Generic;

using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.ForTesting;

namespace DnugLeipzig.Runtime.Tests.Validation
{
	public abstract class With_validator_for<T> : Spec
	{
		T _dataToValidate;
		IValidator<T> _sut;

		protected internal IEnumerable<INotification> Notifications
		{
			get;
			private set;
		}

		protected override void Establish_context()
		{
			_sut = CreateValidator();
			_dataToValidate = CreateDataToValidate();
		}

		protected abstract T CreateDataToValidate();
		protected abstract IValidator<T> CreateValidator();

		protected override void Because()
		{
			Notifications = _sut.Validate(_dataToValidate);
		}
	}
}