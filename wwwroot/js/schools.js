﻿$(document).ready(function () {
    var _targetId = "";
    var _targetSaving = [];

    // TODO:  - Add form validation 
    //        - Add API auth key to api calls


    $(".edit").click(function () {
        //alert($(this).parent().parent().siblings(".school").children(".full-name").html());

        var school = $(this).parent().parent().siblings(".school");
        school.children(".full-name").hide();
        school.children(".short-name").hide();
        school.children(".edits").show();

        var coach = $(this).parent().parent().siblings(".coach");
        coach.children(".coach-name").hide();
        coach.children(".email").hide();
        coach.children(".phone").hide();
        coach.children(".edits").show();

        $(this).addClass("d-none");
        $(this).siblings(".save").removeClass("d-none");
    });


    $(".save").click(function () {
        $(this).siblings(".saving").removeClass("d-none");

        var userEmail = $("#userEmail").val();
        var saveType = $(this).parent().parent().siblings(".id").attr("id") === "newId" ? "new" : "update";

        // First save the data by calling the API
        var id = $(this).parent().parent().siblings(".id").val();

        var $school = $(this).parent().parent().siblings(".school");
        var $coach = $(this).parent().parent().siblings(".coach");
        var $this = $(this);

        var schoolJson = {
            "Id": id,
            "SchoolName": $school.children(".edits").children(".school-name").val(),
            "SchoolNameShort": $school.children(".edits").children(".school-name-short").val(),
            "HeadCoach": {
                "Name": $coach.children(".edits").children(".coach-name").val(),
                "Email": $coach.children(".edits").children(".coach-email").val(),
                "PhoneNumber": $coach.children(".edits").children(".coach-phone").val()
            },
            "AssistantCoach": null,
            "IsEnabled": true
        };


        $.ajax({
            type: "POST",
            url: "../api/SchoolInfo/SaveTheSchool" + "?userEmail=" + userEmail,
            data: JSON.stringify(schoolJson),
            contentType: "application/json",
            error: function (msg) {
                alert('error' + msg);
            },
            success: function (data) {
                if (saveType === "new") {
                    window.location.href = "School";
                } else {
                    saveUpdateSuccessful($this, $school, $coach);
                }
            },
            complete: function () {
                $this.siblings(".saving").addClass("d-none");
            }
        });

    });

    $(".enabled-checkbox").on('change', function () {
        var $saving = $(this).parent().parent().siblings(".actions").children(".icons").children(".saving");
        $saving.removeClass("d-none");

        var userEmail = $("#userEmail").val();
        var id = $(this).parent().parent().siblings(".id").val();
        var $this = $(this);

        var isEnabled = false;
        if ($(this).is(':checked') === true) {
            isEnabled = true;
        } else {
            isEnabled = false;
        }

        //var enabledJson = {
        //    "userEmail": "@userEmail",
        //    "schoolId": id,
        //    "IsEnabled": isEnabled
        //};

        // Save the data
        $.ajax({
            type: "POST",
            url: "../api/SchoolInfo/SaveIsEnabled" + "?userEmail=" + userEmail + "&schoolId=" + id + "&isEnabled=" + isEnabled,
            //data: JSON.stringify(enabledJson),
            //contentType: "application/json",
            //contentType: "application/x-www-form-urlencoded",
            //data: "userEmail=@(userEmail)&schoolId=" + id + "&isEnabled=" + isEnabled,
            error: function (msg) {
                alert('error' + msg);
            },
            success: function (data) {

            },
            complete: function () {
                $saving.addClass("d-none");
            }
        });


    })

    $("#confirmDeleteModal").on("show.bs.modal", function (e) {
        _targetId = e.relatedTarget.parentElement.parentElement.nextElementSibling.value;
        _targetSaving = e.relatedTarget.nextElementSibling;
    });

    $(".yes-delete").on("click", function () {
        if (_targetSaving !== []) $(_targetSaving).removeClass("d-none");

        if (_targetId !== "") {
            //alert('delete id: ' + _targetId);

            $("#confirmDeleteModal").modal('hide');

            var userEmail = $("#userEmail").val();

            // Save the data
            $.ajax({
                type: "POST",
                url: "../api/SchoolInfo/DeleteTheSchool" + "?userEmail=" + userEmail + "&schoolId=" + _targetId,
                error: function (msg) {
                    alert('error' + msg);
                },
                success: function (data) {
                    window.location.href = "School";
                },
                complete: function () {
                    _targetId = "";
                    //$saving.addClass("d-none");
                    if (_targetSaving !== []) $(_targetSaving).addClass("d-none");
                    _targetSaving = [];
                }
            });
        }
    });

    $(".btn-add-school").on("click", function () {
        $(".new-school-item").children(".school, .coach").children(".edits").show();
        $(".new-school-item").removeClass("d-none");
    });

    $(".new-school-item").children(".actions").children(".icons").children(".close").on("click", function () {
        $(".new-school-item").addClass("d-none");
    });

    var saveNewSuccessful = function ($this, $school, $coach) {
        // Create a new element and insert it

    }

    var saveUpdateSuccessful = function ($this, $school, $coach) {
        // Update values rather than reload the page (better user experience)
        $school.children(".full-name").text($school.children(".edits").children(".school-name").val()).show();
        $school.children(".short-name").text($school.children(".edits").children(".school-name-short").val()).show();
        $school.children(".edits").hide();

        $coach.children(".coach-name").text($coach.children(".edits").children(".coach-name").val()).show();
        $coach.children(".email").text($coach.children(".edits").children(".coach-email").val()).show();
        $coach.children(".phone").text($coach.children(".edits").children(".coach-phone").val()).show();
        $coach.children(".edits").hide();

        $this.addClass("d-none");
        $this.siblings(".edit").removeClass("d-none");
    }

});