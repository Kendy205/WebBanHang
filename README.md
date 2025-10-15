<pre> ```WebBanHang (Solution)
│
├── WebBanHang (Main MVC Project)
│   ├── Areas/
│   │   ├── Admin/
│   │   │   ├── Controllers/
│   │   │   │   ├── DashboardController.cs
│   │   │   │   ├── CategoriesController.cs
│   │   │   │   ├── FoodsController.cs
│   │   │   │   ├── OrdersController.cs
│   │   │   │   └── UsersController.cs
│   │   │   └── Views/
│   │   │       ├── Dashboard/
│   │   │       ├── Categories/
│   │   │       ├── Foods/
│   │   │       ├── Orders/
│   │   │       └── Users/
│   │   │
│   │   └── Customer/
│   │       ├── Controllers/
│   │       │   ├── HomeController.cs
│   │       │   ├── FoodsController.cs
│   │       │   ├── CartController.cs
│   │       │   └── OrdersController.cs
│   │       └── Views/
│   │           ├── Home/
│   │           ├── Foods/
│   │           ├── Cart/
│   │           └── Orders/
│   │
│   ├── Controllers/
│   │   └── AccountController.cs (Identity Login/Register)
│   │
│   ├── DTOs/
│   │   ├── CategoryDTO.cs
│   │   ├── FoodDTO.cs
│   │   ├── CartDTO.cs
│   │   └── OrderDTO.cs
│   │
│   ├── ViewComponents/
│   │   ├── CartWidgetViewComponent.cs
│   │   └── CategoryMenuViewComponent.cs
│   │
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── js/
│   │   └── images/
│   │
│   ├── Views/
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml
│   │   │   ├── _AdminLayout.cshtml
│   │   │   └── _LoginPartial.cshtml
│   │   └── Account/
│   │       ├── Login.cshtml
│   │       └── Register.cshtml
│   │
│   ├── FileUpload/
│   │   └── (Uploaded images)
│   │
│   └── Startup.cs / Program.cs
│
├── WebBanHang.BLL (Business Logic Layer)
│   └── Services/
│       ├── ICategoryService.cs
│       ├── CategoryService.cs
│       ├── IFoodService.cs
│       ├── FoodService.cs
│       ├── ICartService.cs
│       ├── CartService.cs
│       ├── IOrderService.cs
│       └── OrderService.cs
│
├── WebBanHang.DAL (Data Access Layer)
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── DbInitializer.cs
│   │
│   ├── Repository/
│   │   ├── IRepository.cs (Generic)
│   │   ├── Repository.cs (Generic)
│   │   ├── IUnitOfWork.cs
│   │   └── UnitOfWork.cs
│   │
│   └── Migrations/
│
└── WebBanHang.Models
    ├── ApplicationUser.cs
    ├── Category.cs
    ├── Food.cs
    ├── Cart.cs
    ├── CartItem.cs
    ├── Order.cs
    ├── OrderDetail.cs
    ├── Payment.cs
    ├── Review.cs
    ├── Voucher.cs
    └── ActivityLog.cs
🚀=============================Hướng dẫn clone dự án======================================🚀
Yêu Cầu :
Visual Studio 2019 hoặc cao hơn
SQL Server 2017 hoặc cao hơn
.NET Framework 4.7.2 trở lên

Bước 1: Clone Repository
bashgit clone https://github.com/your-repo/webbanhang.git
cd webbanhang
Bước 2: Restore NuGet Packages
bash# Trong Package Manager Console (Visual Studio)
Update-Package
Bước 3: Cập nhật Connection String
Mở Web.config và cập nhật connection string:
xml<connectionStrings>
    <add name="DefaultConnection" 
         connectionString="Server=YOUR_SERVER;Database=FoodOrderDB;User Id=sa;Password=YOUR_PASSWORD;" 
         providerName="System.Data.SqlClient" />
</connectionStrings>
Bước 4: Tạo Database
bash# Trong Package Manager Console
Enable-Migrations
Add-Migration InitialCreate
Update-Database
Bước 5: Chạy Application
bash# Nhấn F5 hoặc Ctrl+F5 để debug
        
🚀=================================Hướng Dẫn Sử Dụng=====================================🚀
1. Đăng Ký Tài Khoản

Click "Đăng Ký" trên header
Điền thông tin (Email, Tên, SĐT, Địa chỉ)
Tạo mật khẩu (tối thiểu 6 ký tự)
Click "Đăng Ký"
Tự động redirected đến trang chủ

2. Đăng Nhập

Click "Đăng Nhập"
Nhập Email & Mật khẩu
Tick "Ghi nhớ đăng nhập" nếu muốn lưu cookie
Click "Đăng Nhập"

3. Mua Hàng (Customer)
A. Xem Danh Sách Món Ăn
Trang Chủ → Xem Menu / Customer/Foods

Paging: Chuyển trang bằng nút phía dưới
Lọc:

Chọn danh mục
Nhập giá từ-đến
Chọn sắp xếp (tên, giá, đánh giá)


Tìm kiếm: Gõ từ khóa → Click tìm kiếm

B. Xem Chi Tiết Món Ăn

Click "Chi Tiết" hoặc hình ảnh món ăn
Xem mô tả, giá, đánh giá
Click "Thêm Vào Giỏ" + chọn số lượng
Hoặc "Quay Lại" để tiếp tục mua

C. Quản Lý Giỏ Hàng
Header → Giỏ Hàng / Customer/Cart

Xem danh sách: Tất cả món trong giỏ
Cập nhật số lượng: Nhập số mới → Tự động update
Xóa món: Click nút "Xóa"
Xóa giỏ: Click "Xóa Giỏ Hàng"
Tiếp tục mua: Click "Tiếp Tục Mua Hàng"
Thanh toán: Click "Thanh Toán"

Session & Cookies:

Giỏ hàng lưu trong Database (gắn với UserId)
Lưu Cookie "CartData" 7 ngày (nếu "Ghi nhớ đăng nhập")
Session timeout: 20 phút

D. Thanh Toán
Customer/Orders/Checkout

Địa chỉ giao hàng: Nhập địa chỉ
Số điện thoại: Nhập SĐT liên hệ
Phương thức thanh toán:

Cash (COD): Thanh toán khi nhận
Credit Card: Thẻ tín dụng/ghi nợ
MoMo: Ví MoMo
ZaloPay: Ví ZaloPay


Ghi chú: Thêm ghi chú tùy chọn
Click "Xác Nhận Đặt Hàng"

E. Theo Dõi Đơn Hàng
Customer/Orders/Index

Lọc theo trạng thái:

Tất Cả
Chờ Xử Lý (Pending)
Đã Xác Nhận (Confirmed)
Đang Giao (Delivering)
Hoàn Thành (Completed)
Đã Hủy (Cancelled)


Chi tiết: Click "Chi Tiết" để xem toàn bộ thông tin
Hủy đơn: Chỉ hủy được đơn ở trạng thái Pending

F. Đánh Giá Món Ăn

Sau khi đơn Completed
Click "Chi Tiết Đơn"
Click nút "Đánh Giá" trên từng món
Chọn số sao (1-5)
Viết bình luận (tuỳ chọn)
Click "Gửi Đánh Giá"

G. Quản Lý Tài Khoản
Header → Tài Khoản / Account/Profile

Xem thông tin: Họ tên, Email, SĐT, Địa chỉ
Cập nhật: Sửa thông tin → Click "Cập Nhật"
Đổi mật khẩu: Click "Đổi Mật Khẩu"

Nhập mật khẩu cũ
Nhập mật khẩu mới (2 lần)
Click "Đổi Mật Khẩu"


Quên mật khẩu: Link "Quên Mật Khẩu?" ở trang Đăng Nhập


4. Quản Lý Admin
A. Dashboard
Truy cập: /Admin/Dashboard

Thống kê hôm nay: Số đơn, doanh thu
Thống kê tháng này: Số đơn, doanh thu
Tổng số liệu: Tổng đơn, doanh thu, món, danh mục
Đơn hàng gần đây: 10 đơn mới nhất
Top 5 món bán chạy
Biểu đồ doanh thu 7 ngày

B. Quản Lý Danh Mục
/Admin/Categories

Xem danh sách: Tất cả danh mục
Thêm mới: Click "Thêm Danh Mục"

Nhập tên danh mục
Nhập mô tả
Upload hình ảnh
Click "Thêm"


Sửa: Click "Sửa" → Thay đổi → Click "Cập Nhật"
Xóa: Click "Xóa" → Xác nhận

C. Quản Lý Món Ăn
/Admin/Foods

Lọc & Tìm kiếm:

Chọn danh mục
Gõ từ khóa tên/mô tả
Paging: 10 sản phẩm/trang


Thêm mới: Click "Thêm Món"

Tên món ăn, mô tả, giá
Chọn danh mục
Upload hình
Tick "Còn Hàng"
Click "Thêm"


Sửa: Click "Sửa" → Thay đổi → Click "Cập Nhật"
Xóa: Click "Xóa" → Xác nhận
Bật/Tắt hàng: Click icon dây toggleAJAX) bên cạnh

D. Quản Lý Đơn Hàng
/Admin/Orders

Lọc & Tìm kiếm:

Bộ lọc trạng thái (badge màu)
Tìm theo mã đơn / tên khách / SĐT


Xem chi tiết: Click "Chi Tiết"
Cập nhật trạng thái:

Chọn trạng thái mới từ dropdown
Click "Cập Nhật"
Trạng thái: Pending → Confirmed → Preparing → Delivering → Completed


Hủy đơn: Click nút "Hủy Đơn"
In đơn: Click "In" (PDF format)

E. Quản Lý Người Dùng
/Admin/Users

Lọc & Tìm kiếm:

Lọc theo vai trò (Admin, Customer, Staff)
Tìm theo tên / email


Xem chi tiết: Click "Chi Tiết"
Thêm người dùng: Click "Thêm Người Dùng"

Email, Mật khẩu, Họ tên
SĐT, Địa chỉ
Chọn vai trò
Click "Thêm"


Sửa: Click "Sửa" → Cập nhật → Click "Cập Nhật"
Xóa: Click "Xóa" → Xác nhận (Không xóa được chính mình)
Bật/Tắt: Click icon toggle bên cạnh để kích hoạt/vô hiệu hóa
Reset mật khẩu: Click "Đặt Lại Mật Khẩu"

F. Báo Cáo & Thống Kê
/Admin/Reports

Doanh Thu Theo Ngày:

Chọn khoảng thời gian
Xem biểu đồ
Tổng số đơn, tổng doanh thu, giá trung bình


Top Món Bán Chạy:

Chọn thời gian
Xem top 10 (có thể custom)
Xem số lượng bán, doanh thu


Thống Kê Khách Hàng:

Tính năng: Số đơn, tổng chi tiêu, đơn trung bình
Sắp xếp theo chi tiêu


Xuất Excel: Click "Xuất" (trong phát triển)
