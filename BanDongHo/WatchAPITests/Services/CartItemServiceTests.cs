using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WatchAPI.DTOs;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories.Interfaces;
using WatchAPI.Services;

namespace WatchAPITests.Services;

[TestClass()]
public class CartItemServiceTests
{
    private Mock<IUnitOfWork> _uowMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ICartItemRepository> _cartItemRepoMock;
    private CartItemService _service;

    [TestInitialize]
    public void TestInitialize()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _cartItemRepoMock = new Mock<ICartItemRepository>();

        _uowMock.Setup(uow => uow.CartItems).Returns(_cartItemRepoMock.Object);

        _service = new CartItemService(_uowMock.Object, _mapperMock.Object, NullLogger<CartItemService>.Instance);
    }

    [TestMethod]
    public async Task GetCartAsync_ReturnsMappedCartItems()
    {
        // Arrange
        var cartItems = new List<CartItem>
        {
            new CartItem { WatchId = Guid.NewGuid(), Quantity = 2 },
            new CartItem { WatchId = Guid.NewGuid(), Quantity = 1 },
        };

        _cartItemRepoMock.Setup(repo => repo.GetCartAsync(It.IsAny<string>()))
            .ReturnsAsync(cartItems);

        _mapperMock.Setup(m => m.Map<IEnumerable<CartItemDTO>>(It.IsAny<IEnumerable<CartItem>>()))
            .Returns((IEnumerable<CartItem> ci) =>                
                ci.Select(i => new CartItemDTO
                {
                    WatchId = i.WatchId,
                    Quantity = i.Quantity
                }));

        // Act
        var result = await _service.GetCartAsync(It.IsAny<string>());

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual(cartItems[0].WatchId, result.ElementAt(0).WatchId);
        Assert.AreEqual(cartItems[1].WatchId, result.ElementAt(1).WatchId);
    }
}