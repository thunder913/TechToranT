
var antiForgeryToken = $('input[name=__RequestVerificationToken]').val();
$(document).ready(function () {
    $('#ordersTable').DataTable({
        order: [[3, "desc"]],
        processing: true,
        serverSide: true,
        filter: true,
        ajax: {
            url: '/api/order/all',
            type: 'POST',
            dataType: "json",
            headers:
            {
                'X-CSRF-TOKEN': antiForgeryToken
            }
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        },
        {
            "targets": [6],
            "searchable": false,
            "orderable": false
        }],
        "columns": [
            { "data": "id", "name": "Id", "autoWidth": true },
            { "data": "email", "name": "Email", "autoWidth": true },
            { "data": "fullName", "name": "FullName", "autoWidth": true },
            {
                "data": "date", "name": "Date", "autoWidth": true,
                render: function (data) {
                    var date = new Date(Date.parse(data));
                    var userTimeZoneOffset = new Date().getTimezoneOffset();
                    date.setMinutes(date.getMinutes() - userTimeZoneOffset);
                    return date.toLocaleString();
                }
            },
            {
                "data": "price", "name": "Price", "autoWidth": true,
                render: function (data) {
                    return Number.parseFloat(data).toFixed(2);
                }
            },
            {
                "data": "status", "name": "Status", "autoWidth": true,
                render: function (data) {


                    let selectElement = document.createElement("select");
                    let processTypes = [{ id: 0, text: 'Pending' },
                    { id: 1, text: 'InProcess' },
                    { id: 2, text: 'Cooking' },
                    { id: 3, text: 'Cooked' },
                    { id: 4, text: 'Delivered' },
                    { id: 5, text: 'Completed' },
                    { id: 6, text: 'Cancelled' }];

                    for (let i = 0; i < processTypes.length; i++) {
                        let element = processTypes[i];
                        let option = document.createElement("option");
                        option.text = element.text;
                        option.value = element.id;
                        if (data === element.text) {
                            option.setAttribute("selected", true);
                        }
                        selectElement.appendChild(option);
                    }
                    let formElement = document.createElement('form');
                    formElement.setAttribute('method', 'POST');
                    formElement.classList.add("status");
                    formElement.appendChild(selectElement);
                    var buttonElement = document.createElement('button');
                    buttonElement.innerText = "Edit";
                    buttonElement.classList.add(...['btn', 'btn-success', 'edit']);
                    buttonElement.type = 'button';
                    formElement.appendChild(buttonElement);
                    // TODO make the onclick work!
                    return formElement.outerHTML;
                }
            },
            {
                data: "id",
                "render": function (data) { return "<a href='/Order/Index/" + data + "' class='btn btn-info'>View</a>"; }
            },
        ]
    });
});

$('#ordersTable').on('click', '.edit', function (e) {
    e.preventDefault();
    setEdit(e);
})

function setEdit(e) {
    var form = e.target.parentElement;
    var newProcessingTypeId = form.querySelector('.status select').value;
    var row = form.parentElement.parentElement;
    let table = $('#ordersTable').DataTable();
    var rowData = table.row(row).data();
    let rowId = rowData.id;
    var oldProcessType = rowData.status;
    let antiForgeryToken = $('input[name=__RequestVerificationToken]')[0].value;

    $.ajax({
        type: 'POST',
        url: '/api/Order/EditStatus',
        data: JSON.stringify({ newProcessingTypeId: newProcessingTypeId, orderId: rowId, oldProcessingType: oldProcessType }),
        headers: {
            'X-CSRF-TOKEN': antiForgeryToken
        },
        contentType: 'application/json',
        success: function (res) {
            if (!!res) {
                successNotification("Succesfully changed the Status of the order.");
                table.ajax.reload();
            }
        }
    });
    e.preventDefault();
}


if (window.history.replaceState) {
    window.history.replaceState(null, null, window.location.href);
}