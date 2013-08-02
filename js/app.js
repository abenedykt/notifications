angular.module('powiadamiacz', []).
    config(['$routeProvider', function($routeProvider) {
        $routeProvider.
            when('/page1', {templateUrl: 'partials/page1.html', controller: MessageListCtrl}).
            when('/page2', {templateUrl: 'partials/page2.html', controller: MessageListCtrl}).
            otherwise({redirectTo: '/page1'});
    }]);