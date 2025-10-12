var DateUtil = function () {
    function CompareDatesDDMMYYYY(date1, date2, charSplit) {
        function parseDate(input) {
            let parts = input.split(charSplit);
            let year = parseInt(parts[2]);
            let month = parseInt(parts[1] - 1);
            let date = parseInt(parts[0]);
            return new Date(year, month, date);
        }

        let d1 = parseDate(date1);
        let d2 = parseDate(date2);

        if (d1 > d2) {
            return 1;
        } else if (d1 < d2) {
            return -1;
        } else {
            return 0;
        }
    }

    function ParseISOToDDMMYYYY(iValue, charSplit) {
        if (!iValue) return "";
        let [year, month, day] = iValue.split("-");
        return `${day.split("T")[0]}${charSplit}${month}${charSplit}${year}`;
    }

    function ParseISOToDDMMYYYY_HHMM(iValue, charSplit) {
        if (!iValue) return "";
        let [year, month, day] = iValue.split("-");
        let timePart = day.split("T")[1];
        let [hour, minute] = timePart.split(":");
        return `${
            day.split("T")[0]
        }${charSplit}${month}${charSplit}${year} ${hour}:${minute}`;
    }

    function ParseDDMMYYYYToISO(iValue, charSplit) {
        if (iValue) {
            let dateParts = iValue.split(charSplit);
            let dateObject = new Date(
                dateParts[2],
                dateParts[1] - 1,
                dateParts[0],
                0,
                0,
                0
            );
            dateObject.setHours(dateObject.getHours() + 7);
            let oValue = dateObject.toISOString().split("T")[0];
            return oValue;
        }
        return "";
    }

    function ParseDDMMYYYYToDate(iValue, charSplit) {
        if (iValue) {
            let dateParts = iValue.split(charSplit);
            let date = new Date(
                dateParts[2],
                dateParts[1] - 1,
                dateParts[0],
                0,
                0,
                0
            );
            return date;
        }
        return "";
    }

    function ParseDDMMYYYY_HHMM_ToISO(iValue, charSplit) {
        if (iValue) {
            let dateParts = iValue.split(charSplit);
            let hours = 0,
                minutes = 0;
            if (dateParts.length == 4) {
                let timeParts = dateParts[3].split(":");
                hours = timeParts[0];
                minutes = timeParts[1];
            }
            let dateObject = new Date(
                dateParts[2],
                dateParts[1] - 1,
                dateParts[0],
                hours,
                minutes,
                0
            );
            dateObject.setHours(dateObject.getHours() + 7);
            let oValue = dateObject.toISOString();
            return oValue;
        }
        return "";
    }

    function FormatDateToDDMMYYYY_HHMM(dateString, charSplit) {
        var date = new Date(dateString);
        date.setHours(date.getHours() - 7);
        var day = String(date.getDate()).padStart(2, "0");
        var month = String(date.getMonth() + 1).padStart(2, "0");
        var year = date.getFullYear();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        charSplit = "/";
        return [
            `${day}${charSplit}${month}${charSplit}${year}`,
            hours + ":" + minutes,
        ];
    }

    function FormatDateToDDMMYYYY(dateString, charSplit) {
        var date = new Date(dateString);
        var day = String(date.getDate()).padStart(2, "0");
        var month = String(date.getMonth() + 1).padStart(2, "0");
        var year = date.getFullYear();
        charSplit = "/";
        return `${day}${charSplit}${month}${charSplit}${year}`;
    }

    function FormatDateNowToDDMMYYYY(charSplit) {
        var date = new Date();
        var day = String(date.getDate()).padStart(2, "0");
        var month = String(date.getMonth() + 1).padStart(2, "0");
        var year = date.getFullYear();
        charSplit = "/";
        return `${day}${charSplit}${month}${charSplit}${year}`;
    }

    function GetFirstDateOfQuarterToDDMMYYYY(date, charSplit) {
        const gmonth = date.getMonth();

        const firstMonthOfQuarter = Math.floor(gmonth / 3) * 3;

        var date = new Date(date.getFullYear(), firstMonthOfQuarter, 1);

        var day = String(date.getDate()).padStart(2, "0");
        var month = String(date.getMonth() + 1).padStart(2, "0");
        var year = date.getFullYear();
        charSplit = "/";
        return `${day}${charSplit}${month}${charSplit}${year}`;
    }

    function GetLastDateOfQuarterToDDMMYYYY(date, charSplit) {
        const gmonth = date.getMonth();

        const lastMonthOfQuarter = Math.floor(gmonth / 3) * 3 + 2;

        var date = new Date(date.getFullYear(), lastMonthOfQuarter + 1, 0);

        var day = String(date.getDate()).padStart(2, "0");
        var month = String(date.getMonth() + 1).padStart(2, "0");
        var year = date.getFullYear();
        charSplit = "/";
        return `${day}${charSplit}${month}${charSplit}${year}`;
    }

    function GetQuarterNumber(date) {
        const month = date.getMonth();
        return Math.floor(month / 3) + 1;
    }

    function IsValidDDMMYYYY(dateString) {
        var regexDate = /^\d{2}\/\d{2}\/\d{4}$/;
        if (!regexDate.test(dateString)) {
            return false;
        }

        var parts = dateString.split("/");
        var day = parseInt(parts[0], 10);
        var month = parseInt(parts[1], 10);
        var year = parseInt(parts[2], 10);
        var date = new Date(year, month - 1, day);
        return (
            date.getMonth() + 1 === month &&
            date.getDate() === day &&
            date.getFullYear() === year
        );
    }

    function IsValidISO(dateString) {
        const regexISO = /^\d{4}-\d{2}-\d{2}(?:T\d{2}:\d{2}:\d{2})?$/;

        if (!regexISO.test(dateString)) {
            return false;
        }

        // Tách phần ngày và thời gian nếu có
        const [datePart, timePart] = dateString.split("T");
        const [year, month, day] = datePart.split("-").map(Number);

        const date = new Date(year, month - 1, day);
        if (
            date.getFullYear() !== year ||
            date.getMonth() + 1 !== month ||
            date.getDate() !== day
        ) {
            return false;
        }

        if (!timePart) {
            return true;
        }

        const [hours, minutes, seconds] = timePart.split(":").map(Number);
        return (
            hours >= 0 &&
            hours <= 23 &&
            minutes >= 0 &&
            minutes <= 59 &&
            seconds >= 0 &&
            seconds <= 59
        );
    }

    function CalculateAge(birthDateString) {
        var birthDate = ParseDDMMYYYYToISO(birthDateString, "/");
        birthDate = new Date(birthDate);
        const today = new Date();

        let age = today.getFullYear() - birthDate.getFullYear();

        const monthDiff = today.getMonth() - birthDate.getMonth();
        const dayDiff = today.getDate() - birthDate.getDate();

        if (monthDiff < 0 || (monthDiff === 0 && dayDiff < 0)) {
            age--;
        }

        return age;
    }

    return {
        ParseDDMMYYYYToDate,
        GetFirstDateOfQuarterToDDMMYYYY,
        GetLastDateOfQuarterToDDMMYYYY,
        GetQuarterNumber,
        CompareDatesDDMMYYYY,
        ParseISOToDDMMYYYY,
        ParseISOToDDMMYYYY_HHMM,
        ParseDDMMYYYYToISO,
        ParseDDMMYYYY_HHMM_ToISO,
        FormatDateToDDMMYYYY_HHMM,
        FormatDateToDDMMYYYY,
        FormatDateNowToDDMMYYYY,
        IsValidDDMMYYYY,
        IsValidISO,
        CalculateAge
    };
};
