import config from "../../common/config.js";
import { SEARCH_TYPES } from "../../common/constants.js";
import {
    createChangeEvent,
    createClickEvent,
    createKeyUpEvent,
    isTablet,
} from "../../common/main.js";
import { advanceFetch } from "../../services/search-service.js";
import { AdvanceAppState } from "../../states/search-state.js";
import { createDomEvent } from "../../utils/event-util.js";
import { buildAdvanceState } from "../../utils/search-state-util.js";

const advanceSearchModal = document.getElementById("advancedSearchModal");

const firstInputText = document.querySelector(
    "#advancedSearchModal input:not([type='radio'])"
);

export function showAdvanceModal() {
    const modal =
        bootstrap.Modal.getInstance(advanceSearchModal) ||
        new bootstrap.Modal(advanceSearchModal);
    modal.show();
}

export function hideAdvanceModal() {
    const modal =
        bootstrap.Modal.getInstance(advanceSearchModal) ||
        new bootstrap.Modal(advanceSearchModal);
    modal.hide();
}

export function createShownAdvanceModalEvent(callback) {
    createDomEvent(
        "shown.bs.modal",
        advanceSearchModal,
        callback ? callback : focusFirstInputText
    );
}

export function createHiddenAdvanceModalEvent(callback) {
    createDomEvent("hidden.bs.modal", advanceSearchModal, callback);
}

function focusFirstInputText() {
    if (!isTablet()) {
        firstInputText.focus();
    }
}

export function createAdvanceModalClickAndKeyUpEvents(
    searchType = SEARCH_TYPES.SELF
) {
    createKeyUpEvent(
        "#advancedSearchModal input:not([type='radio'])",
        (_, e) => {
            if (e.key == "Enter") {
                document.getElementById("btn-advance-search").click();
            }
        }
    );

    createClickEvent("#btn-advance-search", () => {
        buildAdvanceState();
        if (searchType == SEARCH_TYPES.SELF) {
            advanceFetch();
        } else if (searchType == SEARCH_TYPES.REDIRECT) {
            var url =
                config.searchUrl +
                "?data=" +
                encodeURIComponent(JSON.stringify(AdvanceAppState));

            window.open(url, "_blank");
        }
    });

    createClickEvent("#btn-advance-refresh", () => {
        document
            .querySelectorAll(
                "#advancedSearchModal select, #advancedSearchModal input"
            )
            .forEach((el) => {
                var tag = el.tagName.toLowerCase();

                if (tag == "select") {
                    const selectedIndex = el.dataset.selectedIndex;
                    if (selectedIndex) {
                        el.selectedIndex = selectedIndex;
                    } else {
                        el.selectedIndex = 0;
                    }
                }

                if (tag === "input") {
                    const type = (
                        el.getAttribute("type") || "text"
                    ).toLowerCase();
                    if (type === "radio") {
                        el.checked = false;
                    } else {
                        el.value = "";
                    }
                }
            });

        focusFirstInputText();
    });

    createDomEvent("mousedown", ".search-advanced__fuzzy input", (target) => {
        if (target.checked) {
            target.dataset.waschecked = "true";
        } else {
            target.dataset.waschecked = "false";
        }
    });

    createDomEvent("click", ".search-advanced__fuzzy input", (target) => {
        if (target.dataset.waschecked === "true") {
            target.checked = false;
            target.dataset.waschecked = "false";
        }
    });

    createClickEvent(".search-advanced__fuzzy label", (target) => {
        var prev = target.previousElementSibling;
        prev.click();
        prev.dispatchEvent(
            new MouseEvent("mousedown", {
                bubbles: true,
                cancelable: true,
                view: window,
            })
        );
    });

    createClickEvent(".search-advanced__close", () => {
        hideAdvanceModal();
    });

    createChangeEvent(
        ".search-advanced__main-select, .search-advanced__operator-select",
        (target) => {
            target
                .closest(".search-advanced__row")
                .querySelector("input:not([type='radio'])")
                ?.focus();
        }
    );
}
