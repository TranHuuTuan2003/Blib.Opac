import "../../../css/libs/swiper.min.css";
import "../../../css/pages/home/style.css";
import { SEARCH_TYPES } from "../../common/constants.js";
import { createHeaderEvents } from "../../components/header/index.js";
import { createSearchBarEvents } from "../../components/search-bar/index.js";
import { createToggleButtonEvent } from "../../components/sidebar-menu/index.js";
import { Swiper } from "../../libs/swiper.min.js";
import { createLazyLoadIntersectionObserve } from "../../utils/lazy-load-util.js";

const input = document.querySelector(
    ".first-home-section .search-bar__inputSearch input"
);
const svg = document.querySelector(
    ".first-home-section .search-bar__inputSearch svg"
);
const searchBar = document.querySelector(".first-home-section .search-bar");

createHeaderEvents();
createSearchBarEvents(SEARCH_TYPES.REDIRECT);
createToggleButtonEvent();

input.addEventListener("focus", () => {
    searchBar.classList.add("grow");
    searchBar.classList.remove("shrink");
});

input.addEventListener("blur", () => {
    searchBar.classList.add("shrink");
    searchBar.classList.remove("grow");
});

svg.addEventListener("blur", () => {
    searchBar.classList.add("shrink");
    searchBar.classList.remove("grow");
});

svg.addEventListener("focus", () => {
    searchBar.classList.add("grow");
    searchBar.classList.remove("shrink");
});

input.focus();

// ===== XỬ LÝ ACTIVE SELECTOR ITEM =====
const selectorItems = document.querySelectorAll(
    ".collectionBlock__selectorItem"
);
if (selectorItems.length > 0) {
    selectorItems[0].classList.add("active");
}
selectorItems.forEach((item) => {
    item.addEventListener("click", () => {
        selectorItems.forEach((i) => i.classList.remove("active"));
        item.classList.add("active");
    });
});

// ===== SWIPER INIT =====
const slideCount = document.querySelectorAll(".swiper .swiper-slide").length;

new Swiper(".swiper", {
    loop: false,
    centeredSlides: slideCount > 1,
    slideToClickedSlide: true,
    pagination: {
        el: ".swiper-pagination",
        clickable: true,
    },
    initialSlide: 1,
    navigation: {
        nextEl: ".linking-section__button-next",
        prevEl: ".linking-section__button-prev",
    },
    breakpoints: {
        768: { slidesPerView: 1 },
        1024: { slidesPerView: 3 },
    },
});

createLazyLoadIntersectionObserve("img");
