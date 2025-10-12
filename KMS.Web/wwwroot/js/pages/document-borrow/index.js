createDomLoadedEvent(() => {
    const backButton = document.querySelector(".first-documentDetail__left");
    createClickEvent(backButton, () => {
        history.back();
    });

    let currentCaptcha = "";

    function generateCaptcha() {
        const canvas = document.getElementById("captcha");
        const ctx = canvas.getContext("2d");
        const chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        const captchaLength = 4;
        let captcha = "";

        // Sinh mã captcha
        for (let i = 0; i < captchaLength; i++) {
            captcha += chars.charAt(Math.floor(Math.random() * chars.length));
        }
        currentCaptcha = captcha;

        // Xóa nội dung cũ
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        // Vẽ nền
        ctx.fillStyle = "#e6f3ff";
        ctx.fillRect(0, 0, canvas.width, canvas.height);

        // Vẽ chữ
        ctx.font = "4rem Arial";
        ctx.fillStyle = "#003a78";
        ctx.textBaseline = "middle";
        ctx.textAlign = "center";
        ctx.fillText(captcha, canvas.width / 2, canvas.height / 2);
    }

    // Gọi khi trang load
    generateCaptcha();
});
