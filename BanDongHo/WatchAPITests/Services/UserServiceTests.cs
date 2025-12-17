using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WatchAPI.DTOs;
using WatchAPI.Models.Entities;
using WatchAPI.Services;

namespace WatchAPITests.Services;

[TestClass()]
public class UserServiceTests
{
    private Mock<UserManager<User>> _userManagerMock;
    private Mock<RoleManager<IdentityRole>> _roleManagerMock;
    private Mock<IMapper> _mapperMock;
    private UserService _service;

    [TestInitialize]
    public void Setup()
    {
        var userStoreMock = new Mock<IUserStore<User>>();
        _userManagerMock = new Mock<UserManager<User>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null
        );

        var roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStoreMock.Object,
            null, null, null, null
        );

        _mapperMock = new Mock<IMapper>();

        _service = new UserService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            NullLogger<UserService>.Instance,
            _mapperMock.Object
        );
    }


    [TestMethod()]
    public async Task GetAllUsersAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User { Id = "1", Email = "admin@gmail.com", UserName = "admin"},
            new User { Id = "2", Email = "user@gmail.com", UserName = "user"},
        }.AsQueryable();

        _userManagerMock.Setup(um => um.Users)
            .Returns(users);

        _mapperMock.Setup(m => m.Map<UserDTO>(It.IsAny<User>()))
            .Returns((User u) => new UserDTO
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            });

        _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { "Admin" });

        // Act
        var result = await _service.GetAllUsersAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("admin", result[0].UserName);
        Assert.AreEqual("user", result[1].UserName);
    }

    [TestMethod()]
    public async Task GetByIdAsync_UserExists_ReturnsUser()
    {
        // Arrange
        var user = new User { Id = "1", Email = "test@gmail.com", UserName = "testuser" };

        _userManagerMock.Setup(um => um.FindByIdAsync(user.Id))
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDTO>(It.IsAny<User>()))
            .Returns((User u) => new UserDTO
            {
                Id = u.Id,
                Email = u.Email,
                UserName = u.UserName
            });

        _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string> { "User" });

        // Act
        var result = await _service.GetByIdAsync(user.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("1", result.Id);
        Assert.AreEqual("testuser", result.UserName);
    }

    [TestMethod()]
    public async Task GetByIdAsync_UserNotFound_ReturnsNull()
    {
        // Arrange
        _userManagerMock.Setup(um => um.FindByIdAsync(It.IsAny<string>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.GetByIdAsync("nonexistent-id");

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod()]
    public async Task UpdateUserAsync_UserExists_ReturnsTrue()
    {
        // Arrange
        var dto = new UserDTO
        {
            Id = "1",
            Email = "test@gmail.com",
            UserName = "testuser",
            Role = "Admin"
        };

        var user = new User
        {
            Id = dto.Id,
            Email = "old@gmail.com",
            UserName = "oldname"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync(dto.Id))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        _userManagerMock.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
            .ReturnsAsync(new List<string>{ "User" });

        _userManagerMock.Setup(um => um.RemoveFromRolesAsync(It.IsAny<User>(), It.IsAny<IEnumerable<string>>()))
            .ReturnsAsync(IdentityResult.Success);

        _roleManagerMock.Setup(rm => rm.RoleExistsAsync(dto.Role))
            .ReturnsAsync(true);

        _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), dto.Role))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.UpdateUserAsync(dto.Id, dto);

        // Assert
        Assert.IsTrue(result);
        _userManagerMock.Verify(um => um.FindByIdAsync(dto.Id), Times.Once);
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once);
        _userManagerMock.Verify(um => um.GetRolesAsync(It.IsAny<User>()), Times.Once);
        _userManagerMock.Verify(um => um.RemoveFromRolesAsync(It.IsAny<User>(), It.Is<IEnumerable<string>>(roles => roles.Contains("User"))), Times.Once);
        _roleManagerMock.Verify(rm => rm.RoleExistsAsync(dto.Role), Times.Once);
        _roleManagerMock.Verify(rm => rm.CreateAsync(It.IsAny<IdentityRole>()), Times.Never);
        _userManagerMock.Verify(um => um.AddToRoleAsync(It.IsAny<User>(), dto.Role), Times.Once);
    }

    [TestMethod()]
    public async Task UpdateUserAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        var dto = new UserDTO
        {
            Id = "1",
            Email = "test@gmail.com",
            UserName = "testuser",
            Role = "Admin"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync(dto.Id))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.UpdateUserAsync(dto.Id, dto);

        // Assert
        Assert.IsFalse(result);
        _userManagerMock.Verify(um => um.FindByIdAsync(dto.Id), Times.Once);
    }

    [TestMethod()]
    public async Task UpdateUserAsync_UpdateFails_ReturnsFalse()
    {
        // Arrange
        var dto = new UserDTO
        {
            Id = "1",
            Email = "test@gmail.com",
            UserName = "testuser",
            Role = "Admin"
        };

        var user = new User
        {
            Id = dto.Id,
            Email = "old@gmail.com",
            UserName = "oldname"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync(dto.Id))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Description = "Update failed" }
            ));

        // Act
        var result = await _service.UpdateUserAsync(dto.Id, dto);

        // Assert
        Assert.IsFalse(result);
        _userManagerMock.Verify(um => um.FindByIdAsync(dto.Id), Times.Once);
        _userManagerMock.Verify(um => um.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [TestMethod()]
    public async Task DeleteUserAsync_UserExists_ReturnsTrue()
    {
        // Arrange
        string id = "1";

        var user = new User
        {
            Id = id,
            Email = "old@gmail.com",
            UserName = "oldname"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync(id))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.DeleteUserAsync(id);

        // Assert
        Assert.IsTrue(result);
        _userManagerMock.Verify(um => um.FindByIdAsync(id), Times.Once);
        _userManagerMock.Verify(um => um.DeleteAsync(user), Times.Once);
    }

    [TestMethod()]
    public async Task DeleteUserAsync_UserNotFound_ReturnsFalse()
    {
        // Arrange
        string id = "1";

        _userManagerMock.Setup(um => um.FindByIdAsync(id))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _service.DeleteUserAsync(id);

        // Assert
        Assert.IsFalse(result);
        _userManagerMock.Verify(um => um.FindByIdAsync(id), Times.Once);
    }

    [TestMethod()]
    public async Task DeleteUserAsync_DeleteFails_ReturnsFalse()
    {
        // Arrange
        string id = "1";

        var user = new User
        {
            Id = id,
            Email = "old@gmail.com",
            UserName = "oldname"
        };

        _userManagerMock.Setup(um => um.FindByIdAsync(id))
            .ReturnsAsync(user);

        _userManagerMock.Setup(um => um.DeleteAsync(user))
            .ReturnsAsync(IdentityResult.Failed(
                new IdentityError { Description = "Delete failed" }
            ));

        // Act
        var result = await _service.DeleteUserAsync(id);

        // Assert
        Assert.IsFalse(result);
        _userManagerMock.Verify(um => um.FindByIdAsync(id), Times.Once);
        _userManagerMock.Verify(um => um.DeleteAsync(user), Times.Once);
    }
}

