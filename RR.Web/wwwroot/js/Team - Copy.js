
var requestUrl = actionURL("createteam", "team");
var idx = [];



function addRiders(th) {
    
    var data = getCookie('Team');
    var teamCreationArr = JSON.parse(data);
    var x = $(th);
    var i, j, item = {};
        
    item.EventId = x.attr('data-eventid');
    item.ContestType = x.attr('data-contesttype');
    item.riderId = x.attr('data-riderid');
    item.IsSubstitute = false;
    item.bullId = 0;
    item.RRTotalpoint = parseFloat(x.attr('data-rrtotalpoint'));
    item.WorldRanking = x.attr('data-worldranking');
    item.RiderPower = parseFloat(x.attr('data-riderpower'));
    item.RiderAvatar = x.attr('data-avatar');

    if (x.attr("data-tier") == 1) {
        i = 0;
    }
    else if (x.attr("data-tier") == 2) {
        i = 1;
    }
    else {
        i = 2;
    }

    if (item.ContestType == 1) {
        item.TeamNumber = 1;
    }
    else {
        item.TeamNumber = 2;
    }

    var riderId = teamCreationArr[i].map(function (el) { return Number(el.riderId) });

    idx = jQuery.inArray(Number(item.riderId), riderId);
    if (idx == -1) {
        if (item.ContestType == 2 ? teamCreationArr[i].length < 1 : teamCreationArr[i].length < 2) {
            if ($(th).hasClass("btn_select")) {
                teamCreationArr[i].push(item);
                $(th).removeClass("btn_select");
                $(th).addClass("btn_remove");
                $(th).html("Remove");
            }
            else {
                if ($(th).hasClass("btn_remove")) {
                    teamCreationArr.splice(idx, 1);
                    $(th).removeClass("btn_remove");
                    $(th).addClass("btn_select");
                    $(th).html("Select");
                }
            }
        }
        else {
            if ($(th).hasClass("btn_remove")) {
                teamCreationArr[i].splice(idx, 1);
                $(th).removeClass("btn_remove");
                $(th).addClass("btn_select");
                $(th).html("Select");
            }
            else {
                bootbox.alert("The Required riders are selected in this Tier!");
            }
        }
    }
    else {
        if ($(th).hasClass("btn_remove")) {
            console.log("remove rider");
            teamCreationArr[i].splice(idx, 1);
            $(th).removeClass("btn_remove");
            $(th).addClass("btn_select");
            $(th).html("Select");
        }
    }
    setCookie(teamCreationArr);
    refreshSelection();
}



function addBulls(th) {
    var data = getCookie('Team');
    var teamCreationArr = JSON.parse(data);
    var x = $(th);
    var i, item = {};
    
    item.EventId = x.attr('data-eventid');
    item.ContestType = x.attr('data-contesttype');
    item.bullId = x.attr('data-bullid');
    item.IsSubstitute = false;
    item.riderId = 0;
    item.Owner = x.attr('data-owner');
    item.PowerRating = parseFloat(x.attr('data-powerrating'));
    item.AverageMark = parseFloat(x.attr('data-averagemark'));
    item.RankRideScore = parseFloat(x.attr('data-rankridescore'));
    item.BullAvatar = x.attr('data-avatar');


    if (x.attr("data-tier") == 1) {
        i = 3;
    }
    else if (x.attr("data-tier") == 2) {
        i = 4;
    }
    else {
        i = 5;
    }
    if (item.ContestType == 1) {
        item.TeamNumber = 1;
    }
    else {
        item.TeamNumber = 2;
    }

    var bullId = teamCreationArr[i].map(function (el) { return Number(el.bullId) });

    idx = jQuery.inArray(Number(item.bullId), bullId);
    if (idx == -1) {
        if (teamCreationArr[i].length < 1) {
            if ($(th).hasClass("btn_select")) {
                
                teamCreationArr[i].push(item);
                $(th).removeClass("btn_select");
                $(th).addClass("btn_remove");
                $(th).html("Remove");
            }
            else {
                if ($(th).hasClass("btn_remove")) {
                    teamCreationArr[i].splice(idx, 1);
                    $(th).removeClass("btn_remove");
                    $(th).addClass("btn_select");
                    $(th).html("Select");
                }
            }
            
        }
        else {
            if ($(th).hasClass("btn_remove")) {
                
                teamCreationArr[i].splice(idx, 1);
                $(th).removeClass("btn_remove");
                $(th).addClass("btn_select");
                $(th).html("Select");
                
            }
            else {
                if (x.attr("data-tier") == 3) {
                    bootbox.alert("The Required Bulls are selected in this Tier!");
                }
                else {
                    bootbox.alert("Required bulls/riders for this tier have been selected. Please select now from Tier 2/Tier 3!");
                }

            }
        }
    }
    else {
        if (item.TeamNumber == 1) {
            if ($(th).hasClass("btn_remove")) {
                teamCreationArr[i].splice(idx, 1);
                $(th).removeClass("btn_remove");
                $(th).addClass("btn_select");
                $(th).html("Select");
                
            }
        }
    }
    setCookie(teamCreationArr);
    refreshSelection();
}

//function addSubstituteBulls(th) {
//    var x = $(th);
//    var i, item = {};
//    item.EventId = x.attr('data-eventid');
//    item.ContestType = x.attr('data-contesttype');
//    item.BullId = x.attr('data-bullid');
//    item.IsSubstitute = true;
//    item.RiderId = 0;

//    if (x.attr("data-tier") == 1 || x.attr("data-tier") == 2) {
//        bootbox.alert("You may select substitute bulls from below tiers!"); return;
//    }
//    else {
//        i = 7;
//    }

//    if (item.ContestType == 1) {
//        item.TeamNumber = 1;
//    }
//    else {
//        item.TeamNumber = 2;
//    }

//    var bullId = teamCreationArr[i].map(function (el) { return el.BullId });

//    idx = jQuery.inArray(item.BullId, bullId);
//    if (idx == -1) {
//        if (teamCreationArr[i].length < 1) {
//            if ($(th).hasClass("material-icons-add")) {
//                $("#tabs-modern-3").find("[data-bullid='" + x.attr('data-bullid') + "']").hide();
//                teamCreationArr[i].push(item);
//                $(th).removeClass("material-icons-add");
//                $(th).addClass("material-icons-remove");
//            }
//            else {
//                if ($(th).hasClass("material-icons-remove")) {
//                    $("#tabs-modern-3").find("[data-bullid='" + x.attr('data-bullid') + "']")
//                        .show();
//                    teamCreationArr[i].splice(idx, 1);
//                    $(th).removeClass("material-icons-remove");
//                    $(th).addClass("material-icons-add");
//                }
//            }
//        }
//        else {
//            if ($(th).hasClass("material-icons-remove")) {
//                $("#tabs-modern-7").find("[data-bullid='" + x.attr('data-bullid') + "']")
//                    .show();
//                teamCreationArr[i].splice(idx, 1);
//                $(th).removeClass("material-icons-remove");
//                $(th).addClass("material-icons-add");
//            }
//            else {
//                bootbox.alert("The Required Bulls are selected in this Tier!");
//            }
//        }
//    }
//    else {
//        if (item.TeamNumber == 1) {
//            if ($(th).hasClass("material-icons-remove")) {
//                $("#tabs-modern-3").find("[data-bullid='" + x.attr('data-bullid') + "']")
//                    .show();
//            }
//        }
//        if ($(th).hasClass("material-icons-remove")) {
//            teamCreationArr[i].splice(idx, 1);
//            $(th).removeClass("material-icons-remove");
//            $(th).addClass("material-icons-add");
//        }
//    }
//}

function generateCode(length) {
    var result = '';
    var characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    var charactersLength = characters.length;
    for (var i = 0; i < length; i++) {
        result += characters.charAt(Math.floor(Math.random() * charactersLength));
    }
    return result;
}

function createTeam(th, userId, isEdit) {
    var data = getCookie('Team');
    var teamCreationArr = JSON.parse(data);
    if (isEdit.toLowerCase()==='true') {
        var EventId = $(th).data('eventid');
        var Contestid = $(th).data('contestId');
        var ContestType = $(th).data('contesttype');
        var TeamNumber = 1;

        if (ContestType == 1) {
            TeamNumber = 1;
        }
        else {
            TeamNumber = 2;
        }
        for (var i = 0; i < 6; i++) {
            for (var j = 0; j < teamCreationArr[i].length; j++) {

                teamCreationArr[i][j].TeamNumber = TeamNumber;
                teamCreationArr[i][j].IsSubstitute = false;
                teamCreationArr[i][j].EventId = EventId;
                teamCreationArr[i][j].ContestType = ContestType;
            }
        }
    }
    var teamList = [];
    for (var i = 0; i < 6; i++) {
        if (i == 0 || i == 1 || i == 2) {
            if (teamCreationArr[i].length > 0) {
                if (teamCreationArr[i] < 2) {
                    bootbox.alert("Number Of riders selected are not valid!");
                    return false;
                }
            }
            else {
                bootbox.alert("Number Of riders selected are not valid!");
                return false;
            }
        }
        else {
            if (teamCreationArr[i].length > 0) {
                if (teamCreationArr[i] < 1) {
                    bootbox.alert("Number Of bulls/riders are not valid!");
                    return false;
                }
            }
            else {
                bootbox.alert("Number Of bulls/riders are not valid!");
                return false;
            }
        }

        for (var j = 0; j < teamCreationArr[i].length; j++) {
           
            teamList.push(teamCreationArr[i][j]);
        }
    }

    var eveID = parseInt($(th).attr('data-eventid'));
    var contID = parseInt($(th).attr('data-contestId'));
    $.ajax({
        url: requestUrl,
        data: {
            teamData: JSON.stringify(teamList),
            eventId: eveID //parseInt(window.location.href.substring(window.location.href.lastIndexOf("/") + 1))
        },
        type: "POST",
        success: function (result) {
            if (result != null) {
                    
                if (result.timeout) {
                    swal(result.message, {
                        icon: "warning",
                        className: "notify-alert",
                        button: false,
                        timer: 5000,
                    });
                    setTimeout(function () {
                        window.location.reload();
                    }, 5300);
                }
                else {
                    location.replace("/checkout/" + result.teamId + "/"
                        + window.location.pathname.split("/")[2] + "/"
                        + result.eventId + "/" + generateCode(5));

                }

                //location.replace("/team/joincontest/" + result.teamId + "/" + window.location.pathname.split("/")[2] + "/" + result.eventId + "");


            }
            else {
                bootbox.alert("You may First Login!!");
            }
        },
        error: function (error) {
            bootbox.alert(error);
        }
    });
}

function arrangeTeamTier(teamCreationArr) {
    //var NewteamCreationArr = [[], [], [], [], [], []];

    //var bullArray = teamCreationArr;
    //var riderArray = teamCreationArr.splice(0, 3);

    //riderArray.reverse();
    //riderArray.forEach((ra, i) => {
    //    if (ra.length > 0) {
    //        ra.forEach((r) => {
    //            NewteamCreationArr[i].push(r);
    //        })
    //    }
    //});

    //bullArray.reverse();
    //bullArray.forEach((ba, i) => {
    //    if (ba.length > 0) {
    //        ba.forEach((b) => {
    //            NewteamCreationArr[(i + 3)].push(b);
    //        })
    //    }

    //});
    return teamCreationArr;

}

function addlongtermRiders(th) {

    var data = getCookie('longtermTeam');
    if (data) {
        var teamCreationArr = JSON.parse(data);
        var x = $(th);
        var i, j, item = {};


        item.RiderId = x.attr('data-riderid');
        item.Tier = x.attr('data-tier');
        item.IsSubstitute = false;
        item.BullId = 0;


        if (x.attr("data-tier") == 1) {
            i = 0;
        }
        else if (x.attr("data-tier") == 2) {
            i = 1;
        }
        else {
            i = 2;
        }

        if (item.ContestType == 1) {
            item.TeamNumber = 1;
        }
        else {
            item.TeamNumber = 2;
        }

        var riderId = teamCreationArr[i].map(function (el) { return el.RiderId });

        idx = jQuery.inArray(item.RiderId, riderId);

        if (idx == -1) {
            if (item.ContestType == 2 ? teamCreationArr[i].length < 1 : teamCreationArr[i].length < 2) {
                if ($(th).hasClass("material-icons-add")) {
                    //$("#tabs-modern-2").
                    //    find("[data-riderid='" + x.attr('data-riderid') + "']").hide();
                    teamCreationArr[i].push(item);
                    $(th).removeClass("material-icons-add");
                    $(th).addClass("material-icons-remove");
                }
                else {
                    if ($(th).hasClass("material-icons-remove")) {
                        //$("#tabs-modern-2").find("[data-riderid='" + x.attr('data-riderid') + "']")
                        //    .show();
                        teamCreationArr.splice(idx, 1);
                        $(th).removeClass("material-icons-remove");
                        $(th).addClass("material-icons-add");
                    }
                }
            }
            else {
                if ($(th).hasClass("material-icons-remove")) {
                    //$("#tabs-modern-6").find("[data-riderid='" + x.attr('data-riderid') + "']")
                    //    .show();
                    teamCreationArr[i].splice(idx, 1);
                    $(th).removeClass("material-icons-remove");
                    $(th).addClass("material-icons-add");
                }
                else {
                    bootbox.alert("The Required riders are selected in this Tier!");
                }
            }
        }
        else {
            //if (item.TeamNumber == 1) {
            //    if ($(th).hasClass("material-icons-remove")) {
            //        $("#tabs-modern-2").find("[data-riderid='" + x.attr('data-riderid') + "']")
            //            .show();
            //    }
            //    else {
            //        if ($(th).hasClass("material-icons-add")) {
            //            $("#tabs-modern-2").
            //                find("[data-riderid='" + x.attr('data-riderid') + "']").hide();
            //        }
            //    }
            //}
            if ($(th).hasClass("material-icons-remove")) {
                teamCreationArr[i].splice(idx, 1);
                $(th).removeClass("material-icons-remove");
                $(th).addClass("material-icons-add");
            }
        }
        setCookie('longtermTeam', JSON.stringify(teamCreationArr), 1);
    }
}

function addlongtermBulls(th) {

    var data = getCookie('longtermTeam');
    var teamCreationArr = JSON.parse(data);
    var x = $(th);
    var i, item = {};

    item.BullId = x.attr('data-bullid');
    item.Tier = x.attr('data-tier');
    item.IsSubstitute = false;
    item.RiderId = 0;

    if (x.attr("data-tier") == 1) {
        i = 3;
    }
    else if (x.attr("data-tier") == 2) {
        i = 4;
    }
    else {
        i = 5;
    }
    if (item.ContestType == 1) {
        item.TeamNumber = 1;
    }
    else {
        item.TeamNumber = 2;
    }

    var bullId = teamCreationArr[i].map(function (el) { return el.BullId });

    idx = jQuery.inArray(item.BullId, bullId);
    if (idx == -1) {
        if (teamCreationArr[i].length < 1) {
            if ($(th).hasClass("material-icons-add")) {
                //$("#tabs-modern-4").find("[data-bullid='" + x.attr('data-bullid') + "']").hide();
                teamCreationArr[i].push(item);
                $(th).removeClass("material-icons-add");
                $(th).addClass("material-icons-remove");
            }
            else {
                if ($(th).hasClass("material-icons-remove")) {
                    //$("#tabs-modern-4").find("[data-bullid='" + x.attr('data-bullid') + "']")
                    //    .show();
                    teamCreationArr[i].splice(idx, 1);
                    $(th).removeClass("material-icons-remove");
                    $(th).addClass("material-icons-add");
                }
            }
        }
        else {
            if ($(th).hasClass("material-icons-remove")) {
                //$("#tabs-modern-4").find("[data-bullid='" + x.attr('data-bullid') + "']")
                //    .show();
                teamCreationArr[i].splice(idx, 1);
                $(th).removeClass("material-icons-remove");
                $(th).addClass("material-icons-add");
            }
            else {
                if (x.attr("data-tier") == 3) {
                    bootbox.alert("The Required Bulls are selected in this Tier!");
                }
                else {
                    bootbox.alert("Required bulls/riders for this tier have been selected. Please select now from Tier 2/Tier 3!");
                }

            }
        }
    }
    else {
        if (item.TeamNumber == 1) {
            //if ($(th).hasClass("material-icons-remove")) {
            //    $("#tabs-modern-4").find("[data-bullid='" + x.attr('data-bullid') + "']")
            //        .show();
            //}
            if ($(th).hasClass("material-icons-remove")) {
                teamCreationArr[i].splice(idx, 1);
                $(th).removeClass("material-icons-remove");
                $(th).addClass("material-icons-add");
            }
        }
    }

    setCookie('longtermTeam', JSON.stringify(teamCreationArr), 1);
}

function createLongTermTeam(th, userId) {

    requestUrl = actionURL("createlongtermteam", "team");

    var data = getCookie('longtermTeam');
    var teamCreationArr = JSON.parse(data);
    var teamList = [];
    for (var i = 0; i < 6; i++) {
        if (i == 0 || i == 1 || i == 2) {
            if (teamCreationArr[i].length > 0) {
                if (teamCreationArr[i] < 2) {
                    bootbox.alert("Number Of riders selected are not valid!");
                    return false;
                }
            }
            else {
                bootbox.alert("Number Of riders selected are not valid!");
                return false;
            }
        }
        else {
            if (teamCreationArr[i].length > 0) {
                if (teamCreationArr[i] < 1) {
                    bootbox.alert("Number Of bulls/riders are not valid!");
                    return false;
                }
            }
            else {
                bootbox.alert("Number Of bulls/riders are not valid!");
                return false;
            }
        }

        //if (i < 5) {
        //    if (teamCreationArr[i].length > 0) {
        //        if (teamCreationArr[i] < 1) {
        //            bootbox.alert("Number Of substitute riders are not valid!");
        //            return false;
        //        }
        //    }
        //    else {
        //        bootbox.alert("Number Of substitute riders are not valid!");
        //        return false;
        //    }
        //}

        //if (i < 8) {
        //    if (teamCreationArr[i].length > 0) {
        //        if (teamCreationArr[i] < 1) {
        //            bootbox.alert("Number Of substitute bulls are not valid!");
        //            return false;
        //        }
        //    }
        //    else {
        //        bootbox.alert("Number Of substitute bulls are not valid!");
        //        return false;
        //    }
        //}
        for (var j = 0; j < teamCreationArr[i].length; j++) {
            teamList.push(teamCreationArr[i][j]);
        }
    }

    var longtermteamId = parseInt($('#longtermteamId').val());

    var fileUpload = $("#Icon").get(0);
    var brandname = $("#TeamBrand").val();
    var file = fileUpload.files[0];

    var form = new FormData();
    form.append("brandName", brandname);
    form.append("iconFile", file);
    form.append("teamData", JSON.stringify(teamList));
    form.append("teamId", longtermteamId);

    $.ajax({
        url: requestUrl,
        processData: false,
        contentType: false,
        data: form,
        type: "POST",
        success: function (result) {
            if (result != null) {
                //location.replace("/checkout/" + result.teamId + "/"
                //    + window.location.pathname.split("/")[2] + "/"
                //    + result.eventId + "/" + generateCode(5));

                //location.replace("/team/joincontest/" + result.teamId + "/" + window.location.pathname.split("/")[2] + "/" + result.eventId + "");

                bootbox.alert("Team successfully created!!");
                setTimeout(function () {
                    window.location.reload();
                }, 3000)
            }
            else {
                bootbox.alert("You may First Login!!");
            }
        },
        error: function (error) {
            bootbox.alert(error);
        }
    });
}

function sendUpdateRequest(elm, teamId) {

    if (typeof teamId != "undefined" && teamId > 0) {
        $(elm).prop('disabled', true);
        $.ajax({
            url: '/send-request-longtermteamupdate/' + teamId,
            type: "POST",
            success: function (result) {
                if (result.status) {
                    bootbox.alert("Request sent successfully!");
                    setTimeout(function () {
                        window.location.reload();
                    }, 3000)
                }
                else {
                    bootbox.alert(result.message);
                }

                $(elm).prop('disabled', false);
            },
            error: function (error) {
                bootbox.alert(error);
            }
        });
    }

}