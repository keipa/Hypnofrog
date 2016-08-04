function showLoader() {
    $('#loadingDiv').show();
};

function hideLoader() {
    $('#loadingDiv').hide();
};

function dosmth() {

    showLoader();

    $.ajax({
        type: "POST",
        url: "/Home/LoadAction",
        complete: function () {
            hideLoader();
        }
    });
};