using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.BLL.Services;
using WebBanHang.DAL.Repository.IRepository;
using WebBanHang.DAL.Repository.UnitOfWork;
using WebBanHang.Models.Models;

namespace WebBanHang.Test.service
{
    [TestClass]
    public class CartServiceTest
    {
        private Mock<IUnitOfWork> _unitOfWorkMock;
        private Mock<ICartRepository> _mockCartRepo;
        private Mock<ICartItemRepository> _mockCartItemRepo;
        private Mock<IFoodRepository> _mockFoodRepo;
        // Đây là service THẬT mà chúng ta sẽ test
        private CartService _cartService;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mockCartRepo = new Mock<ICartRepository>();
            _mockCartItemRepo = new Mock<ICartItemRepository>();
            _mockFoodRepo = new Mock<IFoodRepository>();
            // Thiết lập UnitOfWork để trả về các repository giả lập
            _unitOfWorkMock.Setup(u => u.Carts).Returns(_mockCartRepo.Object);
            _unitOfWorkMock.Setup(u => u.CartItems).Returns(_mockCartItemRepo.Object);
            _unitOfWorkMock.Setup(u => u.Foods).Returns(_mockFoodRepo.Object);
            // Tạo instance của CartService với UnitOfWork giả lập
            _cartService = new CartService(_unitOfWorkMock.Object);
        }
        [TestMethod]
        public async Task AddToCart_NewItem_AddsItemToCart()
        {
            // 1. Arrange
            var userId = "user1";
            var foodId = 1;
            var quantity = 2;
            var cart = new Cart { CartId = 1, UserId = userId };
            var food = new Food { FoodId = foodId, Price = 50000, IsAvailable = true };

            // Mock cho GetCartByUserId
            _mockCartRepo.Setup(r => r.GetCartByUserIdAsync(userId)).ReturnsAsync(cart);
            _mockCartItemRepo.Setup(r => r.GetAllQueryable()).Returns(new List<CartItem>().AsQueryable()); // Cart rỗng

            // Mock cho GetByIdAsync (Food)
            _mockFoodRepo.Setup(r => r.GetByIdAsync(foodId)).ReturnsAsync(food);

            // Mock cho FirstOrDefaultAsync (kiểm tra item đã tồn tại chưa -> null)
            _mockCartItemRepo.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<CartItem, bool>>>()))
                             .ReturnsAsync((CartItem)null);

            // 2. Act
            await _cartService.AddToCart(userId, foodId, quantity);

            // 3. Assert
            // Kiểm tra xem AddAsync có được gọi với ĐÚNG item không
            _mockCartItemRepo.Verify(r => r.AddAsync(It.Is<CartItem>(
                ci => ci.CartId == 1 &&
                      ci.FoodId == foodId &&
                      ci.Quantity == quantity &&
                      ci.Price == 50000
            )), Times.Once);

            // Kiểm tra xem Cart có được Update và Save không
            _mockCartRepo.Verify(r => r.Update(cart), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveAsync(), Times.Once);
        }
    }
}
