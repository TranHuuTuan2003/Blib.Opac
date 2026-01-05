import "../../../css/components/accordion/style.css";
import "../../../css/pages/document-detail/style.css";
import config from "../../common/config.js";
import { SEARCH_TYPES } from "../../common/constants.js";
import { createClickEvent, createScrollEvent } from "../../common/main.js";
import {
    createAdvanceModalClickAndKeyUpEvents,
    createHiddenAdvanceModalEvent,
    createShownAdvanceModalEvent,
} from "../../components/advanced-search/index.js";
import { createHeaderEvents } from "../../components/header/index.js";
import { createSearchBarEvents } from "../../components/search-bar/index.js";
import { createToggleButtonEvent } from "../../components/sidebar-menu/index.js";
import { fetchRestful } from "../../utils/api-util.js";
import {
    createClickSeeMoreSummaryEvent,
    observeForSeeingMore,
} from "../../utils/see-more-util.js";
import { showSuccessToast, showWarningToast } from "../../utils/toastify-util.js";

function createSearchBarHeaderEvents() {
    createShownAdvanceModalEvent();
    createHiddenAdvanceModalEvent();
    createAdvanceModalClickAndKeyUpEvents(SEARCH_TYPES.REDIRECT);
    createSearchBarEvents(SEARCH_TYPES.REDIRECT);
    createHeaderEvents();
}

createSearchBarHeaderEvents();

createToggleButtonEvent();

let currentCaptcha = "";

var bibId = new URLSearchParams(window.location.search).get("bibId");

function generateCaptcha() {
    const canvas = document.getElementById("captcha");
    const ctx = canvas.getContext("2d");

    const dpr = window.devicePixelRatio || 1;
    const width = 155;
    const height = 40;

    canvas.width = width * dpr;
    canvas.height = height * dpr;
    canvas.style.width = width + "px";
    canvas.style.height = height + "px";
    ctx.setTransform(dpr, 0, 0, dpr, 0, 0);

    const chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    const captchaLength = 4;
    let captcha = "";

    for (let i = 0; i < captchaLength; i++) {
        captcha += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    currentCaptcha = captcha;

    const gradient = ctx.createLinearGradient(0, 0, width, height);
    gradient.addColorStop(0, "#f8fbff");
    gradient.addColorStop(1, "#dbeafe");
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, width, height);

    ctx.textBaseline = "middle";
    ctx.textAlign = "center";
    ctx.font = "bold 32px 'Segoe UI', Arial";

    for (let i = 0; i < captcha.length; i++) {
        const x = 30 + i * 32;
        const y = height / 2 + (Math.random() * 6 - 3);
        const angle = Math.random() * 0.4 - 0.2;

        ctx.save();
        ctx.translate(x, y);
        ctx.rotate(angle);
        ctx.fillStyle = "#003a78";
        ctx.fillText(captcha[i], 0, 0);
        ctx.restore();
    }

    for (let i = 0; i < 2; i++) {
        ctx.strokeStyle = "rgba(0,0,0,0.15)";
        ctx.beginPath();
        ctx.moveTo(0, Math.random() * height);
        ctx.bezierCurveTo(
            width / 3, Math.random() * height,
            width / 2, Math.random() * height,
            width, Math.random() * height
        );
        ctx.stroke();
    }

    for (let i = 0; i < 30; i++) {
        ctx.fillStyle = "rgba(0,0,0,0.2)";
        ctx.fillRect(
            Math.random() * width,
            Math.random() * height,
            1.5,
            1.5
        );
    }
}


generateCaptcha();

createClickEvent("#resert-capcha", function (_,e) {
    generateCaptcha();
});

createClickEvent(".first-documentDetail__submit", function (_,e) {
    const input = document.getElementById("captcha-input").value.trim();

    if (input !== currentCaptcha) {
        e.preventDefault(); 
        showWarningToast(toast_maxacnhankhongdung);

        generateCaptcha(); 
        document.getElementById("captcha-input").value = "";
        document.getElementById("captcha-input").focus();
        return;
    }

    submitRequestQueue();
});

function parseDateToISO(dateStr) {
    if (!dateStr) return null;

    const parts = dateStr.split('/');
    if (parts.length !== 3) return null;

    const [day, month, year] = parts.map(Number);

    if (!day || !month || !year) return null;

    const date = new Date(year, month - 1, day);

    if (
        date.getFullYear() !== year ||
        date.getMonth() !== month - 1 ||
        date.getDate() !== day
    ) {
        return null;
    }

    return date.toISOString();
}
function submitRequestQueue() {
    const dobInput = document.getElementById('dob').value.trim();
    const queueDateInput = document.getElementById('time').value?.trim();

    const dobISO = parseDateToISO(dobInput);
    const queueDateISO = parseDateToISO(queueDateInput);

    if (!dobISO) {
        showWarningToast('Ngày sinh không hợp lệ (dd/MM/yyyy)');
        return;
    }

    if (queueDateInput && !queueDateISO) {
        showWarningToast('Ngày hẹn không hợp lệ (dd/MM/yyyy)');
        return;
    }

    const data = {
        ReaderId: null,
        RegId: null,
        CirPlaceId: null,
        BibId: bibId,
        RegisterId: null,
        StatusId: null,
        CardNo: null,
        Type: 'ban_doc',
        AppointmentDate: queueDateISO,
        DateCreated: new Date().toISOString(),
        RequestDate: new Date().toISOString(),
        FullName: document.getElementById('fullname').value.trim(),
        Email: document.getElementById('email').value.trim(),
        Tel: document.getElementById('phone').value.trim(),
        Sex: (() => {
            const v = document.querySelector('input[name="gender"]:checked')?.value;
            return v === 'Nam' ? "0" : v === 'Nữ' ? "1" : null;
        })(),
        CCCD: document.getElementById('idcard').value.trim(),
        Dob: dobISO
    };

    if (!data.FullName || !data.Email || !data.Tel || !data.CCCD) {
        showWarningToast('Vui lòng nhập đầy đủ thông tin bắt buộc');
        return;
    }

    fetchRestful({
        url: config.baseUrlApi + 'Document/insert-requesr-queue',
        method: "POST",
        contentType: "application/json",
        responseType: "json",
        data: data
    }).then((response) => {
        if (response.success) {
            showSuccessToast(toast_guithongtindangky);
        }
        else {
            showWarningToast(response.message);
        }
    });
}
