export function compareStrings(a, b, ignoreCase = false) {
    if (ignoreCase) {
        let ua = a.toUpperCase();
        let ub = b.toUpperCase();
        return compareStrings(ua, ub, false);
    }
    return a > b ? 1 : b > a ? -1 : 0;
}

export function compareNumbers(a, b) {
    if (!a) {
        return -1;
    }
    if (!b) {
        return 1;
    }

    let na = Number(a);
    let nb = Number(b);

    return na > nb ? 1 : nb > na ? -1 : 0;
}
