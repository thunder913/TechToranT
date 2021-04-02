
$('.remove').click(function (e) {

    var form = e.target.parentElement;

    var orderId = form.querySelector('input[name=orderId]').value;
    var antiForgeryToken = $('input[name=__RequestVerificationToken]').val();
    var orderDto = { orderId };
    var data = JSON.stringify(orderDto);

    $.ajax({
        type: 'POST',
        url: '/api/Order/Delete',
        data: data,
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            if (res) {
                var tdElements = form.parentElement.parentElement.querySelectorAll('td');
                var statusElement = tdElements[3];
                var actionsElement = tdElements[4];
                statusElement.textContent = 'Cancelled';
                actionsElement.innerHTML = 'No actions available!';

                successNotification('You successfully cancelled your order!');
            } else {
                dangerNotification("The order must be pending!");
            }

        },
        error: function (res) {
            dangerNotification('Something went wrong, try again!');
        }
    });
})
jQuery(document).ready(function ($) {
    $(".clickable-row").click(function () {
        window.location = $(this).data("href");
    });
});
$("tr td form button").click(function (e) {
    // Do something
    e.stopPropagation();
});