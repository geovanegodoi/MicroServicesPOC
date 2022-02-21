using System.Threading.Tasks;
using Bogus;
using MSPOC.Events.Catalog;
using MSPOC.Order.Service.Consumers;
using MSPOC.Order.Service.Entities;
using NSubstitute;
using Xunit;

namespace MSPOC.Order.Service.UnitTest.Consumers
{
    public class CatalogItemDeletedConsumerTests : CatalogItemConsumerTestsBase<CatalogItemDeleted>
    {
        #pragma warning disable CS4014
        private readonly CatalogItemDeletedConsumer _sut;

        public CatalogItemDeletedConsumerTests()
        {
            _consumeContextMock.Message.Returns(NewCatalogItemDeleted());

            _sut = new CatalogItemDeletedConsumer(_repositoryMock, _mapperMock);
        }

        [Fact]
        public async Task Consome_CatalogItemNotExist_NotRemoveFromDatabase()
        {
            // Arrange
            CatalogItem itemNotExist = null;
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(itemNotExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(0).RemoveAsync(default);
        }
        
        [Fact]
        public async Task Consome_CatalogItemExist_RemoveFromDatabase()
        {
            // Arrange
            var itemExist = NewCatalogItem();
            _repositoryMock.GetAsync(itemExist.Id).ReturnsForAnyArgs(itemExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).RemoveAsync(itemExist);
        }

        private CatalogItemDeleted NewCatalogItemDeleted()
            => new Faker<CatalogItemDeleted>()
            .CustomInstantiator(f => new CatalogItemDeleted 
            ( 
                ItemId : f.Random.Guid() 
            ));

        #pragma warning restore CS4014
    }
}