import { SEARCH_FORM_TYPES } from "../../common/constants.js";
import {
    createClickEvent,
    createInputEvent,
    createKeyUpEvent,
} from "../../common/main.js";
import { fetchRestful } from "../../utils/api-util.js";
import config from "../../common/config.js";
import { showSuccessToast, showWarningToast } from "../../utils/toastify-util.js";

export function login() {
    const txtCardNo = document.getElementById("txtCardNo");
    const btnSubmit = document.getElementById("btnSubmitLogin");
    const modal = document.getElementById("loginModal");
    let OPAC_USER = null;

    btnSubmit.addEventListener("click", async () => {
        const cardNo = txtCardNo.value.trim();
        const password = passwordInput.value.trim();

        fetchRestful({
            url: config.baseUrlApi + "Document/login-opac",
            method: "POST",
            contentType: "application/json",
            responseType: "json",
            data: {
                cardNo,
                password
            }
        }).then((response) => {
            if (response.success) {
                showSuccessToast("Đăng nhập thành công!");
                OPAC_USER = response.data;
                localStorage.setItem("OPAC_USER", JSON.stringify(OPAC_USER));
                renderUserInfo(OPAC_USER); 
                modal.classList.remove("active");
                location.reload();
            } else {
                showWarningToast(response.message);
            }
        });
    });

    const btnUserInfo = document.getElementById("btnUserInfo");
    const btnLogin = document.getElementById("btnLogin");

    function renderUserInfo(user) {
        const userAvatar = document.getElementById("userAvatar");
        const userName = document.getElementById("userName");

        if (user) {
            userAvatar.src = user.avatarUrl ?? "https://i.pinimg.com/736x/bc/43/98/bc439871417621836a0eeea768d60944.jpg"; 
            userName.textContent = user.fullName || "";

            btnLogin.style.display = "none";
            btnUserInfo.style.display = "flex";
        } else {
            btnLogin.style.display = "block"; 
            btnUserInfo.style.display = "none";
        }
    }



    document.addEventListener("DOMContentLoaded", () => {
        const user = localStorage.getItem("OPAC_USER");
        if (user) {
            renderUserInfo(JSON.parse(user));
        }
        else {
            renderUserInfo("");
        }
    });

    const dropdown = document.getElementById("userDropdown");

    btnUserInfo.addEventListener("click", function (e) {
        e.stopPropagation();
        dropdown.style.display =
            dropdown.style.display === "block" ? "none" : "block";
    });

    document.addEventListener("click", function () {
        dropdown.style.display = "none";
    });

    document.getElementById("btnLogout").addEventListener("click", function () {
        localStorage.setItem("OPAC_USER", "");
        renderUserInfo("");
        showSuccessToast("Đăng xuất thành công!");
        location.reload();
    });
}
