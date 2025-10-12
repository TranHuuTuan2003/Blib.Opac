var authentication_method = "admin";
var sync_type = "order"; //order, receipt
var enable_identification_card = false;
var client_site = new UcHelpers().GetCookie("client_site");
var lang = new UcHelpers().GetCookie("lang");

var file_config = {
    reader_avatar: {
        ref_type: "blib_reader_avatar",
        folder_code: "blib_reader_avatar",
    },
    dl_bib_avatar: {
        ref_type: "blib_dl_bib_avatar",
        folder_code: "blib_dl_bib_avatar",
    },
    blib_file_digital: {
        ref_type: "blib_file_digital",
        folder_code: "blib_file_digital",
    },
    ck_finder: {
        ref_type: "ck_finder",
        folder_code: "ck_finder",
    },
    app_logo: {
        folder_code: "app_logo",
        ref_type: "app_logo",
    },
    banner_photo: {
        folder_code: "banner_photo",
        ref_type: "banner_photo",
    }
};

var perm_module_config = {
    role_menu: {
        class_code: "Menu",
        sid_code: "role",
    },
    group_menu: {
        class_code: "Menu",
        sid_code: "group",
    }
};

var perm_data_config = {
    role_cir_place: {
        class_code: "CirPlace",
        sid_code: "role",
    },
    group_cir_place: {
        class_code: "CirPlace",
        sid_code: "group",
    }
};

//var root_url_web = "http://ucvn.vn/blib/";
var root_url_web = "/";

var base_url_signalr = root_url_web + "signalr/";

var socket_client_key = "ucvn";

var ckfinder_upload_url =
    "http://ucvn.vn:5530/Services/Core/Fms_File/ck-upload?refType=" +
    file_config.ck_finder.ref_type +
    "&folderCode=" +
    file_config.ck_finder.folder_code;

var select2_config = {
    minimumResultsForSearch: -1,
    placeholder: "",
    allowClear: true,
};
var select2_config_search = function (code, multiple = false) {
    return {
        dropdownParent: $(code).parent(),
        placeholder: "",
        multiple: multiple,
        allowClear: true,
    };
};

var msg = null;
$.getJSON(root_url_web + "json/" + client_site + "/" + lang + "/config/data/message.json").then(response => {
    msg = response;
})