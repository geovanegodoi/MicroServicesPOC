using System;
using System.Threading.Tasks;
using AutoMapper;
using Bogus;
using FluentAssertions;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using MSPOC.CrossCutting;
using MSPOC.Catalog.Service.Controllers;
using MSPOC.Catalog.Service.Models;
using MSPOC.Events.Catalog;
using NSubstitute;
using Xunit;
using MSPOC.Catalog.Service.UnitTest.Fixtures;
using MSPOC.Catalog.Service.UnitTest.Extensions;

namespace MSPOC.Catalog.Service.UnitTest.Controllers
{
    public class ItemsControllerTests : IClassFixture<ControllerFixture>
    {
        #pragma warning disable CS4014

        private readonly IMapper _mapperMock;
        private readonly IRepository<Entities.Item> _repositoryMock;
        private readonly IPublishEndpoint _publisherMock;
        private readonly ItemsController _sut;
        private readonly ControllerFixture _fixture;

        public ItemsControllerTests(ControllerFixture fixture)
        {
            _mapperMock     = Substitute.For<IMapper>();
            _repositoryMock = Substitute.For<IRepository<Entities.Item>>();
            _publisherMock  = Substitute.For<IPublishEndpoint>();
            _sut            = new ItemsController(_mapperMock, _repositoryMock, _publisherMock);
            _fixture        = fixture;
        }

        [Fact]
        public async Task GetByIdAsync_ItemExist_Return200Ok()
        {
            // Arrange
            var item = _fixture.NewCatalogItem();
            _repositoryMock.GetAsync(item.Id).Returns(item);

            // Act
            var result = await _sut.GetByIdAsync(item.Id);

            // Assert
            (result.Result as OkObjectResult).StatusCode.Should().Be(200);
        }
        
        [Fact]
        public async Task GetByIdAsync_ItemNotExist_Return404NotFound()
        {
            // Arrange
            Entities.Item itemNotFound = null;
            _repositoryMock.GetAsync(default).Returns(itemNotFound);

            // Act
            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            // Assert
            (result.Result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PostAsync_ItemNotExist_PublishItemCreatedEvent()
        {
            // Arrange
            var createDTO = _fixture.NewCreateEditItemDTO();
            var item      = _fixture.NewCatalogItem();
            _mapperMock.Map<Entities.Item>(createDTO).Returns(item);
        
            // Act
            await _sut.PostAsync(createDTO);
        
            // Assert
            VerifyPublishedEvents<CatalogItemCreated>(item, expectedEvents: 1);
        }

        [Fact]
        public async Task PutAsync_ItemNotExist_Return404NotFound()
        {
            // Arrange
            var updateDTO = _fixture.NewCreateEditItemDTO();
            Entities.Item itemNotFound = null;
            _repositoryMock.GetAsync(default).Returns(itemNotFound);

            // Act
            var result = await _sut.PutAsync(Guid.NewGuid(), updateDTO);

            // Assert
            (result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task PutAsync_ItemExist_PublishItemUpdatedEvent()
        {
            // Arrange
            var updateDTO = _fixture.NewCreateEditItemDTO();
            var item      = _fixture.NewCatalogItem();
            _repositoryMock.GetAsync(item.Id).Returns(item);
        
            // Act
            await _sut.PutAsync(item.Id, updateDTO);
        
            // Assert
            VerifyPublishedEvents<CatalogItemUpdated>(item, expectedEvents: 1);
        }

        [Fact]
        public async Task DeleteAsync_ItemNotExist_Return404NotFound()
        {
            // Arrange
            Entities.Item itemNotFound = null;
            _repositoryMock.GetAsync(default).Returns(itemNotFound);
        
            // Act
            var result = await _sut.DeleteAsync(Guid.NewGuid());
        
            // Assert
            (result as NotFoundResult).StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task DeleteAsync_ItemExist_PublishItemDeletedEvent()
        {
            // Arrange
            var item = _fixture.NewCatalogItem();
            _repositoryMock.GetAsync(item.Id).Returns(item);

            // Act
            await _sut.DeleteAsync(item.Id);
        
            // Assert
            VerifyPublishedEvents<CatalogItemDeleted>(item, expectedEvents: 1);
        }

        protected void VerifyPublishedEvents<T>(Entities.Item item, int expectedEvents) where T : class
        {
            var catalogEvent = GetEventByType<T>(item);
            _publisherMock.ReceivedWithAnyArgs(expectedEvents).Publish<T>(catalogEvent);
        }

        private T GetEventByType<T>(Entities.Item item)
        {
            object output = null;

            if (typeof(T) == typeof(CatalogItemCreated))
                output = item.AsItemCreated();
            else if (typeof(T) == typeof(CatalogItemDeleted))
                output = item.AsItemDeleted();
            else if (typeof(T) == typeof(CatalogItemUpdated))
                output = item.AsItemUpdated();

            return (T)output;
        }
        
        #pragma warning restore CS4014
    }
}