export function showErrorToast(text, duration = 2000) {
    Toastify({
        text: text,
        duration: duration,
        gravity: "bottom",
        position: "right",
        style: {
            background: "#dc3545",
            fontSize: 16,
        },
    }).showToast();
}

export function showSuccessToast(text, duration = 2000) {
    Toastify({
        text: text,
        duration: duration,
        gravity: "bottom",
        position: "right",
        style: {
            background: "#198754",
            fontSize: 16,
        },
    }).showToast();
}

export function showWarningToast(text, duration = 5000) {
    Toastify({
        text: text,
        duration: duration,
        gravity: "bottom",
        position: "right",
        style: {
            background: "#ffc107",
            fontSize: 16,
        },
    }).showToast();
}

export function showInfoToast(text, duration = 2000) {
    Toastify({
        text: text,
        duration: duration,
        gravity: "bottom",
        position: "right",
        style: {
            background: "#0d6efd",
            fontSize: 16,
        },
    }).showToast();
}

export function showSecondaryToast(text, duration = 2000) {
    Toastify({
        text: text,
        duration: duration,
        gravity: "bottom",
        position: "right",
        style: {
            background: "#6c757d",
            fontSize: 16,
        },
    }).showToast();
}
