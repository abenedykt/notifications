/**
 * Created with JetBrains WebStorm.
 * User: zos-srv
 * Date: 02.08.13
 * Time: 10:14
 * To change this template use File | Settings | File Templates.
 */
function MessageListCtrl($scope, $http) {
$scope.messages = [
    {"name": "Emil",
     "message" : "Czesc!"},
    {"name": "Ala",
     "message" : "Hej"},
    {"name": "Emil",
     "message" : "Co tam?"},
    {"name": "Ala",
     "message" : "Nic"}
]
}

function DefaultCtrl($scope) {
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

            //$scope.users.push($scope.user);


            chat.server.send($scope.user.name, $scope.user.text);
            $('#message').val('');
        }
    }
    )
    
}


