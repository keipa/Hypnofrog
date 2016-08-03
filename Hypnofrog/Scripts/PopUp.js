﻿(function () {
    $('body').on('click', '.modal-link', function (e) {
        e.preventDefault();
        $(this).attr('data-target', '#modal-container');
        $(this).attr('data-toggle', 'modal');
    });
    $('body').on('click', '.modal-close-btn', function () {
        $('#modal-container').modal('hide');
        $('#modal-container').removeData('bs.modal');
    });
    $('body').on('click', '.new-comment-submit', function () {
        $('#modal-container').modal('hide');
        $('#modal-container').removeData('bs.modal');
    });
    $('#modal-container').on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    });
    $('#CancelModal').on('click', function () {
        return false;
    });
}())