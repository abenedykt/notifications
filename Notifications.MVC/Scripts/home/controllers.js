function LogCtrl($scope) {
 
   $scope.login = function () {
       userName = $scope.user.name;
    }
}

var userName;
var ActiveUsersCount;

function NoticeCtrl($scope) {

    $scope.showChatList =false;  
    $scope.name = userName;
 
    var chatHub = $.connection.chatHub;

    registerClientMethods(chatHub);

    $.connection.hub.start().done(function () {

        chatHub.server.connect(userName);    

        $scope.sendNotification = function () {
            var users = [];

            $(".chk:checked").each(function () {
                users.push($(this).val());
            });

            var message = $("#txtNotification").val();

            chatHub.server.broadcasting(users, message, userName);           
        }  
    });  
}


    
function registerClientMethods(chatHub) {
   
    chatHub.client.onlineUsers = function (count) {

        $("#paragraph").text('Chat('+ count +')');
    };

    chatHub.client.sendNotificationBroadcast = function (notification, name) {
        if (navigator.userAgent.indexOf("Chrome") > -1) {
            if (window.webkitNotifications.checkPermission() == 0) {
                window.webkitNotifications.createNotification(null, "Nowe powiadomienie od: " + name, notification).show();
            }
            else {
                window.webkitNotifications.requestPermission();
            }
        }
        else {
            $.pnotify({
                title: "Nowe powiadomienie od: " + name,
                text: notification,
                hide: false,
                sticker: false
            });
        }

    }

    // send to all except caller client
    chatHub.client.onNewUserConnected = function (id, name) {
        AddUser(chatHub, id, name)
    }

    // send list of active person to caller
    chatHub.client.onConnected = function (id, name, allUsers) {


        for (i = 0; i < allUsers.length; i++) {

            AddUser(chatHub, allUsers[i].ConnectionId, allUsers[i].UserName, id);
        }
    }

    // remove from active list if client disconnect
    chatHub.client.onUserDisconnected = function (id, name) {
        $('#' + id).remove();
        $('#active_' + id).remove();

        var ctrId = 'private_' + id;
        $('#' + ctrId).remove();
    }

    chatHub.client.sendPrivateMessage = function (toUserId, fromName, message) {

        var windowId = 'private_' + toUserId;
        if ($('#' + windowId).length == 0) createPrivateChatWindow(chatHub, toUserId, windowId, fromName);
        var time = new Date();
        $('#' + windowId).find('#divMessage').append('<div><strong>' + fromName + '</strong>(' + time.toLocaleTimeString() + '): ' + message + '</div>');

        // set scrollbar
        var height = $('#' + windowId).find('#divMessage')[0].scrollHeight;
        $('#' + windowId).find('#divMessage').scrollTop(height);
    }
}
   
    function AddUser(chatHub, id, name, actualId) {
        var userChat = "";
        var userNotification = "";

        if (actualId != id) {
            userChat = $('<div style="height:20px;" id="' + id + '"><a style="cursor: pointer;">' + name + '</a></div>');
            userNotification = $('<div id="active_' + id + '"><input name="grupa" class="chk" type="checkbox" value="' + id + '"> ' + name + "<br/></div>");

            $(userChat).click(function () {
                if (actualId != id)
                OpenPrivateChatWindow(chatHub, id, name);
            });
        }

        $("#ActiveUsersChat").append(userChat);
        $("#ActiveUsersNotification").append(userNotification);
    }

    function OpenPrivateChatWindow(chatHub, id, name) {

        var windowId = 'private_' + id;
        if ($('#' + windowId).length > 0) return;
        createPrivateChatWindow(chatHub, id, windowId, name);

    }

    function createPrivateChatWindow(chatHub, id, windowId, name) {

        var div = '<div id="' + windowId + '" class="ui-widget-content draggable" rel="0">' +
                   '<div class="header">' +
                      '<div  style="float:right;">' +
                          '<img id="imgDelete"  style="cursor:pointer;" src="/Content/jqueryUI/delete.png"/>' +
                       '</div>' +

                       '<span class="selText" rel="0">' + name + '</span>' +
                   '</div>' +
                   '<div id="divMessage" class="messageArea">' +

                   '</div>' +
                   '<div class="buttonBar">' +
                      '<input id="txtPrivateMessage" class="msgText" type="text"   />' +
                      '<input id="btnSendMessage" class="submitButton button" type="button" value="Send"   />' +
                   '</div>' +
                '</div>';

        var $div = $(div);

        // DELETE BUTTON IMAGE
        $div.find('#imgDelete').click(function () {
            $('#' + windowId).remove();
        });

        // Send Button event
        $div.find("#btnSendMessage").click(function () {

            $textBox = $div.find("#txtPrivateMessage");
            var msg = $textBox.val();
            if (msg.length > 0) {

                chatHub.server.sendPrivateMessage(id, msg);
                $textBox.val('');
            }
        });

        // Text Box event
        $div.find("#txtPrivateMessage").keypress(function (e) {
            if (e.which == 13) {
                $div.find("#btnSendMessage").click();
            }
        });

        $('#divDraggable').prepend($div);
        $div.draggable({

            handle: ".header",
            stop: function () {
            }
        });

    }

    
       

    
  




    
