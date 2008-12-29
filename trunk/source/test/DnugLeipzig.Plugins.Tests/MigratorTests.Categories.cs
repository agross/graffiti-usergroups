using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.ForTesting;
using DnugLeipzig.Plugins.Migration;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace DnugLeipzig.Plugins.Tests
{
	public partial class MigratorTests : Spec
	{
		[Test]
		public void CreatesNewCategory()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "new";

			using (Mocks.Record())
			{
				categoryRepository = Mocks.StrictMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(categoryName)).Return(null);

				categoryRepository.AddCategory(null);
				LastCall.Constraints(Is.Matching((Category category) => category.Name == categoryName));
			}

			using (Mocks.Playback())
			{
				Migrator fm = new Migrator(categoryRepository, new PostRepository());
				fm.EnsureTargetCategory(categoryName);
			}
		}

		[Test]
		public void DoesNotRecreateExistingCategory()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "existing";

			using (Mocks.Record())
			{
				categoryRepository = Mocks.StrictMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(categoryName)).Return(new Category());
			}

			using (Mocks.Playback())
			{
				Migrator fm = new Migrator(categoryRepository, new PostRepository());
				fm.EnsureTargetCategory(categoryName);
			}
		}

		[Test]
		public void CreatesNewCategoryAndDeletesOldCategoryIfCategoryNameChanges()
		{
			ICategoryRepository categoryRepository;
			IPostRepository postRepository;
			IMemento newState;
			IMemento oldState;
			const string sourceCategoryName = "old";
			const string targetCategoryName = "new";

			using (Mocks.Record())
			{
				oldState = Mocks.StrictMock<IMemento>();
				Expect.Call(oldState.CategoryName).Return(sourceCategoryName);
				Expect.Call(oldState.Fields).Return(new Dictionary<Guid, FieldInfo>());

				newState = Mocks.StrictMock<IMemento>();
				Expect.Call(newState.CategoryName).Return(targetCategoryName);
				Expect.Call(newState.Fields).Return(new Dictionary<Guid, FieldInfo>());

				categoryRepository = Mocks.StrictMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(targetCategoryName)).Return(null);

				Category targetCategory = new Category { Id = int.MaxValue, Name = targetCategoryName };
				Expect.Call(categoryRepository.GetCategory(targetCategoryName)).Return(targetCategory);

				categoryRepository.AddCategory(null);
				LastCall.Constraints(Is.Matching((Category category) => category.Name == targetCategoryName));

				categoryRepository.DeleteCategory(null);
				LastCall.Constraints(Is.Equal(sourceCategoryName));

				// No posts to migrate.
				postRepository = Mocks.StrictMock<IPostRepository>();
				Expect.Call(postRepository.GetByCategory(sourceCategoryName)).Return(new PostCollection());
			}

			using (Mocks.Playback())
			{
				Migrator fm = new Migrator(categoryRepository, postRepository);
				fm.Migrate(new MigrationInfo(oldState, newState));
			}
		}
	}
}