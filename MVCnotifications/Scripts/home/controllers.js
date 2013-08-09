function LogCtrl($scope) {
 
   $scope.login = function () {
       userId = $scope.user.id;
       userName = $scope.user.name;
    }
}

var userId;
var userName;

function NoticeCtrl($scope) {

    $scope.id = userId;
    $scope.name = userName;
}
