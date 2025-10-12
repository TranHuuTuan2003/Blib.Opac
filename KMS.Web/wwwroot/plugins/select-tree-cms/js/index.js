function buildParentItems(data, code) {
    const $parentContainer = $('#drc_' + code);
    // Thêm giá trị mặc định
    $parentContainer.empty(); // Xóa các mục hiện tại
    $parentContainer.append('<li class="parent-item custom-font-small" value>---Chọn---</li>');
    const parentItems = data.filter(item => item.parent_id === 0 || item.parent_id === null);

    parentItems.forEach(parent => {
        var $parentItem = $('<li>').addClass('parent-item custom-font-small').text(parent.name).val(parent.id);

        // Tìm các mục con của mục cha hiện tại
        const childItems = data.filter(item => item.parent_id == parent.id);

        // Nếu mục cha có mục con thì thêm dấu '>'
        if (childItems.length > 0) {
            const $arrow = $('<span>').addClass('arrow').html('<i class="fas fa-caret-right"></i>');
            $parentItem.append($arrow);

            const $dropdownMenu = $('<ul>').addClass('dropdown-menu custom-font-small');

            childItems.forEach(child => {
                const $childItem = $('<li class="child-item">').text(child.name).val(child.id);
                $dropdownMenu.append($childItem);
            });

            $parentItem.append($dropdownMenu);
        } else {
            $parentItem.addClass('not-parent-item');
        }

        $parentContainer.append($parentItem);
    });
    $('.not-parent-item').on('click', function () {
        // Lấy value từ thuộc tính value
        const value = $(this).val();

        // Lấy text bên trong phần tử li
        const text = $(this).text();
        $("#drp_" + code).attr('value', value);
        $("#drp_" + code).text(text);
        $('.dropdown-wrapper ').removeClass('show-dropdown');

    });
    $('.parent-item.custom-font-small').on('click', function () {
        // Lấy value từ thuộc tính value
        const value = $(this).val();

        // Lấy text bên trong phần tử li
        const text = $(this).contents().filter(function () {
            return this.nodeType === 3; // Lấy chỉ văn bản (text node)
        }).text().trim(); // Lấy văn bản của mục cha

        // Cập nhật giá trị và văn bản vào dropdownService
        $("#drp_" + code).attr('value', value);
        $("#drp_" + code).text(text);

        // Đóng dropdown
        $('.dropdown-wrapper').removeClass('show-dropdown');
    });

}

let dropdownTimeout; // Biến dùng để lưu thời gian trì hoãn


// Khi click vào tiêu đề dropdown
$('.dropdown-title').on('click', function (event) {
    clearTimeout(dropdownTimeout); // Xóa bất kỳ timeout nào đang hoạt động

    // Kiểm tra nếu dropdown đã mở
    const $dropdownWrapper = $(this).closest('.dropdown-wrapper');

    // Nếu dropdown đang mở, thì đóng nó
    if ($dropdownWrapper.hasClass('show-dropdown')) {
        $dropdownWrapper.removeClass('show-dropdown');
    }
    // Nếu dropdown chưa mở, thì mở nó
    else {
        $dropdownWrapper.addClass('show-dropdown');
    }

    // Ngừng sự kiện click của dropdown để không bị kích hoạt lại
    event.stopPropagation();
});

// Khi click vào bất kỳ phần tử ngoài dropdown thì ẩn dropdown
$(document).on('click', function (event) {
    // Kiểm tra nếu click vào ngoài dropdown
    if (!$(event.target).closest('.dropdown-wrapper').length) {
        $('.dropdown-wrapper').removeClass('show-dropdown');
    }
});

// Chặn sự kiện click để dropdown không tự động ẩn khi click vào nội bộ của nó
$('.dropdown-wrapper').on('click', function (event) {
    event.stopPropagation();
});