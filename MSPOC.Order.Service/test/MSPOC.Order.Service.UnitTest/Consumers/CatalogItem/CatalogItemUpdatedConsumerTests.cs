using System.Threading.Tasks;
using Bogus;
using MSPOC.Events.Catalog;
using MSPOC.Order.Service.Consumers;
using MSPOC.Order.Service.Entities;
using NSubstitute;
using Xunit;

namespace MSPOC.Order.Service.UnitTest.Consumers
{
    public class CatalogItemUpdatedConsumerTests : CatalogItemConsumerTestsBase<CatalogItemUpdated>
    {
        #pragma warning disable CS4014
        private readonly CatalogItemUpdatedConsumer _sut;

        public CatalogItemUpdatedConsumerTests()
        {
            _consumeContextMock.Message.Returns(NewCatalogItemUpdated());

            _sut = new CatalogItemUpdatedConsumer(_repositoryMock, _mapperMock);
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
            var itemExist = NewCatalogItem();
            _repositoryMock.GetAsync(itemExist.Id).ReturnsForAnyArgs(itemExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).UpdateAsync(itemExist);
        }

        private CatalogItemUpdated NewCatalogItemUpdated()
            => new Faker<CatalogItemUpdated>()
            .CustomInstantiator(f => new CatalogItemUpdated
            (
                ItemId : f.Random.Guid(), 
                Name   : f.Commerce.ProductName(),
                Price  : f.Random.Decimal()
            ));

        #pragma warning restore CS4014
    }
}