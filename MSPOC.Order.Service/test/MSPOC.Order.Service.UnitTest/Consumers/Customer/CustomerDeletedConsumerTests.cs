using System.Threading.Tasks;
using Bogus;
using MSPOC.Events.Catalog;
using MSPOC.Events.Customer;
using MSPOC.Order.Service.Consumers;
using MSPOC.Order.Service.Entities;
using MSPOC.Order.Service.UnitTest.Fixtures;
using NSubstitute;
using Xunit;

namespace MSPOC.Order.Service.UnitTest.Consumers
{
    public class CustomerDeletedConsumerTests 
        : CustomerConsumerTestsBase<CustomerDeleted>, IClassFixture<CustomerConsumerFixture>
    {
        #pragma warning disable CS4014
        private readonly CustomerDeletedConsumer _sut;
        private readonly CustomerConsumerFixture _fixture;

        public CustomerDeletedConsumerTests(CustomerConsumerFixture fixture)
        {
            _sut = new CustomerDeletedConsumer(_repositoryMock, _mapperMock);
            _fixture = fixture;

            _consumeContextMock.Message.Returns(_fixture.NewCatalogItemDeleted());
        }

        [Fact]
        public async Task Consome_CustomerNotExist_NotRemoveFromDatabase()
        {
            // Arrange
            Customer itemNotExist = null;
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(itemNotExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(0).RemoveAsync(default);
        }
        
        [Fact]
        public async Task Consome_CustomerExist_RemoveFromDatabase()
        {
            // Arrange
            var customerExist = _fixture.NewCustomer();
            _repositoryMock.GetAsync(customerExist.Id).ReturnsForAnyArgs(customerExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).RemoveAsync(customerExist);
        }

        #pragma warning restore CS4014
    }
}