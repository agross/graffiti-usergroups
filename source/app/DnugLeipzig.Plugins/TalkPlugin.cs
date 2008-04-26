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
	public class TalkPlugin : GraffitiEvent, ITalkPluginConfiguration
	{
		internal const string Form_CategoryName = "categoryName";
		internal const string Form_CreateTargetCategoryAndFields = "createTargetCategoryAndFields";
		internal const string Form_DateField = "dateField";
		internal const string Form_MigrateFieldValues = "migrate";
		internal const string Form_SpeakerField = "speakerField";
		internal const string Form_YearQueryString = "yearQueryString";
		readonly ICategoryRepository _categoryRepository;

		public TalkPlugin() : this(new CategoryRepository())
		{
			// Initialize with default values.
			CategoryName = "Talks";
			DateField = "Date";
			SpeakerField = "Speaker";
			YearQueryString = "year";
		}

		public TalkPlugin(ICategoryRepository categoryRepository)
		{
			_categoryRepository = categoryRepository;
		}

		public override string Name
		{
			get { return "Talks Plugin"; }
		}

		public override bool IsEditable
		{
			get { return true; }
		}

		public override string Description
		{
			get { return "Extends Graffiti CMS for talks management."; }
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
			try
			{
				HttpContext.Current.Cache.Remove(EventPluginConfiguration.CacheKey);

				// Validation.
				HttpContext.Current.Cache.Remove(TalkPluginConfiguration.CacheKey);

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
				IMemento oldState = CreateMemento();

				CategoryName = categoryName;
				DateField = nvc[Form_DateField];
				SpeakerField = nvc[Form_SpeakerField];
				YearQueryString = nvc[Form_YearQueryString];

				IMemento newState = CreateMemento();

				FieldMigrator migrator = new FieldMigrator();
				if (nvc[Form_CreateTargetCategoryAndFields].IsChecked())
				{
					migrator.EnsureTargetCategory(categoryName);
					migrator.EnsureFields(categoryName, new MigrationInfo(oldState, newState).AllFields);
				}
				if (nvc[Form_MigrateFieldValues].IsChecked())
				{
					migrator.Migrate(new MigrationInfo(oldState, newState));
				}

				return StatusType.Success;
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
		}

		protected override NameValueCollection DataAsNameValueCollection()
		{
			var values = new NameValueCollection();
			values[Form_CategoryName] = HttpUtility.HtmlDecode(CategoryName);
			values[Form_DateField] = DateField;
			values[Form_SpeakerField] = SpeakerField;
			values[Form_YearQueryString] = YearQueryString;

			return values;
		}
		#endregion

		#region Memento
		IMemento CreateMemento()
		{
			return new TalkPluginMemento(this);
		}
		#endregion
	}
}