angular.module('app', ['Hypnofrog.HomeController', 'Hypnofrog.SearchController', 'Hypnofrog.AllUsersController']).config(function ($sceProvider) {
    // Completely disable SCE.  For demonstration purposes only!
    // Do not use in new projects.
    $sceProvider.enabled(false);
});;

