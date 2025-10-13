import { AdvanceAppState } from "../states/search-state.js";

export function handleChangeState(state, section, key, value) {
    var exists = state.request[section].some(function (pair) {
        return pair[0] === key;
    });

    if (exists) {
        state.request[section] = state.request[section].filter(function (pair) {
            return !(pair[0] === key);
        });
    }

    if (value) {
        state.request[section].push([key, value]);
    }
}

export function beforePushing(state, key, value) {
    var found = state.request.searchBy.find((item) => item[0] == key);
    if (found) {
        var index = state.request.searchBy.indexOf(found);
        state.request.searchBy[index][1] = value;
    } else {
        state.request.searchBy.push([key, value]);
    }
}

export function pushInSearchByAdvanceState(state, key, value) {
    const uniqueKeys = ["db_type", "is_unsign", "is_exact", "bt"];

    if (uniqueKeys.includes(key)) {
        const found = state.request.searchBy.find((item) => item[0] === key);
        if (found) {
            found[1] = value;
        } else {
            state.request.searchBy.push([key, value]);
        }
    } else {
        const isOperator = key.endsWith("_operator");
        const prefix = isOperator ? key.replace(/_operator$/, "") : key;

        const count = state.request.searchBy.filter(([k, _]) => {
            if (isOperator) return k.startsWith(`${prefix}_operator_`);
            return k.startsWith(`${prefix}_`) && !k.includes("_operator_");
        }).length;

        const finalKey = isOperator
            ? `${prefix}_operator_${count}`
            : `${prefix}_${count}`;

        state.request.searchBy.push([finalKey, value]);
    }
}

export function buildAdvanceState() {
    var key, value;
    AdvanceAppState.request.searchBy = [];
    document.querySelectorAll("#advancedSearchForm .row").forEach((row) => {
        if (row.classList.contains("search-advanced__conditional-row")) {
            var select = row.querySelector("select");
            key = select.getAttribute("name");
            value = select.value;
            pushInSearchByAdvanceState(AdvanceAppState, key, value);

            row.querySelectorAll("input").forEach((el) => {
                key = el.getAttribute("id");
                value = String(el.checked);
                pushInSearchByAdvanceState(AdvanceAppState, key, value);
            });
        } else {
            key = row.querySelector(".search-advanced__main-select").value;
            value = row.querySelector("input").value;

            if (value) {
                pushInSearchByAdvanceState(AdvanceAppState, key, value);

                key += "_operator";
                value = row.querySelector(
                    ".search-advanced__operator-select"
                ).value;
                pushInSearchByAdvanceState(
                    AdvanceAppState,
                    key,
                    value.toLowerCase()
                );
            }
        }
    });

    return AdvanceAppState;
}
