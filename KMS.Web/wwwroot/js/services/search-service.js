import config from "../common/config.js";
import { SEARCH_FORM_TYPES, SEARCH_STATE_TYPES, SEARCH_TYPES } from "../common/constants.js";
import { closeLoader, isTablet, openLoader } from "../common/main.js";
import { hideAdvanceModal } from "../components/advanced-search/index.js";
import { getCurrentCollectionId } from "../components/collection-tree/db-type.js";
import { getCurrentSelectedQuickField } from "../components/search-bar/index.js";
import {
    AdvanceAppState,
    BasicAppState,
    getCurrentSearchState,
    InitAppState,
    QuickAppState,
    setCurrentPage,
    setSearchType,
} from "../states/search-state.js";
import { isAdvanceSearchClause } from "../utils/advance-clause-util.js";
import { fetchRestful } from "../utils/api-util.js";
import { createLazyLoadIntersectionObserve } from "../utils/lazy-load-util.js";
import { han, handleChangeState, handleChangeStatedleChangeState } from "../utils/search-state-util.js";
import { observeForSeeingMore } from "../utils/see-more-util.js";
import {
    skeletonDocumentCardList,
    skeletonFacetList,
} from "../utils/skeleton-loading-util.js";
import { showWarningToast } from "../utils/toastify-util.js";

var quickInput = document.querySelector(".search-bar input");
var resultBlock = document.getElementById("search-page-result-block");
var facetFilterBlock = document.getElementById("search-page-filter-block");
var isInit = true;
var searchResults = null;

export function validateAndSearch(searchType, btnSearch, formType) {
    var searchBar = btnSearch.closest(".search-bar");
    var input = searchBar.querySelector("input");
    var value = input.value;

    if (!value) {
        showWarningToast("Vui lòng nhập nội dung tìm kiếm!");
        return;
    } else if (value.length == 1) {
        showWarningToast("Vui lòng nhập thêm nội dung tìm kiếm!");
        return;
    }
    if (isAdvanceSearchClause(value)) {
        QuickAppState.request.advanceWhereClause = value;
    } else {
        delete QuickAppState.request.advanceWhereClause;
    }

    var currentSelectedQuickField = getCurrentSelectedQuickField();
    QuickAppState.request.searchBy[0][1] = currentSelectedQuickField;
    QuickAppState.request.searchBy[1][1] = value;
    QuickAppState.page = 1;
    QuickAppState.pageSize = 10;
    QuickAppState.request.filterBy = [];
    QuickAppState.request.sortBy[0][1] = "desc";

    if (searchType == SEARCH_TYPES.SELF) {
        quickFetch(formType);
    } else if (searchType == SEARCH_TYPES.REDIRECT) {
        var url =
            config.searchUrl +
            "?data=" +
            encodeURIComponent(JSON.stringify(QuickAppState));

        window.open(url, "_blank");
    }
}

export function initFetch(formType = SEARCH_FORM_TYPES.SEARCH) {
    if (isTablet()) {
        quickInput.blur();
    }

    setSearchType(SEARCH_STATE_TYPES.INIT);
    // assignTenantIntoState();

    // getFacetFilters(QuickAppState);
    // fetchStatLog(QuickAppState);
    InitAppState.request.sortBy[0][1] = "desc";

    getFacetFilters();
    Promise.allSettled([fetchSearching(InitAppState,formType)]).then(function () {
        handleSearchResults();
    });
}

export function quickFetch(formType = SEARCH_FORM_TYPES.SEARCH) {
    if (isTablet()) {
        quickInput.blur();
    }

    setSearchType(SEARCH_STATE_TYPES.QUICK);
    isInit = false;

    // assignTenantIntoState();

    // getFacetFilters(QuickAppState);
    // fetchStatLog(QuickAppState);
    QuickAppState.page = 1;
    QuickAppState.request.sortBy[0][1] = "desc";
    if (formType == SEARCH_FORM_TYPES.COLLECTION) {
        var collectionId = getCurrentCollectionId();
        handleChangeState(QuickAppState, "filterBy", "type", window.dbType);
        handleChangeState(
            QuickAppState,
            "filterBy",
            "collection",
            collectionId
        );
    }

    if (formType == SEARCH_FORM_TYPES.SEARCH) {
        getFacetFilters();
    }

    Promise.allSettled([fetchSearching(QuickAppState, formType)]).then(function () {
        handleSearchResults();
    });
}

export function basicFetch(formType = SEARCH_FORM_TYPES.SEARCH) {
    if (isTablet()) {
        quickInput.blur();
    }

    setSearchType(SEARCH_STATE_TYPES.BASIC);
    isInit = false;

    // assignTenantIntoState();

    BasicAppState.page = 1;

    // getFacetFilters(BasicAppState);
    // fetchStatLog(BasicAppState);
    BasicAppState.request.sortBy[0][1] = "desc";
    if (formType == SEARCH_FORM_TYPES.COLLECTION) {
        var collectionId = getCurrentCollectionId();
        handleChangeState(QuickAppState, "filterBy", "type", dbType);
        handleChangeState(
            QuickAppState,
            "filterBy",
            "collection",
            collectionId
        );
    }

    if (formType == SEARCH_FORM_TYPES.SEARCH) {
        getFacetFilters();
    }

    Promise.allSettled([fetchSearching(BasicAppState, formType)])
        .then(function () {
            handleSearchResults();
        })
        .finally(function () {
            hideAdvanceModal();
        });
}

export function advanceFetch(formType = SEARCH_FORM_TYPES.SEARCH) {
    if (isTablet()) {
        quickInput.blur();
    }

    setSearchType(SEARCH_STATE_TYPES.ADVANCE);
    isInit = false;

    // assignTenantIntoState();

    AdvanceAppState.page = 1;
    AdvanceAppState.request.sortBy[0][1] = "desc";
    if (formType == SEARCH_FORM_TYPES.COLLECTION) {
        var collectionId = getCurrentCollectionId();
        handleChangeState(QuickAppState, "filterBy", "type", dbType);
        handleChangeState(
            QuickAppState,
            "filterBy",
            "collection",
            collectionId
        );
    }

    // getFacetFilters(AdvanceAppState);
    // fetchStatLog(AdvanceAppState);

    if (formType == SEARCH_FORM_TYPES.SEARCH) {
        getFacetFilters();
    }

    Promise.allSettled([fetchSearching(AdvanceAppState, formType)])
        .then(function () {
            handleSearchResults();
        })
        .finally(function () {
            hideAdvanceModal();
        });
}

export function fetchSearching(state, formType) {
    // if (hasFacetFilter) {
    //     document.querySelector(".result-block").innerHTML =
    //         resultSkeletonLoadingHtml;
    // } else {
    //     document.querySelector(".result-block__extra").innerHTML =
    //         resultSkeletonLoadingHtml;
    // }
    resultBlock.innerHTML = skeletonDocumentCardList;
    window.scrollTo({
        top: 0,
        behavior: "instant",
    });

    var url = formType == SEARCH_FORM_TYPES.SEARCH ? config.searchUrl : config.searchCollectionUrl;
    
    console.log(formType)
    return fetchRestful({
        url: url,
        method: "POST",
        data: state,
        responseType: "html",
        contentType: "application/json",
    })
        .then((response) => {
            searchResults = response;
        })
        .finally(closeLoader);
}

export function handleSearchResults() {
    resultBlock.classList.remove("fade-in");
    resultBlock.classList.add("fade-out");
    setTimeout(() => {
        resultBlock.innerHTML = searchResults;
        resultBlock.classList.remove("fade-out");
        resultBlock.classList.add("fade-in");
        observeForSeeingMore(".documentCard__info-summary");
        createLazyLoadIntersectionObserve(".documentCard__image");
    }, 300);
}

export function getFacetFilters() {
    var state = getCurrentSearchState();
    facetFilterBlock.innerHTML = skeletonFacetList;
    fetchRestful({
        url: `${config.locationVal}/FacetFilter/${state.type}`,
        method: "POST",
        data: {
            searchRequest: state.request,
            codes: ["bt", "au", "kw", "yr", "su", "tn"],
        },
        responseType: "html",
        contentType: "application/json",
    })
        .then((response) => {
            if (response && response.trim() !== "") {
                facetFilterBlock.classList.remove("fade-in");
                facetFilterBlock.classList.add("fade-out");
                setTimeout(() => {
                    facetFilterBlock.innerHTML = response;
                    facetFilterBlock.classList.remove("fade-out");
                    facetFilterBlock.classList.add("fade-in");
                }, 300);
            }
        })
        .finally(closeLoader);
}

export function getPagingFacetFilters(seeMoreButton, paging) {
    var state = getCurrentSearchState();

    openLoader();
    fetchRestful({
        url: `${config.locationVal}/FacetFilter/${state.type}`,
        method: "POST",
        data: {
            searchRequest: state.request,
            codes: ["bt", "au", "kw", "yr", "su"],
            paging: paging,
        },
        responseType: "html",
        contentType: "application/json",
    })
        .then((response) => {
            if (response && response.trim() !== "") {
                const parser = new DOMParser();
                const doc = parser.parseFromString(response, "text/html");
                const el = doc.querySelector(".opacity-25");

                if (el) {
                    seeMoreButton.remove();
                    return;
                }

                const facetFilterParent = seeMoreButton.parentElement;
                seeMoreButton.remove();
                facetFilterParent.innerHTML += response;

                setTimeout(() => {
                    facetFilterParent.scrollTo({
                        top: facetFilterParent.scrollHeight,
                        behavior: "smooth",
                    });
                }, 0);
            }
        })
        .finally(closeLoader);
}

export function fetchPaging(page) {
    if (isNaN(page)) return;

    var state = getCurrentSearchState();

    if (!state) return;
    state.page = page;

    setCurrentPage(page);
    fetchSearching(state).then(handleSearchResults);
}

export function fetchFiltering(checked, value, filterType) {
    var state = getCurrentSearchState();
    state.page = 1;

    if (checked) {
        var exists = state.request.filterBy.some(function (pair) {
            return pair[0] === filterType && pair[1] === value;
        });

        if (!exists) {
            state.request.filterBy.push([filterType, value]);
            state.request.sortBy[0][1] = "desc";
        }
    } else {
        state.request.filterBy = state.request.filterBy.filter(function (pair) {
            return !(pair[0] === filterType && pair[1] === value);
        });
        state.request.sortBy[0][1] = "desc";
    }

    fetchSearching(state).then(handleSearchResults);
}

export function fetchYearPubSorting(value) {
    var state = getCurrentSearchState();

    state.request.sortBy[0][1] = value;
    fetchSearching(state).then(handleSearchResults);
}
