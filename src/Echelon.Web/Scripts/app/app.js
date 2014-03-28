(function () {
    'use strict';

    var app = angular.module('App', []);

    app.controller('QueueController', ['$scope', '$http', '$q', function ($scope, $http, $q) {
        $http.get('api/computerlist')
            .success(function (data) {
                $scope.computers = data;
            });

        var httpPromise = null;
        $scope.selectComputer = function (computer) {
            if (httpPromise)
                httpPromise.resolve();
            httpPromise = $q.defer();
            var url = 'api/queuelist?computer=' + (computer || '');
            $http.get(url, { timeout: httpPromise.promise })
                .success(function (data) {
                    $scope.selectedComputer = computer;
                    $scope.queues = data;
                    httpPromise = null;
                });
        };

        $scope.viewMessages = function(computer, queueName) {
            if (httpPromise)
                httpPromise.resolve();
            httpPromise = $q.defer();
            var url = 'api/messagelist?computer=' + (computer || '') + '&queueName=' + (queueName || '');
            $http.get(url, { timeout: httpPromise.promise })
                .success(function (data) {
                    $scope.selectedQueue = queueName;
                    $scope.messages = data;
                    httpPromise = null;
                });
        };

        $scope.viewMessage = function(computer, queueName, messageId) {
            if (httpPromise)
                httpPromise.resolve();
            httpPromise = $q.defer();
            var url = 'api/message?computer=' + (computer || '') + '&queueName=' + (queueName || '') + '&messageId=' + (messageId || '');
            $http.get(url, { timeout: httpPromise.promise })
                .success(function (data) {
                    $scope.selectedMessage = data;
                    httpPromise = null;
                });
        };

    }]);

    window.App = app;
})();