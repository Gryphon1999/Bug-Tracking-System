// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    // Hide the success alert after 2 seconds
    setTimeout(function () {
        $('.alert-success').fadeOut('slow');
    }, 2000);

    // Hide the error alert after 2 seconds
    setTimeout(function () {
        $('.alert-danger').fadeOut('slow');
    }, 2000);
});

