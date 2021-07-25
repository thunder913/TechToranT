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
                successNotification("Succesfully finished the order!");
                form.parentElement.parentElement.remove();
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

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/orderHub")
    .build();
connection.on("NewPickup", function (item) {
    let tbodyElement = document.querySelector(".pickupTable tbody");
    let tBodyElements = tbodyElement.querySelectorAll('tr');
    for (var i = 0; i < tBodyElements.length; i++) {
        let element = tBodyElements[i];
        if (element.dataset.id === item.id) {
            element.querySelector('.count').innerHTML = item.count;
            return;
        }
    }
    let trElement = getTrElement(item);
    tbodyElement.appendChild(trElement);
});
connection.on("AddItemsToPickup", function (item) {
    let order = item.order;
    let dateUtc = new Date(Date.parse(order.date));
    let userTimeZoneOffset = new Date().getTimezoneOffset();
    dateUtc.setMinutes(dateUtc.getMinutes() - userTimeZoneOffset);
    let date = dateUtc.toLocaleString();
    let orderBody = document.querySelector('#ordersTable tbody');
    let trHtml = `<tr id="${order.id}">
                        <th class="th-sm">
                            ${order.tableNumber}
                        </th>
                        <th class="th-sm">
                            ${date}
                        </th>
                        <th class="th-sm">
                            ${order.fullName}
                        </th>
                        <th class="th-sm">
                            ${order.statusName}
                        </th>
                        <th class="th-sm">
                            ${order.price.toFixed(2)}
                        </th>
                        <th class="th-sm">
                            <form method="post" style="display: inline-block;">
                                <input name="id" value="${order.id}" hidden />
                                <button class="btn btn-success accept">Accept</button>
                            </form>
                            <form method="get" action="/Order/Index/${order.id}" style='display:inline-block;'>
                                <button class="btn btn-info">Info</button>
                            </form>
                        </th>
                    </tr>`
    orderBody.insertAdjacentHTML('beforeend', trHtml);
})
connection.on("NewOrderCookedPercent", function (item) {
    var trElement = document.querySelector(`.activeTable tr[id='${item.orderId}']`);
    trElement.querySelectorAll('th')[4].innerHTML = `${item.cookedPercent}%`;
})
connection.on("RemoveNewWaiterOrder", function (order) {
    let elementToRemove = document.querySelector(`.newOrders #${order.orderId}`);
    elementToRemove.remove();
})
$(".activeTable").on('click', '.pay', function (e) {
    e.preventDefault();
    let trElement = e.target.parentElement.parentElement;
    let id = trElement.id;
    connection.invoke('PayOrder', id);
    e.target.parentElement.innerHTML = `<button class="btn btn-success pay" disabled>Pay</button>`
})
$(".pickupTable").on('click', '.pickupDone', function (e) {
    e.preventDefault();
    let form = e.target.parentElement;
    let id = form.querySelector('input[name=id]').value;
    connection.invoke("FinishPickupItem", id);
    successNotification('Successfully picked the item up!');
    form.parentElement.parentElement.remove();
});
$('#ordersTable').on('click', '.accept', function (e) {
    e.preventDefault();
    var form = e.target.parentElement;
    var id = form.querySelector('input[name=id]').value;
    let item = { newProcessingTypeId: 1, orderId: id, oldProcessingType: 'Pending' };
    connection.invoke('AcceptOrder', item);
    successNotification('You successfully accepted the order!');
    form.parentElement.parentElement.remove();
})
connection.on('UpdateTableStatus', function (item) {
    let trElement = document.querySelector(`.activeTable tr[id='${item.id}']`);
    if (item.processType == 'Completed') {
        trElement.remove();
        successNotification('You finished an order!');
    }
    trElement.querySelectorAll('th')[1].innerHTML = item.processType;
})
connection.on('NewActiveTable', function (item) {
    let tbodyElement = document.querySelector('.activeTable tbody');
    let th1 = getThElement(item.tableNumber);
    let th2 = getThElement(item.processType);
    let th3 = document.createElement('th');
    th3.classList.add('th-sm');
    th3.innerHTML = `<button class="btn btn-danger pay">Pay</button>`;
    let th4 = document.createElement('th');
    let formElement = document.createElement('form');
    let inputElement = document.createElement('input');
    inputElement.name = 'id';
    inputElement.value = item.id;
    inputElement.hidden = true;
    let buttonElement = document.createElement('button');
    buttonElement.classList.add('btn', 'btn-success', 'pickupDone');
    buttonElement.innerHTML = 'Done';
    formElement.appendChild(inputElement);
    formElement.appendChild(buttonElement);
    th4.appendChild(formElement);
    let th5 = getThElement(`${item.readyPercent}%`);
    let trElement = document.createElement('tr');
    trElement.id = item.id;
    trElement.appendChild(th1);
    trElement.appendChild(th2);
    trElement.appendChild(th3);
    trElement.appendChild(th4);
    trElement.appendChild(th5);
    tbodyElement.appendChild(trElement);
})
connection.start().catch(function (err) {
    return console.error(err.toString());
});
function getTrElement(item) {
    let trElement = document.createElement('tr');
    trElement.dataset.id = item.id;
    let th1 = getThElement(item.count);
    th1.classList.add('count');
    let th2 = getThElement(item.name);
    let th3 = getThElement(item.tableNumber);
    let th4 = getThElement(item.clientName);
    let th5 = document.createElement('th');
    let formElement = document.createElement('form');
    let inputElement = document.createElement('input');
    inputElement.name = 'id';
    inputElement.defaultValue = item.id;
    inputElement.hidden = true;
    let buttonElement = document.createElement('button');
    buttonElement.classList.add('btn', 'btn-success', 'pickupDone');
    buttonElement.innerHTML = 'Done';
    formElement.appendChild(inputElement);
    formElement.appendChild(buttonElement);
    th5.appendChild(formElement);
    trElement.appendChild(th1);
    trElement.appendChild(th2);
    trElement.appendChild(th3);
    trElement.appendChild(th4);
    trElement.appendChild(th5);
    return trElement;
}
function getThElement(value) {
    let th1 = document.createElement('th');
    th1.classList.add('th-sm')
    th1.innerHTML = value;
    return th1;
}
