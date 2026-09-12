mergeInto(LibraryManager.library, {
    CopyToClipboard: function (textPtr) {
        var text = UTF8ToString(textPtr);

        var textarea = document.createElement("textarea");
        textarea.value = text;
        document.body.appendChild(textarea);
        textarea.focus();
        textarea.select();

        document.execCommand("copy");

        document.body.removeChild(textarea);
    }
});