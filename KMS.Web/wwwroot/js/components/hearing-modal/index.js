export function openHearingLoader() {
    document.body.classList.add("hearing-modal-open");
    const hearingModal = document.getElementById("hearingModal");
    hearingModal.style.display = "flex";
}

export function closeHearingLoader() {
    document.body.classList.remove("hearing-modal-open");
    setTimeout(() => {
        const hearingModal = document.getElementById("hearingModal");
        hearingModal.style.display = "none";
    }, 300);
}
