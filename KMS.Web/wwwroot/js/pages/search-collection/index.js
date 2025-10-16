import tippy, { hideAll } from "tippy.js";
import "tippy.js/dist/tippy.css"; // optional for styling
import "../../../css/components/digital-documents/style.css";
import "../../../css/components/search-result-card/style.css";
import "../../../css/components/search-result-paginator/style.css";
import "../../../css/pages/search-page/style.css";
import config from "../../common/config.js";
import { DB_TYPE, SEARCH_FORM_TYPES, SEARCH_STATE_TYPES, SEARCH_TYPES } from "../../common/constants.js";
import {
    createChangeEvent,
    createClickEvent,
    createScrollEvent,
    isTablet,
} from "../../common/main.js";
import {
    createAdvanceModalClickAndKeyUpEvents,
    createHiddenAdvanceModalEvent,
    createShownAdvanceModalEvent,
} from "../../components/advanced-search/index.js";
import { createHeaderEvents } from "../../components/header/index.js";
import { createSearchBarEvents } from "../../components/search-bar/index.js";
import { createToggleButtonEvent } from "../../components/sidebar-menu/index.js";
import {
    advanceFetch,
    basicFetch,
    fetchFiltering,
    fetchPaging,
    fetchYearPubSorting,
    getPagingFacetFilters,
    initFetch,
    quickFetch,
} from "../../services/search-service.js";
import {
    AdvanceAppState,
    BasicAppState,
    getCurrentPage,
    getSearchType,
    QuickAppState,
} from "../../states/search-state.js";
import { createClickSeeMoreSummaryEvent } from "../../utils/see-more-util.js";
import { showSuccessToast } from "../../utils/toastify-util.js";
import { fetchRestful } from "../../utils/api-util";
import { getCollectionTree } from "../../components/collection-tree/db-type.js";

const quickInput = document.querySelector(".search-bar input");

function focusQuickInput() {
    if (!isTablet()) {
        quickInput.focus();
    }
}

createHeaderEvents();
createToggleButtonEvent();
createShownAdvanceModalEvent();
createHiddenAdvanceModalEvent(focusQuickInput);
createAdvanceModalClickAndKeyUpEvents(SEARCH_TYPES.SELF, SEARCH_FORM_TYPES.COLLECTION);
createSearchBarEvents(SEARCH_TYPES.SELF, SEARCH_FORM_TYPES.COLLECTION);

focusQuickInput();

getCollectionTree();

createClickEvent("li.page-item:not(.active)", (target) => {
    var currentPage = getCurrentPage();
    var page = null;
    if (target.classList.contains("first")) {
        page = 1;
    } else if (target.classList.contains("next")) {
        page = currentPage + 1;
    } else if (target.classList.contains("previous")) {
        page = currentPage - 1;
    } else if (target.classList.contains("last")) {
        var lastPage = target.dataset.page;
        page = lastPage;
    } else {
        page = target.querySelector("a").textContent;
    }

    page = parseInt(page);
    fetchPaging(page);
    focusQuickInput();
});

createChangeEvent(".search-filter__item input", (target) => {
    var checked = target.checked;
    var value = target.value;
    var filterType = target.dataset.type;

    fetchFiltering(checked, value, filterType);
});

createClickEvent(".see-more-btn", (target) => {
    const code = target.dataset.code;
    const nextPage = parseInt(target.dataset.nextPage);

    const paging = {
        code: code,
        page: nextPage,
        pageSize: 5,
    };

    getPagingFacetFilters(target, paging);
});

createClickEvent(".search-sort__sort-type", (target) => {
    var value = target.dataset.sort;
    fetchYearPubSorting(value);
});

createClickSeeMoreSummaryEvent(".documentCard__info-summary");

createClickEvent(".documentCard__qr-icon", (target) => {
    const popup = target
        .closest(".documentCard__qr-wrapper")
        .querySelector(".documentCard__qr-popup");

    const qrImg = popup.querySelector("img");

    if (!qrImg.src) qrImg.src = qrImg.dataset.src;

    if (!target._tippy) {
        tippy(target, {
            content: `<img src="${qrImg.dataset.src}" width="150" height="150" />`,
            allowHTML: true,
            trigger: "click",
            interactive: true,
            placement: isTablet() ? "right" : "bottom",
            onShow(instance) {
                hideAll({ exclude: instance }); // chỉ cho mở 1 QR
            },
        });
    }

    target._tippy.show();
});

// createClickEvent(".documentCard__qr-icon", (target) => {
//     const wrapper = target.closest(".documentCard__qr-wrapper");
//     const popup = wrapper.querySelector(".documentCard__qr-popup");

//     document
//         .querySelectorAll(".documentCard__qr-popup.is-visible")
//         .forEach((p) => {
//             if (p !== popup) p.classList.remove("is-visible");
//         });

//     popup.classList.toggle("is-visible");
// });

// createClickEvent(document, (target) => {
//     document
//         .querySelectorAll(".documentCard__qr-popup.is-visible")
//         .forEach((popup) => {
//             if (
//                 !popup.contains(target) &&
//                 !target.closest(".documentCard__qr-wrapper")
//             ) {
//                 popup.classList.remove("is-visible");
//             }
//         });
// });

const sideBarLeft = document.querySelector(".main-container__filter");
const sideBarLeftOverlay = document.querySelector(
    ".main-container__search-filter-overlay"
);
const header = document.querySelector("header");
const searchBar = document.querySelector(".search-bar");

createClickEvent(".search-page__result-header-filter", () => {
    openFilterSidebar();
});

createClickEvent(document, (_, e) => {
    if (
        !e.target.closest(".main-container__filter") &&
        !e.target.closest(".search-page__result-header-filter") &&
        !e.target.closest(".menu-toggle--open")
    )
        closeFilterSidebar();
});

function openFilterSidebar() {
    sideBarLeft.classList.add("open");
    sideBarLeftOverlay.classList.add("open");
    document.body.style.overflow = "hidden";
}

function closeFilterSidebar() {
    sideBarLeft.classList.remove("open");
    sideBarLeftOverlay.classList.remove("open");
    document.body.style.overflow = "auto";
}

const searchBarWrapper = document.querySelector(".search-page__main-container");

createScrollEvent(() => {
    if (isTablet()) {
        var headerRect = header.getBoundingClientRect();
        var visibleHeight = Math.max(
            0,
            Math.min(header.offsetHeight, headerRect.bottom)
        );
        sideBarLeft.style.height = `calc(100dvh - ${visibleHeight}px - ${searchBar.clientHeight}px - var(--search-bar-margin-bottom) * 2)`;
    }
});

createClickEvent(".documentCard__save-wrapper", () => {
    showSuccessToast("Lưu tài liệu thành công!");
});

createClickEvent(".documentCard__digital-link", () => {
    window.open(config.locationVal + "/doc-tai-lieu", "_blank");
});
