using System;
using System.Collections.Generic;

using DnugLeipzig.Definitions.Repositories;
using DnugLeipzig.Plugins.Migration;
using DnugLeipzig.Runtime.Repositories;

using Graffiti.Core;

using MbUnit.Framework;

using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace DnugLeipzig.Plugins.Tests
{
	[TestFixture]
	public partial class MigratorTests
	{
		readonly MockRepository _mocks = new MockRepository();

		[TearDown]
		public void TearDown()
		{
			_mocks.ReplayAll();
			_mocks.VerifyAll();
		}

		[Test]
		public void CreatesNewCategory()
		{
			ICategoryRepository categoryRepository;
			const string categoryName = "new";

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(categoryName)).Return(null);

				categoryRepository.AddCategory(null);
				LastCall.Constraints(Is.Matching((Category category) => category.Name == categoryName));
			}

			using (_mocks.Playback())
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

			using (_mocks.Record())
			{
				categoryRepository = _mocks.CreateMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(categoryName)).Return(new Category());
			}

			using (_mocks.Playback())
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

			using (_mocks.Record())
			{
				oldState = _mocks.CreateMock<IMemento>();
				Expect.Call(oldState.CategoryName).Return(sourceCategoryName);
				Expect.Call(oldState.Fields).Return(new Dictionary<Guid, FieldInfo>());

				newState = _mocks.CreateMock<IMemento>();
				Expect.Call(newState.CategoryName).Return(targetCategoryName);
				Expect.Call(newState.Fields).Return(new Dictionary<Guid, FieldInfo>());

				categoryRepository = _mocks.CreateMock<ICategoryRepository>();
				Expect.Call(categoryRepository.GetCategory(targetCategoryName)).Return(null);

				Category targetCategory = new Category { Id = int.MaxValue, Name = targetCategoryName };
				Expect.Call(categoryRepository.GetCategory(targetCategoryName)).Return(targetCategory);

				categoryRepository.AddCategory(null);
				LastCall.Constraints(Is.Matching((Category category) => category.Name == targetCategoryName));

				categoryRepository.DeleteCategory(null);
				LastCall.Constraints(Is.Equal(sourceCategoryName));

				// No posts to migrate.
				postRepository = _mocks.CreateMock<IPostRepository>();
				Expect.Call(postRepository.GetByCategory(sourceCategoryName)).Return(new PostCollection());
			}

			using (_mocks.Playback())
			{
				Migrator fm = new Migrator(categoryRepository, postRepository);
				fm.Migrate(new MigrationInfo(oldState, newState));
			}
		}
	}
}