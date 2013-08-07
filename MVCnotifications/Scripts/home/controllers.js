function DefaultCtrl($scope){
}

function MessageListCtrl($scope) {
 
   $scope.send = function () {

        if (navigator.userAgent.indexOf("Chrome") > -1) {
            if (window.webkitNotifications.checkPermission() == 0) {
                window.webkitNotifications.createNotification(null, 'Powiadomienie!', 'Masz nowa wiadomosc- dokument zostal zatwierdzony/odrzucony').show();
            }
            else {
                window.webkitNotifications.requestPermission();
            }
        }
        else {
            $.pnotify({
                title: 'Powiadomienie!',
                text: 'Masz nowa wiadomosc- dokument zostal zatwierdzony/odrzucony',
                hide: false,
                sticker: false        
            });
        }
    }
}

function ChatCtrl($scope) {

    var chat = $.connection.chatHub;
    
    chat.client.addNewMessageToPage = function (name, message) {
        var encodedName = $('<div />').text(name).html();
        var encodedMsg = $('<div />').text(message).html();
        $('#discussion').append('<h4 class="list-group-item-heading">' + encodedName
            + '</h4><p class="list-group-item-text"> ' + encodedMsg + '</p>');     
    }
    
    $.connection.hub.start().done(function () {
        $scope.add = function () {
            chat.server.send($scope.user.name, $scope.user.text);
            $('#message').val('');
        }
    });
    
}


