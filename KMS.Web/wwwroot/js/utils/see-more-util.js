import { createClickEvent } from "../common/main";

export function observeForSeeingMore(summaryTextSelector) {
    const summaryBlocks = document.querySelectorAll(summaryTextSelector);

    summaryBlocks.forEach((block) => {
        const summaryText = block.querySelector(".summary-text");
        const seeMoreBtn = block.parentElement.querySelector(
            ".btn-see-more-summary"
        );

        if (!summaryText || !seeMoreBtn) return;

        const lineHeight = parseInt(getComputedStyle(summaryText).lineHeight);
        const maxHeight = lineHeight * 2;

        if (summaryText.offsetHeight > maxHeight) {
            seeMoreBtn.classList.add("arrow-down");
            seeMoreBtn.style.display = "inline";
        } else {
            seeMoreBtn.style.display = "none";
        }
    });
}

export function createClickSeeMoreSummaryEvent(summaryTextSelector) {
    createClickEvent(".btn-see-more-summary", (target) => {
        var pTag = target.parentElement.querySelector(summaryTextSelector);
        var clamp = "2";

        if (target && pTag) {
            const isExpanded = target.classList.contains("up");
            target.classList.toggle("up", !isExpanded);
            target.classList.toggle("down", isExpanded);

            target.querySelector("span.up").style.display = isExpanded
                ? "none"
                : "inline";
            target.querySelector("span.down").style.display = isExpanded
                ? "inline"
                : "none";
            pTag.style.webkitLineClamp = isExpanded ? clamp : "initial";

            const textNode = Array.from(target.childNodes).find(
                (n) => n.nodeType === 3
            );

            if (textNode) {
                textNode.textContent = isExpanded ? "Xem thêm" : "Thu gọn";
            }
        }
    });
}
