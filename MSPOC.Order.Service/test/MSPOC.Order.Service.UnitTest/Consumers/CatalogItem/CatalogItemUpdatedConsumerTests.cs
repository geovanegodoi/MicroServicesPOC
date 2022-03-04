using System.Threading.Tasks;
using Bogus;
using FluentAssertions;
using MSPOC.Events.Catalog;
using MSPOC.Order.Service.Consumers;
using MSPOC.Order.Service.Entities;
using MSPOC.Order.Service.UnitTest.Fixtures;
using NSubstitute;
using Xunit;

namespace MSPOC.Order.Service.UnitTest.Consumers
{
    public class CatalogItemUpdatedConsumerTests 
        : CatalogItemConsumerTestsBase<CatalogItemUpdated>, IClassFixture<CatalogItemConsumerFixture>
    {
        #pragma warning disable CS4014
        private readonly CatalogItemUpdatedConsumer _sut;
        private readonly CatalogItemConsumerFixture _fixture;

        public CatalogItemUpdatedConsumerTests(CatalogItemConsumerFixture fixture)
        {
            _sut = new CatalogItemUpdatedConsumer(_repositoryMock, _mapperMock);
            _fixture = fixture;

            _consumeContextMock.Message.Returns(_fixture.NewCatalogItemUpdated());
        }

        [Fact]
        public async Task Consome_CatalogItemNotExist_NotUpdateDatabase()
        {
            // Arrange
            CatalogItem itemNotExist = null;
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(itemNotExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(0).UpdateAsync(default);
        }
        
        [Fact]
        public async Task Consome_CatalogItemExist_UpdateDatabase()
        {
            // Arrange
            var itemExist = _fixture.NewCatalogItem();
            var message   = _consumeContextMock.Message;
            _repositoryMock.GetAsync(itemExist.Id).ReturnsForAnyArgs(itemExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).UpdateAsync(itemExist);
            itemExist.Name.Should().Be(message.Name);
            itemExist.Price.Should().Be(message.Price);
        }

        #pragma warning restore CS4014
    }
}