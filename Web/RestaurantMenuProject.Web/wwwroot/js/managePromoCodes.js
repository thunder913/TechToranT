var antiForgeryToken = $('input[name=__RequestVerificationToken]').val();
$(document).ready(function () {
    $('#ordersTable').DataTable({
        order: [[3, "desc"]],
        processing: true,
        serverSide: true,
        filter: true,
        ajax: {
            url: '/api/promoCode/all',
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
            "targets": [5],
            "searchable": false,
            "orderable": true
        },
        {
            "targets": [7],
            "searchable": false,
            "orderable": false
        }],
        "columns": [
            { "data": "id", "name": "Id", "autoWidth": true },
            { "data": "code", "name": "Code", "autoWidth": true },
            { "data": "usedTimes", "name": "UsedTimes", "autoWidth": true },
            { "data": "maxUsageTimes", "name": "MaxUsageTimes", "autoWidth": true },
            { "data": "promoPercent", "name": "PromoPercent", "autoWidth": true },
            {
                "data": "isDeleted", "name": "IsDeleted", "autoWidth": true,
                render: function (data) {
                    if (data) {
                        return "Yes";
                    }
                    return "No";
                }
            },
            {
                "data": "expirationDate", "name": "ExpirationDate", "autoWidth": true,
                render: function (data) {
                    var date = new Date(Date.parse(data));
                    var userTimeZoneOffset = new Date().getTimezoneOffset();
                    date.setMinutes(date.getMinutes() - userTimeZoneOffset);
                    return date.toLocaleString();
                }
            },
            {
                data: "id",
                "render": function (data) { return "<a href='/Manage/EditPromoCode/" + data + "' class='btn btn-dark action'>Edit and view</a>"; }
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