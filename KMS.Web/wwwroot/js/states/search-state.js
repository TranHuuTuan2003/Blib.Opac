import { SEARCH_STATE_TYPES } from "../common/constants.js";

let selectedSearchType = "init",
    currentFocusSearchType = null,
    currentPage = 1;

export function getSearchType() {
    return selectedSearchType;
}

export function setSearchType(value) {
    selectedSearchType = value;
}

export function getFocusSearchType() {
    return currentFocusSearchType;
}

export function setFocusSearchType(value) {
    currentFocusSearchType = value;
}

export function getCurrentPage() {
    return currentPage;
}

export function setCurrentPage(value) {
    currentPage = value;
}

export var InitAppState = {
    type: SEARCH_STATE_TYPES.INIT,
    page: 1,
    pageSize: 10,
    request: {
        searchBy: [
            ["option", "qs"],
            ["keyword", ""],
        ],
        sortBy: [["year_pub", "desc"]],
        filterBy: [],
    },
};

export var QuickAppState = {
    type: SEARCH_STATE_TYPES.QUICK,
    page: 1,
    pageSize: 10,
    request: {
        searchBy: [
            ["option", "qs"],
            ["keyword", ""],
        ],
        sortBy: [["year_pub", "desc"]],
        filterBy: [],
    },
};

export var BasicAppState = {
    type: SEARCH_STATE_TYPES.BASIC,
    page: 1,
    pageSize: 10,
    request: {
        searchBy: [],
        sortBy: [["year_pub", "desc"]],
        filterBy: [],
    },
};

export var AdvanceAppState = {
    type: SEARCH_STATE_TYPES.ADVANCE,
    page: 1,
    pageSize: 10,
    request: {
        searchBy: [],
        sortBy: [["year_pub", "desc"]],
        filterBy: [],
    },
};

export function getCurrentSearchState() {
    var currentSearchType = getSearchType();
    var state = null;
    if (currentSearchType == SEARCH_STATE_TYPES.INIT) {
        state = InitAppState;
    } else if (currentSearchType == SEARCH_STATE_TYPES.QUICK) {
        state = QuickAppState;
    } else if (currentSearchType == SEARCH_STATE_TYPES.BASIC) {
        state = BasicAppState;
    } else if (currentSearchType == SEARCH_STATE_TYPES.ADVANCE) {
        state = AdvanceAppState;
    }       

    return state;
}
