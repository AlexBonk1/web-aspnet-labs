// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(document).ready(function () {
    $('#product-list-container').on('click', '.page-link', function (e) {
        
        e.preventDefault(); 
        var url = $(this).attr('href'); 

        if (url && url !== '#') {
            $('#product-list-container').load(url);
        }
    });
});