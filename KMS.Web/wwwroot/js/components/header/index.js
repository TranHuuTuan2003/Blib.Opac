import {
    closestElement,
    createClickEvent,
    createScrollEvent,
    isTablet,
} from "../../common/main.js";

export function createHeaderEvents() {
    if (!isTablet()) {
        var header = document.getElementById("header");

        createScrollEvent((win, e) => {
            var y = window.scrollY;

            if (y > 100) {
                header.classList.remove("original");
                header.classList.add("white");
            } else {
                header.classList.remove("white");
                header.classList.add("original");
            }
        });

        const toggleBtnEl = document.querySelector("#collectionToggle");
        const dropdownEl = document.querySelector("#dropdownCollection");
        const tabButtonsEls = document.querySelectorAll(
            ".dropdown-collection__left li"
        );
        const tabContentsEls = document.querySelectorAll(".tab-content");

        createClickEvent(toggleBtnEl, (target, e) => {
            // Toggle dropdown
            e.preventDefault();

            const isOpen = dropdownEl.style.display === "block";
            if (isOpen) {
                dropdownEl.style.display = "none";
                document.body.classList.remove("no-scroll");
            } else {
                dropdownEl.style.display = "block";
                document.body.classList.add("no-scroll");
            }
        });

        createClickEvent(document, (target) => {
            if (
                !closestElement(target, dropdownEl) &&
                !closestElement(target, toggleBtnEl)
            ) {
                dropdownEl.style.display = "none";
                document.body.classList.remove("no-scroll");
            }
        });

        createClickEvent(document, (target) => {
            // Tab switch
            const btn = [...tabButtonsEls].find((el) =>
                closestElement(target, el)
            );
            if (btn) {
                tabButtonsEls.forEach((b) => b.classList.remove("active"));
                btn.classList.add("active");

                const tabId = btn.dataset.tab;
                tabContentsEls.forEach((tab) => {
                    tab.classList.toggle("active", tab.id === tabId);
                });
            }
        });
    }
}
