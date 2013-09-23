function addToNotes() {

    var userName = $('#notesName').val();
    var userId = $('#notesId').val();

    var noteHub = $.connection.noteHub;

    addClientMethods(noteHub);

    $.connection.hub.start().done(function () {
        
        noteHub.server.connect(userName, userId);
        
        $('#sendNotification').click = function () {
            var receivers = [];

            $(".chk:checked").each(function () {
                receivers.push($(this).val());
            });

            var message = $("#txtNotification").val();

            if (receivers.length != 0)
                noteHub.server.broadcasting(receivers, message, userName);
            else
                alert("Zaznacz przynajmniej jedna osobe!");
        };

    });
}

function addClientMethods(noteHub) {

    noteHub.client.clearHistoryOfSendNotifications = function () {
        $('#sendNotifications').empty();
    };

    noteHub.client.getReceivedNotifications = function (date, sender, content) {

        $('#receivedNotifications').append('<li><strong>' + date + ', nadawca: ' + sender + '</strong><br/>' + content + '</li>');
    };

    noteHub.client.getSendNotifications = function (date, receivers, content) {

        $('#sendNotifications').append('<li><strong>' + date + ', odbiorcy: ' + receivers + '</strong><br/>' + content + '</li>');
    };

    noteHub.client.addReceivedNotification = function (date, sender, content) {

        $('#receivedNotifications').prepend('<li><strong>' + date + ', nadawca: ' + sender + '</strong><br/>' + content + '</li>');
    };

    noteHub.client.addSendNotification = function (date, receivers, content) {

        $('#sendNotifications').prepend('<li><strong>' + date + ', odbiorcy: ' + receivers + ' (nie odczytano)</strong><br/>' + content + '</li>');
    };

   //send notification 
    noteHub.client.sendNotificationBroadcast = function (notificationId, notification, name, senderId) {

        if (navigator.userAgent.indexOf("Chrome") > -1) {
            if (window.webkitNotifications.checkPermission() == 0) {
                var note = window.webkitNotifications.createNotification(null, "Nowe powiadomienie od: " + name, notification);

                note.onclick = function () {
                    noteHub.server.addTimeofReading(notificationId, senderId);
                };
                note.onclose = function () {
                    noteHub.server.addTimeofReading(notificationId, senderId);
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
                noteHub.server.addTimeofReading(notificationId, senderId);
            });
        }
    };

    noteHub.client.notificationConfirm = function () {
        alert("Powiadomienie zostalo wyslane");
        $("#txtNotification").val('');
    }; // send to all except caller client

    noteHub.client.onNewUserConnected = function (id, name) {
        AddUser(noteHub, id, name);
    }; // send list of active person to caller

    noteHub.client.onConnected = function (id, name, allUsers) {

        for (var i = 0; i < allUsers.length; i++) {

            AddUser(noteHub, allUsers[i].ConnectionId, allUsers[i].Name, id);
        }
    }; // remove from active list if client disconnect

    noteHub.client.onUserDisconnected = function (id, name) {
       $('#active_' + id).remove(); 
    };
}

//add div with user to active users for notification and chat
function AddUser(noteHub, id, name, actualId) {
    var userNotification = "";
    if (actualId != id) {
       userNotification = $('<div id="active_' + id + '"><input name="grupa" class="chk" type="checkbox" value="' + id + '"> ' + name + "<br/></div>");        
    }
    $("#ActiveUsersNotification").append(userNotification);
}

