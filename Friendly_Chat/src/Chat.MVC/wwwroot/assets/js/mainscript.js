$(document).ready(function () {
    var isnotification = "@ViewBag.IsNewNotification";
    if (isnotification == "yes") {
        $("#li4").find("i").removeClass("fa-envelope");
        $("#li4").find("i").addClass("fa-bell fa-beat-fade");
    }

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/onlineusershub")
        .build();

    connection.on("UserConnected", function (username) {
        //console.log(username + " connected.");
        $("div[data-username='" + username + "']").addClass("online");
    });

    connection.on("UserDisconnected", function (username) {
        //console.log(username + " disconnected.");
        $("div[data-username='" + username + "']").removeClass("online");
    });
    connection.start();

    var menu = '@ViewBag.ActiveMenu';
    $(".navitem").find("li").removeClass("item-active");
    if (menu == "Home") {
        $("#li1").addClass("item-active");
        $.ajax({
            url: "/Home/GetUsers",
            type: "GET",
            dataType: "json",
            success: function (data) {
                var html = '';
                if (data != null) {
                    $.each(data, function (i, val) {
                        if (val.isOnline) {
                            html += '<div onclick="GoChat(\'' + val.toUserName + '\',$(this))" class="discussion"><div class="photo" onclick="Modal(\'' + val.toUserImage + '\');event.stopPropagation()" style = "background-image: url(\'../assets/images/' + val.toUserImage + '\');" ><div data-username="' + val.toUserName + '" class="online" ></div></div><div class="desc-contact"><p class="name">' + val.toUserName + '</p><p class="message">' + val.lastMessage + '</p></div><div class="timer">' + val.newMessagesCount + '</div></div>';
                        }
                        else {
                            html += '<div onclick="GoChat(\'' + val.toUserName + '\',$(this))" class="discussion"><div class="photo" onclick="Modal(\'' + val.toUserImage + '\');event.stopPropagation()" style = "background-image: url(\'../assets/images/' + val.toUserImage + '\');" ><div data-username="' + val.toUserName + '" class="" ></div></div><div class="desc-contact"><p class="name">' + val.toUserName + '</p><p class="message">' + val.lastMessage + '</p></div><div class="timer">' + val.newMessagesCount + '</div></div>';
                        }
                    });
                };
                $("#ChatBox").empty();
                $("#ChatBox").append(html);
            }
        });
    }
    else if (menu == "Media") {
        $("#li5").addClass("item-active");
        var count = 0;
        GetPost(count);
    }
    else if (menu == "Profil") {
        $("#li2").addClass("item-active");
    }
    else if (menu == "Follow") {
        $("#li3").addClass("item-active");
        $("#search").keyup(function () {
            var username = $("#search").val();
            $.ajax({
                url: "/Follow/SearchByUsername",
                type: "GET",
                data: { username: username },
                dataType: "json",
                success: function (data) {
                    var html = "";
                    if (data != null) {
                        $.each(data, function (i, val) {
                            if (val.requestStatus == "sendRequest") {
                                html += '<div class="card"><div class="card-body"style="display:flex;justify-content: space-between;"><div style="display: flex; align-items: center;"><img onclick="Modal(\'' + val.image + '\')" src="../assets/images/' + val.image + '"alt="user"style="width:40px;height:40px;"/><p style="margin-left:20px;margin-top:5px;font-size:20px;font-weight:bold;">' + val.userName + '</p></div><div style="display: flex; align-items: center;"><button class="btn btn-success"  onclick="SendRequest(\'' + val.userName + '\',$(this))" >Send follow request</button><button style="display:none;" class="btn btn-secondary" onclick="WithdrawRequest(\'' + val.userName + '\',$(this))">Withdraw the request</button></div></div></div>';
                            }
                            else if (val.requestStatus == "requested") {
                                html += '<div class="card"><div class="card-body"style="display:flex;justify-content: space-between;"><div style="display: flex; align-items: center;"><img onclick="Modal(\'' + val.image + '\')" src="../assets/images/' + val.image + '"alt="user"style="width:40px;height:40px;"/><p style="margin-left:20px;margin-top:5px;font-size:20px;font-weight:bold;">' + val.userName + '</p></div><div style="display: flex; align-items: center;"><button class="btn btn-secondary" onclick="WithdrawRequest(\'' + val.userName + '\',$(this))">Withdraw the request</button><button style="display:none;" class="btn btn-success"  onclick="SendRequest(\'' + val.userName + '\',$(this))" >Send follow request</button></div></div></div>';
                            }
                            else {//followed
                                html += '<div class="card"><div class="card-body"style="display:flex;justify-content: space-between;"><div style="display: flex; align-items: center;"><img onclick="Modal(\'' + val.image + '\')" src="../assets/images/' + val.image + '"alt="user"style="width:40px;height:40px;"/><p style="margin-left:20px;margin-top:5px;font-size:20px;font-weight:bold;">' + val.userName + '</p></div><div style="display: flex; align-items: center;"><button class="btn btn-danger" onclick="DeleteFollowUp(\'' + val.userName + '\',$(this))">Do not follow</button><button class="btn btn-success" style="display:none;" onclick="SendRequest(\'' + val.userName + '\',$(this))" >Send follow request</button><button class="btn btn-secondary" style="display:none;" onclick="WithdrawRequest(\'' + val.userName + '\',$(this))">Withdraw the request</button></div></div></div>';
                            }
                        });
                    }
                    else {
                        console.log("1sq");
                        html += '<div class="card"><div class="card-body"style="display:-webkit-box;display:-ms-flexbox;display:flex;"><p>Heç bir netice yoxdur</p>';
                    }
                    $('#search_box').empty();
                    $('#search_box').append(html);
                }
            });
        });
    }
    else if (menu == "Notification") {
        $("#li4").addClass("item-active");
        $.ajax({
            url: "/Notification/GetNotifications",
            type: "Get",
            dataType: "json",
            success: function (data) {
                var html = '';
                if (data != null) {
                    $.each(data, function (i, val) {
                        html += '<div class="notification-list notification-list--unread" style="height:90px;width:900px;margin-left:15px;"><div class="notification-list_content"><div class="notification-list_img"><img class="myimg11" onclick="Modal(\'' + val.image + '\')" src="../assets/images/' + val.image + '" alt="user"></div><div class="notification-list_detail"><p style="margin-top:15px; font-size:20px"><b>' + val.fromUser + '</b> sent you a friend request-  ->' + val.date + '</p></div></div><div class="notification-list_feature-img buttons"><button class="btn button-37" onclick="Accept(\'' + val.fromUser + '\',$(this))" role="button">Accept</button><button class="btn button-37-1" onclick="Reject(\'' + val.fromUser + '\',$(this))" role="button">Reject</button></div></div>';
                    });
                }
                else {
                    html += '<img src="../assets/images/nonotification.jpg" alt="user" style="width:300px;margin-left:300px;">';
                }
                $('#notification_box').empty();
                $('#notification_box').append(html);
            }
        });
    };


    //-------MEDIA-----------
    function GetPost(count) {
        $.ajax({
            url: "/Media/GetPosts",
            type: "GET",
            data: { count: count },
            dataType: "json",
            success: function (data) {
                var html = "";
                if (data != null) {
                    $.each(data, function (i, val) {
                        if (val.postType == "image") {
                            html += `<div class="card mb-3" style="width:95%;margin-left:25px;">
                                                                                                                                                                                                                                                                            <img class="card-img-top" onclick="MediaModal('`+ val.postName + `')" src="../assets/media/photos/` + val.postName + `" style="width:100%;max-height:650px;aspect-ratio:2/1;object-fit:contain" alt="Card image cap">
                                                                                                               <div class="card-body">
                                                                                                                           <img class="profil" src="../assets/images/`+ val.userPhoto + `" style="width:60px;border-radius:50%" />
                                                                                                                           <h4 style="display:inline-block;" class="card-title">`+ val.userName + `</h4>
                                                                                                                   <p class="card-text" style="display:inline-block;">
                                                                                                                                               <small class="text-muted">&nbsp;&nbsp;&nbsp;`+ val.date + `</small>
                                                                                                                   </p>
                                                                                                                           <p class="card-text">`+ val.description + `</p>
                                                                                                                        <div class="btn-group dropright" style="display:inline-block">
                                                                                                                               <button class="btn btn-outline-primary dropdown-toggle" onclick="dropDwn($(this))" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                                                                                           <i class="fa-solid fa-paper-plane"></i>
                                                                                                                       </button>
                                                                                                                       <div class="dropdown-menu">
                                                                                                                           <div class="userlist" style="background-color:rgb(224, 96, 182); width:250px; max-height:400px;overflow-y:scroll;overflow-x:hidden;">
                                                                                                                               <div class="user-list-item">
                                                                                                                                   <span class="user-name">User 2</span>
                                                                                                                                   <input type="checkbox" name="selectedUsers" value="user1" style="width:20px;height:20px;margin-right:10px;">
                                                                                                                               </div>
                                                                                                                               <div class="user-list-item">
                                                                                                                                   <span class="user-name">User adminnn 3</span>
                                                                                                                                   <input type="checkbox" name="selectedUsers" value="user1" style="width:20px;height:20px;margin-right:10px;">
                                                                                                                               </div>
                                                                                                                               <div class="user-list-item">
                                                                                                                                   <span class="user-name">User 1123</span>
                                                                                                                                   <input type="checkbox" name="selectedUsers" value="user1" style="width:20px;height:20px;margin-right:10px;">
                                                                                                                               </div>
                                                                                                                               <div class="user-list-item">
                                                                                                                                   <span class="user-name">User 2</span>
                                                                                                                                   <input type="checkbox" name="selectedUsers" value="user1" style="width:20px;height:20px;margin-right:10px;">
                                                                                                                               </div>
                                                                                                                               <div class="user-list-item">
                                                                                                                                   <span class="user-name">User 2</span>
                                                                                                                                   <input type="checkbox" name="selectedUsers" value="user1" style="width:20px;height:20px;margin-right:10px;">
                                                                                                                               </div>
                                                                                                                               <div class="user-list-item">
                                                                                                                                   <span class="user-name">User adminnn 3</span>
                                                                                                                                   <input type="checkbox" name="selectedUsers" value="user1" style="width:20px;height:20px;margin-right:10px;">
                                                                                                                               </div>
                                                                                                                           </div>
                                                                                                                           <button type="button" class="dropdown-item active btn btn-primary">Send</button>
                                                                                                                       </div>
                                                                                                                   </div>
                                                                                                                           <!--like dislike button-->
                                                                    <p class="like-count" style="display:inline-block">25</p>
                                                                    <button type="button" class="btn btn-outline-primary like">
                                                                        <i class="fa-regular fa-heart"></i>
                                                                    </button>
                                                                    <button type="button" class="btn btn-outline-primary dislike" style="display:none;">
                                                                        <i class="fa-solid fa-heart fa-beat" style="color: #e23232;"></i>
                                                                    </button>
                                                                </div>
                                                                                                                                                                                                                                                            </div>
                                                                                                                                                                                                                                                        </div>`;
                        }
                        else if (val.postType == "video") {
                            html += `<div class="card mb-3" style="width:95%;margin-left:25px;">
                                                                                                                                                                                               <video style="width:100%;max-height:650px;aspect-ratio:2/1;object-fit:contain" controls>
                                                                                                                                                                                                           <source src="/assets/media/videos/`+ val.postName + `" type="video/mp4">
                                                                                                                                                                                                   Your browser does not support the video tag.
                                                                                                                                                                                               </video>
                                                                                                                                                                                               <div class="card-body">
                                                                                                                                                                                                                   <img src="../assets/images/`+ val.userPhoto + `" style="width:60px;border-radius:50%" />
                                                                                                                                                                                                           <h4 style="display:inline-block;" class="card-title">`+ val.userName + `</h4>
                                                                                                                                                                                                   <p class="card-text" style="display:inline-block;">
                                                                                                                                                                                                                                       <small class="text-muted">&nbsp;&nbsp;&nbsp;`+ val.date + `</small>
                                                                                                                                                                                                   </p>
                                                                                                                                                                                                         <p class="card-text">`+ val.description + `</p>
                                                                                                                                                                                                 <div style="display:flex;justify-content:space-between">
                                                                                                                                                                                                     <!--Users list for send post-->
                                                                                                                                                                                                     <div class="btn-group dropright" style="display:inline-block">
                                                                                                                                                                                                                 <button type="button" class="btn btn-outline-primary dropdown-toggle" onclick="dropDwn($(this))" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                                                                                                                                                                             <i class="fa-solid fa-paper-plane"></i>
                                                                                                                                                                                                         </button>
                                                                                                                                                                                                         <div class="dropdown-menu">
                                                                                                                                                                                                             <div class="userlist" style="background-color:rgb(224, 96, 182); width:250px; max-height:400px;overflow-y:scroll;overflow-x:hidden;">
                                                                                                                                                                                                                 <div class="user-list-item">
                                                                                                                                                                                                                     <span class="user-name">User 2</span>
                                                                                                                                                                                                                     <input type="checkbox" name="selectedUsers" value="user1" style="width:20px;height:20px;margin-right:10px;">
                                                                                                                                                                                                                 </div>
                                                                                                                                                                                                             </div>
                                                                                                                                                                                                             <button type="button" class="dropdown-item active btn btn-primary">Send</button>
                                                                                                                                                                                                         </div>
                                                                                                                                                                                                     </div>
                                                                                                                                                                                                     <!--like dislike button-->
                                                                                                                                                                                                     <div style="display:inline-block">
                                                                                                                                                                                                         <p class="like-count" style="display:inline-block">25</p>
                                                                                                                                                                                                         <button type="button" class="btn btn-outline-primary like">
                                                                                                                                                                                                             <i class="fa-regular fa-heart"></i>
                                                                                                                                                                                                         </button>
                                                                                                                                                                                                         <button type="button" class="btn btn-outline-primary dislike" style="display:none;">
                                                                                                                                                                                                             <i class="fa-solid fa-heart fa-beat" style="color: #e23232;"></i>
                                                                                                                                                                                                         </button>
                                                                                                                                                                                                     </div>
                                                                                                                                                                                                 </div>
                                                                                                                                                                                             </div>
                                                                                                                                                                                        </div>`;
                        }
                    });
                }
                //$("#posts").empty();
                $("#posts").append(html);
            }
        });
    }
    var x = 0;
    var take = 1;
    var currentScroll = 0;
    var p = 0;
    $(".chatpost").scroll(function () {
        currentScroll = $(this).scrollTop();
        if (p <= currentScroll) {
            x += 1;
            if (x == 55) {
                GetPost(take)
                take += 1;
                x = 0;
                p = 0;
                currentScroll = 0;
            }
        }
        p = currentScroll;
    });

});



//----------HOME-----------------------------
var GetAudio = new Audio("../assets/audio/get.mp3")
function GoChat(name, el) {
    $(".message-active").removeClass("message-active");
    el.addClass("message-active");
    $(".write-message").each(function () {
        $(this).removeData("touser");
    });
    $(".chat-icon").css("display", "none");
    $(".header-chat").css("display", "flex");
    $("#header-name").text(name);
    $.ajax({
        url: "Home/GetOldChat",
        type: "GET",
        dataType: "json",
        data: { toUserName: name },
        success: function (data) {
            var html = '';
            $.each(data, function (i, val) {
                if (val.fromUserName != name) {
                    if (val.date != null) {
                        html += '<div class="message" style="justify-content: center;"><p class="text" style = "background-color: #a71694;">' + val.date + '</p></div>'
                    }
                    html += '<div class="message"><p style="font-size:10px;">' + val.time + '</p><p class="text" style="max-width:300px; word-wrap: break-word;">' + val.message + '</p></div>';
                }
                else {
                    if (val.date != null) {
                        html += '<div class="message" style="justify-content: center;"><p class="text" style = "background-color: #a71694;">' + val.date + '</p></div>'
                    }
                    html += '<div class="message"><div class="response"><p class="text" style="max-width:300px; word-wrap: break-word;">' + val.message + '</p></div><p style="font-size:10px;">' + val.time + '</p></div>';
                }
            });
            $('.messages-chat').empty();
            $('.messages-chat').append(html);
            $('.messages-chat').scrollTop($('.messages-chat').prop("scrollHeight"));
            $('.timer').empty();
        }
    });
    $('.messages-chat').css("display", "block");
    $(".footer-chat").css("display", "block");
    $(".write-message").attr("data-toUser", name);

    //------new message-----
    setInterval(function () {
        $.ajax({
            url: "Home/GetNewMessage",
            type: "GET",
            data: { toUserName: name },
            dataType: "json",
            success: function (data) {
                if (data.length != 0) {
                    var html = '';
                    $.each(data, function (i, val) {
                        html += '<div class="message"><p style="font-size:10px;">' + val.time + '</p><p class="text" style="max-width:300px; word-wrap: break-word;">' + val.message + '</p></div>';
                    });
                    GetAudio.play();
                    $('.messages-chat').append(html);
                    $('.messages-chat').scrollTop($('.messages-chat').prop("scrollHeight"))
                }
            }
        });
    }, 1000);
};
var SendAudio = new Audio("../assets/audio/send.mp3");
$(".write-message").on("keydown", function (event) {
    if (event.keyCode === 13) { // Enter click
        var inputValue = $(this).val();
        if ($.trim(inputValue) !== "") {
            var toUser = $(".write-message").data("touser");
            $.ajax({
                url: "/Home/SendMessage",
                type: "POST",
                data: { toUserName: toUser, content: inputValue },
                success: function () {
                    var now = new Date();
                    var html = '<div class="message" ><div class="response"><p class="text" style="max-width:300px; word-wrap: break-word;">' + inputValue + '</p></div><p style="font-size:10px;">' + now.getHours() + ':' + now.getMinutes() + '</p></div>';
                    $('.messages-chat').append(html);
                    $('.messages-chat').scrollTop($('.messages-chat').prop("scrollHeight"))
                    SendAudio.play();
                    $(".write-message").val("");
                }
            });
        }
    }
});



//---------FOLLOW---------------------------
function SendRequest(name, el) {
    el.prop('disabled', true);
    $.ajax({
        url: "/Follow/SendFollowRequest",
        type: "POST",
        data: { username: name },
        success: function () {
            el.css("display", "none");
            el.closest('.card-body').find('.btn-secondary').show();
            el.closest('.card-body').find('.btn-secondary').prop('disabled', false);
        }
    });
};
function WithdrawRequest(name, el) {
    el.prop('disabled', true);
    $.ajax({
        url: "/Follow/WithdrawFollowRequest",
        type: "POST",
        data: { username: name },
        success: function () {
            el.css("display", "none");
            el.closest('.card-body').find('.btn-success').show();
            el.closest('.card-body').find('.btn-success').prop('disabled', false);
        }
    });
};
function DeleteFollowUp(name, el) {
    el.prop('disabled', true);
    $.ajax({
        url: "/Follow/DeleteFollowUp",
        type: "POST",
        data: { username: name },
        success: function () {
            el.css("display", "none");
            el.closest('.card-body').find('.btn-success').show();
            el.closest('.card-body').find('.btn-success').prop('disabled', false);
        }
    });
};



//--------NOTIFICATION----------------------
function Accept(name, el) {
    el.prop('disabled', true);
    $.ajax({
        url: "/Follow/ToAcceptRequest",
        type: "POST",
        data: { username: name },
        success: function () {
            el.closest('.notification-list').css('display', 'none');
        }
    });
}
function Reject(name, el) {
    el.prop('disabled', true);
    $.ajax({
        url: "/Follow/ToRejectRequest",
        type: "POST",
        data: { username: name },
        success: function () {
            el.closest('.notification-list').css('display', 'none');
        }
    });
}



//--------PROFIL-----------------------------
$("#editProfil, #closeform").click(function () {
    $("#editform").toggle();
    $("#followList").css("display", "none");
});
$("#changeinfo").click(function () {
    $("#editform input").prop("disabled", false);
});
$("#closefollowlist").click(function () {
    $("#followList").css("display", "none");
});
$("#Followers").click(function () {
    $("#editform").css("display", "none");
    $("#followList").css("display", "block");
    $.ajax({
        url: "/Profil/GetFollowers",
        type: "GET",
        dataType: "json",
        success: function (data) {
            var html = "";
            if (data != null) {
                $.each(data, function (i, val) {
                    html += '<li class="list-group-item d-flex justify-content-between align-items-center"><div class="d-flex align-items-center"><img src="../assets/images/' + val.followingUserImage + '" style = "width: 45px; height: 45px" class="rounded-circle" /><div class="ms-3" ><p class="fw-bold mb-1" > ' + val.followingUserName + ' </p><p class="text-muted mb-0" >' + val.followingUserEmail + '</p></div></div></li>'
                });
            }
            $("#follogingList").empty();
            $("#follogingList").append(html);
        }
    });
});
$("#Following").click(function () {
    $("#editform").css("display", "none");
    $("#followList").css("display", "block");
    $.ajax({
        url: "/Profil/GetFollowing",
        type: "GET",
        dataType: "json",
        success: function (data) {
            var html = "";
            if (data != null) {
                $.each(data, function (i, val) {
                    html += '<li class="list-group-item d-flex justify-content-between align-items-center"><div class="d-flex align-items-center"><img src="../assets/images/' + val.followingUserImage + '" style = "width: 45px; height: 45px" class="rounded-circle" /><div class="ms-3" ><p class="fw-bold mb-1" > ' + val.followingUserName + ' </p><p class="text-muted mb-0" >' + val.followingUserEmail + '</p></div></div></li>'
                });
            }
            $("#follogingList").empty();
            $("#follogingList").append(html);
        }
    });
});



//---------MEDIA----------------------------
function dropDwn(el) {
    console.log("button")
    var card = el.closest(".card");
    $.ajax({
        url: "Media/GetSendUsers",
        type: "GET",
        dataType: "json",
        success: function (data) {
            console.log("succes");
            var html = "";
            if (data != null) {
                var userlistDiv = card.find(".userlist");
                $.each(data, function (i, val) {
                    html += `
                                                                                <div class="user-list-item">
                                                                                     <span class="user-name">`+ val.userName + `</span>
                                                                                     <input type="checkbox" value="`+ val.userName + `" style="width:20px;height:20px;margin-right:10px;">
                                                                                </div>`;
                    userlistDiv.empty();
                    userlistDiv.append(html);
                });
            };
        }
    });
};



//--------MODAL------------------------------
var modal = document.getElementById("myModal");
var modalImg = document.getElementById("img01");
var captionText = document.getElementById("caption");
function Modal(image) {
    modal.style.display = "block";
    modalImg.src = "../assets/images/" + image;
};
var span = document.getElementsByClassName("close")[0];
span.onclick = function () {
    modal.style.display = "none";
}



//---------MediaModal-----------------------------
var modal = document.getElementById("myMediaModal");
var modalImg = document.getElementById("img");
var captionText = document.getElementById("caption1");
function MediaModal(image) {
    modal.style.display = "block";
    modalImg.src = "../assets/media/photos/" + image;
};
var span = document.getElementsByClassName("closeModal")[0];
span.onclick = function () {
    modal.style.display = "none";
}