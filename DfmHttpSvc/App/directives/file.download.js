export default {
    bind: function (el, binding, vnode) {
        let iframe = window.document.createElement('iframe');
        iframe.style.cssText = 'position:fixed; display:none; top:-1px; left:-1px;';

        el.appendChild(iframe);

        const onclick = event => {
            event.preventDefault();

            let iframeBody = iframe.contentWindow.document.body;

            var form = document.createElement('form');
            form.setAttribute('method', 'post');
            form.setAttribute('action', binding.value.url);

            var tokenInput = document.createElement("input");
            tokenInput.setAttribute('type', "hidden");
            tokenInput.setAttribute('name', "accessToken");
            tokenInput.setAttribute('value', binding.value.accessToken);

            form.appendChild(tokenInput);

            iframeBody.append(form);
            form.submit();
        }

        el.addEventListener('click', onclick);
    }
}