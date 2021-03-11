(function ($) {
    "use strict";
    $(document).ready(function () {
        tinymce.init({
            selector: 'textarea#EmailBody',
            branding: false,
            height: 400,
            menubar: false,
            //menubar: 'edit insert view format table tools help',
            toolbar: 'undo redo | bold italic underline | outdent indent | coachButton | fontselect fontsizeselect forecolor backcolor | alignleft aligncenter alignright alignjustify formatselect',
            plugins: 'noneditable',
            //noneditable_editable_class: "non-editable",
            setup: function (editor) {

                var insertCoachName = function (tag, text) {
                    editor.insertContent("<span data-school-info='" + tag + "' class='coach-button mceNonEditable'>[" + text + "]</span>");
                };

                editor.addShortcut('meta+alt+N', 'Insert Coach Name', function () {
                    insertCoachName('coach-name', 'COACH NAME');
                });

                editor.addShortcut('meta+alt+S', 'Insert School', function () {
                    insertCoachName('school-name', 'SCHOOL');
                });

                editor.ui.registry.addMenuButton('coachButton', {
                    text: 'Coach info',
                    fetch: function (callback) {
                        var items = [
                            {
                                type: 'menuitem',
                                text: 'Coach Name',
                                shortcut: 'meta+alt+N',
                                onAction: function () {
                                    insertCoachName('coach-name', 'COACH NAME');
                                }
                            },
                            {
                                type: 'menuitem',
                                text: 'School',
                                shortcut: 'meta+alt+S',
                                onAction: function () {
                                    insertCoachName('school-name', 'SCHOOL');
                                }
                            }
                        ];
                        callback(items);
                    }
                });
                
            },

            content_style: '.coach-button { background-color: silver; font-weight: 500; }'

            //https://jsfiddle.net/stvakis/tjh7k20v/8/
            //https://www.tiny.cloud/docs/ui-components/typesoftoolbarbuttons/
            //https://www.tiny.cloud/docs/ui-components/toolbarbuttons/
            //https://www.tiny.cloud/docs/demo/custom-toolbar-button/
            //https://www.tiny.cloud/docs/advanced/keyboard-shortcuts/

        });

    });
})(this.jQuery);