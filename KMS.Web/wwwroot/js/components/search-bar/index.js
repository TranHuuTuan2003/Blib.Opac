import { SEARCH_FORM_TYPES } from "../../common/constants.js";
import {
    createClickEvent,
    createInputEvent,
    createKeyUpEvent,
} from "../../common/main.js";
import { validateAndSearch } from "../../services/search-service.js";
import { showAdvanceModal } from "../advanced-search/index.js";

let currentSelectedQuickField = "qs";

export function getCurrentSelectedQuickField() {
    return currentSelectedQuickField;
}

export function setCurrentSelectedQuickField(value) {
    currentSelectedQuickField = value;
}

function onQuickInputHasValue(inputEl) {
    var searchBar = inputEl.closest(".search-bar");
    var clearBtn = searchBar.querySelector(
        ".search-bar__clear-btn--mobile.clear"
    );
    var searchBtn = searchBar.querySelector(
        ".search-bar__search-btn--mobile.search"
    );
    clearBtn.style.display = "block";
    searchBtn.classList.remove("disabled");
}

function onQuickInputNoValue(inputEl) {
    var searchBar = inputEl.closest(".search-bar");
    var clearBtn = searchBar.querySelector(
        ".search-bar__clear-btn--mobile.clear"
    );
    var searchBtn = searchBar.querySelector(
        ".search-bar__search-btn--mobile.search"
    );
    clearBtn.style.display = "none";
    searchBtn.classList.add("disabled");
}

export function createSearchBarEvents(searchType, formType = SEARCH_FORM_TYPES.SEARCH) {
    createClickEvent(".search-bar__filterSearch", (target) => {
        target.classList.toggle("open");
    });

    createClickEvent(".search-bar__dropdown li[data-type]", (target) => {
        var field = target.getAttribute("data-type") || "qs";
        setCurrentSelectedQuickField(field);
        var searchBar = target.closest(".search-bar");
        var filterLabel = searchBar.querySelector(".search-bar__filterLabel");
        filterLabel.textContent = (target.textContent || "").trim();
        searchBar
            .querySelector(".search-bar__filterSearch")
            .classList.remove("open");
        var inputEl = searchBar.querySelector("input");
        inputEl.focus();
    });

    createClickEvent(document, (target) => {
        if (!target.closest("#filterToggle")) {
            document
                .querySelectorAll(".search-bar__filterSearch")
                .forEach((el) => {
                    el.classList.remove("open");
                });
        }
    });

    createInputEvent(".search-bar__inputSearch input", (target) => {
        if (target.value && target.value.length > 0) {
            onQuickInputHasValue(target);
        } else {
            onQuickInputNoValue(target);
        }
    });

    createClickEvent(".search-bar__actions--mobile .clear", (target) => {
        var input = target.closest(".search-bar").querySelector("input");
        input.value = "";
        input.focus();
    });

    createClickEvent(".search-bar__actions--mobile .search", (target) => {
        validateAndSearch(searchType, target, formType);
    });

    createClickEvent(".search-bar__clear-btn--mobile.advance", () => {
        showAdvanceModal();
    });

    createClickEvent(".search-bar__buttonSearch", (target) => {
        validateAndSearch(searchType, target, formType);
    });

    createKeyUpEvent(".search-bar__inputSearch input", (target, e) => {
        if (e.key == "Enter") {
            target
                .closest(".search-bar")
                .querySelector(".search-bar__buttonSearch")
                .click();
        }
    });
}
