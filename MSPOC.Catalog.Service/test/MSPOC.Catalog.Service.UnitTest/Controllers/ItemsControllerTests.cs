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

namespace MSPOC.Catalog.Service.UnitTest.Controllers
{
    public class ItemsControllerTests
    {
        #pragma warning disable CS4014

        private readonly IMapper _mapperMock;
        private readonly IRepository<Entities.Item> _repositoryMock;
        private readonly IPublishEndpoint _publishMock;
        private readonly ItemsController _sut;


        public ItemsControllerTests()
        {
            _mapperMock     = Substitute.For<IMapper>();
            _repositoryMock = Substitute.For<IRepository<Entities.Item>>();
            _publishMock    = Substitute.For<IPublishEndpoint>();

            _sut = new ItemsController(_mapperMock, _repositoryMock, _publishMock);
        }

        [Fact]
        public async Task GetByIdAsync_ItemExist_Return20Ok()
        {
            // Arrange
            var item = NewCatalogItem();
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
            var createDTO   = NewCreateEditItemDTO();
            var item        = NewCatalogItem();
            var itemCreated = MapToItemCreated(item);
            _mapperMock.Map<Entities.Item>(createDTO).Returns(item);
        
            // Act
            await _sut.PostAsync(createDTO);
        
            // Assert
            _publishMock.ReceivedWithAnyArgs(1).Publish<CatalogItemCreated>(itemCreated);
        }

        [Fact]
        public async Task PutAsync_ItemNotExist_Return404NotFound()
        {
            // Arrange
            var updateDTO = NewCreateEditItemDTO();
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
            var updateDTO   = NewCreateEditItemDTO();
            var item        = NewCatalogItem();
            var itemUpdated = MapToItemUpdated(item);
            _repositoryMock.GetAsync(item.Id).Returns(item);
        
            // Act
            await _sut.PutAsync(item.Id, updateDTO);
        
            // Assert
            _publishMock.ReceivedWithAnyArgs(1).Publish<CatalogItemUpdated>(itemUpdated);
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
            var item        = NewCatalogItem();
            var itemDeleted = MapToItemDeleted(item);
            _repositoryMock.GetAsync(item.Id).Returns(item);

            // Act
            await _sut.DeleteAsync(item.Id);
        
            // Assert
            _publishMock.ReceivedWithAnyArgs(1).Publish<CatalogItemDeleted>(itemDeleted);
        }

        private CreateEditItemDTO NewCreateEditItemDTO()
            => new Faker<CreateEditItemDTO>()
            .CustomInstantiator(f => new CreateEditItemDTO
            (
                Name        : f.Commerce.ProductName(),
                Description : f.Commerce.ProductDescription(),
                Price       : decimal.Parse(f.Commerce.Price()),
                Quantity    : f.Random.Int(min: 1, max: 10)
            ));

        private Entities.Item NewCatalogItem()
            => new Faker<Entities.Item>()
            .CustomInstantiator(f => new Entities.Item
            {
                Id             = f.Random.Guid(),
                Name           = f.Person.FullName,
                Description    = f.Commerce.ProductDescription(),
                Price          = decimal.Parse(f.Commerce.Price()),
                Quantity       = f.Random.Int(min:1, max: 10)
            });
        private CatalogItemCreated MapToItemCreated(Entities.Item item)
            => new CatalogItemCreated(ItemId: item.Id, Name: item.Name, Price : item.Price);

        private CatalogItemUpdated MapToItemUpdated(Entities.Item item)
            => new CatalogItemUpdated(ItemId: item.Id, Name: item.Name, Price : item.Price);

        private CatalogItemDeleted MapToItemDeleted(Entities.Item item)
            => new CatalogItemDeleted(ItemId: item.Id);
        
        #pragma warning restore CS4014
    }
}