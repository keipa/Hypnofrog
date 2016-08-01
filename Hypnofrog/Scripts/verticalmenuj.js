var $window = $(window),
    $html = $('#menu-bar');

$window.resize(function resize() {
    if ($window.width() < 768) {
        // When the side bar is moved to the top, this stops it being fixed in place
        $("#navbar").removeClass('navbar-fixed-top');
        return $html.removeClass('nav-stacked');
    }
    $("#navbar").addClass('navbar-fixed-top');
    $html.addClass('nav-stacked');
}).trigger('resize');