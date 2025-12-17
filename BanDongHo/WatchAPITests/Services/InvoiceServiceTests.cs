using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WatchAPI.DTOs;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories.Interfaces;
using WatchAPI.Services;

namespace WatchAPITests.Services;

[TestClass()]
public class InvoiceServiceTests
{
    private Mock<IUnitOfWork> _uowMock;
    private Mock<IInvoiceRepository> _invoiceRepoMock;
    private Mock<IMapper> _mapperMock;
    private InvoiceService _service;

    [TestInitialize()]
    public void Setup()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _invoiceRepoMock = new Mock<IInvoiceRepository>();
        _mapperMock = new Mock<IMapper>();

        _uowMock.Setup(uow => uow.Invoices).Returns(_invoiceRepoMock.Object);

        _service = new InvoiceService(
            _uowMock.Object,
            _mapperMock.Object,
            NullLogger<InvoiceService>.Instance
        );
    }

    [TestMethod()]
    public async Task GetAllAsync_ReturnsAllInvoices()
    {
        // Arrange
        var invoice = new List<Invoice>
        {
            new Invoice { UserId = "1" },
            new Invoice { UserId = "2" },
        };

        _invoiceRepoMock.Setup(repo => repo.GetAllWithDetailAsync())
            .ReturnsAsync(invoice);

        _mapperMock.Setup(m => m.Map<IEnumerable<InvoiceDTO>>(It.IsAny<IEnumerable<Invoice>>()))
            .Returns((IEnumerable<Invoice> invs) =>
                invs.Select(i => new InvoiceDTO
                { 
                    UserId = i.UserId 
                }));

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("1", result.ElementAt(0).UserId);
        Assert.AreEqual("2", result.ElementAt(1).UserId);
    }

    [TestMethod()]
    public async Task GetByIdAsync_InvoiceExists_ReturnsInvoice()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        var invoice = new Invoice { UserId = "1" };

        _invoiceRepoMock.Setup(repo => repo.GetDetailAsync(id))
            .ReturnsAsync(invoice);

        _mapperMock.Setup(m => m.Map<InvoiceDTO>(It.IsAny<Invoice>()))
            .Returns((Invoice i) =>
                new InvoiceDTO
                {
                    UserId = i.UserId,
                });

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1", result.UserId);
    }

    [TestMethod()]
    public async Task GetByIdAsync_InvoiceNotFound_ReturnsNull()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        _invoiceRepoMock.Setup(repo => repo.GetDetailAsync(id))
            .ReturnsAsync((Invoice?) null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod()]
    public async Task CreateAsync_WatchExists_ReturnsCreatedInvoice()
    {
        // Arrange
        var watchId = Guid.NewGuid();
        var watch = new Watch
        {
            Name = "Test Watch",
        };
        typeof(Watch)
            .GetProperty("Id", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)!
            .SetValue(watch, watchId);

        var dto = new InvoiceDTO
        {
            UserId = "1",
            Details = new List<InvoiceDetailDTO>
            {
                new InvoiceDetailDTO
                {
                    WatchId = watchId,
                    Quantity = 2,
                }
            }
        };

        _uowMock.Setup(u => u.Watches.GetByIdAsync(watchId))
            .ReturnsAsync(watch);

        _uowMock.Setup(u => u.Invoices.CreateAsync(It.IsAny<Invoice>(), It.IsAny<string?>()))
            .Returns(Task.CompletedTask);

        _uowMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<InvoiceDTO>(It.IsAny<Invoice>()))
            .Returns((Invoice inv) => new InvoiceDTO
            {
                UserId = inv.UserId
            });

        // Act
        var result = await _service.CreateAsync(dto, "admin");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(dto.UserId, result.UserId);

        _uowMock.Verify(u => u.Watches.GetByIdAsync(watchId), Times.Once);
        _uowMock.Verify(u => u.Invoices.CreateAsync(It.IsAny<Invoice>(), "admin"), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task CreateAsync_WatchNotFound_ThrowsInvalidOperationException()
    {
        // Arrange
        var watchId = Guid.NewGuid();

        var dto = new InvoiceDTO
        {
            UserId = "1",
            Details = new List<InvoiceDetailDTO>
            {
                new InvoiceDetailDTO
                {
                    WatchId = watchId,
                    Quantity = 1,
                    Price = 0
                }
            }
        };

        _uowMock.Setup(u => u.Watches.GetByIdAsync(watchId))
            .ReturnsAsync((Watch?)null);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>( () => 
            _service.CreateAsync(dto, "admin")
        );

        _uowMock.Verify(u => u.Watches.GetByIdAsync(watchId), Times.AtLeastOnce);
    }

    [TestMethod]
    public async Task CreateAsync_UserIdEmpty_ThrowsArgumentException()
    {
        // Arrange
        var dto = new InvoiceDTO
        {
            UserId = "",
            Details = new List<InvoiceDetailDTO>
            {
                new InvoiceDetailDTO
                {
                    WatchId = Guid.NewGuid(),
                    Quantity = 1,
                    Price = 0
                }
            }
        };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _service.CreateAsync(dto)
        );
    }

    [TestMethod]
    public async Task CreateAsync_NoDetails_ThrowsArgumentException()
    {
        // Arrange
        var dto = new InvoiceDTO
        {
            UserId = "1",
            Details = new List<InvoiceDetailDTO>()
        };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _service.CreateAsync(dto)
        );
    }

    [TestMethod]
    public async Task CreateAsync_InvalidQuantity_ThrowsArgumentException()
    {
        // Arrange
        var dto = new InvoiceDTO
        {
            UserId = "1",
            Details = new List<InvoiceDetailDTO>
            {
                new InvoiceDetailDTO
                {
                    WatchId = Guid.NewGuid(),
                    Quantity = 0,
                    Price = 0
                }
            }
        };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _service.CreateAsync(dto)
        );
    }

    [TestMethod]
    public async Task CreateAsync_PriceProvidedFromClient_ThrowsInvalidOperationException()
    {
        // Arrange
        var dto = new InvoiceDTO
        {
            UserId = "1",
            Details = new List<InvoiceDetailDTO>
            {
                new InvoiceDetailDTO
                {
                    WatchId = Guid.NewGuid(),
                    Quantity = 1,
                    Price = 2500000
                }
            }
        };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => _service.CreateAsync(dto)
        );
    }

    [TestMethod]
    public async Task CreateAsync_InvoiceIdProvidedFromClient_ThrowsArgumentException()
    {
        // Arrange
        var dto = new InvoiceDTO
        {
            Id = Guid.NewGuid(),
            UserId = "1",
            Details = new List<InvoiceDetailDTO>
            {
                new InvoiceDetailDTO
                {
                    WatchId = Guid.NewGuid(),
                    Quantity = 1,
                    Price = 0
                }
            }
        };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _service.CreateAsync(dto)
        );
    }

    [TestMethod]
    public async Task CreateAsync_InvoiceDetailIdProvidedFromClient_ThrowsArgumentException()
    {
        // Arrange
        var dto = new InvoiceDTO
        {
            UserId = "1",
            Details = new List<InvoiceDetailDTO>
            {
                new InvoiceDetailDTO
                {
                    Id = Guid.NewGuid(),
                    WatchId = Guid.NewGuid(),
                    Quantity = 1,
                    Price = 0
                }
            }
        };

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => _service.CreateAsync(dto)
        );
    }
}