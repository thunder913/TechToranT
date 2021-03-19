var notificationSettings = {
    placement: {
        from: 'top',
        align: 'center'
    },
    mouse_over: 'pause',
    type: 'success',
    offset: 50,
    delay: 200
};

function successNotification(message) {
    notificationSettings.type = 'success';
    $.notify({
        message: message
    }, notificationSettings);
}

function dangerNotification(message) {
    notificationSettings.type = 'danger';
    $.notify({
        message: message,
    }, notificationSettings);
}