
$(function () {

    createChat();
    
    var chatHub = $.connection.chatHub;

    addClientMethods(chatHub);

   // $.connection.hub.start().done(function() {

   //     chatHub.server.connect(userName, userId);
   //});
});




function createChat() {
    
    var div = '<div style="width: 108px">' +
        '<ul class="label label-warning" id="ActiveUsersChat" ng-show="showChatList" style="background-color: #ccf285; margin: 0px; padding-bottom: 10px; padding-top: 10px; width: 100px;"></ul>' +
        '<button class="btn btn-success" type="button" style="height: 30px; vertical-align: central; width: 108px;" onClick="showChatList= !showChatList">' +
        '<p id="counter" /> ' +
        '</button>' + '</div>';

    var $div = $(div);

    $('#chat').prepend(div);

    var showChatList = false;

    $("#counter").text("Chat(0)");

}

function addClientMethods(chatHub) {

    //count of active users
    chatHub.client.onlineUsers = function (count) {

        $("#counter").text('Chat(' + count + ')');
    };

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
        var ctrId = 'private_' + id;
        $('#' + ctrId).remove();
    }; 

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

    if (actualId != id) {
        userChat = $('<div style="height:20px;" id="' + id + '"><a style="cursor: pointer;">' + name + '</a></div>');
  
        $(userChat).click(function () {
            if (actualId != id)
                OpenChatWindow(chatHub, id, name);
        });
    }

    $("#ActiveUsersChat").append(userChat);
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
