// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Summary
// Script that refresh location page, if page has been put to background
// Allow to update the timer and solve some delay problems
// Sumarry
var blurred = false;
window.onblur = function () { blurred = true; };
window.onfocus = function () { blurred && (location.reload()); };

// Summary
// Script for timer, based on realtime (starts at 12:05, 12:10, 12:15, etc)
// Summary
var nowT = new Date()
var realMinute = 4 - (nowT.getMinutes() % 5)
var realSecond = 60 - nowT.getSeconds()

const departMinutes = realMinute
let timeSpan = realSecond + realMinute * 60
const timerElement = document.getElementById("timer")

setInterval(() => {
    let minutes = parseInt(timeSpan / 60, 10)
    let seconds = parseInt(timeSpan % 60, 10)

    minutes = minutes < 10 ? "0" + minutes : minutes
    seconds = seconds < 10 ? "0" + seconds : seconds

    if (timeSpan <= 60) {
        redColouring()
    }
    else {
        whiteColouring()
    }
    timerElement.innerText = `${minutes}:${seconds}`
    timeSpan = timeSpan <= 0 ? 300 : timeSpan = timeSpan - 1   
}, 1000)

// Functions that define CSS of timer:
function redColouring() {
    document.getElementById('timer').classList.remove('timerTimeRemains');
    document.getElementById('timer').classList.add('timerTimesUp');
}
function whiteColouring() {
    document.getElementById('timer').classList.remove('timerTimesUp');
    document.getElementById('timer').classList.add('timerTimeRemains');
}