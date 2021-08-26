using Moonglade.Data;
using Moonglade.Data.Entities;
using Moonglade.Data.Infrastructure;
using Moonglade.FriendLink;
using Moq;
using NUnit.Framework;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Moonglade.Core.Tests
{
    [TestFixture]
    public class FriendLinkServiceTests
    {
        private MockRepository _mockRepository;
        private Mock<IBlogAudit> _mockBlogAudit;
        private Mock<IRepository<FriendLinkEntity>> _mockFriendlinkRepo;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new(MockBehavior.Default);
            _mockBlogAudit = _mockRepository.Create<IBlogAudit>();
            _mockFriendlinkRepo = _mockRepository.Create<IRepository<FriendLinkEntity>>();
        }

        private FriendLinkService CreateService()
        {
            return new(_mockFriendlinkRepo.Object, _mockBlogAudit.Object);
        }

        [Test]
        public async Task GetAsync_OK()
        {
            var handler = new GetLinkQueryHandler(_mockFriendlinkRepo.Object);
            await handler.Handle(new(Guid.Empty), CancellationToken.None);

            _mockFriendlinkRepo.Verify(p =>
                p.SelectFirstOrDefaultAsync(It.IsAny<ISpecification<FriendLinkEntity>>(),
                    It.IsAny<Expression<Func<FriendLinkEntity, Link>>>()));
        }

        [Test]
        public async Task GetAllAsync_OK()
        {
            var handler = new GetAllLinksQueryHandler(_mockFriendlinkRepo.Object);
            await handler.Handle(new(), CancellationToken.None);

            _mockFriendlinkRepo.Verify(p => p.SelectAsync(It.IsAny<Expression<Func<FriendLinkEntity, Link>>>()));
        }

        [Test]
        public void AddAsync_InvalidUrl()
        {
            var svc = CreateService();
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await svc.AddAsync("Fubao", "work006");
            });
        }

        [Test]
        public async Task AddAsync_Valid()
        {
            var uid = Guid.NewGuid();
            var friendLinkEntity = new FriendLinkEntity
            {
                Id = uid,
                LinkUrl = "https://dot.net",
                Title = "Choice of 955"
            };
            var tcs = new TaskCompletionSource<FriendLinkEntity>();
            tcs.SetResult(friendLinkEntity);

            _mockFriendlinkRepo.Setup(p => p.AddAsync(It.IsAny<FriendLinkEntity>())).Returns(tcs.Task);

            var svc = CreateService();
            await svc.AddAsync("Choice of 955", "https://dot.net");
            Assert.Pass();
        }

        [Test]
        public async Task DeleteAsync_OK()
        {
            var svc = CreateService();
            await svc.DeleteAsync(Guid.Empty);

            _mockBlogAudit.Verify();
            Assert.Pass();
        }

        [Test]
        public void UpdateAsync_InvalidUrl()
        {
            var svc = CreateService();
            Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await svc.UpdateAsync(Guid.Empty, "Fubao", "work006");
            });
        }

        [Test]
        public async Task UpdateAsync_LinkNull()
        {
            _mockFriendlinkRepo.Setup(p => p.GetAsync(It.IsAny<Guid>()));

            var svc = CreateService();
            await svc.UpdateAsync(Guid.Empty, "work", "https://996.icu");

            _mockFriendlinkRepo.Verify(p => p.UpdateAsync(It.IsAny<FriendLinkEntity>()), Times.Never);
        }

        [Test]
        public async Task UpdateAsync_OK()
        {
            _mockFriendlinkRepo.Setup(p => p.GetAsync(It.IsAny<Guid>())).Returns(ValueTask.FromResult(new FriendLinkEntity
            {
                Id = Guid.Empty,
                LinkUrl = "https://dot.net",
                Title = "Choice of 955"
            }));

            var svc = CreateService();
            await svc.UpdateAsync(Guid.Empty, "work", "https://996.icu");

            _mockFriendlinkRepo.Verify(p => p.UpdateAsync(It.IsAny<FriendLinkEntity>()));
            _mockBlogAudit.Verify(p => p.AddEntry(BlogEventType.Content, BlogEventId.FriendLinkUpdated, It.IsAny<string>()));
        }
    }
}
