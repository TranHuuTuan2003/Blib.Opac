var UcFormHelpers = function () {
    function Select2LoadData(selector, data) {
        $(selector).select2({
            dropdownParent: $(selector).parent(),
            multiple: true,
            allowClear: true,
            data: data,
        });
    }

    function DrawOptionsSelect(
        data,
        value = "id",
        label = "name",
        type,
        dataAttributes = {}
    ) {
        let html = "";

        if (type === "select") {
            html += '<option value="">---Chọn---</option>';
        } else if (type === "all") {
            html += '<option value="all">---Tất cả---</option>';
        }

        if (Array.isArray(data)) {
            for (let i = 0; i < data.length; i++) {
                const item = data[i];
                let attrs = "";

                if (
                    typeof dataAttributes === "object" &&
                    dataAttributes !== null &&
                    Object.keys(dataAttributes).length > 0
                ) {
                    for (let key in dataAttributes) {
                        const itemField = dataAttributes[key];
                        if (item.hasOwnProperty(itemField)) {
                            let attrValue = item[itemField];
                            if (Array.isArray(attrValue)) {
                                attrValue = attrValue.join(",");
                            }
                            attrs += ` data-${key}="${attrValue}"`;
                        }
                    }
                }

                html += `<option value="${item[value]}"${attrs}>${item[label]}</option>`;
            }
        }

        return html;
    }

    function DrawOptionsSelectWithApi(
        selectors = [],
        endPoint,
        key = "id",
        value = "name",
        type = "select",
        isSort = true,
        configAjax = {}
    ) {
        return $.ajax({
            url: base_url_api + endPoint,
            type: "GET",
            headers: {
                Authorization: "Bearer " + new UcHelpers().GetAccessToken(),
            },
            success: (response) => {
                if (!response.data || response.data.length === 0) {
                    return;
                }

                if (
                    response.data[0][key] == null ||
                    response.data[0][value] == null
                ) {
                    return;
                }

                const sortedData = isSort
                    ? response.data.sort((a, b) =>
                          a[value].localeCompare(b[value], "vi", {
                              sensitivity: "accent",
                          })
                      )
                    : response.data;
                selectors.forEach((selector) => {
                    $(selector).append(
                        DrawOptionsSelect(sortedData, key, value, type)
                    );
                });
            },
            ...configAjax,
            error: (response) =>
                console.error("Lỗi khi lấy dữ liệu:", response),
        });
    }

    function DataTableDropdown(row, id, column) {
        let defaultContent = column.defaultContent;
        let dataAttributes = column.dataAttributes;
        let attributes = "";
        if (dataAttributes) {
            for (let i = 0; i < dataAttributes.length; i++) {
                if (row.hasOwnProperty(dataAttributes[i])) {
                    attributes +=
                        "data-" +
                        dataAttributes[i] +
                        "='" +
                        row[dataAttributes[i]] +
                        "'";
                }
            }
        }

        var actionHtml = "";
        let did = "",
            code = "",
            type = "",
            title = "",
            className = "",
            classNameIcon = "";
        actionHtml += '<div class="text-end">';
        actionHtml += '<div class="dropdown dropdown-action">';
        actionHtml +=
            '<a class="action-icon dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false"><i class="material-icons">more_vert</i></a>';
        actionHtml += '<div class="dropdown-menu dropdown-menu-right">';
        if (defaultContent && defaultContent.length) {
            for (let i = 0; i < defaultContent.length; i++) {
                did = defaultContent[i].id;
                code = defaultContent[i].code;
                type = defaultContent[i].type;
                title = defaultContent[i].title;
                className = defaultContent[i].className;
                classNameIcon = defaultContent[i].classNameIcon;

                if (type == "button") {
                    actionHtml += "<a ";

                    if (did) {
                        actionHtml += 'id="' + did + '" ';
                    }

                    if (attributes) {
                        actionHtml += attributes;
                    }

                    if (id) {
                        actionHtml += ' data-id="' + id + '"';
                    }

                    actionHtml +=
                        ' class="dropdown-item ' +
                        className +
                        '"><i class="' +
                        classNameIcon +
                        '"></i>' +
                        title +
                        "</a>";
                } else if (type == "modal") {
                    actionHtml += "<a ";

                    if (did) {
                        actionHtml += 'id="' + did + '" ';
                    }

                    if (attributes) {
                        actionHtml += attributes;
                    }

                    if (id) {
                        actionHtml += ' data-id="' + id + '"';
                    }

                    actionHtml +=
                        ' class="dropdown-item ' +
                        className +
                        '" data-bs-toggle="modal" data-bs-target="#modal' +
                        code +
                        '"><i class="' +
                        classNameIcon +
                        '"></i>' +
                        title +
                        "</a>";
                }
            }
        }

        actionHtml += "</div>";
        actionHtml += "</div>";
        actionHtml += "</div>";

        return actionHtml;
    }

    function DataTableButton(row, id, column) {
        let defaultContent = column.defaultContent;
        let dataAttributes = column.dataAttributes;
        let attributes = "";
        if (dataAttributes) {
            for (let i = 0; i < dataAttributes.length; i++) {
                if (row.hasOwnProperty(dataAttributes[i])) {
                    attributes +=
                        "data-" +
                        dataAttributes[i] +
                        "='" +
                        row[dataAttributes[i]] +
                        "'";
                }
            }
        }

        var actionHtml = "";
        let code = "",
            type = "",
            title = "",
            className = "",
            classNameIcon = "";
        if (defaultContent && defaultContent.length) {
            for (let i = 0; i < defaultContent.length; i++) {
                code = defaultContent[i].code;
                type = defaultContent[i].type;
                title = defaultContent[i].title;
                className = defaultContent[i].className;
                classNameIcon = defaultContent[i].classNameIcon;

                if (type == "button") {
                    actionHtml +=
                        "<a " +
                        attributes +
                        ' data-id="' +
                        id +
                        '" class="' +
                        className +
                        '"><i class="' +
                        classNameIcon +
                        '"></i></a>';
                } else if (type == "modal") {
                    actionHtml +=
                        "<a " +
                        attributes +
                        ' data-bs-toggle="modal" data-bs-target="#modal' +
                        code +
                        '" data-id="' +
                        id +
                        '" class="dropdown-item ' +
                        className +
                        '"><i class="' +
                        classNameIcon +
                        '"></i>' +
                        title +
                        "</a>";
                }
            }
        }

        return actionHtml;
    }

    function DataTableConfigRender(columns) {
        let nColumns = [];
        let orders = [];
        let orderTypes = [];
        let className = "";
        let action = false;
        for (let i = 0; i < columns.length; i++) {
            className = columns[i].className;

            if (columns[i].hasOwnProperty("order")) {
                if (columns[i].order) {
                    orders.push([i, columns[i].order]);
                }
            }
            if (columns[i].hasOwnProperty("orderType")) {
                if (columns[i].orderType) {
                    orderTypes.push({ type: columns[i].orderType, targets: i });
                }
            }
            if (columns[i].hasOwnProperty("type")) {
                if (columns[i].type == "dropdown") {
                    action = true;
                    nColumns.push({
                        orderable: false,
                        width: "1%",
                        render: function (d, t, r, m) {
                            return DataTableDropdown(r, r.id, columns[i]);
                        },
                    });
                } else if (columns[i].type == "button") {
                    action = true;
                    nColumns.push({
                        orderable: false,
                        width: "1%",
                        render: function (d, t, r, m) {
                            return DataTableButton(r, r.id, columns[i]);
                        },
                    });
                }
            } else {
                if (columns[i].colType.includes("text")) {
                    className += " text-start";
                } else if (columns[i].colType.includes("date")) {
                    className += " text-center";
                } else if (columns[i].colType.includes("number")) {
                    className += " text-end";
                } else if (columns[i].colType.includes("number_money")) {
                    className += " text-end";
                } else if (columns[i].colType.includes("badges")) {
                    className += " text-center";
                }

                if (columns[i].colType.includes("text_link")) {
                    nColumns.push({
                        ...columns[i],
                        className: className,
                        orderable: true,
                        render: function (d, t, r, m) {
                            return (
                                '<a data-id="' +
                                r.id +
                                '" class="link ' +
                                columns[i].data +
                                '">' +
                                d +
                                "</a>"
                            );
                        },
                    });
                } else if (columns[i].colType.includes("images")) {
                    nColumns.push({
                        ...columns[i],
                        className: className,
                        orderable: true,
                        render: function (d, t, r, m) {
                            return (
                                '<image class="table-img" src="' +
                                columns[i].data +
                                '" />'
                            );
                        },
                    });
                } else if (columns[i].colType.includes("input_text")) {
                    nColumns.push({
                        ...columns[i],
                        className: className,
                        orderable: true,
                        render: function (d, t, r, m) {
                            return (
                                "<input " +
                                (columns[i].disabled == true
                                    ? "disabled"
                                    : "") +
                                ' type="text" class="form-control form-control-sm ' +
                                columns[i].data +
                                '" value="' +
                                d +
                                '" />'
                            );
                        },
                    });
                } else if (columns[i].colType.includes("input_number")) {
                    nColumns.push({
                        ...columns[i],
                        className: className,
                        orderable: true,
                        render: function (d, t, r, m) {
                            return (
                                "<input " +
                                (columns[i].disabled == true
                                    ? "disabled"
                                    : "") +
                                ' type="number" class="form-control form-control-sm ' +
                                columns[i].data +
                                '" value="' +
                                d +
                                '" />'
                            );
                        },
                    });
                } else if (columns[i].colType.includes("input_date")) {
                    nColumns.push({
                        ...columns[i],
                        className: className,
                        orderable: true,
                        render: function (d, t, r, m) {
                            return (
                                "<input " +
                                (columns[i].disabled == true
                                    ? "disabled"
                                    : "") +
                                ' type="text" placeholder="dd/mm/yyyy" class="form-control form-control-sm datetimepicker ' +
                                columns[i].data +
                                '" value="' +
                                d +
                                '" />'
                            );
                        },
                    });
                } else if (columns[i].colType.includes("number_money")) {
                    nColumns.push({
                        ...columns[i],
                        className: className,
                        orderable: true,
                        render: function (d, t, r, m) {
                            return new UcHelpers().NumberToMoney(d);
                        },
                    });
                } else if (columns[i].colType.includes("checkbox")) {
                    className += " text-center";

                    nColumns.push({
                        ...columns[i],
                        orderable: false,
                        className: className,
                        render: function (d, t, r, m) {
                            return `<label class="custom_check me-1 c-pointer">
  										<input type="checkbox">
  										<span class="checkmark border-secondary"></span>
  									</label>`;
                        },
                    });
                } else if (columns[i].colType.includes("checkall")) {
                    className += " text-center";

                    nColumns.push({
                        ...columns[i],
                        orderable: false,
                        className: className,
                        title: `<label class="custom_check me-1 c-pointer">
                                    <input type="checkbox" name="rememberme" class="rememberme check_all_item">
                                    <span class="checkmark border-secondary"></span>
                                </label>`,
                        render: function (d, t, r, m) {
                            return `<label class="custom_check me-1 c-pointer">
  										<input type="checkbox" name="rememberme" class="rememberme" data-id="${r.id}">
  										<span class="checkmark border-secondary"></span>
  									</label>`;
                        },
                    });
                } else if (columns[i].colType.includes("custom")) {
                    nColumns.push({
                        ...columns[i],
                        className: className,
                    });
                } else {
                    nColumns.push({
                        ...columns[i],
                        className: className,
                        orderable: true,
                        render: function (d, t, r, m) {
                            if (columns[i].hasOwnProperty("options")) {
                                options = columns[i].options;
                                if (columns[i].options) {
                                    for (
                                        let j = 0;
                                        j < columns[i].options.length;
                                        j++
                                    ) {
                                        if (columns[i].options[j].value == d) {
                                            return (
                                                '<span class="badge rounded-pill bg-' +
                                                columns[i].options[j].type +
                                                '">' +
                                                columns[i].options[j].label +
                                                "</span>"
                                            );
                                        }
                                    }
                                }
                            }
                            if (columns[i].colType.includes("date")) {
                                if (d && typeof d == "string") {
                                    if (d.includes("T")) {
                                        if (
                                            columns[i].colType.includes(
                                                "datetime"
                                            )
                                        ) {
                                            return new DateUtil().ParseISOToDDMMYYYY_HHMM(
                                                d,
                                                "/"
                                            );
                                        } else {
                                            return new DateUtil().ParseISOToDDMMYYYY(
                                                d,
                                                "/"
                                            );
                                        }
                                    }
                                }
                                return d;
                            }
                            return d;
                        },
                    });
                }
            }
        }
        return { nColumns, orders, action, orderTypes };
    }

    function DataTableAjaxClientSide(
        code,
        columns,
        data,
        extConfig = {},
        disabledFixedColumns = true,
        autoWidth = false
    ) {
        let dt;
        if ($.fn.DataTable.isDataTable(code)) {
            $(code).DataTable().clear().destroy();
            $(code).empty();
        }

        let config = DataTableConfigRender(columns);
        let configDataTable = {
            data: data,
            order: config.orders,
            columns: config.nColumns,
            columnDefs: [
                { className: "text-center", targets: "_all" },
                ...config.orderTypes,
            ],
            autoWidth: autoWidth,
            ...extConfig,
        };

        if (config.action && disabledFixedColumns) {
            configDataTable.fixedColumns = {
                leftColumns: -1,
                rightColumns: 1,
            };
        }

        dt = $(code).DataTable({
            ...configDataTable,
        });

        $(code).css("width", "100%");

        return dt;
    }

    function DataTableEmptyAjaxClientSide(
        code,
        columns,
        data,
        extConfig = {},
        disabledFixedColumns = true,
        autoWidth = false
    ) {
        let dt;
        if ($.fn.DataTable.isDataTable(code)) {
            $(code).DataTable().clear().destroy();
            $(code).empty();
        }

        let config = DataTableConfigRender(columns);
        let configDataTable = {
            data: data,
            order: config.orders,
            columns: config.nColumns,
            columnDefs: [
                { className: "text-center", targets: "_all" },
                ...config.orderTypes,
            ],
            autoWidth: autoWidth,
            ...extConfig,
        };

        if (config.action && disabledFixedColumns) {
            configDataTable.fixedColumns = {
                leftColumns: -1,
                rightColumns: 1,
            };
        }

        dt = $(code).DataTable({
            ...configDataTable,
        });

        $(code).css("width", "100%");

        return dt;
    }

    function DataTableAjaxServerSide(
        code,
        url,
        columns,
        sField,
        extConfig = {},
        disabledFixedColumns = true,
        autoWidth = false
    ) {
        let dt;
        if ($.fn.DataTable.isDataTable(code)) {
            $(code).DataTable().clear().destroy();
        }
        let config = DataTableConfigRender(columns);
        let configDataTable = {
            autoWidth: autoWidth,
            searching: false,
            processing: true,
            serverSide: true,
            order: config.orders,
            ajax: {
                url: url,
                type: "POST",
                headers: {
                    Authorization: "Bearer " + new UcHelpers().GetAccessToken(),
                },
                data: function (d) {
                    return $.extend({}, d, {
                        oSearch: JSON.stringify(sField),
                    });
                },
                dataSrc: function (json) {
                    if (!json || !json.data) {
                        console.error("Invalid JSON response:", json);
                        return [];
                    }
                    return json.data;
                },
            },
            columns: config.nColumns,
            columnDefs: [{ className: "text-center", targets: "_all" }],
            ...extConfig,
        };

        if (config.action && disabledFixedColumns) {
            configDataTable.fixedColumns = {
                leftColumns: -1,
                rightColumns: 1,
            };
        }

        dt = $(code).DataTable({
            ...configDataTable,
        });

        $(code).css("width", "100%");

        return dt;
    }

    function RefreshFormValues(form_code, ignore_ids = []) {
        var elements = document.querySelectorAll(
            "." + form_code + " .form-control-sm"
        );
        let id, type, element, multiple;
        for (let i = 0; i < elements.length; i++) {
            element = elements[i];
            id = element.getAttribute("id");
            if (ignore_ids.includes(id)) {
                continue;
            }
            type = element.getAttribute("type");
            multiple = element.getAttribute("multiple");

            if (type == "text" || type == "password" || type == "hidden") {
                $("." + form_code + " #" + id).val("");
            }
            if (type == "number") {
                $("." + form_code + " #" + id).val("");
            }
            if (type == "checkbox") {
                $("." + form_code + " #" + id).prop("checked", false);
            }
            if (type == "textarea") {
                $("." + form_code + " #" + id).val("");
            }
            if (type == "select") {
                if (id.includes("sel_")) {
                    $("." + form_code + " #" + id)
                        .prop("selectedIndex", 0)
                        .trigger("change");
                } else if (id.includes("sel2_")) {
                    $("." + form_code + " #" + id)
                        .select2(
                            select2_config_search(
                                "." + form_code + " #" + id,
                                multiple == "" ? true : false
                            )
                        )
                        .val("")
                        .trigger("change");
                }
            }
            if (type == "editor") {
                window[form_code + "_" + id].setData("");
            }
        }
        if (typeof window["UcLoadDefaultValue_" + form_code] === "function") {
            window["UcLoadDefaultValue_" + form_code]();
        }
    }

    function FormFieldsToObject(form_fields) {
        const value = form_fields.fields.reduce((fields, item) => {
            fields[item.code] = item.value;
            return fields;
        }, {});

        return value;
    }

    function SetFormValues(form_code, values) {
        var elements = document.querySelectorAll(
            "." + form_code + " .form-control-sm"
        );
        let code = "";
        for (let i = 0; i < elements.length; i++) {
            let id = elements[i].getAttribute("id");
            let type = elements[i].getAttribute("type");

            code = "";
            if (type == "text" || type == "password") {
                if (id.includes("txt_")) {
                    code = id.replace("txt_", "");
                } else if (id.includes("txt2_")) {
                    code = id.replace("txt2_", "");
                } else if (id.includes("num_")) {
                    code = id.replace("num_", "");
                } else if (id.includes("pwd_")) {
                    code = id.replace("pwd_", "");
                } else if (id.includes("dap_")) {
                    code = id.replace("dap_", "");
                } else if (id.includes("dtp_")) {
                    code = id.replace("dtp_", "");
                } else if (id.includes("dat_")) {
                    code = id.replace("dat_", "");
                } else if (id.includes("tmp_")) {
                    code = id.replace("tmp_", "");
                }
            } else if (type == "textarea") {
                if (id.includes("txa_")) {
                    code = id.replace("txa_", "");
                }
            } else if (type == "select") {
                if (id.includes("sel_")) {
                    code = id.replace("sel_", "");
                } else if (id.includes("sel2_")) {
                    code = id.replace("sel2_", "");
                } else if (id.includes("drp_")) {
                    code = id.replace("drp_", "");
                }
            } else if (type == "hidden") {
                if (id.includes("hd_")) {
                    code = id.replace("hd_", "");
                }
            } else if (type == "editor") {
                if (id.includes("cke_")) {
                    code = id.replace("cke_", "");
                }
            } else if (type == "checkbox") {
                if (id.includes("chk_")) {
                    code = id.replace("chk_", "");
                }
            } else if (type == "number") {
                if (id.includes("num_")) {
                    code = id.replace("num_", "");
                }
            } else {
                code = id;
            }

            for (var key of Object.keys(values)) {
                if (key == code) {
                    let value = values[key];

                    if (id.includes("dap_")) {
                        if (value) {
                            $("." + form_code + " #" + id).val(
                                new DateUtil().FormatDateToDDMMYYYY(value, "/")
                            );
                        } else {
                            $("." + form_code + " #" + id).val("");
                        }
                    } else if (id.includes("chk_")) {
                        $("." + form_code + " #" + id).prop("checked", value);
                    } else if (id.includes("sel2_")) {
                        $("." + form_code + " #" + id)
                            .select2(
                                select2_config_search(
                                    "." + form_code + " #" + id
                                )
                            )
                            .val(value)
                            .trigger("change");
                    } else if (id.includes("sel_")) {
                        $("." + form_code + " #" + id).val(value);
                    } else if (id.includes("drp_")) {
                        $("." + form_code + " #" + id).attr("value", value);
                    } else if (id.includes("cke_")) {
                        window[form_code + "_" + id].setData(value);
                    } else if (id.includes("dat_")) {
                        value = new DateUtil().FormatDateToDDMMYYYY_HHMM(
                            value,
                            "/"
                        );
                        $("." + form_code + " #" + id).val(value[0]);
                        if (value[1] != "0:0" && value[1] != "00:00") {
                            $(
                                "." +
                                    form_code +
                                    " #" +
                                    id.replace("dat_", "dat2_")
                            ).val(value[1]);
                        }
                    } else {
                        $("." + form_code + " #" + id).val(value);
                    }
                    break;
                }
            }
        }
    }

    function GetFormValues(form_code) {
        var oForm = {
            fields: [],
        };

        var elements = document.querySelectorAll(
            "." + form_code + " .form-control-sm"
        );
        let val = "";
        let code = "";

        for (let i = 0; i < elements.length; i++) {
            let id = elements[i].getAttribute("id");
            let type = elements[i].getAttribute("type");
            val = "";
            code = "";
            if (type == "text" || type == "password") {
                val = $("." + form_code + " #" + id).val();
                if (id.includes("txt_")) {
                    code = id.replace("txt_", "");
                } else if (id.includes("txt2_")) {
                    code = id.replace("txt2_", "");
                } else if (id.includes("num_")) {
                    code = id.replace("num_", "");
                } else if (id.includes("pwd_")) {
                    code = id.replace("pwd_", "");
                } else if (id.includes("txa_")) {
                    code = id.replace("txa_", "");
                } else if (id.includes("dap_")) {
                    code = id.replace("dap_", "");
                } else if (id.includes("dtp_")) {
                    code = id.replace("dtp_", "");
                } else if (id.includes("dat_")) {
                    code = id.replace("dat_", "");
                    if ($("." + form_code + " #" + "dat2_" + code).val()) {
                        val +=
                            "/" +
                            $("." + form_code + " #" + "dat2_" + code).val();
                    }
                } else if (id.includes("tmp_")) {
                    code = id.replace("tmp_", "");
                }
            } else if (type == "textarea") {
                val = $("." + form_code + " #" + id).val();
                if (id.includes("txa_")) {
                    code = id.replace("txa_", "");
                }
            } else if (type == "select") {
                if (id.includes("sel_")) {
                    val = $("." + form_code + " #" + id).val();
                    code = id.replace("sel_", "");
                } else if (id.includes("drp_")) {
                    val = $("." + form_code + " #" + id).attr("value");
                    code = id.replace("drp_", "");
                } else if (id.includes("sel2_")) {
                    val = $("." + form_code + " #" + id).val();
                    code = id.replace("sel2_", "");
                    if (val == null || val.length == 0) {
                        val = null;
                    }
                }
            } else if (type == "editor") {
                val = window[form_code + "_" + id].getData();
                if (id.includes("cke_")) {
                    code = id.replace("cke_", "");
                }
            } else if (type == "hidden") {
                let _typeof = elements[i].getAttribute("data-typeof");
                val = $("." + form_code + " #" + id).val();

                if (_typeof == "string") {
                    val = String(val);
                } else if (_typeof == "number") {
                    val = parseInt(val);
                } else if (_typeof == "bool") {
                    if (val == "true") {
                        val = true;
                    } else {
                        val = false;
                    }
                }

                if (id.includes("hd_")) {
                    code = id.replace("hd_", "");
                }
            } else if (type == "checkbox") {
                let _typeof = elements[i].getAttribute("data-typeof");
                val = $("." + form_code + " #" + id).is(":checked");

                if (_typeof == "string") {
                    if (val) {
                        val = "true";
                    } else {
                        val = "false";
                    }
                } else if (_typeof == "strNumber") {
                    if (val) {
                        val = "1";
                    } else {
                        val = "0";
                    }
                } else if (_typeof == "number") {
                    if (val) {
                        val = 1;
                    } else {
                        val = 0;
                    }
                } else if (_typeof == "bool") {
                    if (val) {
                        val = true;
                    } else {
                        val = false;
                    }
                }

                if (id.includes("chk_")) {
                    code = id.replace("chk_", "");
                }
            } else if (type == "number") {
                val = $("." + form_code + " #" + id).val();
                if (id.includes("num_")) {
                    code = id.replace("num_", "");
                }
            } else if (type == "textarea") {
                val = $("." + form_code + " #" + id).val();
                if (id.includes("txa_")) {
                    code = id.replace("txa_", "");
                }
            }

            if (val || val === false) {
                oForm.fields.push({
                    code: code,
                    value: val,
                });
            } else {
                if ($("." + form_code + " ." + code + "_required").length > 0) {
                    Toast().ShowToastWarning(
                        $("." + form_code + " label.ctrl_" + code)
                            .text()
                            .replace(" *", "") + " không được để trống"
                    );

                    scrollToControl(id);

                    return null;
                }
            }
        }
        return oForm;
    }

    function scrollToControl(code) {
        document.getElementById(`${code}`).scrollIntoView({
            behavior: "smooth",
            block: "start",
        });
        $("#" + code).focus();
    }

    function CheckValidatedFormValues(form_code) {
        var inputs = $("." + form_code).find("input");
        let code = "";
        let dataType = "";

        for (let i = 0; i < inputs.length; i++) {
            code = inputs.eq(i).attr("id").replace("is_validated_", "");
            if (inputs.eq(i).val() === "false") {
                Toast().ShowToastWarning(
                    "Vui lòng nhập đúng định dạng " +
                        $("label.ctrl_" + code)
                            .text()
                            .replace(" *", "")
                );

                dataType = inputs.eq(i).attr("data-type");
                code = dataType + "_" + code;
                scrollToControl(code);

                return false;
            }
        }

        return true;
    }

    function Compare2Objects(obj1, obj2) {
        if (obj1 === obj2) return true;

        if (
            typeof obj1 !== "object" ||
            typeof obj2 !== "object" ||
            obj1 === null ||
            obj2 === null
        ) {
            return false;
        }

        const keys1 = Object.keys(obj1);
        const keys2 = Object.keys(obj2);

        if (keys1.length !== keys2.length) return false;

        for (let key of keys1) {
            if (
                !keys2.includes(key) ||
                !Compare2Objects(obj1[key], obj2[key])
            ) {
                return false;
            }
        }

        return true;
    }

    function AddRowToTable(tableCode, data = {}, draw = false) {
        $(tableCode).DataTable().row.add(data).draw(draw);
    }

    function AddRowsToTable(tableCode, data = [], draw = false) {
        $(tableCode).DataTable().rows.add(data).draw(draw);
    }

    function UpdateRowOfTable(tableCode, idOfRow, newData, draw = false) {
        var table = $(tableCode).DataTable();
        let row = table.rows((idx, data, node) => data.id === idOfRow);
        table.row(row).data(newData).draw(draw);
    }

    function RemoveRowOfTable(tableCode, element, draw = false) {
        $(tableCode).DataTable().row(element.parents("tr")).remove().draw(draw);
    }

    function RemoveRowsOfTable(tableCode, rows) {
        $(tableCode).DataTable().rows(rows).remove().draw(false);
    }

    function RemoveAllRowsOfTable(tableCode) {
        $(tableCode).DataTable().rows().remove().draw();
    }

    return {
        DrawOptionsSelect,
        DrawOptionsSelectWithApi,
        DataTableAjaxServerSide,
        DataTableAjaxClientSide,
        DataTableEmptyAjaxClientSide,
        GetFormValues,
        RefreshFormValues,
        FormFieldsToObject,
        SetFormValues,
        CheckValidatedFormValues,
        Compare2Objects,
        Select2LoadData,
        AddRowToTable,
        AddRowsToTable,
        UpdateRowOfTable,
        RemoveRowOfTable,
        RemoveRowsOfTable,
        RemoveAllRowsOfTable,
    };
};
