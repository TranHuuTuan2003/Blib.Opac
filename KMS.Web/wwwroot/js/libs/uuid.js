var Uuid = (function () {
    function Uuidv4() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'
            .replace(/[xy]/g, function (c) {
                const r = Math.random() * 16 | 0,
                    v = c == 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
    }

    function sha1(input) {
        const utf8 = new TextEncoder().encode(input);
        return crypto.subtle.digest('SHA-1', utf8).then((hashBuffer) => {
            const hashArray = Array.from(new Uint8Array(hashBuffer));
            const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join('');
            return hashHex;
        });
    }

    function Uuidv5(name, namespace) {
        return sha1(namespace + name).then((hash) => {
            let uuid = hash.substring(0, 8) + '-' +
                hash.substring(8, 12) + '-5' +
                hash.substring(13, 16) + '-' +
                (parseInt(hash.substring(16, 18), 16) & 0x3f | 0x80).toString(16) +
                hash.substring(18, 20) + '-' +
                hash.substring(20, 32);
            return uuid;
        });
    }

    function Uuidv6() {
        const date = new Date().getTime();
        const timestamp = (date - new Date('1582-10-15').getTime()) * 10000;

        const timeLow = (timestamp & 0xffffffff).toString(16).padStart(8, '0');
        const timeMid = ((timestamp >> 32) & 0xffff).toString(16).padStart(4, '0');
        const timeHighAndVersion = ((timestamp >> 48) & 0x0fff | 0x6000).toString(16).padStart(4, '0');
        const clockSeq = (Math.random() * 0x3fff | 0x8000).toString(16).padStart(4, '0');
        const node = Array.from(crypto.getRandomValues(new Uint8Array(6))).map(b => b.toString(16).padStart(2, '0')).join('');

        const uuid = `${timeHighAndVersion}-${timeMid}-${timeLow}-${clockSeq}-${node}`;
        return uuid;
    }

    return { Uuidv4, Uuidv5, Uuidv6 }
})