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
            if (res) {
                finalPriceElement.textContent = getTotalPrice();
                var removedName = trElement.querySelectorAll('td')[1].textContent;
                successNotification(`Successfully removed "${removedName}" from the basket!`);
                trElement.remove();
            } else {
                dangerNotification("Something went wrong, try again!");
            }
            
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


let finalPriceElement = $('span.price')[0]
finalPriceElement.textContent = getTotalPrice();

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
$('tr').on('click', '.addPromoCode', function (e) {
    e.preventDefault();
    let tdElement = e.target.parentElement.parentElement;
    let antiForgeryToken = tdElement.querySelector('input[name=__RequestVerificationToken]').value;
    var code = tdElement.querySelector('input[name=tableCode]').value;

    $.ajax({
        type: 'POST',
        url: '/api/Basket/AddPromoCode',
        data: JSON.stringify({ code: code }),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            if (res) {
                tdElement.querySelector('input[name=tableCode]').remove();
                tdElement.querySelector('.addPromoCode').remove();
                let spanElement = document.createElement('span');
                let buttonElement = document.createElement('button');
                buttonElement.innerHTML = 'Remove';
                buttonElement.classList.add('btn', 'btn-danger', 'removeCode');
                spanElement.innerHTML = 'Code ' + res.code + ' has been used! It expires on ' + res.expirationDate + '!';

                tdElement.appendChild(spanElement);
                tdElement.appendChild(buttonElement);
                successNotification(`Successfully added a promo code!`);
            } else {
                dangerNotification("Something went wrong, try again!");
            }

        }
    });
})

$('tr').on('click', '.removeCode', function (e) {
    e.preventDefault();
    let antiForgeryToken = e.target.parentElement.querySelector('form input[name=__RequestVerificationToken]').value;

    $.ajax({
        type: 'POST',
        url: '/api/Basket/RemovePromoCode',
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            successNotification('You successfuly removed your promo code!');
            let tdElement = e.target.parentElement;
            tdElement.querySelector('span').remove();
            tdElement.querySelector('button').remove();

            let inputElement = document.createElement('input');
            inputElement.name = 'tableCode';
            inputElement.placeholder = 'Promo code';

            let buttonElement = document.createElement('button');
            buttonElement.type = 'submit';
            buttonElement.classList.add('btn', 'btn-dark', 'addPromoCode');
            buttonElement.innerHTML = 'Add';

            let formElement = tdElement.querySelector('form');
            formElement.appendChild(inputElement);
            formElement.appendChild(buttonElement);
        }
    });
})