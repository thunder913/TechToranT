$('.add').click(function (e) {

    var form = e.target.parentElement;
    var formdata = {
        'id': form.querySelector('input[name=Id]').value,
        'count': form.querySelector('input[name=Count]').value,
        'type': form.querySelector('input[name=Type]').value
    }

    var antiForgeryToken = $('input[name=__RequestVerificationToken]').val();

    $.ajax({
        type: 'POST',
        url: '/api/Basket/',
        data: JSON.stringify(formdata),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            var liElement = form.parentElement.parentElement;
            var productName = liElement.querySelector('h1').textContent;
            successNotification('Added ' + productName + ' to the basket! (' + formdata.count + ')');

        },
        error: function (res) {
            dangerNotification('Something went wrong, try again and check the count!');
        }
    });
})