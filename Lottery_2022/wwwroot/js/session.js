// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Summary
// Loop that blocks validation button (or unlock it) depending on remaining time and count of numbers checked
// (Could be refactored, as consume ressources)
// Summary
setInterval(() => {
    var now = new Date()
    var realtime = now.getMinutes() % 5
    if (realtime != 4) {
        $('input[type=checkbox').on('change', function (e) {
            if ($('input[type=checkbox]:checked').length > 6) {
                $(this).prop('checked', false)
                alert("Vous ne pouvez pas choisir plus de 6 numéros ! Veuillez désélectionner des numéros pour en choisir d'autres.")
            }
            else if ($('input[type=checkbox]:checked').length == 6) {
                activeHtml()
            }
            else {
                blockHtml()
            }
        })
    }
    else {
        blockHtml();
    }    
}, 200)

//Functions that change CSS depending on conditions of the loop above:
function blockHtml() {
    document.getElementById('gameSubmit').disabled = 'disabled';
    document.getElementById('gameSubmit').classList.remove('activeButton');
    document.getElementById('gameSubmit').classList.add('disabledButton');
}
function activeHtml() {
    document.getElementById('gameSubmit').disabled = '';
    document.getElementById('gameSubmit').classList.add('activeButton');
    document.getElementById('gameSubmit').classList.remove('disabledButton');
}