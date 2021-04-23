﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Moonglade.Core;
using Moonglade.Web.Pages;
using Moq;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Moonglade.Web.Tests.Pages
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class ArchiveModelTests
    {
        private MockRepository _mockRepository;
        private Mock<IPostQueryService> _mockPostQueryService;

        [SetUp]
        public void SetUp()
        {
            _mockRepository = new(MockBehavior.Default);
            _mockPostQueryService = _mockRepository.Create<IPostQueryService>();
        }

        private ArchiveModel CreateArchiveModel()
        {
            return new(_mockPostQueryService.Object);
        }

        [Test]
        public async Task OnGet_StateUnderTest_ExpectedBehavior()
        {
            var fakeArchives = new List<Archive>
            {
                new (996,9,6),
                new (251,3,5)
            };

            _mockPostQueryService.Setup(p => p.GetArchiveAsync())
                .Returns(Task.FromResult((IReadOnlyList<Archive>)fakeArchives));

            var archiveModel = CreateArchiveModel();

            await archiveModel.OnGet();

            Assert.IsNotNull(archiveModel.Archives);
        }
    }
}