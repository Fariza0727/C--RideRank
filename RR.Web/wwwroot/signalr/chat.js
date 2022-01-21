"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.start().catch(function (err) {
    return console.error(err.toString());
});

if (typeof (document.getElementById("sendButton")) != "undefined" && document.getElementById("sendButton") != null) {
    
 


    connection.on("JoinedContest", function (userInfo) {
        var contestId = userInfo.groupid;
        var userId = document.getElementById("userId").value;
        var elm_ = $('.messenger-list li#' + contestId);
        if (elm_) {
            var exist_ = $('.messenger-list li#' + userInfo.userid).length > 0;
            if (!exist_ && userId != userInfo.userid) {
                $('.user-name small', elm_).append(", " + userInfo.username + ",");
                var removeComma = $('.user-name small', elm_).text().replace(/^,|,$/g, '');
                $('.user-name small', elm_).text(removeComma);

                var template_ = `<li id="` + userInfo.userid + `" onclick="loadComponentView(this,'` + userInfo.userid + `', true)">
                            <span data-count="0" class="new_message"></span>
                        <span class="online_icon"></span>
                        <div class="user-img">
                            <a href="#" class="img">
                                <img src="/images/profilePicture/`+ userInfo.pic + `" alt="user" class="mCS_img_loaded">
                            </a>
                        </div>
                        <div class="user-details">
                            <a href="#" class="user-name">`+ userInfo.username + `</a>
                            <span class="last-message-date"></span>
                        </div>
                    </li>`;
                $('.messenger-list .mCSB_container').append(template_);
            }
            if (connectedUserId && connectedUserId == chat.userId) {
                loadGroupComponentView(elm_, contestId, contestId);
            }
            else {



            }
        }
    });

    connection.on("ReceiveMessage", function (chat) {
        var elm_ = null;
        if (chat.contestId > 0) {
            elm_ = $('.messenger-list li#' + chat.contestId);

            if (elm_) {
                var chatroomid = document.getElementById("chatcontestId").value;
                if (chatroomid && chatroomid == chat.contestId) {
                    loadGroupComponentView(elm_, chat.contestId);
                }
                else {
                    var count_ = parseInt($('.new_message', elm_).attr('data-count'));
                    if (count_ > 0)
                        count_ = (count_ + 1);
                    else
                        count_ = 1;
                    $('.last-message', elm_).text(chat.message);
                    $('.new_message', elm_).text(count_).attr('data-count', count_);
                }

            }

        }
        else if (chat.userId) {

            elm_ = $('.messenger-list li#' + chat.userId);
            if (elm_) {
                var connectedUserId = document.getElementById("connectedUserId").value;
                if (connectedUserId && connectedUserId == chat.userId) {
                    loadComponentView(elm_, connectedUserId);
                }
                else {
                    var count_ = parseInt($('.new_message', elm_).attr('data-count'));
                    if (count_ > 0)
                        count_ = (count_ + 1);
                    else
                        count_ = 1;
                    $('.last-message', elm_).text(chat.message);
                    $('.new_message', elm_).text(count_).attr('data-count', count_);
                }
            }
        }
        else {

        }

    });

    connection.on("SeenedMessages", function (Userid) {
        debugger;
        var elm_ = $('.messenger-list li#' + Userid);
        if (elm_) {
            var connectedUserId = document.getElementById("connectedUserId").value;
            if (connectedUserId && connectedUserId == Userid) {
                $('#chatmessagesContainer .chat.sent-message').each(function () {
                    $('.text-msg .zmdi', $(this)).addClass('seened');
                })
            }
        }
    });

    connection.on("UserConnected", function (connectionId) {

        var elm_ = $('.messenger-list li#' + connectionId);
        if (elm_)
            $('.online_icon', elm_).removeClass('offline');

        //var groupElement = document.getElementById("group");
        //var option = document.createElement("option");
        //option.text = connectionId;
        //option.value = connectionId;
        //groupElement.add(option);

    });

    connection.on("UserDisconnected", function (connectionId) {
        //var groupElement = document.getElementById("group");
        //for (var i = 0; i < groupElement.length; i++) {
        //    if (groupElement.options[i].value == connectionId) {
        //        groupElement.remove(i);
        //    }
        //}
    });



    document.getElementById("sendButton").addEventListener("click", function (event) {

        var message = document.getElementById("message").value;

        var contestid = document.getElementById("chatcontestId").value;
        var connectedUserId = document.getElementById("connectedUserId").value;
        var connectedUserName = document.getElementById("connectedUserName").value;
        var connectedUserAvatar = document.getElementById("connectedUserAvatar").value;
        var userId = document.getElementById("userId").value;

        var data = { contestId: contestid, UserId: userId, UserName: connectedUserName, ConnectedUserid: connectedUserId, Avatar: connectedUserAvatar, Message: message };
        $.post('/UserChats/SendMessage', data)
            .done(function (result) {
                if (result.status) {
                    if (parseInt(contestid) <= 0) {
                        var li_ = $('.messenger-list li#' + connectedUserId);
                        loadComponentView(li_, connectedUserId);
                    }
                    document.getElementById("message").value = "";
                }
                else
                    console.log(status.message)
            })
            .fail(function (xhr, status, error) {
                // error handling
            });

        event.preventDefault();
    });

    if (typeof (document.getElementById("searchchatuser")) != "undefined" && document.getElementById("searchchatuser") != null) {
        document.getElementById("searchchatuser").addEventListener("keyup", function (event) {
            var searchText = $(this).val();
            $('ul.messenger-list li').each(function () {
                var currentLiText = $(this).text(),
                    showCurrentLi = currentLiText.indexOf(searchText) !== -1;

                $(this).toggle(showCurrentLi);
            });
            event.preventDefault();
        });
    }

    $(window).on('load', function () {

        var sessionConnecteUser = getCookie("connecteduser");
        var sessionContestUser = getCookie("connectecontest");

        if (!sessionConnecteUser && !sessionContestUser)
            $('.messenger-list li').first().trigger('click');

        if (sessionConnecteUser) {
            $('.messenger-list li#' + sessionConnecteUser).trigger('click');
        }
        
        if (sessionContestUser) {
            $('.messenger-list li#' + sessionContestUser).trigger('click');
        }
    })

}

var joinChatSingal = function (userId) {
    setTimeout(function () {
        if (connection.connectionState == 'Connected') {
            connection.invoke("JoinUser", userId).catch(function (err) {
                return console.error(err.toString());
            });
        }
    }, 1000)

}

var formatAMPM = function (date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}
var loadComponentView = function (elm, conectedUserid, isSeen) {
   
    if (typeof conectedUserid !== "undefined") {
        setCookie("connecteduser", conectedUserid,30);
        $.post("/Component/GetUserChats", { aspuserid: conectedUserid, isSeend: isSeen })
            .done(function (data) {
                $('#connectedUserId').val(conectedUserid);
                $("#chatMessagesComponent").html(data);
                $('.new_message', elm).attr('data-count', 0).text('');
                if (window.innerWidth <= 991) {
                    $('#project-list').trigger('click');
                }
            }).fail(function (xhr, status, error) {
                console.log(error);
            })
            .always(function () {
                

                    var height_ = 0;

                    $("#chatMessagesComponent .chat ").each(function (i, elm) {
                        height_ += $(elm).outerHeight();
                    });
                    $("#chatMessagesComponent .chat-date ").each(function (e, elm) {
                        height_ += $(elm).outerHeight();
                    });

                    $(".content").mCustomScrollbar(
                        {
                            setTop: height_ + "px"
                        });
                

            });


    }
};
var loadGroupComponentView = function (elm, contestid) {
    
    var userId = document.getElementById("userId").value;

    if (typeof contestid !== "undefined") {
        setCookie("connectecontest", contestid, 30);
        $.post("/Component/GetGroupChats", { aspuserid: userId, contestId: contestid, userId })
            .done(function (data) {


                $("#chatMessagesComponent").html(data);
                $('.groupMsg-title').html($('.user-name', elm).html());
                $('#chatcontestId').val(contestid);
                $('.new_message', elm).attr('data-count', 0).text('');
                if (window.innerWidth <= 991) {
                    $('#project-list').trigger('click');
                }
            }).fail(function (xhr, status, error) {
                console.log(error);
            })
            .always(function () {
                var height_ = 0;


                $("#chatMessagesComponent .chat ").each(function (e, elm) {
                    height_ += $(elm).outerHeight();
                });

                $("#chatMessagesComponent .chat-date ").each(function (e, elm) {
                    height_ += $(elm).outerHeight();
                });

                $(".content").mCustomScrollbar(
                    {
                        setTop: height_ + "px"
                    });

            });


    }
};
var joinChatRoom = function (contestId, userId) {
    setTimeout(function () {
        if (connection.connectionState == 'Connected') {
            connection.invoke("JoinGroup", contestId, userId).catch(function (err) {
                return console.error(err.toString());
            });
        }
    }, 1000)

}

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function checkCookie() {
    var user = getCookie("username");
    if (user != "") {
        alert("Welcome again " + user);
    } else {
        user = prompt("Please enter your name:", "");
        if (user != "" && user != null) {
            setCookie("username", user, 365);
        }
    }
}



//document.getElementById("joinGroup").addEventListener("click", function (event) {
//    connection.invoke("JoinGroup", "PrivateGroup").catch(function (err) {
//        return console.error(err.toString());
//    });
//    event.preventDefault();
//});
