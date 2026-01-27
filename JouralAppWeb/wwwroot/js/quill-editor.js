window.quillEditor = (function () {
    const editors = {};

    function init(editorId) {
        const el = document.getElementById(editorId);
        if (!el) return;

        // Avoid double init
        if (editors[editorId]) return;

        const quill = new Quill(el, {
            theme: 'snow',
            placeholder: 'Write your thoughts here...',
            modules: {
                toolbar: [
                    [{ header: [1, 2, 3, false] }],
                    ['bold', 'italic', 'underline', 'strike'],
                    [{ list: 'ordered' }, { list: 'bullet' }],
                    [{ align: [] }],
                    ['link', 'image'],
                    ['clean']
                ]
            }
        });

        editors[editorId] = quill;
    }

    function setHtml(editorId, html) {
        const quill = editors[editorId];
        if (!quill) return;
        quill.root.innerHTML = html || "";
    }

    function getHtml(editorId) {
        const quill = editors[editorId];
        if (!quill) return "";
        return quill.root.innerHTML;
    }

    return { init, setHtml, getHtml };
})();
