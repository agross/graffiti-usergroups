using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Mapping;
using DnugLeipzig.Definitions.Plugins.Talks;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Definitions.Validation;
using DnugLeipzig.Runtime.Plugins.Migration;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Plugins.Talks
{
	public partial class TalkPlugin : GraffitiEvent, ITalkPluginConfigurationProvider, ISupportsMemento
	{
		readonly IPostRepository _postRepository;
		readonly IMapper<NameValueCollection, Settings> _settingsMapper;
		readonly IValidator<Settings> _settingsValidator;

		public TalkPlugin() : this(IoC.Resolve<IPostRepository>(),
		                           IoC.Resolve<IMapper<NameValueCollection, Settings>>(),
		                           IoC.Resolve<IValidator<Settings>>())
		{
			// Initialize default values.
			CategoryName = "Talks";
			DateField = "Date";
			SpeakerField = "Speaker";
			YearQueryString = "year";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TalkPlugin"/> class.
		/// This constructor is used for dependency injection in unit testing scenarios.
		/// </summary>
		internal TalkPlugin(IPostRepository postRepository,
		                    IMapper<NameValueCollection, Settings> settingsMapper,
		                    IValidator<Settings> settingsValidator)
		{
			_postRepository = postRepository;
			_settingsMapper = settingsMapper;
			_settingsValidator = settingsValidator;

			EnableEventHandlers = true;
		}

		public override string Name
		{
			get { return "Talk Plugin"; }
		}

		public override bool IsEditable
		{
			get { return true; }
		}

		public override string Description
		{
			get { return "Extends Graffiti CMS for talks management."; }
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

		#region ITalkPluginConfigurationProvider Members
		public string DateField
		{
			get;
			set;
		}

		public string SortRelevantDateField
		{
			get { return DateField; }
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
		#endregion

		public override void Init(GraffitiApplication ga)
		{
			Debug.WriteLine("Init Talk Plugin");

			ga.BeforeValidate += Post_Validate;
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

			var validation = IoC.Resolve<ITalkValidator>(this).Validate(post).Interpret();
			if (validation.Failed)
			{
				validation.ThrowAsException();
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

			Settings settings = new Settings();
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

				CategoryName = settings.CategoryName;
				DateField = settings.Date;
				SpeakerField = settings.Speaker;
				YearQueryString = settings.YearQueryString;

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

				PluginMigrator.MigrateSettings(settings.CreateTargetCategoryAndFields,
				                               settings.MigrateFieldValues,
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
			nvc[Fields.Date] = DateField;
			nvc[Fields.Speaker] = SpeakerField;
			nvc[Fields.YearQueryString] = YearQueryString;

			return nvc;
		}
		#endregion

		#region ISupportsMemento
		public IMemento CreateMemento()
		{
			return new TalkPluginMemento(this);
		}
		#endregion
	}
}