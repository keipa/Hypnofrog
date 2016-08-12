angular.module("Hypnofrog.HomeController", [])
    .controller("IndexCtrl",
    [
        "$scope", "$http", function($scope, $http) {
            $http.get("Home/IndexVm")
                .success(function(data) {
                    $scope.model = data;
                    var tags = data.Tags;
                    tags = tags.split(";");
                    for (i = 0; i < tags.length; i++) {
                        tags[i] = tags[i].split(",");
                        tags[i][1] = parseInt(tags[i][1]);
                    }
                    WordCloud(document.getElementById("my_canvas"),
                    {
                        list: tags,
                        fontFamily: "Times, serif",
                        color: function(word, weight) {
                            return (word === tags[0][0]) ? "#f02222" : "#000000";
                        },
                        gridSize: Math.round(16 * $("my_canvas").width() / 1024),
                        weightFactor: function(size) {
                            return size * 15;
                        },
                        rotateRatio: 0.5,
                        backgroundColor: "#ffffff"
                    });

                });
        }
    ]);