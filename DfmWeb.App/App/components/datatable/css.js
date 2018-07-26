export function cssToObj(css) {
    var obj = {};
    if (!css) {
        return obj;
    }
    var elements = css.split(';');
    elements.filter(function (el) {
        return !!el;
    }).map(function (el) {
        let s = el.split(':');
        let key = s.shift().trim();
        let value = s.join(':').trim();
        obj[key] = value;
    });
    return obj;
}

export function mergeStyles(css1, css2) {
    let obj1 = cssToObj(css1);
    let obj2 = cssToObj(css2);
    return Object.assign({}, obj1, obj2);
}