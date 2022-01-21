function loadPage(page) {
    if (page === 'become-a-player')
        window.location.href = "/Become-a-player";
    else
        window.location.href = "/Login";
}

function onBegin() {
    $("#preloader").removeClass('loaded');
    $("#preloader").addClass('loaderActive');
}

function onComplete() {
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');
}

function onFailed() {
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');
}

function onRegisterSuccess(response) {
    $("#response").html(response);
    if (response.indexOf("alert-success") > -1) {
        var url = $("#dasboardURL").attr('href');
        if (url === '') { url = "/MyAccount"; }
        setTimeout(function () { window.location.href = url; }, 3000);
    }
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');
}

function onForgotPasswordSuccess(response) {
    $("#fpRespone").html(response);
    if (response.indexOf("alert-success") > -1) {
        $("#fpBody").hide();
        $("#fpForm").trigger("reset");
        setTimeout(function () { $("#fpRespone").html(''); $("#fpBody").show(); $('#pwdModal').modal('toggle'); }, 5000);
    }
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');

}

function onChangePasswordSuccess(response) {
    $("#fpRespone").html(response);
    if (response.indexOf("alert-success") > -1) {
        $("#fpBody").hide();
        window.location.href = "/login";
    }
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');
    setTimeout(function () { $("#fpRespone").html(''); $("#fpBody").show(); $('#pwdModal').modal('toggle'); }, 5000);
}

function onResetSuccess(response) {
    $("#response").html(response);
    if (response.indexOf("alert-success") > -1) {
        $("#rpForm").trigger("reset");
    }
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');
    setTimeout(function () {
        $("#response").html('');
        if (response.indexOf("alert-success") > -1) {
            window.location.href = "/login";
        }
    }, 5000);
}

function onSentSuccess(response) {
    $("#response").html(response);
    if (response.indexOf("alert-success") > -1) {
        $("#cuForm").trigger("reset");
    }
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');

    setTimeout(function () {
        $("#response").html('');
    }, 10000);
}

function validatePlayer() {
    var isValidatePlayer = true;
    $('#tabs-product-2 input').each(function () {
        var this_ = $(this);
        var value_ = $(this).val();
        if ($(this).attr('id') !== $("#PlayerType").attr('id')) {
            if (value_.length <= 0) {
                this_.next('span').removeClass('field-validation-valid').addClass('field-validation-error');
                isValidatePlayer = false;
            }
            else {
                if ($(this).attr('id') === $("#Password").attr('id')) {
                    var passw = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$@!%&*?])[A-Za-z\d#$@!%&*?].{7,}$/;
                    if (value_.match(passw)) {
                        this_.next('span').removeClass('field-validation-error').addClass('field-validation-valid');
                    }
                    else {
                        this_.next('span').removeClass('field-validation-valid').addClass('field-validation-error').text("Password Should contain atleast 8 characters with 1 special character,1 lowercase and 1 uppercase character!!");
                    }
                }
                else if ($(this).attr('id') === $("#ConfirmPassword").attr('id')) {
                    if ($(this).val() === $("#Password").val()) {
                        this_.next('span').removeClass('field-validation-error').addClass('field-validation-valid');
                    }
                    else {
                        this_.next('span').removeClass('field-validation-valid').addClass('field-validation-error').text("The password and confirmation password do not match.!!");
                    }
                }
                else if ($(this).attr('id') === $("#PhoneNumber").attr('id')) {
                    if (value_.length > 8 && value_.length <= 15) {
                        this_.next('span').removeClass('field-validation-error').addClass('field-validation-valid');
                    }
                    else {
                        this_.next('span').removeClass('field-validation-valid').addClass('field-validation-error').text("Invalid Phone Number!!");
                        isValidatePlayer = false;
                    }
                }
                else if ($(this).attr('id') === $("#Email").attr('id')) {
                    if (IsEmail(value_)) {
                        this_.next('span').removeClass('field-validation-error').addClass('field-validation-valid');
                    }
                    else {
                        this_.next('span').removeClass('field-validation-valid').addClass('field-validation-error').text("The Email Address is incorrect!!");
                        isValidatePlayer = false;
                    }
                }
                else {
                    this_.next('span').removeClass('field-validation-error').addClass('field-validation-valid');
                    isValidatePlayer = true;
                }
            }
        }
    });
    //if (isValidatePlayer) {
    //    loadMakePaymentTab();
    //}
}

function IsEmail(email) {
    var regex = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (!regex.test(email)) {
        return false;
    } else {
        return true;
    }
}

function loadPlayerTypeInfoTab() {
    $(".registerStep").each(function () {
        $(this).removeClass('active');
    });
    $(".tab-pane").each(function () {
        $(this).removeClass('active show');
    });
    $("#tabs-product-2").addClass('active show');
    $("#registerStep1").addClass('active');
    return false;
}

function loadMakePaymentTab() {
    //var userDetail = {
    //    "UserName": $("#UserName").val(),
    //    "Email": $("#Email").val(),
    //    "DOB": $("#DateOfBirth").val(),
    //    "PhoneNumber": $("#PhoneNumber").val(),
    //    "Address1": $("#Address1").val(),
    //    "Address2": $("#Address2").val()
    //};
    //sessionStorage.setItem("UserDetail", JSON.stringify(userDetail));
    $(".registerStep").each(function () {
        $(this).removeClass('active');
    });
    $(".tab-pane").each(function () {
        $(this).removeClass('active show');
    });
    $("#_tab2").removeClass('active');
    $("#_tab3").addClass('active');
    $("#tabs-product-3").addClass('active show');
    if ($("#PlayerType").val() === "NOVICE PLAYER") {
        $("#_subscribePlan").val("25");
        $("._amount").text("$25");
    }
    else if ($("#PlayerType").val() === "PRO PLAYER") {
        $("#_subscribePlan").val("50");
        $("._amount").text("$50");
    }
    else {
        $("#_subscribePlan").remove();
        $("._amount").html("<p><input type='radio' name='Amount' value='200' checked />$200 Annual Subscription</p>\
                                <p> <input type='radio' name='Amount' value='55'/>$55 Quarterly Subscription</p>");
    }
    $("#registerStep1").addClass('active');

    return false;
}

function loadPersonalInfoTab() {
    $(".registerStep").each(function () {
        $(this).removeClass('active');
    });
    $(".tab-pane").each(function () {
        $(this).removeClass('active show');
    });
    $("#_tabl1").removeClass('active');
    $("#_tabl3").removeClass('active');
    $("#_tab2").addClass('active');
    if ($("#tabs-product-2").hasClass("fade")) {
        $("#tabs-product-2").removeClass("fade");
        $("#tabs-product-1").addClass("fade");
        $("#tabs-product-3").addClass("fade");
    }
    $("#tabs-product-2").addClass('active show');
    $("#registerStep2").addClass('active');
    $("#PlayerType").val($("input[name='pType']:checked").val());
    if ($("input[name='pType']:checked").val() === "INTERMEDIATE PLAYER") {
        $("#PlanType").val($("#ddlPlan").val());
    }
    else {
        $("#PlanType").val("yearly");
    }
    return false;
}

function onPlayerRegisterSuccess(response) {
    if (response.indexOf("alert-success") > -1) {
        $("#formRegister").trigger("reset");
        $("#tabs-product-2").html(response);
        $(".registerStep").each(function () {
            $(this).removeClass('active');
        });
        $(".tab-pane").each(function () {
            $(this).removeClass('active show');
        });
        $("#tabs-product-2").addClass('active show');
        $("#registerStep3").addClass('active');
        setTimeout(function () {
            window.location.href = "/MyAccount"
        },3000)
    }
    else {
        $("#response").html(response);
       
    }
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');
}
function onFreePlayerRegisterSuccess(response) {
    $("#response").html(response);
    if (response.indexOf("alert-success") > -1) {
        $("#formRegister").trigger("reset");
    }
    $("#preloader").removeClass('loaderActive');
    $("#preloader").addClass('loaded');
    setTimeout(function () {
        $("#response").html('');
        if (response.indexOf("alert-success") > -1) {
            window.location.href = "/login";
        }
    }, 10000);
}
function loadStates() {
    var Id = $("#Country").val();
    if (Id === "") {
        $("#City option").remove();
        $("#State option").remove();
        var option = $('<option>').attr('value', "").html("Select State");
        $('#State').append(option);
        option = $('<option>').attr('value', "").html("Select City");
        $('#City').append(option);
    }
    else {
        var url = "/bindState/" + Id;
        $.ajax({
            url: url,
            error: function (resp, text) {
                $output.html('Server error: ' + text);
                setTimeout(function () {
                    $output.removeClass("active");
                }, 4000);
            },
            success: function (resp) {
                $("#State option").remove();
                $("#City option").remove();
                option = $('<option>').attr('value', "").html("Select City");
                $('#City').append(option);
                $.each(resp, function () {
                    var option = $('<option>').attr('value', this.value).html(this.text);
                    $('#State').append(option);
                });
            }
        });
    }
}

function loadCity() {
    var cid = $("#Country").val();
    var sid = $("#State").val();
    if (cid === "" || sid === "") {
        $("#City option").remove();
        option = $('<option>').attr('value', "").html("Select City");
        $('#City').append(option);
    }
    else {
        var url = "/bindcity/" + sid + "/" + cid;
        $.ajax({
            url: url,
            error: function (resp, text) {
                $output.html('Server error: ' + text);
                setTimeout(function () {
                    $output.removeClass("active");
                }, 4000);
            },
            success: function (resp) {
                $("#City option").remove();
                $.each(resp, function () {
                    var option = $('<option>').attr('value', this.value).html(this.text);
                    $('#City').append(option);
                });
            }
        });
    }
}

function onProfileEditSuccess(response) {
    //$("#editInfoModel").modal('hide');
    //$("#myModal").find("._body").html("");
    //$("#myModal").find("._body").append("<div>" + response + "</div>");
    //$("#myModal").modal('show');
    $("#response").html(response);
    setTimeout(function () {
        $("#response").html('');
        window.location.reload(); $('#editInfoModel').modal('toggle');
    }, 2000);
}

function onPrivateContestSuccess(response) {
    $("#response").html('');
    $("#response").html(response);
    $("#myModal").modal('show');
    if (response.indexOf("alert-success") > -1) {
        if ($("#contestId") !== null) {
            window.location.href = "/team-formation/"
                + $("#contestId").val() + "/"
                + $("#eventId").val() + "";
        }
    }
}

$('#myModal').on('hidden.bs.modal', function (e) {
    $(this)
        .find("input,textarea,select")
        .val('')
        .end()
        .find("input[type=checkbox], input[type=radio]")
        .prop("checked", "")
        .end();
});

$(document).ready(function () {



    $("#tabs-product-2").on('keyup', 'input', function () {
        validatePlayer();
    });

    //$("#DateOfBirth").datetimepicker({
    //    language: 'en',
    //    format: 'm/d/Y',
    //    theme: 'dark',
    //    timepicker: false,
    //    maxDate: 0
    //});
    //$("#DOB").datetimepicker({
    //    language: 'en',
    //    format: 'm/d/Y',
    //    theme: 'dark',
    //    timepicker: false,
    //    maxDate: 0
    //});

    $('#formRegister').submit(function () {
        var accept = $("#hdnAccept").val();
        if ($(this).valid() && accept === "") {
            $('#tcpop').modal('show');
            return false;
        }
        else if ($(this).valid() && accept !== "") {
            return true;
        }
        return false;
    });
});

function acceptTC() {
    $("#hdnAccept").val('1');
    $('#formRegister').submit();
}

function onBeginSubscribe() {
    $("#form-output-global").html('<p><span class="icon text-middle fa fa-circle-o-notch fa-spin icon-xxs"></span><span>Sending</span></p>');
    $("#form-output-global").addClass('active');
}

function onCompleteSubscribe() {
    $("#form-output-global").removeClass('active');
}

function onFailedSubscribe() {
    $("#form-output-global").removeClass('active');
}

function onSuccessSubscribe(response) {
    $("#form-output-global").removeClass('active');
    $("#subscribeForm").trigger("reset");
    if (response === "success") {
        swal("You have subscribed successfully.", {
            icon: "success"
        });
    }
    else {
        swal("Oops! Something went wrong, Please try again.", {
            icon: "warning"
        });
    }
}

function onBeginReferral() {
    $("#form-output-global").html('<p><span class="icon text-middle fa fa-circle-o-notch fa-spin icon-xxs"></span><span>Sending</span></p>');
    $("#form-output-global").addClass('active');
}

function onCompleteReferral() {
    $("#form-output-global").removeClass('active');
}

function onFailedReferral() {
    $("#form-output-global").removeClass('active');
}

function onSuccessReferral(response) {
    $("#form-output-global").removeClass('active');
    $("#sendReferralForm").trigger("reset");
    if (response === "success") {
        swal("You have invited your friend successfully.", {
            icon: "success"
        });
    }
    else {
        swal("Oops! Something went wrong, Please try again.", {
            icon: "warning"
        });
    }
}

function ActiveLink() {
    $(".rd-nav-item").each(function () { $(this).removeClass('active'); });
    var pathname = window.location.pathname.split("/")[1];
    if (pathname.toLowerCase() === "page") {
        pathname = window.location.pathname.split("/")[2];
    }
    switch (pathname.toLowerCase()) {
        case "":
        case "conteststandings":
            $(".home").addClass("active");
            break;
        case "about-us":
            $(".about_us").addClass("active");
            break;
        case "rules-scoring":
            $(".rules").addClass("active");
            break;
        case "riders":
        case "rider":
            $(".riders").addClass("active");
            break;
        case "bulls":
        case "bull":
            $(".bulls").addClass("active");
            break;
        case "events":
        case "contest":
        case "event":
        case "team":
        case "team-formation":
            $(".contest").addClass("active");
            break;
        case "rr-store":
            $(".rrstore").addClass("active");
            break;
        case "news":
            $(".news").addClass("active");
            break;
    }
}

function ShowPage(th) {
    $("#_tabl1").addClass('active');
    $("#_tabl3").removeClass('active');
    $("#_tab2").removeClass('active');

    $("#tabs-product-1").removeClass("fade");
    $("#tabs-product-1").addClass("show active");
    $("#tabs-product-2").addClass("fade");
    $("#tabs-product-3").addClass("fade");
    $("#tabs-product-3").removeClass("show");
    $("#tabs-product-3").removeClass("active");
}

function ShowValidationPage(th) {
    $("#_tabl3").addClass('active');
    $("#_tabl1").removeClass('active');
    $("#_tab2").removeClass('active');

    $("#tabs-product-3").removeClass("fade");
    $("#tabs-product-3").addClass("show active");
    $("#tabs-product-2").addClass("fade");
    $("#tabs-product-1").addClass("fade");
    $("#tabs-product-1").removeClass("show");
    $("#tabs-product-1").removeClass("active");
}

function ShowPlayerType(th) {
    $("#_tabl1").addClass('active');
    $("#_tabl3").removeClass('active');
    $("#_tab2").removeClass('active');

    $("#tabs-product-1").removeClass("fade");
    $("#tabs-product-1").addClass("show active");
    $("#tabs-product-3").addClass("fade");
    $("#tabs-product-2").addClass("fade");
    $("#tabs-product-2").removeClass("show");
    $("#tabs-product-2").removeClass("active");
}


function buyToken() {
    var plan = $("input[name='Amount']:checked").val();
    window.location.href = "/checkout/0/0/0/" + plan;
}

function setview(id) {
    if (id === 1) {
        $("#popupTitle").text('NOVICE PLAYER');
        $("#np").show();
        $("#ip").hide();
        $("#pp").hide();
    }
    else if (id === 2) {
        $("#popupTitle").text('INTERMEDIATE PLAYER');
        $("#np").hide();
        $("#ip").show();
        $("#pp").hide();
    }
    else if (id === 3) {
        $("#popupTitle").text('PRO PLAYER');
        $("#np").hide();
        $("#ip").hide();
        $("#pp").show();
    }
    else {
        $("#np").hide();
        $("#ip").hide();
        $("#pp").hide();
        $("#playerDescription").modal('hide');
    }
}

function onTransactionSuccess(response) {
    $("#fpRespone").html(response);
    var url = '';
    if (response.indexOf("alert-success") > -1) {
        window.location.href = "/thank-you";
    }
    //$("#preloader").removeClass('loaderActive');
}

function isNumber(evt) {
    evt = evt ? evt : window.event;
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
            var typeofelement = $(this).attr(id);

            if ($(this).val() === "") {

                if (typeofelement === "AwardTypeId") {

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

function validateContestJoin(type, fee, token, current) {

    var tokenVal = parseFloat($("#hdn_walletToken").val());
    if (type === "Token" && fee > tokenVal) {
        swal("Oops! you do not have enough token to join the contest. Please co-ordinate with admin at info@rankridefantasy.com", {
            icon: "warning",
        });
        return false;
    }
    else {
        var url = $(current).attr('data-url');
        window.location.href = url;
    }
}

var count = 0;
$(function () {

    $("#addButton").click(function () {
        count = $('#contestWinnerDT tbody tr.trnewrow').length;
        var winnerCount = parseInt($('#Winners').val());
        if (count < winnerCount) {
            var newTr = $(".trclone").clone();
            $(newTr).show();
            $(newTr).removeClass('trclone');
            $(newTr).addClass('trnewrow');
            $(newTr).find('td select').addClass('validate');

            $(newTr).find('td Input').addClass('validate');
            $("#contestWinnerDT").append(newTr);
        }
        else {
            swal("You have already inserted sufficient number of rows for inserting the winners. ", {
                icon: "warning",
            });
        }
    });
});

//function onSubscribedRegisterSuccess(response) {
//    $("#fpRespone").html(response);
//    if (response.indexOf("alert-success") > -1) {
//        $("#fpBody").hide();
//        $("#fpForm").trigger("reset");
//        var url = '';
//        if (url === '') { url = "/"; }
//        setTimeout(function () { window.location.href = url; }, 3000);
//    }
//    //$("#preloader").removeClass('loaderActive');
//}

function restrictPlayer(th) {
    var state = $("#State").val();
    var date = $("#Date").val();
    var city = $("#City").val();
    var zipCode = $("#ZipCode").val();
    $.ajax({
        url: actionURL("enablevalidplayer", "account"),
        type: "POST",
        data: { state: state, dob: date },
        error: function (resp, text) {
            alert(text);
        },
        success: function (resp) {
            debugger;
            if (resp.errorMessage === null) {
                //$("#_tab2").attr("onclick", 'return loadPersonalInfoTab()');
                //$("#player").attr("onclick",'return loadPersonalInfoTab()');
                $("#_errormsg").text('');
                if (resp.isNovice) {
                    $("._playerTypeNovice").removeAttr("style");
                    $("._playerTypeNovice").css("diplay", "block");
                }
                if (resp.isIntermediate) {
                    $("._playerTypeInter").removeAttr("style");
                    $("._playerTypeInter").css("diplay", "block");
                }
                if (resp.isPro) {
                    $("._playerTypePro").removeAttr("style");
                    $("._playerTypePro").css("diplay", "block");
                }
                ShowPage();
                $("#lblState").text(state);
                $("#lblCity").text(city);
                $("#lblZipCode").text(zipCode);
                $("#DateOfBirth").val(date);
                $("#StateName").val(state);
                $("#CityName").val(city);
                $("#PostCode").val(zipCode);

            }
            else {
                //$("#_tab2").removeAttr("onclick");
                //$("#player").removeAttr("onclick");
                //$("._playerTypeNovice").hide('');
                //$("._playerTypeInter").hide('');
                //$("._playerTypePro").hide('');
                $("#invalidStateResponse").html(resp.errorMessage);
                $("#invalidStateResponse").show();
                setTimeout(function () {
                    $("#invalidStateResponse").hide();
                    
                }, 2500);
            }
        }
    });

}

function delay(callback, ms) {
    var timer = 0;
    return function () {
        var context = this, args = arguments;
        clearTimeout(timer);
        timer = setTimeout(function () {
            callback.apply(context, args);
        }, ms || 0);
    };
}

var initTables = [];
function initDataTable(isReinit) {
    $('.responsive_dataTable').each(function (i, table) {
      
        var responsiveSettings = {};
        if ($(table).data('triggercolum')) {
            if ($(table).data('triggercolum_responsivesize') && $(table).data('triggercolum_responsivesize') > window.innerWidth)
            {
                responsiveSettings = {
                    className: 'control',
                    targets: $(table).data('triggercolum')
                }
            }
          }
        
        
        var dataTable_ = $(table).dataTable({
                "responsive": true,
                "searching": false,
                "paging": false,
                "processing": false,
                "serverSide": false,
                "filter": false,
                "info": false,
                "ordering": false,
                "columnDefs":
                    [
                        responsiveSettings
                    ]
        })
        })
    
}

