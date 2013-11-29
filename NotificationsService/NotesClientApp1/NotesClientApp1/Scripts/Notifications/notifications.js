var noteHub;
var connection;

function addUserToNotes(noteName, noteId, noteUrl)
{
    sessionStorage.setItem("name", noteName);
    sessionStorage.setItem("id", noteId);

    connection = $.hubConnection(noteUrl);
    noteHub = connection.createHubProxy('NotificationsHub');
    addClientMethods(noteHub);

    connection.start({ jsonp: true }).done(function () {
        noteHub.invoke("Connect", sessionStorage.getItem("name"), sessionStorage.getItem("id"));
    });   
}

function addClientMethods(noteHub) {
 
    noteHub.on('onConnected', function (allUsers) {

        $("#ActiveUsersNotifications").empty();
        
        for (var i = 0; i < allUsers.length; i++)
            if (sessionStorage.getItem("id") != null) {
                AddUser(chatHub, allUsers[i].EmployeeId, allUsers[i].Name);
                OnlineUsers(allUsers.length-1);
            }   
    });

    noteHub.on('onUserDisconnected', function (userId, allUsers) {

        $("#ActiveUsersNotifications").empty();

        for (var i = 0; i < allUsers.length; i++)
            if (sessionStorage.getItem("id") != userId && sessionStorage.getItem("name") != null) {
                AddUser(noteHub, allUsers[i].EmployeeId, allUsers[i].Name);
                OnlineUsers(allUsers.length-1);
            }

        var height = $('#' + ctrId).find('#divMessages')[0].scrollHeight;
        $('#' + ctrId).find('#divMessages').scrollTop(height);;
    });

    noteHub.on('clearHistoryOfSendNotifications', function () {
        $('#sendNotifications').empty();
    });
    noteHub.on('getReceivedNotifications', function (date, sender, content) {
        $('#receivedNotifications').append('<li><strong>' + date + ', nadawca: ' + sender + '</strong><br/>' + content + '</li>');
    });
    noteHub.on('getSendNotifications', function (date, receivers, content) {
        $('#sendNotifications').append('<li><strong>' + date + ', odbiorcy: ' + receivers + '</strong><br/>' + content + '</li>');
    });
    noteHub.on('addReceivedNotification', function (date, sender, content) {

        $('#receivedNotifications').prepend('<li><strong>' + date + ', nadawca: ' + sender + '</strong><br/>' + content + '</li>');
    });
    noteHub.on('addSendNotification', function (date, receivers, content) {

        $('#sendNotifications').prepend('<li><strong>' + date + ', odbiorcy: ' + receivers + ' (nie odczytano)</strong><br/>' + content + '</li>');

    });
    noteHub.on('sendNotificationBroadcast', function (notificationId, notification, name, senderId){
        if (navigator.userAgent.indexOf("Chrome") > -1) {
            if (window.webkitNotifications.checkPermission() == 0) {
                var note = window.webkitNotifications.createNotification(null, "Nowe powiadomienie od: " + name, notification);

                note.onclick = function () {
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
            ).click(function () {
                chatHub.server.addTimeofReading(notificationId, senderId);
            });
        }
    });
    noteHub.on('notificationConfirm', function () {
        alert("Powiadomienie zostalo wyslane");
        $("#txtNotification").val('');
    });
}

function AddUser(noteHub, userId, name) {
    var userOfNotifications = "";
    if (sessionStorage.getItem("id") != userId)
    {
        userOfNotifications = $('<div class="noteUserLink" id="' + userId + '"><span class="chat-icon chat-green-icon chat-icon-bullet"></span><a>' + name + '</a></div>');
        if (showList) $("#ActiveUsersNotifications").show();

        $(userOfNotifications).click(function () {
            if (sessionStorage.getItem("id") != userId) {
                OpenChatWindow(noteHub, userId, name);
                $('#private_' + userId).click();
            }
        });
    }
    $("#ActiveUsersNotifications").append(userOfNotifications);
}

function OnlineUsers(count) {

    if (sessionStorage.getItem("name") != null)
        $("#UsersOfNotificationsCounter").text('online(' + count + ')');

    if (count == 0) {
        $("#UsersOfNotificationsCounter").text('online(0)');
    }
};

function CreateControlPanelForNotifications()
{
    //cos do wyswietlania aktywnych uzytkownikow z mozliwoscia wyboru do kogo wyslac wiadomosc oraz okienko z miejscem na wpisanie wiadomosci, z historia wyslanych i otrzymanych powiadomien
};
