(function ($) {
    "use strict";
    $(document).ready(function () {
        tinymce.init({
            selector: 'textarea#Email',
            branding: false,
            menubar: 'edit insert view format table tools help'
            //toolbar: 'undo redo | styleselect | bold italic | link image'
        });

    });
})(this.jQuery);