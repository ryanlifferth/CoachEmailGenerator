$(document).ready(function () {

    $("#schoolName").on("input", function () {
        if ($(this).val().length > 1) {
            searchSchool($(this).val());
            console.log('Search on ' + $("#schoolName").val());
        } else if ($(this).val().length === 0) {
            $('.search-results').html('');
            $('.results-count').hide();
            console.log('Cleared');
        }
    });

    function searchSchool(schoolName) {
        $('.search-results').html('');


        var schoolSearch = {
            "PartitionKey": "M_SOCCER_NCAA_D1",
            "SchoolName": schoolName
        };

        $.ajax({
            type: "POST",
            url: "../api/SchoolInfo/SearchSchools",
            data: JSON.stringify(schoolSearch),
            contentType: "application/json",
            error: function (msg) {
                alert('error' + msg);
            },
            success: function (data) {
                $('.results-count').show();
                $('.results-count .count').html(data.length);
                $('.results-count .found').html(data.length === 1 ? ' school found' : ' schools found');

                if (data && data.length > 0) {
                    $('.search-results').html(data.map(ResultTemplate).join(''));
                    //data.forEach(item => {
                    //    $('.search-results').append('<div class="row">' + item.schoolName + '</div>');
                    //
                    //    console.log(item.schoolNameShort);
                    //});
                }

            },
            complete: function () {
                // Do something
            }
        });
    }

    const ResultTemplate = ({ schoolName, schoolNameShort, division, coaches}) => `
        <div class="result-row">
            <div class="school">
                <span class="full-name">${schoolName}</span>
                <span class="short-name">${schoolNameShort}</span>
                <span class="right-content">
                    <span class="division">${division.replace("_", " ")}</span>
                </span>
            </div>
            ${coaches.map(coachTemplate).join('')}
        </div>
    `;

    const coachTemplate = ({ name, email, phoneNumber }) => `
        <div class="coach">
            <span class="name">${name}</span>
            <span class="email">${email}</span>
            <span class="phone">${phoneNumber && phoneNumber.length > 0 ? $phoneNumber : ''}</span>
        </div>
    `;


});