using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reflection;
using WatchAPI.DTOs.Watch;
using WatchAPI.Models.Entities;
using WatchAPI.Repositories.Interfaces;
using WatchAPI.Services;

namespace WatchAPITests.Services;

[TestClass()]
public class WatchServiceTests
{
    private Mock<IUnitOfWork> _uowMock;
    private Mock<IWatchRepository> _watchRepoMock;
    private Mock<IMapper> _mapperMock;
    private Mock<ILogger<WatchService>> _loggerMock;
    private WatchService _service;

    [TestInitialize]
    public void Setup()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _watchRepoMock = new Mock<IWatchRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<WatchService>>();

        _uowMock.Setup(u => u.Watches).Returns(_watchRepoMock.Object);

        _service = new WatchService(
            _uowMock.Object,
            _mapperMock.Object,
            _loggerMock.Object
        );
    }

    [TestMethod]
    public async Task GetAllAsync_ReturnsAllWatches()
    {
        // Arrange
        var watches = new List<Watch>
        {
            new Watch { Name = "Omega" },
            new Watch { Name = "Seiko" }
        };
        typeof(Watch)
            .GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watches[0], Guid.NewGuid());
        typeof(Watch)
            .GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watches[1], Guid.NewGuid());

        _watchRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(watches);

        _mapperMock.Setup(m => m.Map<IEnumerable<WatchUserDTO>>(watches))
            .Returns(new List<WatchUserDTO>
            {
                new WatchUserDTO { Id = watches[0].Id, Name = "Omega" },
                new WatchUserDTO { Id = watches[1].Id, Name = "Seiko" }
            });

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        var resultList = result.ToList();
        Assert.AreEqual(2, resultList.Count);
        Assert.AreEqual("Omega", resultList[0].Name);
        Assert.AreEqual("Seiko", resultList[1].Name);
    }

    [TestMethod]
    public async Task GetAllAsync_WhenNoWatches_ReturnsEmptyList()
    {
        // Arrange
        _watchRepoMock.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<Watch>());

        _mapperMock.Setup(m => m.Map<IEnumerable<WatchUserDTO>>(It.IsAny<IEnumerable<Watch>>()))
            .Returns(new List<WatchUserDTO>());

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenExists_ReturnsWatch()
    {
        // Arrange
        var id = Guid.NewGuid();
        var watch = new Watch { Name = "Casio" };
        typeof(Watch)
            .GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watch, id);


        _watchRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(watch);

        _mapperMock.Setup(m => m.Map<WatchUserDTO?>(watch))
            .Returns(new WatchUserDTO { Id = id, Name = "Casio" });

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("Casio", result.Name);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _watchRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Watch?)null);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetByNameAsync_WhenExists_ReturnsWatch()
    {
        // Arrange
        var id = Guid.NewGuid();
        var watch = new Watch { Name = "Rolex" };
        typeof(Watch)
            .GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watch, id);

        _watchRepoMock.Setup(r => r.GetByNameAsync("Rolex"))
            .ReturnsAsync(watch);

        // Act
        var result = await _service.GetByNameAsync("Rolex");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("Rolex", result.Name);
    }

    [TestMethod]
    public async Task GetByNameAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        _watchRepoMock.Setup(r => r.GetByNameAsync("NonExistent"))
            .ReturnsAsync((Watch?)null);

        // Act
        var result = await _service.GetByNameAsync("NonExistent");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task GetAllAdminAsync_ReturnsAllWatches()
    {
        // Arrange
        var watches = new List<Watch>
        {
            new Watch { Name = "Omega" },
            new Watch { Name = "Seiko" }
        };
        typeof(Watch)
            .GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watches[0], false);
        typeof(Watch)
            .GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watches[1], true);

        _watchRepoMock.Setup(r => r.GetAllAdminAsync(null))
            .ReturnsAsync(watches);

        _mapperMock.Setup(m => m.Map<IEnumerable<WatchAdminDTO>>(watches))
            .Returns(new List<WatchAdminDTO>
            {
                new WatchAdminDTO { Id = watches[0].Id, Name = "Omega" },
                new WatchAdminDTO { Id = watches[1].Id, Name = "Seiko" }
            });

        // Act
        var result = await _service.GetAllAdminAsync();

        // Assert
        Assert.IsNotNull(result);
        var resultList = result.ToList();
        Assert.AreEqual(2, resultList.Count);
        Assert.AreEqual("Omega", resultList[0].Name);
        Assert.AreEqual("Seiko", resultList[1].Name);
    }

    [TestMethod]
    public async Task GetAllAdminAsync_WhenNoWatches_ReturnsEmptyList()
    {
        // Arrange
        _watchRepoMock.Setup(r => r.GetAllAdminAsync(null))
            .ReturnsAsync(new List<Watch>());

        _mapperMock.Setup(m => m.Map<IEnumerable<WatchAdminDTO>>(It.IsAny<IEnumerable<Watch>>()))
            .Returns(new List<WatchAdminDTO>());

        // Act
        var result = await _service.GetAllAdminAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count());
    }

    [TestMethod]
    public async Task GetAdminByIdAsync_WhenExists_ReturnsWatch()
    {
        // Arrange
        var id = Guid.NewGuid();
        var watch = new Watch { Name = "Casio" };
        typeof(Watch)
            .GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watch, id);

        _watchRepoMock.Setup(r => r.GetAdminByIdAsync(id))
            .ReturnsAsync(watch);

        _mapperMock.Setup(m => m.Map<WatchAdminDTO?>(watch))
            .Returns(new WatchAdminDTO { Id = id, Name = "Casio" });

        // Act
        var result = await _service.GetAdminByIdAsync(id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(id, result.Id);
        Assert.AreEqual("Casio", result.Name);
    }

    [TestMethod]
    public async Task GetAdminByIdAsync_WhenNotFound_ReturnsNull()
    {
        // Arrange
        var id = Guid.NewGuid();

        _watchRepoMock.Setup(r => r.GetAdminByIdAsync(id))
            .ReturnsAsync((Watch?)null);

        // Act
        var result = await _service.GetAdminByIdAsync(id);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task CreateAsync_NameNotExists_ReturnsCreatedWatch()
    {
        // Arrange
        var dto = new WatchCreateDTO
        {
            Name = "NewWatch",
            Price = 2500000,
            Category = "Men",
            Brand = "SRWATCH",
            ImageFile = Mock.Of<IFormFile>(),
        };

        _watchRepoMock.Setup(r => r.GetByNameAsync(dto.Name))
            .ReturnsAsync((Watch?)null);

        var mappedWatch = new Watch { Name = dto.Name };

        _mapperMock.Setup(m => m.Map<Watch>(dto))
            .Returns(mappedWatch);

        _mapperMock.Setup(m => m.Map<WatchAdminDTO>(mappedWatch))
                   .Returns(new WatchAdminDTO { Name = mappedWatch.Name });

        // Act
        var result = await _service.CreateAsync(dto, "testuser");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(dto.Name, result.Name);

        _watchRepoMock.Verify(r => r.GetByNameAsync(dto.Name), Times.Once);
        _watchRepoMock.Verify(r => r.CreateAsync(mappedWatch, "testuser"), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task CreateAsync_NameExists_ThrowsArgumentException()
    {
        // Arrange
        var dto = new WatchCreateDTO
        {
            Name = "NewWatch",
            Price = 2500000,
            Category = "Men",
            Brand = "SRWATCH",
            ImageFile = Mock.Of<IFormFile>(),
        };

        var existingWatch = new Watch { Name = dto.Name };

        _watchRepoMock.Setup(r => r.GetByNameAsync(dto.Name))
            .ReturnsAsync(existingWatch);

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await _service.CreateAsync(dto, "testuser")
        );

        _watchRepoMock.Verify(r => r.GetByNameAsync(dto.Name), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_WatchExists_ReturnsTrue()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var dto = new WatchUpdateDTO
        {
            Name = "OldWatch",
            Price = 2500000,
            Category = "Men",
            Brand = "SRWATCH",
        };

        _watchRepoMock.Setup(r => r.GetByNameAsync(dto.Name))
            .ReturnsAsync((Watch?)null);

        _watchRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Watch { Name = "OldWatch" });

        // Act
        var result = await _service.UpdateAsync(id, dto, "testuser");

        // Assert
        Assert.IsTrue(result);
        _watchRepoMock.Verify(r => r.GetByNameAsync(dto.Name), Times.Once);
        _watchRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        _watchRepoMock.Verify(r => r.Update(It.IsAny<Watch>(), "testuser"), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_WatchNotFound_ReturnsFalse()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var dto = new WatchUpdateDTO
        {
            Name = "OldWatch",
            Price = 2500000,
            Category = "Men",
            Brand = "SRWATCH",
        };

        _watchRepoMock.Setup(r => r.GetByNameAsync(dto.Name))
            .ReturnsAsync((Watch?)null);

        _watchRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Watch?)null);

        // Act
        var result = await _service.UpdateAsync(id, dto, "testuser");

        // Assert
        Assert.IsFalse(result);
        _watchRepoMock.Verify(r => r.GetByNameAsync(dto.Name), Times.Once);
        _watchRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [TestMethod]
    public async Task UpdateAsync_NameExists_ThrowsArgumentException()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        var dto = new WatchUpdateDTO
        {
            Name = "OldWatch",
            Price = 2500000,
            Category = "Men",
            Brand = "SRWATCH",
        };

        _watchRepoMock.Setup(r => r.GetByNameAsync(dto.Name))
            .ReturnsAsync(new Watch { Name = "OldWatch" });

        // Act & Assert
        await Assert.ThrowsExceptionAsync<ArgumentException>(async () =>
            await _service.UpdateAsync(id, dto, "testuser")
        );

        _watchRepoMock.Verify(r => r.GetByNameAsync(dto.Name), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_WatchExists_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();

        _watchRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(new Watch { Name = "ToDelete" });

        // Act
        var result = await _service.DeleteAsync(id, "adminuser");

        // Assert
        Assert.IsTrue(result);
        _watchRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
        _watchRepoMock.Verify(r => r.DeleteAsync(id, "adminuser"), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task DeleteAsync_WatchNotFound_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();

        _watchRepoMock.Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Watch?)null);

        // Act
        var result = await _service.DeleteAsync(id, "adminuser");

        // Assert
        Assert.IsFalse(result);
        _watchRepoMock.Verify(r => r.GetByIdAsync(id), Times.Once);
    }

    [TestMethod]
    public async Task RestoreAsync_WatchExists_ReturnsTrue()
    {
        // Arrange
        var id = Guid.NewGuid();

        var watch = new Watch { Name = "Casio" };
        typeof(Watch)
            .GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watch, true);

        _watchRepoMock.Setup(r => r.GetAdminByIdAsync(id))
            .ReturnsAsync(watch);

        // Act
        var result = await _service.RestoreAsync(id, "adminuser");

        // Assert
        Assert.IsTrue(result);
        _watchRepoMock.Verify(r => r.GetAdminByIdAsync(id), Times.Once);
        _watchRepoMock.Verify(r => r.RestoreAsync(id, "adminuser"), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [TestMethod]
    public async Task RestoreAsync_WatchNotFound_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();

        _watchRepoMock.Setup(r => r.GetAdminByIdAsync(id))
            .ReturnsAsync((Watch?)null);

        // Act
        var result = await _service.RestoreAsync(id, "adminuser");

        // Assert
        Assert.IsFalse(result);
        _watchRepoMock.Verify(r => r.GetAdminByIdAsync(id), Times.Once);
    }

    [TestMethod]
    public async Task RestoreAsync_WatchNotDeleted_ReturnsFalse()
    {
        // Arrange
        var id = Guid.NewGuid();

        var watch = new Watch { Name = "Casio" };
        typeof(Watch)
            .GetProperty("IsDeleted", BindingFlags.Public | BindingFlags.Instance)!
            .SetValue(watch, false);

        _watchRepoMock.Setup(r => r.GetAdminByIdAsync(id))
            .ReturnsAsync(watch);

        // Act
        var result = await _service.RestoreAsync(id, "adminuser");

        // Assert
        Assert.IsFalse(result);
        _watchRepoMock.Verify(r => r.GetAdminByIdAsync(id), Times.Once);
    }
}
