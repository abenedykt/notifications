$(function () {

    var div = '<div id="divForDraggableWindows"></div>' +
            '<div class="chat-div" >' +
                '<button class="chat-btn" type="button" onClick="show()">' +
                '<span id="triangleIcon" class="chat-icon chat-white-icon chat-icon-triangle"></span> <span class="chat-btn-text">czat</span> <span class="chat-btn-counter" id="counter" />' +
                '</button>' +
                '<ul class="chat-label" id="ActiveUsersChat"></ul>' +
            '</div>';

    var $div = $(div);

    $("body").prepend('<div id="divForDraggableWindows"></div>');
    $('#chat').prepend(div);
    $("#ActiveUsersChat").hide();
    $("#counter").text('online(0)');
});

var showList = false;
var zIndex = 1000000;
var leftPosition = 0;
var topPosition = 100;
var chatHub;
var connection;
var countUser = 0;

function show() {
    if (showList == false) {
        if (countUser > 0)
            $("#ActiveUsersChat").show();
        showList = true;
        $("#triangleIcon").css("background-position", "-64px -16px");

    } else {
        $("#ActiveUsersChat").hide();
        showList = false;
        $("#triangleIcon").css("background-position", "0px -16px");
    }
}

function addToChat(chatName, chatId, chatUrl) {
    sessionStorage.setItem("name", chatName);
    sessionStorage.setItem("id", chatId);

    connection = $.hubConnection(chatUrl);
    chatHub = connection.createHubProxy('ChatHub');
    addClientMethods(chatHub);

    connection.start({ jsonp: true }).done(function () {
        chatHub.invoke("Connect", sessionStorage.getItem("name"), sessionStorage.getItem("id"));
    });
}

function addClientMethods(chatHub) {

    chatHub.on('onConnected', function (allUsers) {

        $("#ActiveUsersChat").empty();

        for (var i = 0; i < allUsers.length; i++)
            if (sessionStorage.getItem("id") != null) {
                AddUser(chatHub, allUsers[i].EmployeeId, allUsers[i].Name);
                OnlineUsers(allUsers.length - 1);
            }
    });

    chatHub.on('onUserDisconnected', function (userId, allUsers) {

        $("#ActiveUsersChat").empty();

        for (var i = 0; i < allUsers.length; i++)
            if (sessionStorage.getItem("id") != userId && sessionStorage.getItem("name") != null) {
                AddUser(chatHub, allUsers[i].EmployeeId, allUsers[i].Name);
                OnlineUsers(allUsers.length - 1);
            }

        var ctrId = 'private_' + userId;
        $('#' + ctrId).find('#divMessages').append('<div class="chat-messages-area-message"><span class="chat-messages-area-logout">Użytkownik wylogował się!</span></div>')

        var height = $('#' + ctrId).find('#divMessages')[0].scrollHeight;
        $('#' + ctrId).find('#divMessages').scrollTop(height);;
    });

    chatHub.on('createNewWindow', function (userId, fromName, message) {

        var windowId = 'private_' + userId;
        if ($('#' + windowId).length == 0) {
            createChatWindow(chatHub, userId, windowId, fromName);
            chatHub.invoke('sendMessage', true, userId, fromName, message);
        }
        else
            chatHub.invoke('sendMessage', false, userId, fromName, message);
    });

    chatHub.on('addMessage', function (saving, userId, userName, fromName, message, date) {

        var windowId = 'private_' + userId;

        if ($('#' + windowId).length == 0) {
            createChatWindow(chatHub, userId, windowId, userName);
            if (saving) chatHub.invoke('getHistory', userId);
        }

        $('#' + windowId).find('#divMessages').append('<div class="chat-messages-area-message"><span class="chat-messages-area-span1">' + date + ' ' + fromName + '</span> <span class="chat-messages-area-span2">' + message + '</span></div>');

        var height = $('#' + windowId).find('#divMessages')[0].scrollHeight;
        $('#' + windowId).find('#divMessages').scrollTop(height);
    });
}

function OnlineUsers(count) {
    countUser = count;
    if (sessionStorage.getItem("name") != null)
        $("#counter").text('online(' + count + ')');

    if (count == 0) {
        $("#ActiveUsersChat").hide();
        $("#counter").text('online(0)');
        showList = false;
    }
};

function AddUser(chatHub, userId, name) {
    var userChat = "";
    if (sessionStorage.getItem("id") != userId) {
        userChat = $('<div class="userLink" id="' + userId + '"><span class="chat-icon chat-green-icon chat-icon-bullet"></span><a>' + name + '</a></div>');
        if (showList) $("#ActiveUsersChat").show();

        $(userChat).click(function () {
            if (sessionStorage.getItem("id") != userId) {
                OpenChatWindow(chatHub, userId, name);
                $('#private_' + userId).click();
            }
        });
    }
    $("#ActiveUsersChat").append(userChat);
}

function OpenChatWindow(chatHub, toUserId, name) {

    var windowId = 'private_' + toUserId;
    if ($('#' + windowId).length == 0) {
        createChatWindow(chatHub, toUserId, windowId, name);
        $('#' + windowId).find('#chatHeader').addClass('chat-header-top').removeClass('chat-header-bottom');
        $('#' + windowId).siblings().find('#chatHeader').removeClass('chat-header-top').addClass('chat-header-bottom');
        $('#' + windowId).find('#txtMessage').focus();
        chatHub.invoke('getHistory', toUserId);
    }
}

function createChatWindow(chatHub, toUserId, windowId, name) {

    var div = '<div id="' + windowId + '" class="chat-window">' +
                    '<div id="chatHeader" class="chat-header chat-header-bottom">' +
                        '<div id="imgClose" class="chat-header-close" > <span class="chat-icon chat-red-icon chat-icon-iks"></span> </div>' +
                        '<div class="chat-header-name"> <span class="chat-header-name-span1">Rozmowa</span><span class="chat-header-name-span2"> ' + name + '</span> </div>' +
                    '</div>' +
                    '<div id="divMessages" class="chat-messages-area"> </div>' +
                    '<div class="chat-message"><textarea id="txtMessage" type="text"/> </div>' +
                    '<div class="chat-send"><input id="btnSendMessage" type="button" class="chat-send-btn" value="Wyślij"/> </div>' +
                '</div>';

    var $div = $(div);

    $div.css("left", leftPosition);
    $div.css("top", topPosition);
    $div.css("z-index", zIndex++);

    leftPosition = leftPosition + 40;
    if (leftPosition > 600) leftPosition = 0;

    topPosition = topPosition + 25;
    if (topPosition > 600) topPosition = 0;

    $div.find('#imgClose').click(function () {
        $('#' + windowId).remove();
    });

    $div.find('#btnSendMessage').click(function () {
        $textBox = $div.find('#txtMessage');
        var message = $textBox.val();

        var tag = new RegExp('<[a-zA-Z/]{1,15}.*?>', 'g');
        clearMessage = message.replace(tag, '');

        if (clearMessage.length > 0) {
            chatHub.invoke('sendMessage', toUserId, clearMessage);
            $textBox.val('');
        }
    });

    $div.find('#txtMessage').keypress(function (e) {
        if (e.which == 13) {
            $div.find('#btnSendMessage').click();
            e.preventDefault();
        }
    });



    $("#divForDraggableWindows").prepend($div);


    $div.draggable({
        start: function (event, ui) {
            $(this).css("z-index", zIndex++);
        },
        handle: ".chat-header",
        cursor: 'move',
        opacity: 0.8,
        stack: $div,
        stop: function () {
        }
    });

    $('#' + windowId).click(function () {
        $('#' + windowId).addClass('chat-window-top').removeClass('chat-window-bottom');
        $('#' + windowId).siblings().removeClass('chat-window-top').addClass('chat-window-bottom');
        $('#' + windowId).css("z-index", zIndex++);

        $('#' + windowId).find('#chatHeader').addClass('chat-header-top').removeClass('chat-header-bottom');
        $('#' + windowId).siblings().find('#chatHeader').removeClass('chat-header-top').addClass('chat-header-bottom');
    });
}
