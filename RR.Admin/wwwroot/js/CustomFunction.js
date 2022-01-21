function loadPage(page) {
    if (page === 'register')
        window.location.href = "/Register";
    else
        window.location.href = "/Login";
}

function onBegin() {
    $("#preloader").addClass('loaderActive');
}

function onComplete() {
    $("#preloader").removeClass('loaderActive');
}

function onFailed() {
    $("#preloader").removeClass('loaderActive');
}

function onRegisterSuccess(response) {
    $("#response").html(response);
    if (response.indexOf("alert-success") > -1) {
        var url = $("#dasboardURL").attr('href');
        if (url === '') { url = "/MyAccount"; }
        setTimeout(function () { window.location.href = url; }, 3000);
    }
    $("#preloader").removeClass('loaderActive');
}

function onForgotPasswordSuccess(response) {
    $("#fpRespone").html(response);
    if (response.indexOf("alert-success") > -1) {
        $("#fpBody").hide();
        $("#fpForm").trigger("reset");
    }
    $("#preloader").removeClass('loaderActive');
    setTimeout(function () { $("#fpRespone").html(''); $("#fpBody").show(); $('#pwdModal').modal('toggle'); }, 5000);
}

function onResetSuccess(response) {
    $("#response").html(response);
    if (response.indexOf("alert-success") > -1) {
        $("#rpForm").trigger("reset");
    }
    $("#preloader").removeClass('loaderActive');
    setTimeout(function () {
        $("#response").html('');
        if (response.indexOf("alert-success") > -1) {
            window.location.href = "/login";
        }

    }, 5000);
}

function onWinnerSuccess(response) {
    $("#preloader").removeClass('loaderActive');
    swal("Winners rank has been added successfully.", {
        icon: "success",
    });
    setTimeout(function () { window.location.reload(); }, 2000);
}

function isNumber(evt) {
    evt = (evt) ? evt : window.event;
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function delThisRow(x) {
    $(x).parents('tr').first().remove();
}

function fncvalidate() {

    var count = 0;
    var regurnFlag = true;
    if (regurnFlag) {
        $(".validate").each(function () {
            count = count + 1;
            if ($(this).val() === "") {

                if (this.id === "dropdownaward") {
                    return true;
                }
                $(this).focus().css({ "border-color": "red" });
                regurnFlag = false;
                return false;
            }
            else {
                $(this).focus().css({ "border-color": "" });
            }
        });
    }

    if (count === 0) {
        swal("Please add atleast one winner for this contest.", {
            icon: "warning",
        });
        return false;
    }
    return regurnFlag;
}

function GetAwardDropdown(elm_, value_) {

    var text_ = $('option[value="' + value_ + '"]', elm_).text();

    if (text_ === "Cash" || text_ === "Token") {

        if ($(elm_).closest("td").next("td").find("#Awardtextbox").length > 0) {
            $(elm_).closest("td").next("td").find("#Awardtextbox").css('display', 'block');
        }
        if ($(elm_).closest("td").next("td").find("#AwardDropdown").length > 0) {
            $(elm_).closest("td").next("td").find("#AwardDropdown").css('display', 'none');
            $(elm_).closest("td").next("td").find('#dropdownaward').removeClass('validate');
        }
    }
    else {

        if ($(elm_).closest("td").next("td").find("#Awardtextbox").length > 0) {
            $(elm_).closest("td").next("td").find("#Awardtextbox").css('display', 'none');
            $(elm_).closest("td").next("td").find('#Value').removeClass('validate');
        }
        if ($(elm_).closest("td").next("td").find("#AwardDropdown").length > 0) {
            $(elm_).closest("td").next("td").find("#AwardDropdown").css('display', 'block');
        }


        $.ajax({
            url: "/ContestWinnerManagement/Getawards/",
            method: "Post",
            data: { id: value_ },
            datatype: "Json"
        })
            .done(function (data) {
                $(elm_).closest("td").next("td").find("#dropdownaward").empty();
                $.each(data, function (data, value) {

                    $(elm_).closest("td").next("td").find("#dropdownaward").append($("<option></option>").val(value.id).html(value.message));

                });

            });
    }

}

function DeleteWinner(id) {
    var url = "/DeleteContestWinner/" + id;
    swal({
        title: "Are you sure?",
        text: "You Want Delete This Contest winner?",
        icon: "warning",
        buttons: true,
        dangerMode: true,
    })
        .then((willDelete) => {
            if (willDelete) {
                $.ajax({
                    type: "POST",
                    url: url,
                    success: function (result) {
                        if (result !== null) {
                            swal("Success! Contest Winner deleted successfully!", {
                                icon: "success",
                            });
                            setTimeout(function () { window.location.reload(); }, 2000);
                        }
                    },
                    error: function () {
                        alert("error");
                    }
                });
            }
        });
}

function ActiveLink() {
    
    $(".nav-item").each(function () { $(this).removeClass('active'); });
    var pathname = window.location.pathname.split("/")[1];
    $('.' + pathname+'').addClass("active");
    switch (pathname.toLowerCase()) {
        case "":
            $(".dashboard").addClass("active");
            break;
        case "news":
            $(".news").addClass("active");
            break;
        case "cms":
            $(".cms").addClass("active");
            break;
        case "players":
        case "player":
            $(".user").addClass("active");
            break;
        case "banner":
            $(".banner").addClass("active");
            break;
        case "bulls":
            $(".bull").addClass("active");
            break;
        case "riders":
        case "rider":
            $(".rider").addClass("active");
            break;
        case "events":
        case "event":
            $(".event").addClass("active");
            break;
        case "awards":
            $(".awards").addClass("active");
            break;
        case "contests":
        case "contestwinners":
            $(".contest").addClass("active");
            break;
        case "usercontests":
            $(".usercontests").addClass("active");
            break;
        case "award-type":
            $(".awardtype").addClass("active");
            break;
        case "partners":
        case "partner":
            $(".sponsor").addClass("active");
            break;
        case "transactions":
            $(".transactions").addClass("active");
            break;

    }
}
var count = 0;
$(function () {

    $("#addButton, #addButton2").on('click', function () {
        var $card_wrapper = $(this).closest('.card');
        count = $card_wrapper.find('#contestWinnerDT tbody tr.trnewrow').length;

        var winnerCount = parseInt($('#WinnerCount').val());
        if (count < winnerCount) {
            var newTr = $card_wrapper.find(".trclone").clone();
            $(newTr).show();
            $(newTr).removeClass('trclone');
            $(newTr).addClass('trnewrow');
            $(newTr).find('td select').addClass('validate');
            $(newTr).find('td Input').addClass('validate');

            $card_wrapper.find("#contestWinnerDT").append(newTr);
        }
        else {
            swal("You have already inserted sufficient number of rows for inserting the winners. ", {
                icon: "warning",
            });
        }
    });
});    

function getBaseUrl(parma) {
    debugger;

    var re = new RegExp(/^.*\//);
    var url_ = re.exec(window.location.href).replace(/\/$/, "");
    return url_+parma;
}


/* For Export Buttons available inside jquery-datatable "server side processing" - Start
- due to "server side processing" jquery datatble doesn't support all data to be exported
- below function makes the datatable to export all records when "server side processing" is on */

function fullexport_action(e, dt, button, config) {
    var self = this;
    var oldStart = dt.settings()[0]._iDisplayStart;
    dt.one('preXhr', function (e, s, data) {
        // Just this once, load all data from the server...
        data.start = 0;
        data.length = 2147483647;
        dt.one('preDraw', function (e, settings) {
            // Call the original action function
            if (button[0].className.indexOf('buttons-copy') >= 0) {
                $.fn.dataTable.ext.buttons.copyHtml5.action.call(self, e, dt, button, config);
            } else if (button[0].className.indexOf('buttons-excel') >= 0) {
                try {
                    $.fn.dataTable.ext.buttons.excelHtml5.available(dt, config) ?
                        $.fn.dataTable.ext.buttons.excelHtml5.action.call(self, e, dt, button, config) :
                        $.fn.dataTable.ext.buttons.excelFlash.action.call(self, e, dt, button, config);
                } catch (e) {
                    console.log(e);
                }
            } else if (button[0].className.indexOf('buttons-csv') >= 0) {
                $.fn.dataTable.ext.buttons.csvHtml5.available(dt, config) ?
                    $.fn.dataTable.ext.buttons.csvHtml5.action.call(self, e, dt, button, config) :
                    $.fn.dataTable.ext.buttons.csvFlash.action.call(self, e, dt, button, config);
            } else if (button[0].className.indexOf('buttons-pdf') >= 0) {
                $.fn.dataTable.ext.buttons.pdfHtml5.available(dt, config) ?
                    $.fn.dataTable.ext.buttons.pdfHtml5.action.call(self, e, dt, button, config) :
                    $.fn.dataTable.ext.buttons.pdfFlash.action.call(self, e, dt, button, config);
            } else if (button[0].className.indexOf('buttons-print') >= 0) {
                $.fn.dataTable.ext.buttons.print.action(e, dt, button, config);
            }
            dt.one('preXhr', function (e, s, data) {
                // DataTables thinks the first item displayed is index 0, but we're not drawing that.
                // Set the property to what it was before exporting.
                settings._iDisplayStart = oldStart;
                data.start = oldStart;
            });
            // Reload the grid with the original page. Otherwise, API functions like table.cell(this) don't work properly.
            setTimeout(dt.ajax.reload, 0);
            // Prevent rendering of the full data to the DOM
            return false;
        });
    });
    // Requery the server with the new one-time export settings
    dt.ajax.reload();
};
//For Export Buttons available inside jquery-datatable "server side processing" - End
