angular.module('powiadamiacz', []).
    config(['$routeProvider', function($routeProvider) {
        $routeProvider.
            when('/default', {templateUrl: 'partials/default.html', controller: DefaultCtrl}).
            when('/page1', {templateUrl: 'partials/page1.html', controller: ChatCtrl}).
            when('/page2', {templateUrl: 'partials/page2.html', controller: MessageListCtrl}).
            otherwise({redirectTo: '/default'});
    }]);
