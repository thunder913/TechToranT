$('.finishOrder').on('click', function (e) {
    var form = e.target.parentElement;
    var id = form.querySelector('input[name=id]').value;
    var antiForgeryToken = form.querySelector('input[name=__RequestVerificationToken]').value;
    $.ajax({
        type: 'POST',
        url: '/api/Order/FinishOrder/' + id,
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            if (!!res) {
                successNotification("Succesfully accepted the order!");
            } else {
                dangerNotification("Something went wrong, try again!");
            }
        },
        error: function (err) {
            dangerNotification('Something went wrong, try again!');
        }
    });
    e.preventDefault();
})