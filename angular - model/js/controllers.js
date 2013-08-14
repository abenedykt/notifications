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

function ChatCtrl($scope){
    $scope.user = {};
    $scope.users = [{ name:"John", text:"Hello!"}]
    $scope.add = function () {

        $scope.users.push($scope.user);
        $scope.user = {"name": $scope.user.name
        };
    }
}


