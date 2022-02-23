using System;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MSPOC.CrossCutting;
using MSPOC.Customer.Service.Controllers;
using MSPOC.Customer.Service.Models;
using MSPOC.Customer.Service.UnitTest.Extensions;
using MSPOC.Customer.Service.UnitTest.Fixtures;
using MSPOC.Events.Customer;
using NSubstitute;
using Xunit;

namespace MSPOC.Customer.Service.UnitTest.Controllers
{
    public class CustomersControllerTests : IClassFixture<ControllerFixture>
    {
#pragma warning disable CS4014

        private readonly IMapper _mapperMock;
        private readonly IRepository<Entities.Customer> _repositoryMock;
        private readonly IPublishEndpoint _publishMock;
        private readonly CustomersController _sut;
        private readonly ControllerFixture _fixture;

        public CustomersControllerTests(ControllerFixture controllerFixture)
        {
            _mapperMock             = Substitute.For<IMapper>();
            _repositoryMock         = Substitute.For<IRepository<Entities.Customer>>();
            _publishMock            = Substitute.For<IPublishEndpoint>();
            _fixture                = controllerFixture;
            _fixture.MapperMock     = _mapperMock;
            _fixture.RepositoryMock = _repositoryMock;

            _sut = new CustomersController(_mapperMock, _repositoryMock, _publishMock);
        }

        [Fact]
        public async Task GetByIdAsync_CustomerExist_Return200Ok()
        {
            // Arrange
            _fixture.SetupCustomerExist();

            // Act
            var result = await _sut.GetByIdAsync(_fixture.CustomerId);

            // Assert
            (result.Result as OkObjectResult).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task GetByIdAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            _fixture.SetupCustomerNotExist();

            // Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PostAsync_CustomerNotExist_PublishCustomerCreatedEvent()
        {
            // Arrange
            _fixture.SetupCustomerNotExist();

            // Act
            await _sut.PostAsync(_fixture.CreateEditDTO);

            // Assert
            _publishMock.Received(1).Publish<CustomerCreated>(_fixture.CreatedEvent);
        }

        [Fact]
        public async Task PutAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            _fixture.SetupCustomerNotExist();

            // Act
            var result = await _sut.PutAsync(Guid.NewGuid(), _fixture.CreateEditDTO);

            // Assert
            (result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PutAsync_CustomerExist_PublishCustomerUpdatedEvent()
        {
            // Arrange
            _fixture.SetupCustomerExist();

            // Act
            await _sut.PutAsync(_fixture.CustomerId, _fixture.CreateEditDTO);

            // Assert
            _publishMock.Received(1).Publish<CustomerUpdated>(_fixture.UpdatedEvent);
        }

        [Fact]
        public async Task DeleteAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            _fixture.SetupCustomerNotExist();

            // Act
            var result = await _sut.DeleteAsync(Guid.NewGuid());

            // Assert
            (result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteAsync_CustomerExist_PublishCustomerDeletedEvent()
        {
            // Arrange
            _fixture.SetupCustomerExist();

            // Act
            await _sut.DeleteAsync(_fixture.CustomerId);

            // Assert
            _publishMock.Received(1).Publish<CustomerDeleted>(_fixture.DeletedEvent);
        }

        [Fact]
        public async Task GetCustomerAddressAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            _fixture.SetupCustomerNotExist();

            // Act
            var result = await _sut.GetCustomerAddressAsync(_fixture.CustomerId);

            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetCustomerAddressAsync_CustomerAddressNotExist_Return404NotFound()
        {
            // Arrange
            _fixture.SetupCustomerAddressNotExist();

            // Act
            var result = await _sut.GetCustomerAddressAsync(_fixture.CustomerId);

            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task GetCustomerAddressAsync_CustomerAndAddressExist_Return200Ok()
        {
            // Arrange
            _fixture.SetupCustomerExist();

            // Act
            var result = await _sut.GetCustomerAddressAsync(_fixture.CustomerId);

            // Assert
            (result.Result as OkObjectResult).StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task UpdateCustomerAddressAsync_CustomerNotExist_Return404NotFound()
        {
            // Arrange
            _fixture.SetupCustomerNotExist();

            // Act
            var result = await _sut.UpdateCustomerAddressAsync(_fixture.CustomerId, _fixture.CreateEditAddressDTO);

            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task UpdateCustomerAddressAsync_CustomerExist_PublishCustomerUpdatedEvent()
        {
            // Arrange
            _fixture.SetupCustomerExist();

            // Act
            var result = await _sut.UpdateCustomerAddressAsync(_fixture.CustomerId, _fixture.CreateEditAddressDTO);

            // Assert
            _publishMock.Received(1).Publish<CustomerUpdated>(_fixture.UpdatedEvent);
        }

#pragma warning restore CS4014
    }
}