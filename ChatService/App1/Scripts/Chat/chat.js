$(function () {
    //div z chatem 
    var div = '<div class="chat-div" >' +
           '<ul class="chat-label" id="ActiveUsersChat" ></ul>' +
           '<button class="chat-btn" type="button" onClick="show()">' +
           '<p id="counter" /> ' +
           '</button>' + '</div>';

    var $div = $(div);
   
    $('#chat').prepend(div);
    $("#ActiveUsersChat").hide();
    
    chatHub = $.connection.chatHub;

    addClientMethods(chatHub);
    
    $.connection.hub.start().done(function ()
    {
        //jeśli w zmiennej sesyjnej znajduje się już wartość (uzytkownik juz sie logował) wykonaj reconnect
        if (sessionStorage.getItem("name") != null) 
        {
            chatHub.server.connect(sessionStorage.getItem("name"), sessionStorage.getItem("id"));
        }
        else {
            $("#counter").text('Chat(0)');
        }
    });
    
});

var showList= false;
var zIndex = 0;
var leftPosition = 0;
var topPosition = 100;
var chatHub;

function show() {
    if (showList == false) {
        $("#ActiveUsersChat").show();
        showList = true;
    } else {
        $("#ActiveUsersChat").hide();
        showList =false;      
    }
}

function addToChat()
{
    //dodanie zmiennych sesyjnych po zalogowaniu
    sessionStorage.setItem("name", $('#chatName').val());
    sessionStorage.setItem("id", $('#chatId').val());
    
    $.connection.hub.start().done(function () {
        chatHub.server.connect(sessionStorage.getItem("name"), sessionStorage.getItem("id"));
    });   
}

function addClientMethods(chatHub) {
 
    //licznik aktywnych uzytkowników(na przycisku z chatem)
    chatHub.client.onlineUsers = function (count) {

        if (sessionStorage.getItem("name") != null)
            $("#counter").text('Chat(' + count + ')');

        if (count == 0) {
            $("#ActiveUsersChat").hide();
            showList = false;
        }
            
    };

    //dodanie logującego się użytkownika do wyświetlanej listy aktywnych u innych użytkowników
    chatHub.client.onNewUserConnected = function (userId, name) {

        if (sessionStorage.getItem("name") != null)
        AddUser(chatHub, userId, name, 0);
    }; 

    //dodanie wszystkich zalogowanych użytkowników do wyświetlanej listy aktywnych u logującego się użytkownika
    chatHub.client.onConnected = function (userId, allUsers) {
        
        $("#ActiveUsersChat").clear;
        for (var i = 0; i < allUsers.length; i++) 
            AddUser(chatHub, allUsers[i].EmployeeId, allUsers[i].Name, userId);    
    }; 

    chatHub.client.onUserDisconnected = function (id, name) {
        $('#' + id).remove();

        var ctrId = 'private_' + id;
        $('#' + ctrId).find('#divMessages').append('<div class="message"><strong>Użytkownik wylogował się!</strong></div>')

        var height = $('#' + ctrId).find('#divMessages')[0].scrollHeight;
        $('#' + ctrId).find('#divMessages').scrollTop(height);;
    };

    //stworzenie nowego okna rozmowy u odbiorcy wiadomości
    chatHub.client.createNewWindow = function (userId, fromName, message) {

        var windowId = 'private_' + userId;
        if ($('#' + windowId).length == 0) {
            createChatWindow(chatHub, userId, windowId, fromName);
            chatHub.server.sendMessage(true, userId, fromName, message);
        } else chatHub.server.sendMessage(false, userId, fromName, message);
    };

    chatHub.client.addMessage = function (userId, fromName, message, date) {

        var windowId = 'private_' + userId;

        $('#' + windowId).find('#divMessages').append('<div class="message"><strong>' + fromName + '</strong>(<i>' + date + '</i>): ' + message + '</div>');

        var height = $('#' + windowId).find('#divMessages')[0].scrollHeight;
        $('#' + windowId).find('#divMessages').scrollTop(height);
    };
}

//dodanie uzytkownika do wyswietlanej listy aktywnych
function AddUser(chatHub, userId, name, actualUserId) {
    var userChat = "";

    if (actualUserId != userId)
    {
        userChat = $('<div style="height:20px;" id="' + userId + '"><a style="cursor: pointer;">' + name + '</a></div>');

        $(userChat).click(function () {
            if (actualUserId != userId)
                OpenChatWindow(chatHub, userId, name);
        });
    }

    $("#ActiveUsersChat").append(userChat);
}

//stworzenie okna rozmowy po kliknięciu na osobę na liście aktywnych
function OpenChatWindow(chatHub, toUserId, name) {

    var windowId = 'private_' + toUserId;
    if ($('#' + windowId).length == 0) {
        createChatWindow(chatHub, toUserId, windowId, name);
       // chatHub.server.getHistory(toUserId);
    }
}

function createChatWindow(chatHub, toUserId, windowId, name) {

    var div = '<div id="' + windowId + '" class="ui-widget-content draggable" rel="0" style="z-index:' + (zIndex++) + '; position:absolute; left:' + leftPosition + 'px; top:' + topPosition + 'px;">' +
        '<div class="header">' +
        '<div  style="float:right;">' +
        '<img id="imgClose"  style="cursor:pointer;" src="/Content/Chat/images/closeChat.png"/>' +
        '</div>' +
        '<span rel="0">' + name + '</span>' +
        '</div>' +
        '<div id="divMessages" class="messageArea">' +
        '</div>' +
        '<div class="buttonBar">' +
        '<input id="txtMessage" class="msgText" type="text"   />' +
        '<input id="btnSendMessage" class="button" type="button" value="Wyslij"   />' +
        '</div>' +
        '</div>';

    var $div = $(div);

    leftPosition = leftPosition + 40;
    if (leftPosition > 600) leftPosition = 0;

    topPosition = topPosition + 25;

    $div.find('#imgClose').click(function () {
        $('#' + windowId).remove();
    });

    $div.find('#btnSendMessage').click(function () {

        $textBox = $div.find('#txtMessage');
        var message = $textBox.val();
        if (message.length > 0) {

            chatHub.server.privateMessage(toUserId, message);
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

    $div.click(function () {
        $(this).addClass('top').removeClass('bottom');
        $(this).siblings().removeClass('top').addClass('bottom');
        $(this).css("z-index", zIndex++);

    });
}
