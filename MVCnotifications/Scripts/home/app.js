angular.module('powiadamiacz', []).
    config(['$routeProvider','$locationProvider', function($routeProvider, $locationProvider) {
        $routeProvider.
            when('/default', {templateUrl: '/Home/Default', controller: 'DefaultCtrl',}).
            when('/page1', { templateUrl: '/Home/Page1', controller: 'ChatCtrl', }).
            when('/page2', {templateUrl:'/Home/Page2', controller: 'MessageListCtrl',}).
            otherwise({ redirectTo: '/default' });

        $locationProvider.html5Mode(false).hashPrefix('!');
    }]);
