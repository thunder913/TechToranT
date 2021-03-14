//Function to add a background behing the item

function addBackgroundImage(controllerName) {
    $('ul li div').each(function (i) {
        $(this).css('background-image', 'url(/img/' + controllerName + '/' + this.id + ".jpg)");
    });
}
