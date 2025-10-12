var UcHelpers = function () {
    function GetFrequencyTextByCode(code) {
        var rs = '';
        if (code[0] == 'D') {
            rs = 'Ngày';
        }
        else if (code[0] == 'W') {
            rs = 'Tuần';
        }
        else if (code[0] == 'M' || code[0] == 'N') {
            rs = 'Tháng';
        }
        else if (code[0] == 'Y') {
            rs = 'Năm';
        }
        return rs;
    }
    function GetCookie(cookieName) {
        const name = cookieName + "=";
        const decodedCookie = decodeURIComponent(document.cookie);
        const cookieArray = decodedCookie.split(';');
        for (let i = 0; i < cookieArray.length; i++) {
            let cookie = cookieArray[i].trim();
            if (cookie.indexOf(name) === 0) {
                return cookie.substring(name.length, cookie.length);
            }
        }
        return null; // Return null if the cookie is not found
    }
    function GetUserInfo() {
        return localStorage.getItem('UcUserInfo');
    }
    function GetAccessToken() {
        let userInfo = JSON.parse(localStorage.getItem('UcUserInfo'));
        if (userInfo) {
            return userInfo.accessToken;
        }
        return '';
    }
    function GetUserId() {
        let userInfo = JSON.parse(localStorage.getItem('UcUserInfo'));
        if (userInfo) {
            return userInfo.userId;
        }
        return '';
    }
    function GetUserName() {
        let userInfo = JSON.parse(localStorage.getItem('UcUserInfo'));
        if (userInfo) {
            return userInfo.userName;
        }
        return '';
    }
    function SetUserInfo(data) {
        localStorage.setItem('UcUserInfo', JSON.stringify(data));
    }
    function GetLangLabel(code) {
        let oLang = JSON.parse(localStorage.getItem('UcLang'));
        return oLang[code];
    }
    function LoadLangLabel(current_lang) {
        UcAjax.get(root_url_web + "json/" + client_site + "/" + current_lang + "/config/data/label.json")
            .done((data) => {
                localStorage.setItem('UcLang', JSON.stringify(data));
            })
            .catch((error) => {

            });
    }
    function TruncateString(str, num) {
        if (str) {
            if (str.length > num) {
                return str.slice(0, num) + "...";
            } else {
                return str;
            }
        }
        else {
            return str;
        }
    }
    function NumberToMoney(num) {
        return parseInt(num).toLocaleString('vi', { style: 'currency', currency: 'VND' });
    }
    function NumberToRoman(num) {
        const romanNumerals = [
            { value: 1000, numeral: 'M' },
            { value: 900, numeral: 'CM' },
            { value: 500, numeral: 'D' },
            { value: 400, numeral: 'CD' },
            { value: 100, numeral: 'C' },
            { value: 90, numeral: 'XC' },
            { value: 50, numeral: 'L' },
            { value: 40, numeral: 'XL' },
            { value: 10, numeral: 'X' },
            { value: 9, numeral: 'IX' },
            { value: 5, numeral: 'V' },
            { value: 4, numeral: 'IV' },
            { value: 1, numeral: 'I' }
        ];

        let result = '';
        for (let i = 0; i < romanNumerals.length; i++) {
            while (num >= romanNumerals[i].value) {
                result += romanNumerals[i].numeral;
                num -= romanNumerals[i].value;
            }
        }
        return result;
    }
    function base64ToBlob(base64, mimeType = '') {
        const byteCharacters = atob(base64);
        const byteArrays = [];

        for (let i = 0; i < byteCharacters.length; i += 512) {
            const slice = byteCharacters.slice(i, i + 512);
            const byteNumbers = new Array(slice.length);
            for (let j = 0; j < slice.length; j++) {
                byteNumbers[j] = slice.charCodeAt(j);
            }
            byteArrays.push(new Uint8Array(byteNumbers));
        }

        return new Blob(byteArrays, { type: mimeType });
    }

    return {
        GetCookie,
        GetFrequencyTextByCode,
        GetUserInfo,
        SetUserInfo,
        LoadLangLabel,
        GetLangLabel,
        GetUserId,
        GetUserName,
        GetAccessToken,
        TruncateString,
        NumberToMoney,
        NumberToRoman,
        base64ToBlob
    };
}