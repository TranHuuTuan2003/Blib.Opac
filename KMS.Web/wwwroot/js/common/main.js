import { createDomEvent, createEventManager } from "../utils/event-util.js";

export function openLoader() {
    document.body.classList.add("loader-modal-open");
    document.getElementById("loaderModal").style.display = "flex";
}

export function closeLoader() {
    document.body.classList.remove("loader-modal-open");
    document.body.style.removeProperty("--scrollbar-width");
    document.querySelector(".loader").classList.add("hide");
    setTimeout(() => {
        document.getElementById("loaderModal").style.display = "none";
        document.querySelector(".loader").classList.remove("hide");
    }, 300);
}

export function createClickEvent(target, callback) {
    createDomEvent("click", target, callback);
}

export function createChangeEvent(target, callback) {
    createDomEvent("change", target, callback);
}

export function createInputEvent(target, callback) {
    createDomEvent("input", target, callback);
}

export function createKeyUpEvent(target, callback) {
    createDomEvent("keyup", target, callback);
}

export function createScrollEvent(callback) {
    const manager = createEventManager("scroll", window);
    manager.add((e) => callback(window, e));
}

export function createDomLoadedEvent(callback) {
    const manager = createEventManager("DOMContentLoaded", document);
    manager.add((e) => callback(document, e));
}

export function scrollToElement(code) {
    document.querySelector(code).scrollIntoView({
        behavior: "smooth",
        block: "center",
    });
}

export function closestElement(startEl, targetEl) {
    let el = startEl;
    while (el) {
        if (el === targetEl) return el;
        el = el.parentElement;
    }
    return null;
}

export function isTablet() {
    return window.matchMedia("(max-width: 1199px)").matches;
}

export function isMobile() {
    return window.matchMedia("(max-width: 768px)").matches;
}
