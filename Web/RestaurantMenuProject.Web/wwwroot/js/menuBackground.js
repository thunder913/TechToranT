//Function to add a background behing the menu item
function addBackgroundImage(controllerName) {
    $('ul li div').each(function (i) {
        console.log(controllerName);
        $(this).css('background-image', 'url(/img/' + controllerName + '/' + this.id + ".jpg)");
    });
}
