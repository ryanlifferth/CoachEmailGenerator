$(document).ready(function () {

    var _schoolNameEl = document.querySelector(".coach-button[data-school-info='school-name']");
    var _schoolNameDefaultVal = (_schoolNameEl) ? _schoolNameEl.innerText : undefined;
    var _schoolNameShortEl = document.querySelector(".coach-button[data-school-info='school-name-short']");
    var _schoolNameShortDefaultVal = (_schoolNameShortEl) ? _schoolNameShortEl.innerText : undefined;
    var _coachNameEl = document.querySelector(".coach-button[data-school-info='coach-name']");
    var _coachNameDefaultVal = (_coachNameEl) ? _coachNameEl.innerText : undefined;
    var _coachEmailEl = document.querySelector(".coach-button[data-school-info='coach-email']");
    var _coachEmailDefaultVal = (_coachEmailEl) ? _coachEmailEl.innerText : undefined;
    var _coachPhoneEl = document.querySelector(".coach-button[data-school-info='coach-phone']");
    var _coachPhoneDefaultVal = (_coachPhoneEl) ? _coachPhoneEl.innerText : undefined;
    var _coachEmailHeaderDefaultVal = $(".preview-pane .email-header .header-row .to").innerText;

    $(".school-item-preview").hover(function () { // mouse over
        var $this = this;
        var dataItems = [
            'school-name',
            'school-name-short',
            'coach-name',
            'coach-email',
            'coach-phone'];

        dataItems.forEach(function (dataItem) {
            document.querySelectorAll(".coach-button[data-school-info='" + dataItem + "']").forEach(function (item) {
                replaceDefaultValWithValue(item, $($this).data(dataItem));
            });
        });

        var coachRegEx = $(this).data("coach-name").match("[^ ]* (.*)");
        if (coachRegEx && coachRegEx.length >= 1) _coachNameEl.innerText = "Coach " + coachRegEx[1];

        $(".preview-pane .email-header .header-row .to").text($(this).data("coach-email"));


    }, function () {  //mouse out

        var defaultVals = [
            { 'name': 'school-name', 'defaultVal': _schoolNameDefaultVal },
            { 'name': 'school-name-short', 'defaultVal': _schoolNameShortDefaultVal },
            { 'name': 'coach-name', 'defaultVal': _coachNameDefaultVal },
            { 'name': 'coach-email', 'defaultVal': _coachEmailDefaultVal },
            { 'name': 'coach-phone', 'defaultVal': _coachPhoneDefaultVal }];

        for (const [key, value] of Object.entries(defaultVals)) {
            document.querySelectorAll(".coach-button[data-school-info='" + value.name + "']").forEach(function (item) {
                replaceValueWithDefaultVal(item, value.defaultVal);
            });
        }

        $(".preview-pane .email-header .header-row .to").text(_coachEmailHeaderDefaultVal);

    });

    function replaceDefaultValWithValue(el, value) {
        el.innerText = value;
        el.classList.add("hover-val");
    }

    function replaceValueWithDefaultVal(el, value) {
        el.innerText = value;
        el.classList.remove("hover-val");
    }


    //var previewPaneHeight = $('.preview-pane').height();
    var previewPaneHeight = $('.preview-pane').outerHeight().toFixed(1);
    var schoolListHeight = $('.preview-school-list').height();

    if (previewPaneHeight > schoolListHeight) {
        //$('.preview-school-list').height(previewPaneHeight + 'px');
        $('.preview-school-list').css({ 'maxHeight': previewPaneHeight + 'px' });
    }

    // I hate this, but I'm being a bit lazy with my layout, so this is a hack
    $(".school-count").width($(".preview-school-list").width());  // could also do outerWidth


    $("#previewForm").submit(function (e) {
        e.preventDefault();

        // Show the "Creating emails..." spinner
        $(".emails-loading").removeClass("d-none");

        // Hide the "Send Email" button (and show the "Sending" button)
        $(".send").addClass("d-none");
        $(".sending").removeClass("d-none");

        this.submit();
        
    });

});