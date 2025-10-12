import "../../../css/components/hearing-modal/hearing-modal.css";
import "../../../css/pages/chatbot/font.css";
import "../../../css/pages/chatbot/style.css";
import config from "../../common/config.js";
import { SEARCH_TYPES } from "../../common/constants.js";
import {
    createClickEvent,
    createInputEvent,
    createKeyUpEvent,
    isMobile,
    isTablet,
} from "../../common/main.js";
import { createAdvanceModalClickAndKeyUpEvents } from "../../components/advanced-search/index.js";
import {
    closeHearingLoader,
    openHearingLoader,
} from "../../components/hearing-modal/index.js";
import { createSearchBarEvents } from "../../components/search-bar/index.js";
import { createToggleButtonEvent } from "../../components/sidebar-menu/index.js";
import { AdvanceAppState, QuickAppState } from "../../states/search-state.js";
import { fetchRestful } from "../../utils/api-util.js";
import { pushInSearchByAdvanceState } from "../../utils/search-state-util.js";
import { showInfoToast, showWarningToast } from "../../utils/toastify-util.js";
import { Uuidv4 } from "../../utils/uuid-util.js";

document.querySelector("button[data-model=ucgpt]").remove();

createToggleButtonEvent();
createSearchBarEvents(SEARCH_TYPES.REDIRECT);
createAdvanceModalClickAndKeyUpEvents(SEARCH_TYPES.REDIRECT);

QuickAppState.pageSize = 3;
AdvanceAppState.pageSize = 3;

var conversation = document.getElementById("conversation");
var conversationInput = document.getElementById("conversation-input");
var listConversationButtons = document.querySelector(
    ".chatbot__list-conversation-buttons"
);
const leftColumn = document.querySelector(".chatbot__left-column");
const leftColumnOverlay = document.querySelector(
    ".chatbot__left-column-overlay"
);

var conversationState = {};

var selectedConversationElement = null;
var selectedModelButton = document.querySelector(
    ".chatbot__model-switch button"
);

function buildConversationButton(text, id) {
    var html =
        `<a class="chatbot__left-column-button" data-id="${id}">` +
        text +
        "</a>";
    return html;
}

function buildInitialBotMessage() {
    var html =
        '<div class="chatbot__bot-message-container">' +
        '    <div class="chatbot__bot-avatar">' +
        `        <img src="${config.locationVal}/img/chatbot/ucvn_bot.png" />` +
        "    </div>" +
        '    <div class="chatbot__message-wrapper">' +
        '        <p class="chatbot__sentence-token">Xin chào! Mình là UC Bot, trợ lý ảo của UCVN.' +
        "        </p>" +
        '        <p class="chatbot__sentence-token">Bạn đang cần tìm sách, tài liệu, hay cần hỗ trợ tra cứu?' +
        "            Mình luôn sẵn sàng giúp!</p>" +
        "    </div>" +
        "</div>";

    return html;
}

function buildBotTypingMessage() {
    var html =
        '<div id="typing-message" class="chatbot__bot-message-container">' +
        '    <div class="chatbot__bot-avatar">' +
        `        <img src="${config.locationVal}/img/chatbot/ucvn_bot.png" />` +
        "    </div>" +
        '    <div class="chatbot__message-wrapper typing">' +
        '        <div class="chatbot__sentence-token dot-elastic"></div>' +
        "    </div>" +
        "</div>";

    return html;
}

function buildBotMessage(output) {
    var html =
        '<div class="chatbot__bot-message-container">' +
        '    <div class="chatbot__bot-avatar">' +
        `        <img src="${config.locationVal}/img/chatbot/ucvn_bot.png" />` +
        "    </div>" +
        '    <div class="chatbot__message-wrapper">' +
        `<p class="chatbot__sentence-token">${output?.normalText?.[0]}</p>` +
        '<ul class="chatbot__doc-list">' +
        (output.books?.length > 0
            ? output.books
                  .map((book, i) => {
                      return (
                          "<li>" +
                          `<a href="${
                              book.link
                          }" target="_blank" class="chatbot__sentence-token">${
                              book.title +
                              (book.author ? " - " + book.author : "") +
                              (book.year ? " - " + book.year : "")
                          }</a>` +
                          "</li>" +
                          (i != output.books.length - 1 ? "<hr />" : "")
                      );
                  })
                  .join("")
            : "") +
        "</ul>" +
        output.normalText
            .map((text, i) => {
                if (i != 0)
                    return `<p class="chatbot__sentence-token">${text}</p>`;
            })
            .join("") +
        '        <div class="chatbot__message-wrapper-actions">' +
        '            <a class="chatbot__message-action">' +
        `                <img src="${config.locationVal}/img/icons/copy_o.svg" />` +
        "            </a>" +
        '            <a class="chatbot__message-action">' +
        `                <img src="${config.locationVal}/img/icons/sound_o.svg" />` +
        "            </a>" +
        '            <a class="chatbot__message-action">' +
        `                <img src="${config.locationVal}/img/icons/like_o.svg" />` +
        "            </a>" +
        '            <a class="chatbot__message-action">' +
        `                <img src="${config.locationVal}/img/icons/share_f.svg" />` +
        "            </a>" +
        '            <a class="chatbot__message-action">' +
        `                <img src="${config.locationVal}/img/icons/refresh_f.svg" />` +
        "            </a>" +
        "        </div>" +
        "    </div>" +
        "</div>";

    return html;
}

function buildBotErrorMessage(output) {
    var html =
        '<div class="chatbot__bot-message-container">' +
        '    <div class="chatbot__bot-avatar">' +
        `        <img src="${config.locationVal}/img/chatbot/ucvn_bot.png" />` +
        "    </div>" +
        '    <div class="chatbot__message-wrapper">' +
        `<p class="chatbot__sentence-token">${output}</p>` +
        "    </div>" +
        "</div>";

    return html;
}

function buildMyMessage(value) {
    var html =
        '<div class="chatbot__my-message-container">' +
        '    <div class="chatbot__message-wrapper">' +
        `        <p class="chatbot__sentence-token">${value}` +
        "        </p>" +
        "    </div>" +
        '    <div class="chatbot__message-wrapper-actions">' +
        '        <a class="chatbot__message-action">' +
        `            <img src="${config.locationVal}/img/icons/copy_o.svg" />` +
        "        </a>" +
        '        <a class="chatbot__message-action">' +
        `            <img src="${config.locationVal}/img/icons/pen_o.svg" />` +
        "        </a>" +
        "    </div>" +
        "</div>";

    return html;
}

document.querySelector("#conversation-input").focus();

createClickEvent(".chatbot__left-column-button", (target) => {
    if (selectedConversationElement) {
        selectedConversationElement.classList.remove("active");
        var currentId = selectedConversationElement.dataset.id;
        conversationState[currentId] = conversation.innerHTML || "";
    }

    target.classList.add("active");
    selectedConversationElement = target;
    var id = target.dataset.id;

    conversation.innerHTML = conversationState[id] || "";
    if (!isMobile()) {
        conversationInput.focus();
    } else {
        closeLeftColumn();
    }
    scrollToBottom();
});

createClickEvent(".chatbot__new-conversation-button", (target) => {
    var id = Uuidv4();
    listConversationButtons.insertAdjacentHTML(
        "afterbegin",
        buildConversationButton("Mới", id)
    );
    var newButton = listConversationButtons.querySelector(
        ".chatbot__left-column-button"
    );
    selectedConversationElement = newButton;
    newButton.classList.add("active");

    conversation.innerHTML = conversation.innerHTML || "";
    conversation.innerHTML += buildInitialBotMessage();
});

createClickEvent(".chatbot__action.link", () => {
    alert("Đính kèm file!");
});

const recognition = new (window.SpeechRecognition ||
    window.webkitSpeechRecognition)();

createClickEvent(".chatbot__action.voice", (target) => {
    try {
        let transcript = "";
        recognition.lang = "vi-VN";
        recognition.continuous = false;
        recognition.interimResults = false;

        recognition.onstart = () => openHearingLoader();
        recognition.onresult = (event) => {
            transcript = event.results[0][0].transcript;
        };
        recognition.onerror = (event) =>
            showWarningToast("Không kết nối được với voice!");
        recognition.onend = () => {
            closeHearingLoader();
            setTimeout(() => {
                if (transcript) {
                    var container = target.closest(".input-container");
                    var input = container.querySelector("input");
                    input.value = transcript;
                    onHasValueMobileInput(container);
                    if (!isTablet()) {
                        input.focus();
                    }
                    container.querySelector(".chatbot__action.send").click();
                }
            }, 300);
        };

        recognition.start();
    } catch (error) {
        showWarningToast(
            "Trình duyệt của bạn không hỗ trợ tính năng nghe âm thanh!"
        );
    }
});

createClickEvent(".chatbot__action.send", (target) => {
    sendRequest(target);
});

function splitOutput(output) {
    const lines = output
        .split("\n")
        .map((l) => l.trim())
        .filter((l) => l !== "");

    let books = [];
    let normalText = [];
    let current = null;

    for (let line of lines) {
        if (/^\d+\.\s/.test(line)) {
            if (current) books.push(current);
            current = {
                title: line.replace(/^\d+\.\s*/, "").replace(/\*\*/g, ""),
            };
        } else if (/^- (\*\*)?Tác giả:(\*\*)?/.test(line) && current) {
            current.author = line
                .replace(/^- (\*\*)?Tác giả:(\*\*)?/, "")
                .trim();
        } else if (/^- (\*\*)?Năm xuất bản:(\*\*)?/.test(line) && current) {
            current.year = line
                .replace(/^- (\*\*)?Năm xuất bản:(\*\*)?/, "")
                .trim();
        } else if (/^- (\*\*)?\[.*\](\*\*)?/.test(line) && current) {
            const url = line.match(/\((.*?)\)/)?.[1];
            current.link = url;
        } else {
            // dòng không thuộc sách
            // nếu current null (trước sách) hoặc line không phải thuộc sách (sau sách) → push normalText
            if (!current || (current && !line.startsWith("-"))) {
                normalText.push(line.replace(/\*\*/g, ""));
            }
        }
    }

    if (current) books.push(current);

    return { books, normalText };
}

function findAnswerWithLegacyApi(state, value) {
    fetchRestful({
        url: config.locationVal + "/send?hasFacetFilter=true",
        method: "POST",
        responseType: "html",
        contentType: "application/json",
        data: state,
    })
        .then((response) => {
            if (response && response.trim() != "") {
                document.getElementById("typing-message").remove();
                conversation.innerHTML += response;
            } else {
                showWarningToast(
                    "Hệ thống đã xảy ra lỗi không mong muốn! Bạn vui lòng thử lại sau."
                );
            }
        })
        .catch(onChatbotError)
        .finally(() => {
            var isRenamed = selectedConversationElement.dataset.rename;
            if (!isRenamed) {
                selectedConversationElement.textContent = value;
                selectedConversationElement.dataset.rename = true;
            }
            scrollToBottom();
            currentLegacySearchType = state.type;
        });
}

const acronymCriteriaList = ["ti", "au", "yr", "kw"];
var acronymCriteriaState = {};
var currentLegacySearchType = null;

function sendRequest(sendButtonElement) {
    var container = sendButtonElement.closest(".input-container");
    var input = container.querySelector("input");

    if (isMobile()) {
        input.blur();
    }

    var value = input.value.trim();

    if (!value) {
        showWarningToast(
            "Bạn cần nhập thông tin để thực hiện cuộc trò chuyện này!"
        );
        input.value = "";
        input.focus();
        onNoValueMobileInput(container);
        return;
    }
    if (value.length == 1) {
        showWarningToast(
            "Bạn cần nhập thêm thông tin để thực hiện cuộc trò chuyện này!"
        );
        input.value = "";
        input.focus();
        onNoValueMobileInput(container);
        return;
    }

    QuickAppState.request.searchBy[1][1] = value;
    QuickAppState.page = 1;
    QuickAppState.request.filterBy = [];
    QuickAppState.request.sortBy[0][1] = "desc";

    conversation.innerHTML += buildMyMessage(value);
    conversation.innerHTML += buildBotTypingMessage();
    input.value = "";
    onNoValueMobileInput(container);
    if (!isTablet()) {
        input.focus();
    }

    scrollToBottom();

    var model = selectedModelButton.dataset.model;

    if (model == "ucbrain") {
        fetchRestful({
            url: "https://thuvien.ftu.edu.vn:5530/Services/ChatBot/chat/",
            method: "POST",
            contentType: "application/json",
            data: {
                message: value,
            },
        })
            .then((response) => {
                if (response) {
                    // empty => dùng quick search
                    if (!response.message || response.results.length == 0) {
                        findAnswerWithLegacyApi(QuickAppState, value);
                    }
                    // ngược lại phân tách response tìm advance
                    else {
                        var data = response.results[0].input_fields_clean;
                        acronymCriteriaState = {};
                        acronymCriteriaList.forEach((ac) => {
                            if (data[ac]) {
                                acronymCriteriaState[ac] = data[ac]
                                    .split(";")
                                    .map((kw) => kw.trim());
                            }
                        });
                        AdvanceAppState.page = 1;
                        AdvanceAppState.request.searchBy = [];
                        AdvanceAppState.request.filterBy = [];
                        AdvanceAppState.request.sortBy[0][1] = "desc";
                        for (var key in acronymCriteriaState) {
                            acronymCriteriaState[key].forEach((value) => {
                                pushInSearchByAdvanceState(
                                    AdvanceAppState,
                                    key,
                                    value
                                );
                                pushInSearchByAdvanceState(
                                    AdvanceAppState,
                                    key + "_operator",
                                    "and"
                                );
                            });
                        }
                        findAnswerWithLegacyApi(AdvanceAppState, value);
                    }
                }
            })
            .catch(onChatbotError)
            .finally(() => scrollToBottom());
    } else if (model == "ucgpt") {
        fetchRestful({
            url: "https://ucvn.vn/n8n/webhook/f2254068-8714-4f84-ad6f-6f3a4cbf0ba3",
            method: "POST",
            contentType: "application/json",
            data: {
                token: "UCVN",
                msg: value,
            },
        })
            .then((response) => {
                if (response) {
                    document.getElementById("typing-message").remove();
                    var output = splitOutput(response.output);
                    conversation.innerHTML += buildBotMessage(output);
                }
            })
            .catch(onChatbotError)
            .finally(() => scrollToBottom());
    }
}

function onChatbotError() {
    document.getElementById("typing-message").remove();
    conversation.innerHTML += buildBotErrorMessage(
        "Hệ thống đã xảy ra sự cố không mong muốn! Bạn vui lòng thử lại sau."
    );
}

var header = document.getElementById("header");
var footer = document.getElementById("footer");
var modelContainer = document.querySelector(".chatbot__model-container");

function scrollUntilClosestTouch(divAs, divB, container) {
    if (!divB || !container) return;

    const rectContainer = container.getBoundingClientRect();
    const rectB = divB.getBoundingClientRect();
    const topB = rectB.top - rectContainer.top;

    let closestDiv = null;
    let minDistance = Infinity;

    divAs.forEach((divA) => {
        const rectA = divA.getBoundingClientRect();
        const bottomA = rectA.bottom - rectContainer.top;

        // kiểm tra divA có trong viewport container không
        const isInViewport =
            rectA.bottom > rectContainer.top &&
            rectA.top < rectContainer.bottom;

        if (isInViewport) {
            const distance = Math.abs(topB - bottomA);
            if (distance < minDistance) {
                minDistance = distance;
                closestDiv = divA;
            }
        }
    });

    if (closestDiv) {
        // case: có divA trong viewport → chạm bottomA và topB
        const rectA = closestDiv.getBoundingClientRect();
        const bottomA = rectA.bottom - rectContainer.top;

        const targetScroll = container.scrollTop + (topB - bottomA);

        container.scrollTo({
            top: targetScroll,
            behavior: "smooth",
        });
    } else {
        // fallback: không có divA → cho top divB chạm top container
        const targetScroll =
            container.scrollTop + (rectB.top - rectContainer.top);
        container.scrollTo({
            top: targetScroll,
            behavior: "smooth",
        });
    }
}

function scrollToBottom() {
    var items = document.querySelectorAll(".chatbot__my-message-container");
    if (items.length == 0) return;
    var divB = items[items.length - 1];

    setTimeout(() => {
        scrollUntilClosestTouch(
            [header, footer, modelContainer],
            divB,
            conversation
        );
    }, 50);
}

createKeyUpEvent(
    "#conversation-input, #conversation-input-mobile",
    (target, e) => {
        if (e.key === "Enter" && !e.shiftKey) {
            e.preventDefault();
            sendRequest(target);
        }
    }
);

function onHasValueMobileInput(container) {
    container.querySelector(".send").classList.remove("disabled");
    if (isMobile()) {
        container.querySelector(".send").style.display = "block";
        container.querySelector(".voice").style.display = "none";
    }
}

function onNoValueMobileInput(container) {
    container.querySelector(".send").classList.add("disabled");
    if (isMobile()) {
        container.querySelector(".voice").style.display = "block";
        container.querySelector(".send").style.display = "none";
    }
}

createInputEvent("#conversation-input, #conversation-input-mobile", (target, e) => {
    var container = target.closest(".input-container");
    var value = target.value;
    if (value) {
        onHasValueMobileInput(container);
    } else {
        onNoValueMobileInput(container);
    }
});

createClickEvent(".chatbot__show-more-result", (target) => {
    var page = target.dataset.nextPage;
    var state =
        currentLegacySearchType == "quick" ? QuickAppState : AdvanceAppState;
    state.page = page;

    fetchRestful({
        url: config.locationVal + "/send?hasFacetFilter=false",
        method: "POST",
        responseType: "html",
        contentType: "application/json",
        data: state,
    }).then((response) => {
        if (response && response.trim() != "") {
            const parser = new DOMParser();
            const doc = parser.parseFromString(response, "text/html");
            const el = doc.querySelector(".end");

            if (el) {
                showInfoToast(
                    "Hiện tại không còn kết quả nào để hiển thị thêm!"
                );
                target.remove();
                return;
            }

            var wrapper = target.closest(".chatbot__message-wrapper");
            wrapper.querySelector("ul").innerHTML += response;
            var index = state.pageSize * (state.page - 1) + 1;
            var lis = document.querySelectorAll("#extra-document-" + index);
            if (lis.length == 0) return;
            var newLi = lis[lis.length - 1];
            scrollUntilClosestTouch([modelContainer], newLi, conversation);
            target.dataset.nextPage = parseInt(page) + 1;
        } else {
            showWarningToast(
                "Hệ thống đã xảy ra sự cố không mong muốn! Bạn vui lòng thử lại sau."
            );
        }
    });
});

document.querySelector(".chatbot__new-conversation-button").click();

createClickEvent(".chatbot__model-switch button", (target) => {
    if (selectedModelButton) selectedModelButton.classList.remove("active");
    selectedModelButton = target;
    selectedModelButton.classList.add("active");
    if (!isMobile()) {
        conversationInput.focus();
    }
});

selectedModelButton.click();

function openLeftColumn() {
    leftColumn.classList.add("open");
    leftColumnOverlay.classList.add("show");
}

function closeLeftColumn() {
    leftColumn.classList.remove("open");
    leftColumnOverlay.classList.remove("show");
}

createClickEvent(".chatbot__open-left-column", () => {
    openLeftColumn();
});

createClickEvent(".chatbot__close-left-column", () => {
    closeLeftColumn();
});

createClickEvent(document, (target) => {
    if (target.closest(".chatbot__open-left-column")) return;

    if (!target.closest(".chatbot__left-column")) {
        if (leftColumn.classList.contains("open")) {
            closeLeftColumn();
        }
    }
});

createClickEvent(document, (target) => {
    if (target.closest(".hearing-modal-overlay")) {
        closeHearingLoader();
        recognition.stop();
    }
});
