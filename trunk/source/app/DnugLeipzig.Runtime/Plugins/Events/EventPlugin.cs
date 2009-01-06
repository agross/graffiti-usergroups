using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Configuration.Plugins;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Mapping;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Plugins.Migration;
using DnugLeipzig.Runtime.Plugins.Talks;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Events
{
	public partial class EventPlugin : GraffitiEvent, IEventPluginConfigurationProvider, ISupportsMemento
	{
		readonly IPostRepository _postRepository;
		readonly IGraffitiCommentSettings _settings;
		readonly IMapper<NameValueCollection, EventPluginSettings> _settingsMapper;
		readonly IValidator<EventPluginSettings> _settingsValidator;

		public EventPlugin() : this(IoC.Resolve<IPostRepository>(),
		                            IoC.Resolve<IGraffitiCommentSettings>(),
		                            IoC.Resolve<IMapper<NameValueCollection, EventPluginSettings>>(),
									IoC.Resolve<IValidator<EventPluginSettings>>())
		{
			// Initialize default values.
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
			RegistrationListField = "Registration list";
			RegistrationMailSubject = "New Registration";
			// TODO			DefaultRegistrationRecipient = _settings.Email;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventPlugin"/> class.
		/// This constructor is used for dependency injection in unit testing scenarios.
		/// </summary>
		internal EventPlugin(IPostRepository postRepository,
		                     IGraffitiCommentSettings commentSettings,
		                     IMapper<NameValueCollection, EventPluginSettings> settingsMapper,
							 IValidator<EventPluginSettings> settingsValidator)
		{
			_postRepository = postRepository;
			_settings = commentSettings;
			_settingsMapper = settingsMapper;
			_settingsValidator = settingsValidator;

			EnableEventHandlers = true;
		}

		public override string Name
		{
			get { return "Event Plugin"; }
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
		/// Gets or sets a value indicating whether to enable post validation and post default event handlers. Event handlers will
		/// be disabled during migration.
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

		#region IEventPluginConfigurationProvider Members
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

		public string RegistrationListField
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

		#region ISupportsMemento Members
		public IMemento CreateMemento()
		{
			return new EventPluginMemento(this);
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

			if (!_postRepository.GetCategoryNameOf(post).Equals(CategoryName, StringComparison.OrdinalIgnoreCase))
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

			if (post[LocationUnknownField].IsSelected() && !post[LocationField].IsNullOrEmptyTrimmed())
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

			if (!_postRepository.GetCategoryNameOf(post).Equals(CategoryName, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			// Set default location if no location is given.
			if (!post[LocationUnknownField].IsSelected() && post[LocationField].IsNullOrEmptyTrimmed())
			{
				post[LocationField] = DefaultLocation;
				post.ForcePropertyUpdate();
			}

			// Set default number maximum number of registrations.
			if (post[MaximumNumberOfRegistrationsField].IsNullOrEmptyTrimmed())
			{
				post[MaximumNumberOfRegistrationsField] = DefaultMaximumNumberOfRegistrations;
				post.ForcePropertyUpdate();
			}

			// Set default registration recipient.
			if (post[RegistrationRecipientField].IsNullOrEmptyTrimmed())
			{
				post[RegistrationRecipientField] = DefaultRegistrationRecipient;
				post.ForcePropertyUpdate();
			}
		}

		#region Settings
		protected override FormElementCollection AddFormElements()
		{
			return Fields.AsFormElements();
		}

		public override StatusType SetValues(HttpContext context, NameValueCollection nvc)
		{
			IMemento oldState;
			IMemento newState;

			EventPluginSettings settings = new EventPluginSettings();
			_settingsMapper.Map(nvc, settings);

			try
			{
				var validation = _settingsValidator.Validate(settings).Interpret();
				if (validation.Failed)
				{
					SetMessage(context, validation.Message);
					return validation.StatusType;
				}

				// Write back.
				oldState = CreateMemento();

				CategoryName = HttpUtility.HtmlEncode(nvc[Fields.CategoryName]);
				StartDateField = nvc[Fields.StartDate];
				EndDateField = nvc[Fields.EndDate];
				SpeakerField = nvc[Fields.Speaker];
				DateFormat = nvc[Fields.DateFormat];
				ShortEndDateFormat = nvc[Fields.ShortEndDateFormat];
				LocationField = nvc[Fields.Location];
				UnknownText = nvc[Fields.UnknownText];
				LocationUnknownField = nvc[Fields.LocationUnknown];
				YearQueryString = nvc[Fields.YearQueryString];
				DefaultLocation = nvc[Fields.DefaultLocation];
				RegistrationNeededField = nvc[Fields.RegistrationNeeded];
				RegistrationRecipientField = nvc[Fields.RegistrationRecipient];
				DefaultRegistrationRecipient = nvc[Fields.DefaultRegistrationRecipient];
				MaximumNumberOfRegistrationsField = nvc[Fields.MaximumNumberOfRegistrations];
				DefaultMaximumNumberOfRegistrations = nvc[Fields.DefaultMaximumNumberOfRegistrations];
				RegistrationListField = nvc[Fields.RegistrationList];
				RegistrationMailSubject = nvc[Fields.RegistrationMailSubject];

				newState = CreateMemento();
			}
			catch (Exception ex)
			{
				SetMessage(context, String.Format("Error: {0}", ex.Message));
				return StatusType.Error;
			}

			try
			{
				EnableEventHandlers = false;

				PluginMigrator.MigrateSettings(nvc[Fields.CreateTargetCategoryAndFields].IsSelected(),
				                               nvc[Fields.MigrateFieldValues].IsSelected(),
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

			nvc[Fields.CategoryName] = HttpUtility.HtmlDecode(CategoryName);
			nvc[Fields.StartDate] = StartDateField;
			nvc[Fields.EndDate] = EndDateField;
			nvc[Fields.Speaker] = SpeakerField;
			nvc[Fields.DateFormat] = DateFormat;
			nvc[Fields.ShortEndDateFormat] = ShortEndDateFormat;
			nvc[Fields.Location] = LocationField;
			nvc[Fields.UnknownText] = UnknownText;
			nvc[Fields.LocationUnknown] = LocationUnknownField;
			nvc[Fields.YearQueryString] = YearQueryString;
			nvc[Fields.DefaultLocation] = DefaultLocation;
			nvc[Fields.RegistrationNeeded] = RegistrationNeededField;
			nvc[Fields.RegistrationRecipient] = RegistrationRecipientField;
			nvc[Fields.DefaultRegistrationRecipient] = DefaultRegistrationRecipient;
			nvc[Fields.MaximumNumberOfRegistrations] = MaximumNumberOfRegistrationsField;
			nvc[Fields.DefaultMaximumNumberOfRegistrations] = DefaultMaximumNumberOfRegistrations;
			nvc[Fields.RegistrationList] = RegistrationListField;
			nvc[Fields.RegistrationMailSubject] = RegistrationMailSubject;

			return nvc;
		}
		#endregion
	}
}