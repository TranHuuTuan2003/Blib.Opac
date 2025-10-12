var Notification = (function () {
    function Confirm(content, callbackOK, callbackCannel) {
        alertify.confirm(content,
            function () {
                if (callbackOK) {
                    callbackOK();
                }
            },
            function () {
                if (callbackCannel) {
                    callbackCannel();
                }
            })
    }

    return { Confirm };
})