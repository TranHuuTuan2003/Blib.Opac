import {
    closestElement,
    createClickEvent,
    createScrollEvent,
    isTablet,
} from "../../common/main.js";

const header = document.querySelector("header");
const sideBarMenu = document.querySelector(".sidebar-menu");
const sideBarMenuOverlay = document.querySelector(".sidebar-menu__overlay");
const openSidebarBtn = document.querySelector(".menu-toggle--open");
const closeSidebarBtn = document.querySelector(".menu-toggle--close");

createScrollEvent(() => {
    if (isTablet()) {
        var headerRect = header.getBoundingClientRect();
        var headerHeight = Math.max(0, headerRect.bottom);
        const paddingY = parseFloat(
            window.getComputedStyle(searchBarWrapper).paddingBottom
        );

        if (headerHeight == 0) return;

        sideBarMenu.style.height = `calc(100dvh - ${headerHeight}px - ${
            searchBar.clientHeight
        }px - ${paddingY * 2}px + var(--facet-sidebar-padding, 0px) + 2px)`;
    }
});

export function openSidebar() {
    sideBarMenu.style.right = "0";
    sideBarMenuOverlay.style.display = "block";
    document.body.style.overflow = "hidden";
    openSidebarBtn.style.display = "none";
    closeSidebarBtn.style.display = "block";
}

export function closeSidebar() {
    sideBarMenu.style.right = "-300px";
    sideBarMenuOverlay.style.display = "none";
    document.body.style.overflow = "auto";
    openSidebarBtn.style.display = "block";
    closeSidebarBtn.style.display = "none";
}

export function createToggleButtonEvent() {
    createClickEvent(openSidebarBtn, () => {
        openSidebar();
    });

    createClickEvent(closeSidebarBtn, () => {
        closeSidebar();
    });

    createClickEvent(document, (target, e) => {
        if (
            !closestElement(target, sideBarMenu) &&
            !closestElement(target, closeSidebarBtn) &&
            !closestElement(target, openSidebarBtn) &&
            !e.target.closest(".search-page__result-header-filter") &&
            !e.target.closest(".main-container__search-filter")
        ) {
            closeSidebar();
        }
    });
}
