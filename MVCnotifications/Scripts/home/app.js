angular.module('notifications', []).
    config(['$routeProvider', '$locationProvider', function ($routeProvider, $locationProvider) {
        $routeProvider.
            when('/logowanie', { templateUrl: '/Home/Logowanie', controller: 'LogCtrl', }).
            when('/powiadomienia', { templateUrl: '/Home/Powiadomienia', controller: 'NoticeCtrl', }).
            otherwise({ redirectTo: '/logowanie' });

        $locationProvider.html5Mode(false).hashPrefix('!');
    }]);

   
