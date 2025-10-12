export function isAdvanceSearchClause(input) {
    const fields = [
        "qs",
        "ti",
        "au",
        "su",
        "pb",
        "no",
        "kw",
        "so",
        "bn",
        "bc",
        "yr",
        "lg",
        "bt",
        "cl",
        "ci",
        "uc",
    ];
    const fieldGroup = fields.join("|");
    const regex = new RegExp(
        `^(\\s*(AND|OR|NOT)?\\s*(${fieldGroup}):[^:]+)+$`,
        "i"
    );
    return regex.test(input);
}
