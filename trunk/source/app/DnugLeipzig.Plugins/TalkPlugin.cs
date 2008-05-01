using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Web;

using DnugLeipzig.Definitions.Configuration;
using DnugLeipzig.Definitions.Extensions;
using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Plugins.Migration;
using DnugLeipzig.Runtime;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

namespace DnugLeipzig.Plugins
{
	public class TalkPlugin : GraffitiEvent, ITalkPluginConfiguration, ISupportsMemento
	{
		internal const string Form_CategoryName = "categoryName";
		internal const string Form_CreateTargetCategoryAndFields = "createTargetCategoryAndFields";
		internal const string Form_DateField = "dateField";
		internal const string Form_MigrateFieldValues = "migrate";
		internal const string Form_SpeakerField = "speakerField";
		internal const string Form_YearQueryString = "yearQueryString";
		readonly ICategoryRepository _categoryRepository;
		readonly IPostRepository _postRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="TalkPlugin"/> class.
		/// </summary>
		public TalkPlugin() : this(new CategoryRepository(), new PostRepository())
		{
			// Initialize with default values.
			CategoryName = "Talks";
			DateField = "Date";
			SpeakerField = "Speaker";
			YearQueryString = "year";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TalkPlugin"/> class.
		/// This constructor is used for dependency injection in unit testing scenarios.
		/// </summary>
		/// <param name="categoryRepository">The category repository.</param>
		/// <param name="postRepository">The post repository.</param>
		internal TalkPlugin(ICategoryRepository categoryRepository, IPostRepository postRepository)
		{
			if (categoryRepository == null)
			{
				throw new ArgumentNullException("categoryRepository");
			}

			if (postRepository == null)
			{
				throw new ArgumentNullException("postRepository");
			}

			_categoryRepository = categoryRepository;
			_postRepository = postRepository;

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
		/// Gets or sets a value indicating whether to enable validation and default valie event handlers. Event handlers will be disabled during migration.
		/// </summary>
		/// <value><c>true</c> if event handlers are enabled; otherwise, <c>false</c>.</value>
		static bool EnableEventHandlers
		{
			get;
			set;
		}

		#region ITalkPluginConfiguration Members
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

			if (!_postRepository.GetCategoryName(post).Equals(CategoryName, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}

			// Validate input.
			if (!post[DateField].IsNullOrEmptyTrimmed())
			{
				if (!Validator.ValidateDate(post[DateField]))
				{
					throw new ValidationException("Please enter a valid date.", DateField);
				}
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
			       	                    "Graffiti talks category",
			       	                    "Enter the name of the category in which you store talks, e.g. \"Talks\"."),
			       	new TextFormElement(Form_DateField,
			       	                    "Talk date and time custom field",
			       	                    "Enter the name of the custom field in which the talk's date (and, optionally, time) is stored, e.g. \"Date\". This field will be validated to be either empty or hold a correct date time/value."),
			       	new TextFormElement(Form_SpeakerField,
			       	                    "Speaker custom field",
			       	                    "Enter the name of the custom field in which the speaker's name is stored, e.g. \"Speaker\"."),
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

				if (!Validator.ValidateExisting(nvc[Form_YearQueryString]))
				{
					throw new ValidationException("Please enter a year query string parameter.");
				}

				// Write back.
				oldState = CreateMemento();

				CategoryName = categoryName;
				DateField = nvc[Form_DateField];
				SpeakerField = nvc[Form_SpeakerField];
				YearQueryString = nvc[Form_YearQueryString];

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
			nvc[Form_DateField] = DateField;
			nvc[Form_SpeakerField] = SpeakerField;
			nvc[Form_YearQueryString] = YearQueryString;

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