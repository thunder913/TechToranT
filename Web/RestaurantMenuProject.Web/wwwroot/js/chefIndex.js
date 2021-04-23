var connection = new signalR.HubConnectionBuilder()
    .withUrl("/orderHub")
    .build();

$('#ordersTable').on('click', '.accept', function (e) {
    e.preventDefault();
    let form = e.target.parentElement;
    let orderId = form.dataset.orderid;
    connection.invoke("ChefApproveOrder", orderId);
    successNotification("You got the order!");
    form.parentElement.parentElement.remove();
})

connection.on("RemoveNewChefOrder", function (order) {
    let elementToRemove = document.querySelector(`.newOrders #${order.orderId}`);
    elementToRemove.remove();
})

connection.on("RemoveFoodToPrepare", function (item) {
    let element = document.querySelector(`[data-orderid="${item.orderId}"][data-foodid="${item.foodId}"]`);
    let trElement = element.parentElement.parentElement;
    if (Number(trElement.querySelector('.count').innerHTML) - 1 == 0) {
        trElement.remove();
    } else {
        trElement.querySelector('.count').innerHTML--;
    }
})

connection.on("AddItemsToPickup", function (items) {
    let tableDivs = document.querySelectorAll('.activeOrders div');

    for (var i = 0; i < items.itemsToCook.length; i++) {
        let categoryItem = items.itemsToCook[i];

        // Getting the tBodyElement
        let tBody;
        for (var j = 0; j < tableDivs.length; j++) {
            let h2Text = tableDivs[j].querySelector('h2').innerHTML;
            if (h2Text === categoryItem.categoryName) {
                tBody = tableDivs[j].querySelector('tbody');
                break;
            }
        }

        for (var j = 0; j < categoryItem.itemsToCook.length; j++) {
            let item = categoryItem.itemsToCook[j];

            let trElement = document.createElement('tr');
            let th1 = getThElement(item.foodName);
            let th2 = getThElement(item.count);
            th2.classList.add('count')
            let th3 = document.createElement('th');
            th3.classList.add('th-sm');

            let formElement = document.createElement('form');
            formElement.dataset.orderid = item.orderId;
            formElement.dataset.foodid = item.foodId;
            let foodType = 'Dish';
            if (categoryItem.foodType === 1) {
                foodType = 'Drink';
            }
            formElement.dataset.dishtype = foodType;
            formElement.dataset.count = item.count;

            let buttonElement = document.createElement('button');
            buttonElement.classList.add('btn-success', 'ready');
            buttonElement.innerHTML = 'Ready';

            formElement.appendChild(buttonElement);
            th3.appendChild(formElement);

            trElement.appendChild(th1);
            trElement.appendChild(th2);
            trElement.appendChild(th3);

            tBody.appendChild(trElement);
        }
    }
})

connection.on("NewChefOrder", function (item) {

    let dateUtc = new Date(Date.parse(item.date));
    let userTimeZoneOffset = new Date().getTimezoneOffset();
    dateUtc.setMinutes(dateUtc.getMinutes() - userTimeZoneOffset);
    let date = dateUtc.toLocaleString();
    let name = item.name;
    let status = item.statusName;
    let price = item.price.toFixed(2);
    let orderId = item.orderId;


    let trHtml = `<tr id="${orderId}">
                        <th class="th-sm" >
                            1
                            </th >
                            <th class="th-sm">
                            ${date}
                            </th>
                            <th class="th-sm">
                            ${name}
                            </th>
                            <th class="th-sm">
                            ${status}
                            </th>
                            <th class="th-sm">
                            ${price}$
                            </th>
                            <th class="th-sm">
                                <form method="post" style="display: inline-block;" data-orderId="${orderId}">
                                    <button class="btn btn-success accept">Accept</button>
                                </form>
                                <form method="get" style='display:inline-block;' action="/Order/Index/${orderId}">
                                    <button class="btn btn-info">Info</button>
                                </form>
                            </th>
                        </tr >`;

    let tbodyElement = document.querySelector('#ordersTable tbody');

    tbodyElement.insertAdjacentHTML('beforeend', trHtml);
})



$(".activeOrders").on('click', '.ready', function (e) {
    e.preventDefault();
    let form = e.target.parentElement;
    let orderId = form.dataset.orderid;
    let foodId = form.dataset.foodid;
    let dishType = form.dataset.dishtype;
    let count = --form.dataset.count;
    var item = { orderId: orderId, foodId: foodId, foodType: dishType };
    connection.invoke("AddPickupItem", item);
    if (count == 0) {
        form.parentElement.parentElement.remove();
    } else {
        form.parentElement.parentElement.querySelector(".count").innerHTML = count;
    }
    successNotification('Successfully made the ' + dishType.toLowerCase() + '!');
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});

function getThElement(value) {
    let th1 = document.createElement('th');
    th1.classList.add('th-sm')
    th1.innerHTML = value;
    return th1;
}