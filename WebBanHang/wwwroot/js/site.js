// site.js
//alert("hehe")
$(document).ready(function () {
    //alert("hehe")
    // 1. Kiểm tra URL động đã được định nghĩa trong Layout chưa
    if (typeof appSettings === 'undefined' || !appSettings.addToCartUrl) {
        console.error("appSettings.addToCartUrl is not defined. AJAX will not work.");
        return;
    }

    var addToCartUrl = appSettings.addToCartUrl;
    var loginUrl = appSettings.loginUrl;

    // 2. LẮNG NGHE SỰ KIỆN CHỈ MỘT LẦN KHI DOM SẴN SÀNG
    $('.add-to-cart-btn').on('click', function (e) {
        e.preventDefault();
        //alert("hehe")
        // Đoạn code AJAX...
        var foodId = $(this).data('food-id');
        var quantity = 1;

        $.ajax({
            type: "POST",
            url: addToCartUrl, // <-- SỬ DỤNG BIẾN GLOBAL
            data: {
                foodId: foodId,
                quantity: quantity
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    // Cập nhật giao diện giỏ hàng
                    //$('#cart-item-count').text(response.totalItems);
                    //$('#cart-total-amount').text('$' + response.totalAmount.toFixed(2));
                } else {
                    alert("Lỗi: " + response.message);
                    if (response.message.includes("cần đăng nhập")) {
                        window.location.href = loginUrl;
                    }
                }
            },
            error: function (xhr, status, error) {
                alert("Đã xảy ra lỗi khi kết nối đến server: " + error);
            }
        });
    });
});