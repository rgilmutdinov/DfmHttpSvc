export default function openLink(link) {
    var a = window.document.createElement('a');
    a.target = '_blank';
    a.href = link;
    let e = window.document.createEvent("MouseEvents", true, true);
    e.initMouseEvent("click", true, true, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
    a.dispatchEvent(e);
}