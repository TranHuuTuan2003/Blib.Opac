import $ from "jquery";
import "jstree";
import "../../../css/libs/jstree.min.css";
import { fetchRestful } from "../../utils/api-util";
import config from "../../common/config";
import { AdvanceAppState, BasicAppState, getSearchType, QuickAppState, setSearchType } from "../../states/search-state";
import { advanceFetch, basicFetch, quickFetch } from "../../services/search-service";
import { SEARCH_FORM_TYPES, SEARCH_STATE_TYPES } from "../../common/constants";

const arrow_icon = `
    <svg width="14" height="16" viewBox="0 0 14 16" fill="none" xmlns="http://www.w3.org/2000/svg">
        <path d="M13.7188 12.3303C13.6808 12.4195 13.6266 12.5009 13.5588 12.5703L10.9088 15.2203C10.7218 15.4209 10.4402 15.5035 10.1745 15.4356C9.90877 15.3678 9.70127 15.1603 9.63341 14.8945C9.56554 14.6288 9.64814 14.3472 9.84879 14.1603L11.2188 12.7903H4.99879C3.7285 12.7929 2.50962 12.2888 1.61233 11.3896C0.715036 10.4905 0.213463 9.27054 0.218792 8.00026V1.28026C0.218792 0.866043 0.554578 0.530256 0.968792 0.530256C1.38301 0.530256 1.71879 0.866043 1.71879 1.28026V8.00026C1.71345 8.87271 2.05698 9.71112 2.67297 10.329C3.28896 10.9469 4.12633 11.2929 4.99879 11.2903H11.2288L9.85879 9.93026C9.56634 9.63744 9.56634 9.16307 9.85879 8.87026C9.99868 8.72862 10.1897 8.64932 10.3888 8.65026C10.5881 8.64817 10.7796 8.72765 10.9188 8.87026L13.5688 11.5103C13.6369 11.5832 13.6911 11.6679 13.7288 11.7603C13.7956 11.945 13.792 12.1479 13.7188 12.3303Z" fill="#4B5563"></path>
    </svg>
`;

export function getCollectionTree() {
    setSearchType(SEARCH_STATE_TYPES.QUICK);
    fetchRestful({
        url: config.baseUrlApi + "Collection/GetCollectionByDbType?dbType=" + dbType,
        responseType: "json",
        method: "GET",
    }).then((response) => {
        if (response.success) {
            createTree(response.data);
        }
    }).catch().finally();
}

function formatLocaleNumber(number) {
    number = number ?? 0;
    return number.toLocaleString("vi-VN", { maximumFractionDigits: 0 });
}

function createNode(item, level = 1, isFirstChild = false) {
    const hasChildren =
        Array.isArray(item.children) && item.children.length > 0;

    let prefix = "";
    if (level > 1 && isFirstChild) {
        // Node không phải cấp 1 và là node đầu tiên => hiện icon
        prefix = `<span class="arrow-turn-bottom-right-icon">${arrow_icon}</span>`;
    }

    return {
        id: item.id,
        text: `<span style="padding-left: ${prefix ? (level - 2) * 24 : (level - 1) * 24
            }px" class="jstree-ileaf">${prefix} ${item.text}</span> 
               <span class="float-tree-number">${formatLocaleNumber(
                item.total_bib
            )}</span>`,
        children: hasChildren
            ? item.children.map((child, index) =>
                createNode(child, level + 1, index === 0)
            )
            : [],
    };
}

function findNodeInTree(tree, nodeId) {
    for (let node of tree) {
        if (node.id == nodeId) return true; // Tìm thấy trong node gốc

        if (node.children && node.children.length > 0) {
            if (findNodeInTree(node.children, nodeId)) return true; // Tìm thấy trong children
        }
    }
    return false;
}

var currentCollectionId = null;

export function getCurrentCollectionId() {
    return currentCollectionId;
}

function createTree(data) {
    var id = "#search-collection-tree";
    $(id).jstree("destroy").empty();
    $(id)
        .jstree({
            core: {
                themes: {
                    responsive: true,
                    dots: false,
                },
                data: data.map((item) => createNode(item)),
                initially_selected: data?.length ? [data[0].id] : [],
            },
            types: {
                default: {
                    icon: false,
                },
                file: {
                    icon: false,
                },
            },
            plugins: ["types"],
        })
        .on("ready.jstree", function () {
            $("a.jstree-anchor").each(function () {
                const level = $(this).parents("li").length - 1;
                $(this).css("padding-left", `${level * 24}px`);
            });

            var firstNodeId = "";

            if (Array.isArray(data) && data.length > 0) {
                // if (redirectedCollectionId) {
                //     var check = findNodeInTree(data, redirectedCollectionId); // Tìm trong toàn bộ cây
                //     firstNodeId = check ? redirectedCollectionId : data[0].id;
                // } else {
                //     firstNodeId = data[0].id;
                // }

                firstNodeId = data[0].id;

                $(this).jstree("select_node", firstNodeId);
                $(this).jstree("open_node", firstNodeId);
            }
        })
        .on("select_node.jstree", function (e, data) {
            var id = data.node.id;
            currentCollectionId = id;
            var selectedSearchType = getSearchType();
            if (selectedSearchType == "quick") {
                QuickAppState.page = 1;
                quickFetch(SEARCH_FORM_TYPES.COLLECTION);
            } else if (selectedSearchType == "basic") {
                BasicAppState.page = 1;
                basicFetch(SEARCH_FORM_TYPES.COLLECTION);
            } else if (selectedSearchType == "advance") {
                AdvanceAppState.page = 1;
                advanceFetch(SEARCH_FORM_TYPES.COLLECTION);
            }
        })
        .on("activate_node.jstree", function (e, data) {
            var instance = $(id).jstree(true);
            var node = data.node;
            var opened = node.state.opened;
            var selected = node.state.selected;
            if (node.children && node.children.length > 0) {
                if (opened && selected) {
                    instance.close_node(node);
                } else {
                    instance.open_node(node);
                }
            }
        })
        .off("contextmenu.jstree", ".jstree-anchor");
}
