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
    public class CustomerUpdatedConsumerTests 
        : CustomerConsumerTestsBase<CustomerUpdated>, IClassFixture<CustomerConsumerFixture>
    {
        #pragma warning disable CS4014
        private readonly CustomerUpdatedConsumer _sut;
        private readonly CustomerConsumerFixture _fixture;

        public CustomerUpdatedConsumerTests(CustomerConsumerFixture fixture)
        {
            _sut = new CustomerUpdatedConsumer(_repositoryMock, _mapperMock);
            _fixture = fixture;

            _consumeContextMock.Message.Returns(_fixture.NewCatalogItemUpdated());
        }

        [Fact]
        public async Task Consome_CustomerNotExist_NotUpdateDatabase()
        {
            // Arrange
            Customer customerNotExist = null;
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(customerNotExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(0).UpdateAsync(default);
        }
        
        [Fact]
        public async Task Consome_CustomerExist_UpdateDatabase()
        {
            // Arrange
            var customerExist = _fixture.NewCustomer();
            _repositoryMock.GetAsync(customerExist.Id).ReturnsForAnyArgs(customerExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).UpdateAsync(customerExist);
        }

        #pragma warning restore CS4014
    }
}