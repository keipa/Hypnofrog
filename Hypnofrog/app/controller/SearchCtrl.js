angular.module("Hypnofrog.SearchController", [])
    .controller("SearchCtrl",
    [
        "$scope", "$http", function($scope, $http) {
            $scope.init = function(searchstring) {
                $scope.searchstring = searchstring;

                $http.get("SearchVm/" + $scope.searchstring)
                    .success(function(data) {
                        $scope.model = data;
                    });
            };

        }
    ]);