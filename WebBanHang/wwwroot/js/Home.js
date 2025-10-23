$(document).ready(function () {
    updateHeaderCart();
    // Bắt sự kiện click trên tất cả các phần tử có class 'add-to-cart-btn'
    // Sử dụng 'body' để lắng nghe sự kiện, giúp nó hoạt động với cả các phần tử được tải bằng AJAX sau này (event delegation)
    $('body').on('click', '.add-to-cart', function (e) {
        e.preventDefault(); // Ngăn chặn hành vi mặc định của thẻ <a>

        // Lấy foodId từ thuộc tính data-food-id của nút được click
        var foodId = $(this).data('food-id');
        var quantity = 1; // Mặc định thêm 1 sản phẩm

        
        // Thực hiện gọi AJAX
        $.ajax({
            url:'/api/cart/add', // Endpoint của API
            method: 'POST',
            contentType: 'application/json',
            // Dữ liệu gửi đi phải là một chuỗi JSON
            data: JSON.stringify({
                foodId: foodId,
                quantity: quantity
            }),
            success: function (response) {
                // Xử lý khi API trả về thành công
                if (response.success) {
                    console.log('API response:', response);
                    // Hiển thị thông báo thành công
                    Swal.fire({
                        toast: true,
                        position: 'top-end', // Hiển thị ở góc trên bên phải
                        icon: 'success',
                        title: 'Đã thêm vào giỏ hàng!',
                        showConfirmButton: false,
                        timer: 1000, // Tự động tắt sau 2 giây
                        timerProgressBar: true,
                        didOpen: (toast) => {
                            toast.addEventListener('mouseenter', Swal.stopTimer)
                            toast.addEventListener('mouseleave', Swal.resumeTimer)
                        }
                    });     
                    updateHeaderCart();
                    //if (response.data && response.data.cartItemCount !== undefined) {
                    //    // Cập nhật số lượng sản phẩm
                    //    $('#header-cart-count').text(response.data.cartItemCount);

                    //    // Cập nhật tổng tiền (format lại thành 2 chữ số thập phân)
                    //    var formattedTotal = '$' + response.data.cartTotal.toFixed(2);
                    //    $('#header-cart-total').text(formattedTotal);
                    //}
                } else {
                    // Xử lý khi API trả về success = false
                    Swal.fire({
                        icon: 'error',
                        title: 'Thất bại',
                        text: response.message || 'Có lỗi xảy ra, không thể thêm sản phẩm.'
                    });
                }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                // Xử lý khi có lỗi HTTP (ví dụ: 401 Unauthorized, 500 Internal Server Error)
                console.error("AJAX Error:", textStatus, errorThrown);

                if (jqXHR.status === 401) {
                    // Nếu người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                    alert('Bạn cần đăng nhập để thực hiện chức năng này.');
                    window.location.href = '/Identity/Account/Login'; // Điều chỉnh URL nếu cần
                } else {
                    // Các lỗi khác
                    alert('Đã xảy ra lỗi. Vui lòng thử lại.');
                }
            }
        });
    });

});

function updateHeaderCart() {
    $.get('/api/cart/summary', function (res) {
        if (res.success) {
            $('#header-cart-count').text(res.data.cartItemCount);
            $('#header-cart-total').text(formatCurrency(res.data.cartTotal));
        }
    });
}

function formatCurrency(value) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value || 0);
}