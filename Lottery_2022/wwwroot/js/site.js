// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$('input[type=checkbox').on('change', function (e) {
    if ($('input[type=checkbox]:checked').length > 6) {
        $(this).prop('checked', false);
        alert("Vous ne pouvez pas choisir plus de 6 numéros ! Veuillez désélectionner des numéros pour en choisir d'autres.");
    }
    else if ($('input[type=checkbox]:checked').length < 6) {
        document.getElementById('gameSubmit').disabled = 'disabled';
        document.getElementById('gameSubmit').classList.remove('button');
        document.getElementById('gameSubmit').classList.add('disabledButton');
    }
    else {
        document.getElementById('gameSubmit').disabled = '';
        document.getElementById('gameSubmit').classList.remove('disabledButton');
        document.getElementById('gameSubmit').classList.add('button');
    }
})