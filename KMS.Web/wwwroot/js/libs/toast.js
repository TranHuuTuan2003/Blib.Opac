var Toast = (function () {
    function ShowToastWarning(title, content) {
        if (title && content) {
            document.getElementById("solidwarningToastTitle").textContent = title;
            document.getElementById("solidwarningToastContent").textContent = content;
        }
        else {
            document.getElementById("solidwarningToastContent").textContent = title;
        }
        document.getElementById("solidwarningToastBtn").click();
    }

    function ShowToastInfo(title, content) {
        if (title && content) {
            document.getElementById("solidinfoToastTitle").textContent = title;
            document.getElementById("solidinfoToastContent").textContent = content;
        }
        else {
            document.getElementById("solidinfoToastContent").textContent = title;
        }
        document.getElementById("solidinfoToastBtn").click();
    }

    function ShowToastSuccess(title, content) {
        if (title && content) {
            document.getElementById("solidsuccessToastTitle").textContent = title;
            document.getElementById("solidsuccessToastContent").textContent = content;
        }
        else {
            document.getElementById("solidsuccessToastContent").textContent = title;
        }
        document.getElementById("solidsuccessToastBtn").click();
    }

    function ShowToastError(title, content) {
        if (title && content) {
            document.getElementById("soliddangerToastTitle").textContent = title;
            document.getElementById("soliddangerToastContent").textContent = content;
        }
        else {
            document.getElementById("soliddangerToastContent").textContent = title;
        }
        document.getElementById("soliddangerToastBtn").click();
    }

    return { ShowToastWarning, ShowToastInfo, ShowToastSuccess, ShowToastError };
})