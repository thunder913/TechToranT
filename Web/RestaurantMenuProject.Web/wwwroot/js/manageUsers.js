
var antiForgeryToken = $('input[name=__RequestVerificationToken]').val();
$(document).ready(function () {
    $('#usersTable').DataTable({
        order: [[4, "desc"]],
        processing: true,
        serverSide: true,
        filter: true,
        ajax: {
            url: '/api/users',
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
            { "data": "roles", "name": "Roles", "autoWidth": true },
            { "data": "name", "name": "Name", "autoWidth": true },
            {
                "data": "createdOn", "name": "CreatedOn", "autoWidth": true,
                render: function (data) {
                    var date = new Date(Date.parse(data));
                    var userTimeZoneOffset = new Date().getTimezoneOffset();
                    date.setMinutes(date.getMinutes() - userTimeZoneOffset);
                    return date.toLocaleString();
                }
            },
            {
                "data": "deletedOn", "name": "DeletedOn", "autoWidth": true,
                render: function (data) {
                    if (data) {
                        var date = new Date(Date.parse(data));
                        var userTimeZoneOffset = new Date().getTimezoneOffset();
                        date.setMinutes(date.getMinutes() - userTimeZoneOffset);
                        return date.toLocaleString();
                    } else {
                        return "Account is active!"
                    }
                }
            },
            {
                data: "id",
                "render": function (data) { return "<a href='/Manage/EditUser/" + data + "' class='btn btn-primary'>Edit</a> <a href='/Order/All/" + data + "' class='btn btn-info'>Orders</a>"; }
            },
        ]
    });
});
if (window.history.replaceState) {
    window.history.replaceState(null, null, window.location.href);
}