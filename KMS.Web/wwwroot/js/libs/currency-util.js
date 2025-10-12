var CurrencyUtil = function () {
    function formatCurrencyVND(value) {
        if (!value) return "";
        return new Intl.NumberFormat("vi-VN", {
            style: "currency",
            currency: "VND",
        }).format(String(value));
    }

    function unformatCurrencyVND(value) {
        if (!value) return "";
        return String(value).replace(/\D/g, "");
    }

    return {
        formatCurrencyVND,
        unformatCurrencyVND,
    };
};
