angular.module("Hypnofrog.UserController", [])
    .controller("UserCtrl",
    [
        "$scope", "$http", function ($scope, $http) {
            $scope.init = function (username) {
                $scope.username = username;
            };
            $http.get("UserVM/" + $scope.username)
                .success(function(data) {
                    $scope.model = data;
                });
        },

    ]);