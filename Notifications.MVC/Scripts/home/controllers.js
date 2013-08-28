function LogCtrl($scope) {
 
   $scope.login = function () {
       userName = $scope.user.name;
       userId = $scope.user.id;
    }
}

var userName;
var ActiveUsersCount;
var userId;

function NoticeCtrl($scope) {

    $scope.showChatList =false;  
    $scope.name = userName;
 
    var chatHub = $.connection.chatHub;

    registerClientMethods(chatHub);

    $.connection.hub.start().done(function () {

        chatHub.server.connect(userName, userId);

        $scope.sendNotification = function () {
            var receivers = [];

            $(".chk:checked").each(function () {
                receivers.push($(this).val());
            });

            var message = $("#txtNotification").val();

            chatHub.server.broadcasting(receivers, message, userName);
        }  
    });  
}

    
function registerClientMethods(chatHub) {

    chatHub.client.getReceivedNotifications = function (date, sender, content) {

            $('#receivedNotifications').append('<li><strong>' + date + ' r., nadawca: ' + sender + '</strong><br/>' + content +'</li>')
        };

    chatHub.client.getSendNotifications = function (date, receivers, content) {

        $('#sendNotifications').append('<li><strong>' + date + ' r., odbiorcy: ' + receivers + '</strong><br/>' + content + '</li>')
    };


    //count of active users
    chatHub.client.onlineUsers = function (count) {

        $("#counter").text('Chat('+ count +')');
    };

    //send notification 
    chatHub.client.sendNotificationBroadcast = function (notification, name) {
        var notification;
        if (navigator.userAgent.indexOf("Chrome") > -1) {
            if (window.webkitNotifications.checkPermission() == 0) {
                notification = window.webkitNotifications.createNotification(null, "Nowe powiadomienie od: " + name, notification);
                notification.show();
                //notification.onclick = function () {
                //    window.focus();
                //    this.cancel();
                //};
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
            //$.pnotify.onclick = function () {
            //    window.focus();
            //    this.cancel();
            //};
        }
    }


    chatHub.client.confirmation = function(){
        alert("Potwierdzenie zostalo wyslane");
        $("#txtNotification").val('');

    }

    // send to all except caller client
    chatHub.client.onNewUserConnected = function (id, name) {
        AddUser(chatHub, id, name)
    }

    // send list of active person to caller
    chatHub.client.onConnected = function (id, name, allUsers) {

        for (i = 0; i < allUsers.length; i++) {

            AddUser(chatHub, allUsers[i].ConnectionId, allUsers[i].Name, id);
        }
    }

    // remove from active list if client disconnect
    chatHub.client.onUserDisconnected = function (id, name) {
        $('#' + id).remove();
        $('#active_' + id).remove();

        var ctrId = 'private_' + id;
        $('#' + ctrId).remove();
    }


    //send message
    chatHub.client.sendPrivateMessage = function (toUserId, fromName, message, time) {

        var windowId = 'private_' + toUserId;
        if ($('#' + windowId).length == 0) createPrivateChatWindow(chatHub, toUserId, windowId, fromName);
        
        $('#' + windowId).find('#divMessage').append('<div class="message"><strong>' + fromName + '</strong>(' + time + '): ' + message + '</div>');

        // set scrollbar
        var height = $('#' + windowId).find('#divMessage')[0].scrollHeight;
        $('#' + windowId).find('#divMessage').scrollTop(height);
    }
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
                OpenPrivateChatWindow(chatHub, id, name);
            });
        }

        $("#ActiveUsersChat").append(userChat);
        $("#ActiveUsersNotification").append(userNotification);
    }


//create new window if don't create
    function OpenPrivateChatWindow(chatHub, id, name) {

        var windowId = 'private_' + id;
        if ($('#' + windowId).length > 0) return;
        createPrivateChatWindow(chatHub, id, windowId, name);
    }

//create window for chat between two person
    function createPrivateChatWindow(chatHub, id, windowId, name) {

        var div = '<div id="' + windowId + '" class="ui-widget-content draggable label label-warning" rel="0">' +
                   '<div class="header">' +
                      '<div  style="float:right;">' +
                          '<img id="imgDelete"  style="cursor:pointer;" src="/Content/jqueryUI/delete.png"/>' +
                       '</div>' +
                       '<span class="selText" rel="0">' + name + '</span>' +
                   '</div>' +
                   '<div id="divMessage" class="messageArea">' +
                   '</div>' +
                   '<div class="buttonBar">' +
                      '<input id="txtPrivateMessage" class="msgText form-control" type="text"   />' +
                      '<input id="btnSendMessage" class="submitButton button btn btn-warning" type="button" value="Wyslij"   />' +
                   '</div>' +
                '</div>';

        chatHub.server.getMessages(id);

        
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

    
       

    
  




    
