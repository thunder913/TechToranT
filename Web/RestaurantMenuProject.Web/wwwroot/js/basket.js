$('.add').click(function (e) {

    let tdElement = e.target.parentElement;
    let trElement = tdElement.parentElement;
    var { data, antiForgeryToken, finalPriceElement } = getElementAndAntiForgeryToken(tdElement, trElement);

    $.ajax({
        type: 'POST',
        url: '/api/Basket/Add',
        data: JSON.stringify(data),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            trElement.querySelector('td[class=name]').textContent = res.name;
            trElement.querySelector('td[class=quantity]').textContent = res.quantity;
            trElement.querySelector('td[class=price]').textContent = Number.parseFloat(res.price).toFixed(2) + '$';
            trElement.querySelector('td[class=totalPrice]').textContent = Number.parseFloat(res.price * res.quantity).toFixed(2) + '$';
            finalPriceElement.textContent = getTotalPrice();
            successNotification(`Successfully added one more "${res.name}"!`);
        }
    });
})
$('.removeAll').click(function (e) {
    let tdElement = e.target.parentElement;
    let trElement = tdElement.parentElement;
    var { data, antiForgeryToken, finalPriceElement } = getElementAndAntiForgeryToken(tdElement, trElement);

    $.ajax({
        type: 'POST',
        url: '/api/Basket/RemoveAll',
        data: JSON.stringify(data),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            trElement.remove();
            finalPriceElement.textContent = getTotalPrice();
            successNotification(`Successfully removed "${res.name}" from the basket!`);
        }
    });
})

$('.remove').click(function (e) {
    let tdElement = e.target.parentElement;
    let trElement = tdElement.parentElement;
    var { data, antiForgeryToken, finalPriceElement } = getElementAndAntiForgeryToken(tdElement, trElement);

    $.ajax({
        type: 'POST',
        url: '/api/Basket/RemoveOne',
        data: JSON.stringify(data),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            if (!res) {
                trElement.remove();
                successNotification(`Successfully removed "${res.name}" from the basket!`);
            }
            else {
                trElement.querySelector('td[class=name]').textContent = res.name;
                trElement.querySelector('td[class=quantity]').textContent = res.quantity;
                trElement.querySelector('td[class=price]').textContent = Number.parseFloat(res.price).toFixed(2) + '$';
                trElement.querySelector('td[class=totalPrice]').textContent = Number.parseFloat(res.price * res.quantity).toFixed(2) + '$';
            }
            finalPriceElement.textContent = getTotalPrice();
            successNotification(`Successfully removed one "${res.name}"!`);
        }
    });
})


let finalPriceElement = $('span.price')[0].textContent = getTotalPrice();

    function getElementAndAntiForgeryToken(tdElement, trElement) {
        let antiForgeryToken = tdElement.querySelector('input[name=__RequestVerificationToken]').value;
        let id = $(trElement).data('id');
        let type = $(trElement).data('type');
        let data = { id, type };

        let finalPriceElement = $('span.price')[0];

        return { data, antiForgeryToken, finalPriceElement };
    }

function getTotalPrice() {
    var result;
    $.ajax({
        type: 'GET',
        async: false,
        url: '/api/Basket/GetPrice',
        success: function (res) {
            result = Number.parseFloat(res).toFixed(2) + '$';
        }
    });

    return result;
}