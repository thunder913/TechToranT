$('.getNumber').click(function (e) {
    e.preventDefault();
    $.ajax({
        type: 'GET',
        url: '/api/Table/GetNextNumber',
        contentType: 'application/json',
        success: function (res) {
            e.target.parentElement.querySelector('input').value = res;
        }
    });
})