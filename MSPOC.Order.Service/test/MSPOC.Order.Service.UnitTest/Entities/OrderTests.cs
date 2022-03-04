using System.Collections.Generic;
using FluentAssertions;
using Xunit;
using Entity = MSPOC.Order.Service.Entities;

namespace MSPOC.Order.Service.UnitTest.Entities
{
    public class OrderTests
    {
        private readonly Entity.Order _sut = new(); 

        [Fact]
        public void CalculateOrderTotalPrice_Scenario_ExpectedBehavior()
        {
            // Arrange
            _sut.OrderItems = new List<Entity.OrderItem>()
            {
                new Entity.OrderItem { Price = 2, Quantity = 2 }, // 4
                new Entity.OrderItem { Price = 3, Quantity = 3 }, // 9
            };
        
            // Act
            var output = _sut.CalculateOrderTotalPrice();
        
            // Assert
            output.Should().Be(expected: 13);
        }
        
    }
}