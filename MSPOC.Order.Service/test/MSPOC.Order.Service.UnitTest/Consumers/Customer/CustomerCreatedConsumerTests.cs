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
    public class CustomerCreatedConsumerTests 
        : CustomerConsumerTestsBase<CustomerCreated>, IClassFixture<CustomerConsumerFixture>
    {
        #pragma warning disable CS4014
        private readonly CustomerCreatedConsumer _sut;
        private readonly CustomerConsumerFixture _fixture;

        public CustomerCreatedConsumerTests(CustomerConsumerFixture fixture)
        {
            _sut = new CustomerCreatedConsumer(_repositoryMock, _mapperMock);
            _fixture = fixture;

            _consumeContextMock.Message.Returns(_fixture.NewCustomerCreated());
        }

        [Fact]
        public async Task Consome_CustomerNotExist_InsertDatabase()
        {
            // Arrange
            Customer itemNotExist = null;
            _repositoryMock.GetAsync(default).ReturnsForAnyArgs(itemNotExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(1).CreateAsync(default);
        }
        
        [Fact]
        public async Task Consome_CustomerExist_NotInsertDatabase()
        {
            // Arrange
            var customerExist = _fixture.NewCustomer();
            _repositoryMock.GetAsync(customerExist.Id).ReturnsForAnyArgs(customerExist);
        
            // Act
            await _sut.Consume(_consumeContextMock);
        
            // Assert
            _repositoryMock.ReceivedWithAnyArgs(0).CreateAsync(default);
        }

        #pragma warning restore CS4014
    }
}