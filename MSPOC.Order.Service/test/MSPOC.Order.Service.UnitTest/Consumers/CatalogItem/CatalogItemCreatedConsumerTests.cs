using System.Threading.Tasks;
using Bogus;
using MSPOC.Events.Catalog;
using MSPOC.Order.Service.Consumers;
using MSPOC.Order.Service.Entities;
using NSubstitute;
using Xunit;

namespace MSPOC.Order.Service.UnitTest.Consumers
{
    public class CatalogItemCreatedConsumerTests : CatalogItemConsumerTestsBase<CatalogItemCreated>
    {
        #pragma warning disable CS4014
        private readonly CatalogItemCreatedConsumer _sut;

        public CatalogItemCreatedConsumerTests()
        {
            _consumeContextMock.Message.Returns(NewCatalogItemCreated());

            _sut = new CatalogItemCreatedConsumer(_repositoryMock, _mapperMock);
        }

        [Fact]
        public async Task Consome_CatalogItemNotExist_InsertDatabase()
        {
            // Arrange
            CatalogItem itemNotExist = null;
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(itemNotExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).CreateAsync(default);
        }
        
        [Fact]
        public async Task Consome_CatalogItemExist_NotInsertDatabase()
        {
            // Arrange
            var itemExist = NewCatalogItem();
            _repositoryMock.GetAsync(itemExist.Id).ReturnsForAnyArgs(itemExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(0).CreateAsync(default);
        }

        private CatalogItemCreated NewCatalogItemCreated()
            => new Faker<CatalogItemCreated>()
            .CustomInstantiator(f => new CatalogItemCreated
            (
                ItemId : f.Random.Guid(), 
                Name   : f.Commerce.ProductName(),
                Price  : f.Random.Decimal()
            ));

        #pragma warning restore CS4014
    }
}