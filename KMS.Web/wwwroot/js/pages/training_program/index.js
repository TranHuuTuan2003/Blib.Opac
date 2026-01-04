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
