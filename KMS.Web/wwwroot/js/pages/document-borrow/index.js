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
import { showSuccessToast } from "../../utils/toastify-util.js";

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

function generateCaptcha() {
    const canvas = document.getElementById("captcha");
    const ctx = canvas.getContext("2d");
    const chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
    const captchaLength = 4;
    let captcha = "";

    for (let i = 0; i < captchaLength; i++) {
        captcha += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    currentCaptcha = captcha;

    ctx.clearRect(0, 0, canvas.width, canvas.height);

    ctx.fillStyle = "#e6f3ff";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    ctx.font = "4rem Arial";
    ctx.fillStyle = "#003a78";
    ctx.textBaseline = "middle";
    ctx.textAlign = "center";
    ctx.fillText(captcha, canvas.width / 2, canvas.height / 2);
}

generateCaptcha();