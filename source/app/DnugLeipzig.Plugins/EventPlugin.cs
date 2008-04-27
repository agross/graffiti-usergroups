using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Plugins.Migration;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	public class EventPlugin : GraffitiEvent, IEventPluginConfiguration, ISupportsMemento
	{
		internal const string Form_CategoryName = "categoryName";
		internal const string Form_CreateTargetCategoryAndFields = "createTargetCategoryAndFields";
		internal const string Form_DateFormat = "dateFormat";
		internal const string Form_DefaultLocation = "defaultLocation";
		internal const string Form_DefaultMaximumNumberOfRegistrations = "defaultMaximumNumberOfRegistrations";
		internal const string Form_DefaultRegistrationRecipient = "defaultRegistrationRecipient";
		internal const string Form_EndDateField = "endDateField";
		internal const string Form_LocationField = "locationField";
		internal const string Form_LocationUnknownField = "locationUnknown";
		internal const string Form_MaximumNumberOfRegistrationsField = "maximumNumberOfRegistrations";
		internal const string Form_MigrateFieldValues = "migrate";
		internal const string Form_NumberOfRegistrationsField = "numberOfRegistrations";
		internal const string Form_RegistrationMailSubject = "registrationMailSubject";
		internal const string Form_RegistrationNeededField = "registrationNeeded";
		internal const string Form_RegistrationRecipientField = "registrationRecipient";
		internal const string Form_ShortEndDateFormat = "shortDateFormat";
		internal const string Form_SpeakerField = "speakerField";
		internal const string Form_StartDateField = "startDateField";
		internal const string Form_UnknownText = "unknownText";
		internal const string Form_YearQueryString = "yearQueryString";
		readonly ICategoryRepository _categoryRepository;

		public EventPlugin() : this(new CategoryRepository())
		{
			// Initialize with default values.
			CategoryName = "Events";
			StartDateField = "Start Date";
			EndDateField = "End Date";
			SpeakerField = "Speaker";
			DateFormat = "on {0:D}, at {0:t}";
			ShortEndDateFormat = "at {0:t}";
			LocationField = "Location";
			UnknownText = "(To be announced)";
			YearQueryString = "year";
			LocationUnknownField = "Location is unknown";
			RegistrationNeededField = "Registration needed";
			RegistrationRecipientField = "Registration recipient e-mail address";
			MaximumNumberOfRegistrationsField = "Maximum number of registrations";
			NumberOfRegistrationsField = "Number of registrations";
			RegistrationMailSubject = "New Registration";
			DefaultRegistrationRecipient = CommentSettings.Get().Email;

			EnableEventHandlers = true;
		}

		public EventPlugin(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		public override string Name
		{
			get { return "Events Plugin"; }
		}

		public override bool IsEditable
		{
			get { return true; }
		}

		public override string Description
		{
			get { return "Extends Graffiti CMS for events management."; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether to enable validation and default valie event handlers. Event handlers will be disabled during migration.
		/// </summary>
		/// <value><c>true</c> if event handlers are enabled; otherwise, <c>false</c>.</value>
		static bool EnableEventHandlers
		{
			get;
			set;
		}

		public string DefaultLocation
		{
			get;
			set;
		}

		public string DefaultMaximumNumberOfRegistrations
		{
			get;
			set;
		}

		public string DefaultRegistrationRecipient
		{
			get;
			set;
		}

		#region IEventPluginConfiguration Members
		public string SortRelevantDateField
		{
			get { return StartDateField; }
		}

		public string CategoryName
		{
			get;
			set;
		}

		public string SpeakerField
		{
			get;
			set;
		}

		public string YearQueryString
		{
			get;
			set;
		}

		public string DateFormat
		{
			get;
			set;
		}

		public string EndDateField
		{
			get;
			set;
		}

		public string LocationField
		{
			get;
			set;
		}

		public string ShortEndDateFormat
		{
			get;
			set;
		}

		public string StartDateField
		{
			get;
			set;
		}

		public string UnknownText
		{
			get;
			set;
		}

		public string LocationUnknownField
		{
			get;
			set;
		}

		public string RegistrationNeededField
		{
			get;
			set;
		}

		public string RegistrationRecipientField
		{
			get;
			set;
		}

		public string MaximumNumberOfRegistrationsField
		{
			get;
			set;
		}

		public string NumberOfRegistrationsField
		{
			get;
			set;
		}

		public string RegistrationMailSubject
		{
			get;
			set;
		}
		#endregion

		public override void Init(GraffitiApplication ga)
		{
			Debug.WriteLine("Init Event Plugin");

			ga.BeforeValidate += Post_Validate;
			ga.BeforeInsert += Post_SetDefaultValues;
			ga.BeforeUpdate += Post_SetDefaultValues;
		}

		internal void Post_Validate(DataBuddyBase dataObject, EventArgs e)
		{
			if (!EnableEventHandlers)
			{
				return;
			}

			Post post = dataObject as Post;
			if (post == null)
			{
				return;
			}

			if (post.Category.Name != CategoryName)
			{
				return;
			}

			// Validate input.
			DateTime? startDate = null;
			DateTime? endDate = null;

			if (!post[StartDateField].IsNullOrEmptyTrimmed())
			{
				if (!Validator.ValidateDate(post[StartDateField]))
				{
					throw new ValidationException("Please enter a valid date.", StartDateField);
				}

				startDate = DateTime.Parse(post[StartDateField]);
			}

			if (!post[EndDateField].IsNullOrEmptyTrimmed())
			{
				if (!Validator.ValidateDate(post[EndDateField]))
				{
					throw new ValidationException("Please enter a valid date.", EndDateField);
				}

				endDate = DateTime.Parse(post[EndDateField]);
			}

			if (!startDate.HasValue && endDate.HasValue)
			{
				throw new ValidationException("Please enter a start date if the end date is set.", StartDateField);
			}

			if (startDate.HasValue && endDate.HasValue && startDate > endDate)
			{
				throw new ValidationException("Please enter a start date that is less or equal than the end date.",
				                              StartDateField,
				                              EndDateField);
			}

			if (post[LocationUnknownField].IsChecked() && !post[LocationField].IsNullOrEmptyTrimmed())
			{
				throw new ValidationException(
					String.Format(
						"Either check the '{0}' field the or enter a location. You can leave the '{0}' field unchecked and the '{1}' field empty to apply the default location value '{2}'.",
						LocationUnknownField,
						LocationField,
						DefaultLocation),
					LocationUnknownField,
					LocationField);
			}

			if (!post[MaximumNumberOfRegistrationsField].IsNullOrEmptyTrimmed())
			{
				if (!Validator.ValidateInt(post[MaximumNumberOfRegistrationsField]))
				{
					throw new ValidationException("Please enter a valid integer value.", MaximumNumberOfRegistrationsField);
				}

				int maximumNumberOfRegistrations = int.Parse(post[MaximumNumberOfRegistrationsField]);

				if (!Validator.ValidateRange(maximumNumberOfRegistrations, 0, int.MaxValue))
				{
					throw new ValidationException(String.Format("Please enter a value greater or equal than 0."),
					                              MaximumNumberOfRegistrationsField);
				}
			}

			if (!post[NumberOfRegistrationsField].IsNullOrEmptyTrimmed())
			{
				if (!Validator.ValidateInt(post[NumberOfRegistrationsField]))
				{
					throw new ValidationException("Please enter a valid integer value.", NumberOfRegistrationsField);
				}

				int numberOfRegistrations = int.Parse(post[NumberOfRegistrationsField]);

				if (!Validator.ValidateRange(numberOfRegistrations, 0, int.MaxValue))
				{
					throw new ValidationException(String.Format("Please enter a value greater or equal than 0."),
					                              NumberOfRegistrationsField);
				}
			}

			if (!post[RegistrationRecipientField].IsNullOrEmptyTrimmed())
			{
				if (!Validator.ValidateEmail(post[RegistrationRecipientField]))
				{
					throw new ValidationException(String.Format("Please enter a valid e-mail address."), RegistrationRecipientField);
				}
			}
		}

		internal void Post_SetDefaultValues(DataBuddyBase dataObject, EventArgs e)
		{
			if (!EnableEventHandlers)
			{
				return;
			}

			Post post = dataObject as Post;
			if (post == null)
			{
				return;
			}

			if (post.Category.Name != CategoryName)
			{
				return;
			}

			// Set default location if no location is given.
			if (!post[LocationUnknownField].IsChecked() && post[LocationField].IsNullOrEmptyTrimmed())
			{
				post[LocationField] = DefaultLocation;
				Util.ForcePropertyUpdate(post);
			}

			// Set default number maximum number of registrations.
			if (post[MaximumNumberOfRegistrationsField].IsNullOrEmptyTrimmed())
			{
				post[MaximumNumberOfRegistrationsField] = DefaultMaximumNumberOfRegistrations;
				Util.ForcePropertyUpdate(post);
			}

			// Set default registration recipient.
			if (post[RegistrationRecipientField].IsNullOrEmptyTrimmed())
			{
				post[RegistrationRecipientField] = DefaultRegistrationRecipient;
				Util.ForcePropertyUpdate(post);
			}
		}

		#region Settings
		protected override FormElementCollection AddFormElements()
		{
			return new FormElementCollection
			       {
			       	new CheckFormElement(Form_CreateTargetCategoryAndFields,
			       	                     "Create category and fields",
			       	                     "Check to automatically create the category and custom fields.",
			       	                     true),
			       	new CheckFormElement(Form_MigrateFieldValues,
			       	                     "Migrate custom field values",
			       	                     "Check to automatically migrate posts and custom field values if category and/or field names change.",
			       	                     true),
			       	new TextFormElement(Form_CategoryName,
			       	                    "Graffiti events category",
			       	                    "Enter the name of the category to store events, e.g. \"Events\"."),
			       	new TextFormElement(Form_StartDateField,
			       	                    "\"Event start date and time\" field",
			       	                    "Enter the name of the custom text field to store the events's start date (and, optionally, time), e.g. \"Start Date\". This field will be validated to be either empty or hold a correct date time/value."),
			       	new TextFormElement(Form_EndDateField,
			       	                    "\"Event end date and time\" field",
			       	                    "Enter the name of the custom text field to store the events's end date (and, optionally, time), e.g. \"End Date\". This field will be validated to be either empty or hold a correct date time/value."),
			       	new TextFormElement(Form_SpeakerField,
			       	                    "\"Speaker\" field",
			       	                    "Enter the name of the custom text field to store the speaker's name, e.g. \"Speaker\"."),
			       	new TextFormElement(Form_DateFormat,
			       	                    "Date/time format",
			       	                    "Enter .NET format string to be used for the start date and the end date, e.g. \"on {0:D}, at {0:t}\". Leave blank to use the Graffiti date format from web.config."),
			       	new TextFormElement(Form_ShortEndDateFormat,
			       	                    "Short end date/time format",
			       	                    "Enter .NET format string to be used for the end date if the start date and end date of the event is the same day, e.g. \"at {0:t}\". Leave blank to use the Graffiti date format from above."),
			       	new TextFormElement(Form_LocationField,
			       	                    "Event location field",
			       	                    "Enter the name of the custom text field to store the event's location, e.g. \"Location\"."),
			       	new TextFormElement(Form_LocationUnknownField,
			       	                    "\"Location unknown\" field",
			       	                    "Enter the name of the custom checkbox field to store if the event location is unknown, e.g. \"Location is unknown\"."),
			       	new TextFormElement(Form_DefaultLocation,
			       	                    "Default event location",
			       	                    "Enter the default value of the locaton if you don't enter one, e.g. \"Initech Corp., Floor 1\". This value will be used to fill the event location field above if \"Location unknown\" is not checked."),
			       	new TextFormElement(Form_UnknownText,
			       	                    "\"Unknown\" text",
			       	                    "Enter the text to be displayed if event information (dates, speaker, location) is not yet known, e.g. \"(To be announced)\"."),
			       	new TextFormElement(Form_RegistrationNeededField,
			       	                    "\"Registration needed\" field",
			       	                    "Enter the name of the custom checkbox field to store if registration for the event is needed, e.g. \"Registration needed\"."),
			       	new TextFormElement(Form_RegistrationRecipientField,
			       	                    "\"Registration recipient e-mail address\" field",
			       	                    "Enter the name of the custom textbox field to store the registration recipient e-mail address, e.g. \"Registration recipient e-mail address\"."),
			       	new TextFormElement(Form_DefaultRegistrationRecipient,
			       	                    "Default registration recipient e-mail address",
			       	                    "Enter the default registration e-mail address, e.g. \"registration@example.com\"."),
			       	new TextFormElement(Form_RegistrationMailSubject,
			       	                    "Registration e-mail subject",
			       	                    "Enter the registration e-mail subject, e.g. \"New Registration\"."),
			       	new TextFormElement(Form_MaximumNumberOfRegistrationsField,
			       	                    "\"Maximum number of registrations\" field",
			       	                    "Enter the name of the custom textbox field to store the maximum number of registrations for the event, e.g. \"Maximum number of registrations\"."),
			       	new TextFormElement(Form_DefaultMaximumNumberOfRegistrations,
			       	                    "Default maximum number of registrations",
			       	                    "Enter the default maximum number of registrations, e.g. \"100\". If that number is reached, new registrations will not be possible. Leave blank to allow unlimited registrations."),
			       	new TextFormElement(Form_NumberOfRegistrationsField,
			       	                    "\"Number of registrations\" field",
			       	                    "Enter the name of the custom textbox field to store the number of received registrations for the event, e.g. \"Number of registrations\"."),
			       	new TextFormElement(Form_YearQueryString,
			       	                    "Query string parameter for paging by year",
			       	                    "Enter a value for the query string parameter used to display talks of a specific year.")
			       };
		}

		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			IMemento oldState;
			IMemento newState;

			try
			{
				HttpContext.Current.Cache.Remove(EventPluginConfiguration.CacheKey);

				// Validation.
				if (!Validator.ValidateExisting(nvc[Form_CategoryName]))
				{
					throw new ValidationException("Please enter a category name.");
				}

				string categoryName = HttpUtility.HtmlEncode(nvc[Form_CategoryName]);
				if (!nvc[Form_CreateTargetCategoryAndFields].IsChecked() && !_categoryRepository.IsExistingCategory(categoryName))
				{
					throw new ValidationException(String.Format("The category '{0}' does not exist.", categoryName), StatusType.Warning);
				}

				if (!nvc[Form_DefaultRegistrationRecipient].IsNullOrEmptyTrimmed())
				{
					if (!Validator.ValidateEmail(nvc[Form_DefaultRegistrationRecipient]))
					{
						throw new ValidationException("Please enter a valid e-mail address for the default registration recipient.");
					}
				}

				if (!nvc[Form_DefaultMaximumNumberOfRegistrations].IsNullOrEmptyTrimmed())
				{
					if (!Validator.ValidateInt(nvc[Form_DefaultMaximumNumberOfRegistrations]))
					{
						throw new ValidationException(
							"Please enter a valid integer value for the default maximum number of registrations.");
					}

					if (!Validator.ValidateRange(int.Parse(nvc[Form_DefaultMaximumNumberOfRegistrations]), 0, int.MaxValue))
					{
						throw new ValidationException(
							"Please enter a value greater or equal than 0 for the default maximum number of registrations.");
					}
				}

				if (!Validator.ValidateExisting(nvc[Form_YearQueryString]))
				{
					throw new ValidationException("Please enter a year query string parameter.");
				}

				// Write back.
				oldState = CreateMemento();

				CategoryName = categoryName;
				StartDateField = nvc[Form_StartDateField];
				EndDateField = nvc[Form_EndDateField];
				SpeakerField = nvc[Form_SpeakerField];
				DateFormat = nvc[Form_DateFormat];
				ShortEndDateFormat = nvc[Form_ShortEndDateFormat];
				LocationField = nvc[Form_LocationField];
				UnknownText = nvc[Form_UnknownText];
				LocationUnknownField = nvc[Form_LocationUnknownField];
				YearQueryString = nvc[Form_YearQueryString];
				DefaultLocation = nvc[Form_DefaultLocation];
				RegistrationNeededField = nvc[Form_RegistrationNeededField];
				RegistrationRecipientField = nvc[Form_RegistrationRecipientField];
				DefaultRegistrationRecipient = nvc[Form_DefaultRegistrationRecipient];
				MaximumNumberOfRegistrationsField = nvc[Form_MaximumNumberOfRegistrationsField];
				DefaultMaximumNumberOfRegistrations = nvc[Form_DefaultMaximumNumberOfRegistrations];
				NumberOfRegistrationsField = nvc[Form_NumberOfRegistrationsField];
				RegistrationMailSubject = nvc[Form_RegistrationMailSubject];

				newState = CreateMemento();
			}
			catch (ValidationException ex)
			{
				SetMessage(context, ex.Message);
				return ex.Severity;
			}
			catch (Exception ex)
			{
				SetMessage(context, String.Format("Error: {0}", ex.Message));
				return StatusType.Error;
			}

			try
			{
				EnableEventHandlers = false;

				PluginMigrator.MigrateSettings(nvc[Form_CreateTargetCategoryAndFields].IsChecked(),
				        nvc[Form_MigrateFieldValues].IsChecked(),
				        newState,
				        oldState);
				return StatusType.Success;
			}
			catch (Exception ex)
			{
				SetMessage(context, String.Format("Error while migrating category and fields: {0}", ex.Message));
				return StatusType.Error;
			}
			finally
			{
				EnableEventHandlers = true;
			}
		}

		protected override NameValueCollection DataAsNameValueCollection()
		{
			var nvc = new NameValueCollection();

			nvc[Form_CategoryName] = HttpUtility.HtmlDecode(CategoryName);
			nvc[Form_StartDateField] = StartDateField;
			nvc[Form_EndDateField] = EndDateField;
			nvc[Form_SpeakerField] = SpeakerField;
			nvc[Form_DateFormat] = DateFormat;
			nvc[Form_ShortEndDateFormat] = ShortEndDateFormat;
			nvc[Form_LocationField] = LocationField;
			nvc[Form_UnknownText] = UnknownText;
			nvc[Form_LocationUnknownField] = LocationUnknownField;
			nvc[Form_YearQueryString] = YearQueryString;
			nvc[Form_DefaultLocation] = DefaultLocation;
			nvc[Form_RegistrationNeededField] = RegistrationNeededField;
			nvc[Form_RegistrationRecipientField] = RegistrationRecipientField;
			nvc[Form_DefaultRegistrationRecipient] = DefaultRegistrationRecipient;
			nvc[Form_MaximumNumberOfRegistrationsField] = MaximumNumberOfRegistrationsField;
			nvc[Form_DefaultMaximumNumberOfRegistrations] = DefaultMaximumNumberOfRegistrations;
			nvc[Form_NumberOfRegistrationsField] = NumberOfRegistrationsField;
			nvc[Form_RegistrationMailSubject] = RegistrationMailSubject;

			return nvc;
		}
		#endregion

		#region ISupportsMemento Members
		public IMemento CreateMemento()
		{
			return new EventPluginMemento(this);
		}
		#endregion
	}
}