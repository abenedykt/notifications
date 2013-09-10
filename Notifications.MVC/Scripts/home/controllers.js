function LogCtrl($scope) {
    $scope.login = function () {
        userName = $scope.user.name;
        userId = $scope.user.id;
    };
}

var userName;
var userId;
var zIndex = 0;
var leftPosition = 0;
var topPosition = 100;


function NoticeCtrl($scope) {
    $scope.showChatList = false;
    $scope.name = userName;

    var chatHub = $.connection.chatHub;

    addClientMethods(chatHub);

    $.connection.hub.start().done(function () {

        chatHub.server.connect(userName, userId);

        $scope.sendNotification = function () {
            var receivers = [];

            $(".chk:checked").each(function () {
                receivers.push($(this).val());
            });

            var message = $("#txtNotification").val();

            if (receivers.length != 0)
                chatHub.server.broadcasting(receivers, message, userName);
            else
                alert("Zaznacz przynajmniej jedna osobe!");
        };
    });
}

function addClientMethods(chatHub) {

    chatHub.client.clearHistoryOfSendNotifications = function() {
        $('#sendNotifications').empty();
    };

    chatHub.client.getReceivedNotifications = function (date, sender, content) {

        $('#receivedNotifications').append('<li><strong>' + date + ', nadawca: ' + sender + '</strong><br/>' + content + '</li>');
    };

    chatHub.client.getSendNotifications = function (date, receivers, content) {

        $('#sendNotifications').append('<li><strong>' + date + ', odbiorcy: ' + receivers + '</strong><br/>' + content + '</li>');
    };
    
    chatHub.client.addReceivedNotification = function (date, sender, content) {

        $('#receivedNotifications').prepend('<li><strong>' + date + ', nadawca: ' + sender + '</strong><br/>' + content + '</li>');
    };

    chatHub.client.addSendNotification = function (date, receivers, content) {

        $('#sendNotifications').prepend('<li><strong>' + date + ', odbiorcy: ' + receivers + ' (nie odczytano)</strong><br/>' + content + '</li>');
    };

    //count of active users
    chatHub.client.onlineUsers = function (count) {

        $("#counter").text('Chat(' + count + ')');
    };

    //send notification 
    chatHub.client.sendNotificationBroadcast = function(notificationId, notification, name, senderId) {

        if (navigator.userAgent.indexOf("Chrome") > -1) {
            if (window.webkitNotifications.checkPermission() == 0) {
                var note = window.webkitNotifications.createNotification(null, "Nowe powiadomienie od: " + name, notification);
               
                note.onclick = function() {
                    chatHub.server.addTimeofReading(notificationId, senderId);
                };
                note.onclose = function () {
                    chatHub.server.addTimeofReading(notificationId, senderId);
                };             
                note.show();
            } else {
                window.webkitNotifications.requestPermission();
            }
        } else {
            $.pnotify({
                    title: "Nowe powiadomienie od: " + name,
                    text: notification,
                    hide: false,
                    sticker: false,
                    history: false
                }
            ).click(function() {
                chatHub.server.addTimeofReading(notificationId, senderId);
            });
        }
    };

    chatHub.client.notificationConfirm = function () {
        alert("Powiadomienie zostalo wyslane");
        $("#txtNotification").val('');
    }; // send to all except caller client

    chatHub.client.onNewUserConnected = function (id, name) {
        AddUser(chatHub, id, name);
    }; // send list of active person to caller

    chatHub.client.onConnected = function (id, name, allUsers) {

        for (var i = 0; i < allUsers.length; i++) {

            AddUser(chatHub, allUsers[i].ConnectionId, allUsers[i].Name, id);
        }
    }; // remove from active list if client disconnect

    chatHub.client.onUserDisconnected = function (id, name) {
        $('#' + id).remove();
        $('#active_' + id).remove();

        var ctrId = 'private_' + id;
        $('#' + ctrId).remove();
    }; //send message

    chatHub.client.createNewWindow = function (toUserId, fromName, message) {

        var windowId = 'private_' + toUserId;
        if ($('#' + windowId).length == 0) {
            createChatWindow(chatHub, toUserId, windowId, fromName);
            chatHub.server.sendMessage(true, toUserId, fromName, message);
        } else chatHub.server.sendMessage(false, toUserId, fromName, message);
    };

    chatHub.client.addMessage = function (toUserId, fromName, message, date) {

        var windowId = 'private_' + toUserId;

        $('#' + windowId).find('#divMessages').append('<div class="message"><strong>' + fromName + '</strong>(<i>' + date + '</i>): ' + message + '</div>');

        // set scrollbar
        var height = $('#' + windowId).find('#divMessages')[0].scrollHeight;
        $('#' + windowId).find('#divMessages').scrollTop(height);
    };
}

//add div with user to active users for notification and chat
function AddUser(chatHub, id, name, actualId) {
    var userChat = "";
    var userNotification = "";

    if (actualId != id) {
        userChat = $('<div style="height:20px;" id="' + id + '"><a style="cursor: pointer;">' + name + '</a></div>');
        userNotification = $('<div id="active_' + id + '"><input name="grupa" class="chk" type="checkbox" value="' + id + '"> ' + name + "<br/></div>");

        $(userChat).click(function () {
            if (actualId != id)
                OpenChatWindow(chatHub, id, name);
        });
    }

    $("#ActiveUsersChat").append(userChat);
    $("#ActiveUsersNotification").append(userNotification);
}

//create new window if click to user (if don't create)
function OpenChatWindow(chatHub, toUserId, name) {

    var windowId = 'private_' + toUserId;
    if ($('#' + windowId).length == 0) {
        createChatWindow(chatHub, toUserId, windowId, name);
        chatHub.server.getHistory(toUserId);
    }
}

//create window for chat between two person
function createChatWindow(chatHub, toUserId, windowId, name) {


    var div = '<div id="' + windowId + '" class="ui-widget-content draggable label label-warning" rel="0" style="z-index:' + (zIndex++) + '; position:absolute; left:' + leftPosition + 'px; top:' + topPosition + 'px;">' +
        '<div class="header">' +
        '<div  style="float:right;">' +
        '<img id="imgClose"  style="cursor:pointer;" src="/Content/images/close.png"/>' +
        '</div>' +
        '<span class="selText" rel="0">' + name + '</span>' +
        '</div>' +
        '<div id="divMessages" class="messageArea">' +
        '</div>' +
        '<div class="buttonBar">' +
        '<input id="txtMessage" class="msgText form-control" type="text"   />' +
        '<input id="btnSendMessage" class="submitButton button btn btn-warning" type="button" value="Wyslij"   />' +
        '</div>' +
        '</div>';

    var $div = $(div);

    leftPosition = leftPosition + 40;
    if (leftPosition > 600) leftPosition = 0;

    topPosition = topPosition + 25;


    // DELETE BUTTON IMAGE
    $div.find('#imgClose').click(function () {
        $('#' + windowId).remove();
    });

    // Send Button event
    $div.find('#btnSendMessage').click(function () {

        $textBox = $div.find('#txtMessage');
        var message = $textBox.val();
        if (message.length > 0) {

            chatHub.server.privateMessage(toUserId, message);
            $textBox.val('');
        }
    });

    // Text Box event
    $div.find('#txtMessage').keypress(function (e) {
        if (e.which == 13) {
            $div.find('#btnSendMessage').click();
        }
    });

    $('#divDraggable').prepend($div);

    $div.draggable({
        start: function (event, ui) {
            $(this).css("z-index", zIndex++);
        },
        handle: ".header",
        cursor: 'move',
        opacity: 0.65,
        stack: $div,
        stop: function () {
        }
    });

    $div.click(function () {
        $(this).addClass('top').removeClass('bottom');
        $(this).siblings().removeClass('top').addClass('bottom');
        $(this).css("z-index", zIndex++);

    });
}
