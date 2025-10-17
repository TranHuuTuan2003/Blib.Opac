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

const backButton = document.querySelector(".first-documentDetail__left");

if (backButton) {
    createClickEvent(backButton, () => {
        window.close();
    });
}

createClickEvent(".back-link", (_, e) => {
    e.preventDefault();

    document.querySelectorAll(".accordion__content").forEach((content) => {
        content.style.display = "none";
    });

    document.querySelectorAll(".accordion__header .arrow").forEach((arrow) => {
        arrow.classList.remove("open");
    });
});

const scrollBtn = document.getElementById("scrollToTopBtn");

createScrollEvent(() => {
    const y = window.scrollY;

    if (y > 300) {
        scrollBtn.classList.add("show");
        scrollBtn.classList.remove("fade");
    } else if (y > 0 && y <= 300) {
        scrollBtn.classList.add("show", "fade");
    } else {
        scrollBtn.classList.remove("show", "fade");
    }
});

createClickEvent(scrollBtn, () => {
    scrollToTop();
});

function scrollToTop() {
    window.scrollTo({
        top: 0,
        behavior: "smooth",
    });
}

observeForSeeingMore(".second-documentDetail__summary");
createClickSeeMoreSummaryEvent(".second-documentDetail__summary");

createClickEvent(".second-documentDetail__title-action.save", () => {
    showSuccessToast("Lưu tài liệu thành công!");
});

createClickEvent(".like-group, .dislike-icon", () => {
    showSuccessToast("Cảm ơn đánh giá của bạn!");
});

const relatedListEl = document.querySelector(
    ".library-container__suggested-books ul"
);

/*getRelatedDocuments();*/

function getRelatedDocuments() {
    fetchRestful({
        url: config.locationVal + "/tai-lieu-lien-quan?slug=" + slug,
        method: "POST",
        contentType: "application/json",
        responseType: "html",
    }).then((response) => {
        if (response && response.trim() != "") {
            relatedListEl.classList.add("fade-out");
            setTimeout(() => {
                relatedListEl.classList.remove("skeleton-list", "fade-out");
                relatedListEl.classList.add("real-content");
                relatedListEl.innerHTML = response;
                requestAnimationFrame(() => {
                    relatedListEl.classList.add("fade-in");
                });
            }, 500);
        }
    });
}
