$(function () {

    //div z chatem 
    var div = '<div class="chat-div" >' +
           '<button class="chat-btn" type="button" onClick="show()">' +
           '<span id="triangleIcon" class="chat-icon chat-white-icon chat-icon-triangle"></span><span class="chat-btn-text">czat</span><span class="chat-btn-counter" id="counter" />' +
           '</button>' +
           '<ul class="chat-label" id="ActiveUsersChat"></ul>' +
           '</div>';

    var $div = $(div);
  
    $('#chat').prepend(div);
    $("#ActiveUsersChat").hide();

    connection = $.hubConnection('http://localhost:59537/');
    chatHub = connection.createHubProxy('ChatHub');
  
    addClientMethods(chatHub);
    
    connection.start().done(function ()
    {
        if (sessionStorage.getItem("name") != null) 
        {
            chatHub.invoke("Connect", sessionStorage.getItem("name"), sessionStorage.getItem("id"));
        }
        else {
            $("#counter").text('online(0)');
        }
    });
    
});

var showList= false;
var zIndex = 0;
var leftPosition = 0;
var topPosition = 100;
var chatHub;
var connection;

function show() {
    if (showList == false) {
        $("#ActiveUsersChat").show();
        showList = true;
        $("#triangleIcon").css("background-position", "0px -16px");

    } else {
        $("#ActiveUsersChat").hide();
        showList = false;
        $("#triangleIcon").css("background-position", "-64px -16px");
    }
}

function addToChat()
{
    sessionStorage.setItem("name", $('#chatName').val());
    sessionStorage.setItem("id", $('#chatId').val());
    
    connection.start().done(function () {
        chatHub.invoke("Connect", sessionStorage.getItem("name"), sessionStorage.getItem("id"));
    });   
}

function addClientMethods(chatHub) {
 
    chatHub.on('onlineUsers', function (count) {

        if (sessionStorage.getItem("name") != null)
            $("#counter").text('online(' + count + ')');

        if (count == 0) {
            $("#ActiveUsersChat").hide();
            showList = false;
        }
    });
     
    chatHub.on('onNewUserConnected', function (userId, name) {

        if (sessionStorage.getItem("name") != null)
            AddUser(chatHub, userId, name, 0);
    });

    
    chatHub.on('onConnected', function (userId, name, allUsers) {

        $("#ActiveUsersChat").clear;
        for (var i = 0; i < allUsers.length; i++)
            AddUser(chatHub, allUsers[i].EmployeeId, allUsers[i].Name, userId);
    });

    chatHub.on('onUserDisconnected', function (id, name) {
        $('#' + id).remove();

        var ctrId = 'private_' + id;
        $('#' + ctrId).find('#divMessages').append('<div class="message"><strong>Użytkownik wylogował się!</strong></div>')

        var height = $('#' + ctrId).find('#divMessages')[0].scrollHeight;
        $('#' + ctrId).find('#divMessages').scrollTop(height);;
    });

    chatHub.on( 'createNewWindow', function (userId, fromName, message) {

        var windowId = 'private_' + userId;
        if ($('#' + windowId).length == 0) {
            createChatWindow(chatHub, userId, windowId, fromName);
            chatHub.invoke('sendMessage',true, userId, fromName, message);
        } else chatHub.invoke('sendMessage',false, userId, fromName, message);
    });


    chatHub.on('addMessage', function (userId, fromName, message, date) {

        var windowId = 'private_' + userId;

        $('#' + windowId).find('#divMessages').append('<div class="chat-messages-area-message"><span class="chat-messages-area-span1">' + fromName + '(' + date + ')</span> <span class="chat-messages-area-span2">' + message + '</span></div>');

        var height = $('#' + windowId).find('#divMessages')[0].scrollHeight;
        $('#' + windowId).find('#divMessages').scrollTop(height);
    });

}

function AddUser(chatHub, userId, name, actualUserId) {
    var userChat = "";
    if (actualUserId != userId)
    {
        userChat = $('<div class="userLink" id="' + userId + '"><span class="chat-icon chat-green-icon chat-icon-bullet"></span><a>' + name + '</a></div>');

        $(userChat).click(function () {
            if (actualUserId != userId) {
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
        chatHub.invoke('getHistory', toUserId);
    }
}

function createChatWindow(chatHub, toUserId, windowId, name) {

    var div = '<div id="' + windowId + '" class="chat-window">' +
            '<div id="chatHeader" class="chat-header chat-header-bottom">' +
             '<div id="imgClose" class="chat-header-close" ><span class="chat-icon chat-red-icon chat-icon-iks"></span></div>' +
                '<div class="chat-header-name"><span class="chat-header-name-span1">Rozmowa </span><span class="chat-header-name-span2"> ' + name + '</span></div>' +
               
            '</div>' +
            '<div id="divMessages" class="chat-messages-area">' +
            '</div>' +
            '<div class="chat-message"><textarea id="txtMessage" type="text" rows="4"/></div>' +
            '<div class="chat-send" ><input id="btnSendMessage" type="button" class="chat-send-btn" value="Wyślij"   /> </div>' +
        '</div>';

    var $div = $(div);

    $div.css("left", leftPosition);
    $div.css("top", topPosition);
    $div.css("z-index", ++zIndex);

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
        if (message.length > 0) {

            chatHub.invoke('privateMessage', toUserId, message);
            $textBox.val('');
        }
    });

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

    $('#'+windowId).click(function () {
        $(this).addClass('chat-window-top').removeClass('chat-window-bottom');
        $(this).siblings().removeClass('chat-window-top').addClass('chat-window-bottom');
        $(this).css("z-index", zIndex++);

        $('#' + windowId).find('#chatHeader').addClass('chat-header-top').removeClass('chat-header-bottom');
        $('#' + windowId).siblings().find('#chatHeader').removeClass('chat-header-top').addClass('chat-header-bottom');
    });
}
