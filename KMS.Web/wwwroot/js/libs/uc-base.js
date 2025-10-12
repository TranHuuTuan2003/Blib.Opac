class UcBase {
    constructor(base_url, endPoint, jsonConfigPath) {
        this.base_url = base_url;
        this.endPoint = endPoint;
        this.jsonConfigPath = jsonConfigPath;

        this.#initializeDefaults();

        const runQueue = () => {
            this.#domReadyFired = true;
            this.#domReadyQueue.forEach((fn) => fn());
            this.#domReadyQueue = [];
        };

        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", () => {
                setTimeout(() => {
                    runQueue();
                }, 0);
            });
        } else {
            runQueue();
        }
    }

    #domReadyQueue = [];
    #domReadyFired = false;
    #isInitTable = true;

    #initializeDefaults() {
        this.tableInstance = null;
        this.isServerSideTable = false;
        this.tableData = [];
        this.tableCode = "#tbl_search";
        this.tableConfig = {};
        this.scrollToTopOnPagination = true;
        this.enableSmoothScrollToTop = true;
        this.enableLazyLoadForTable = false;
        this.lazyLoadColumnClass = [];
        this.checkboxColumnClass = [];
        this.formName = "";
        this.formCode = "";
        this.modalCode = null;
        this.oValue = {};
        this.oSearch = {};
        // this.hdKey = "#hdKey";
        this.hiddenKeyValue = null;
        this.inputFileCode = "#input_file";
        this.fileId = null;
        this.refId = null;
        this.refType = null;
        this.folderCode = null;
        this.labelCode = null;
        this.refreshTableWithoutApiInsert = false;
        this.refreshTableWithoutApiUpdate = false;
        this.refreshTableWithoutApiDelete = false;
        this.notifyWhenNotFound = false;
        this.btnSaveCode = "#btn-save";
        this.forceOverrideSaveEvent = false;
        this.btnAddCode = "#btn-add";
        this.forceOverrideAddEvent = false;
        this.btnRefreshCode = "#btn-refresh";
        this.forceOverrideRefreshEvent = false;
        this.btnGoBackCode = "#btn-go-back";
        this.forceOverrideGoBackEvent = false;
        this.btnCloseCode = "#btn-close";
        this.forceOverrideCloseEvent = false;
        this.needToBeUuid = false;
        this.needToBeDMY = [];
        this.needToBeISO = [];
        this.needToBeInt = [];
        this.needToBeEmpty = [];
        this.needToUnformatCurrency = [];
        this.defaultValueIfNone = [];
        this.selectOptions = [];
        this.promiseOptions = [];
        this.enableFixedColumn = true;
        this.jsonConfig = null;
        this.editCodeOfTable = ".action-edit";
        this.forceOverrideEditEventOfTable = false;
        this.deleteCodeOfTable = ".action-delete";
        this.forceOverrideDeleteEventOfTable = false;
        this.detailCodeOfTable = ".action-detail";
        this.forceOverrideDetailEventOfTable = false;
        this.saveCodeOfTable = ".action-save";
        this.forceOverrideSaveEventOfTable = false;
        this.queryInsert = "insert-item";
        this.queryDelete = "delete-item";
        this.queryParamsDelete = {};
        this.useQueryFormatDelete = false;
        this.queryUpdate = "update-item";
        this.queryGetItemById = "get-item-by-id";
        this.queryParamsGetItemById = {};
        this.useQueryFormatGetItemById = false;
        this.queryGetItems = "get-items";
        this.queryParamsGetItems = {};
        this.useQueryFormatGetItems = false;
        this.queryGetList = "get-items";
        this.queryParamsGetList = {};
        this.useQueryFormatGetList = false;
        this.isFormSearch = false;
        this.isAdvanceForm = false;
        this.btnSearchCode = "#btn-search";
        this.forceOverrideSearchEvent = false;
        this.querySearch = "search";
        this.queryParamsSearch = {};
        this.useQueryFormatSearch = false;
        this.queryAdvanceSearch = "search";
        this.advanceModalCode = "";
        this.advanceFormCode = "";
        this.btnAdvanceRefreshCode = "#btn-advanced-refresh";
        this.closeAdvanceModalWhenResultFound = true;
        this.forceOverrideAdvanceRefreshEvent = false;
        this.btnAdvanceSearchCode = "#btn-advanced-search";
        this.forceOverrideAdvanceSearchEvent = false;
        this.infoToastConfig = {};
        this.successToastConfig = {};
        this.warningToastConfig = {};
        this.dangerToastConfig = {};
    }

    #serverSideTableAjaxConfig = (oSearch) => {
        return {
            url:
                this.base_url +
                this.endPoint +
                "/" +
                this.querySearch +
                this.buildUrlParams(
                    this.queryParamsSearch,
                    this.useQueryFormatSearch
                ),
            type: "POST",
            headers: {
                Authorization: "Bearer " + new UcHelpers().GetAccessToken(),
            },
            data: function (d) {
                return $.extend({}, d, {
                    oSearch: JSON.stringify(oSearch),
                });
            },
            dataSrc: (json) => {
                let data = [];
                let recordsTotal = 0;
                let recordsFiltered = 0;

                if (Array.isArray(json.data)) {
                    data = json.data;
                    recordsTotal = json.recordsTotal || 0;
                    recordsFiltered = json.recordsFiltered || 0;
                } else if (json.data && Array.isArray(json.data.data)) {
                    data = json.data.data;
                    recordsTotal = json.data.recordsTotal || 0;
                    recordsFiltered = json.data.recordsFiltered || 0;

                    json.recordsTotal = recordsTotal;
                    json.recordsFiltered = recordsFiltered;
                } else {
                    console.error("Invalid JSON response:", json);
                }

                this.tableData = data;

                if (this.tableData.length === 0) {
                    if (!this.#isInitTable && this.notifyWhenNotFound) {
                        this.showWarning("Không tìm thấy dữ liệu!");
                    }
                } else {
                    if (this.isFormSearch && this.advanceModalCode) {
                        if (this.closeAdvanceModalWhenResultFound) {
                            const advanceModal = this.#getModalInstance(
                                this.advanceModalCode
                            );
                            advanceModal.hide();
                        }
                    }
                }

                this.#isInitTable = false;
                return this.tableData;
            },
        };
    };

    #mergeCallbacks(baseFn, extraFn) {
        return function (...args) {
            if (typeof baseFn === "function") baseFn.apply(this, args);
            if (typeof extraFn === "function") extraFn.apply(this, args);
        };
    }

    /**
     * @typedef {Object} DefaultValue
     * @property {string} key - Tên thuộc tính. Ví dụ `id`, `code`, ...
     * @property {string} value - Giá trị mặc định của thuộc tính. Ví dụ `0`, ``, `00000000-0000-0000-0000-000000000000`, ...
     */

    /**
     * @typedef {Object} ToastConfig
     * @property {boolean} animation - Có dùng hiệu ứng chuyển tiếp `fade` cho Toast hay không. Mặc định là `true`.
     * @property {boolean} autohide - Toast có tự động đóng hay không. Mặc định là `true`.
     * @property {number} delay - Thời gian hiển thị của Toast (tính theo mili giây). Mặc định là `5000` (5 giây).
     */

    /**
     * @typedef {Object} SelectOptions
     * @property {string} base_url - Base_url api của select. Ví dụ: `http://localhost:8080/`.
     * @property {string} end_point - End_point của api. Ví dụ: `category/get-items`.
     * @property {string} selectCode - Code của select. Có thể là 1 mảng hoặc 1 chuỗi. Ví dụ: "#sel_category", [".sel_category", ".sel_category_2"], ...
     * @property {string} label - Trường thuộc tính làm label của option.
     * @property {string} value - Trường thuộc tính làm value của option.
     * @property {string} orderBy - Sắp xếp label. asc: tăng dần, desc: giảm dần. Mặc định là tăng dần.
     * @property {string} selectType - Loại select. Gồm `select`, `all` hoặc rỗng. Mặc định là rỗng.
     * @property {boolean} [isSelect2] - Sử dụng select2 (mặc định là false).
     * @property {boolean} [searching] - Bật tìm kiếm cho select2 (mặc định là false).
     * @property {boolean} [multiple] - Bật multiple cho select2 (mặc định là false).
     * @property {Array<string>} extraColumns - Danh sách các thuộc tính sẽ được api truy vấn và trả về. Mặc định api sẽ trả về thuộc tính được gán cho cấu hình `label` và `value`. Ví dụ ['name','price',...] => Truy vấn **SELECT**.
     * @property {Object} extraConditions - Cấu hình truy vấn theo điều kiện cho api. Object gồm các cặp key - value bên trong, tương ứng sẽ là truy vấn `where key_1 = 'value_1' and ...` Ví dụ: { code: "blib" } sẽ truy vấn theo điều kiện là cột `code` với giá trị là `blib` => Truy vấn **WHERE**.
     * @property {Object} extraOrders - Cấu hình truy vấn theo điều kiện cho api. Object gồm các cặp key - value bên trong, tương ứng sẽ là truy vấn `order by key_1 value_1, ...` Ví dụ: { code: "asc" } sẽ sắp xếp theo điều kiện là cột `code` theo chiều tăng dần => Truy vấn **ORDER BY**. Mặc định sẽ có sẵn điều kiện sắp xếp được cấu hình cho `label` và `order`.
     * @property {Array<Object>} [data] - Mảng dữ liệu để vẽ Select. **Lưu ý**: Các thuộc tính liên quan đến API sẽ không hoạt động.
     * @property {Object} [dataAttributes] - Object chứa cặp key - value phục vụ tạo data attribute cho thẻ option. data-`key`="item[`value`]"
     */

    /**
     * @typedef {Object} SelectOption
     * @property {Array<Object>} [data] - Mảng dữ liệu để vẽ Select.
     * @property {string} selectCode - Code của select. Có thể là 1 mảng hoặc 1 chuỗi. Ví dụ: "#sel_category", [".sel_category", ".sel_category_2"], ...
     * @property {string} label - Trường thuộc tính làm label của option.
     * @property {string} value - Trường thuộc tính làm value của option.
     * @property {string} orderBy - Sắp xếp label. asc: tăng dần, desc: giảm dần. Mặc định là tăng dần.
     * @property {string} selectType - Loại select. Gồm `select`, `all` hoặc rỗng. Mặc định là rỗng.
     * @property {boolean} [isSelect2] - Sử dụng select2 (mặc định là false).
     * @property {boolean} [searching] - Bật tìm kiếm cho select2 (mặc định là false).
     * @property {boolean} [multiple] - Bật multiple cho select2 (mặc định là false).
     * @property {Object} [dataAttributes] - Object chứa cặp key - value phục vụ tạo data attribute cho thẻ option. data-`key`="item[`value`]"
     */

    /**
     * @typedef {Object} PromiseOptions
     * @property {string} base_url - Base_url api của promise. Ví dụ: `http://localhost:8080/`.
     * @property {string} end_point - End_point của api. Ví dụ: `category/get-items`.
     * @property {Array<string>} extraColumns - Danh sách các thuộc tính sẽ được api truy vấn và trả về. Mặc định api sẽ trả về thuộc tính được gán cho cấu hình `label` và `value`. Ví dụ ['name','price',...] => Truy vấn **SELECT**.
     * @property {Object} extraConditions - Cấu hình truy vấn theo điều kiện cho api. Object gồm các cặp key - value bên trong, tương ứng sẽ là truy vấn `where key_1 = 'value_1' and ...` Ví dụ: { code: "blib" } sẽ truy vấn theo điều kiện là cột `code` với giá trị là `blib` => Truy vấn **WHERE**.
     * @property {Object} extraOrders - Cấu hình truy vấn theo điều kiện cho api. Object gồm các cặp key - value bên trong, tương ứng sẽ là truy vấn `order by key_1 value_1, ...` Ví dụ: { code: "asc" } sẽ sắp xếp theo điều kiện là cột `code` theo chiều tăng dần => Truy vấn **ORDER BY**. Mặc định sẽ có sẵn điều kiện sắp xếp được cấu hình cho `label` và `order`.
     * @property {(data: Object) => void} assignVariable - Hàm trả về `response.data` khi nhận được dữ liệu từ api. Ví dụ: function(data) { this.category = data; }, (data) => { this.category = data; }
     */

    /**
     * @typedef {Object} Config
     * @property {string} tableCode - Code của bảng. Ví dụ: `#tbl_search`. Mặc định là `#tbl_search`.
     * @property {boolean} isServerSideTable - Cờ xác định có phải table được vẽ bên phía server hay không. Mặc định là `false`.
     * @property {boolean} enableLazyLoadForTable - Cờ xác định có áp dụng lazy load cho tài nguyên ảnh trong bảng hay không. Cần cấu hình class của các cột cho trường `lazyLoadColumnClass`. Mặc định là `false`.
     * @property {Array<string>} lazyLoadColumnClass - Mảng chứa class của các cột (td) sẽ được áp dụng lazy load. Ví dụ: ['avatar', 'book_image', 'cover_photo', ...].Mặc định là `[]`.
     * @property {Array<string>} checkboxColumnClass - Mảng chứa class của các cột (td) sẽ được tạo checkbox theo data dạng `true/false` của cột. Ví dụ: ['is_active', 'require_digit', 'require_uppercase', ...].Mặc định là `[]`. `Gợi ý`: Nên đặt class của cột giống data của nó.
     * @property {boolean} scrollToTopOnPagination - Cờ xác định có áp dụng cuộn lên đầu trang sau khi phân trang hay không. Mặc định là `true`. Sử dụng cấu hình `enableSmoothScrollToTop` để có thể bật/tắt hiệu ứng cuộn mượt mà.
     * @property {boolean} enableSmoothScrollToTop - Cờ xác định có áp dụng hiệu ứng cuộn mượt mà lên đầu trang sau khi phân trang hay không. Mặc định là `true`.
     * @property {Array<Object>} tableData - Data của bảng.
     * @property {Object} tableConfig - Cấu hình vẽ bảng của bảng (DataTables). Mặc định là `{}`.
     * @property {boolean} refreshTableWithoutApiInsert - Cờ xác định liệu bảng có được làm mới lại bằng API hay không sau khi thêm. `true` để không làm mới bằng API, `false` để làm mới bằng API. Mặc định là `false`.
     * @property {boolean} refreshTableWithoutApiUpdate - Cờ xác định liệu bảng có được làm mới lại bằng API hay không sau khi sửa. `true` để không làm mới bằng API, `false` để làm mới bằng API. Mặc định là `false`.
     * @property {boolean} refreshTableWithoutApiDelete - Cờ xác định liệu bảng có được làm mới lại bằng API hay không sau khi xóa. `true` để không làm mới bằng API, `false` để làm mới bằng API. Mặc định là `false`.
     * @property {boolean} notifyWhenNotFound - Cờ xác định liệu có hiện thông báo khi không tìm thấy hay không **(Chỉ áp dụng cho server side table)**. Mặc định là `false`.
     * @property {string} formCode - Class của form (ví dụ: `add_new`). Lưu ý: Không cần `.` hay `#` đằng trước formCode. Nếu đây là form search, formCode sẽ là code của form tìm kiếm nhanh.
     * @property {string} formName - Tên của form (sử dụng để làm tên hiển thị trên modal).
     * @property {string} modalCode - Code của modal. Ví dụ `#modalAdd`, `.modalEdit`
    //  * @property {string} hdKey - Code của trường ẩn, phục vụ Thêm - Sửa (Cần thêm <input id="..." class="..." type="hidden" /> vào form với id tương ứng). Ví dụ: `#hd_key`.
     * @property {string} inputFileCode - Code của input với type="file". Ví dụ: `#input_file`. Mặc định là `#input_file`.
     * @property {string} fileId - fileId của ảnh (id của ảnh trong database). Có thể không cần nếu chưa có id của ảnh.
     * @property {string} refId - refId của ảnh (Thuộc tính này quan hệ 1 - 1 với fileId).
     * @property {string} refType - refType của nơi lưu trữ ảnh.
     * @property {string} folderCode - folderCode của nơi lưu trữ ảnh.
     * @property {string} labelCode - Code của trường label trong modal. Ví dụ: `#modal-label`, ...
     * @property {boolean} needToBeUuid - Cờ xác định liệu trường id của oValue có phải là UUID hay không.
     * @property {string} btnSaveCode - Code của nút lưu. Ví dụ: `#btn-save`, `.btn-save`. Mặc định là `#btn-save`.
     * @property {string} btnAddCode - Code của nút thêm. Ví dụ: `#btn-add`, `.btn-add`. Mặc định là `#btn-add`.
     * @property {string} btnRefreshCode - Code của nút làm mới. Ví dụ: `#btn-refresh`, `.btn-refresh`. Mặc định là `#btn-refresh`.
     * @property {string} btnCloseCode - Code của nút đóng modal. Ví dụ: `#btn-close`, `.btn-close`. Mặc định là `#btn-close`.
     * @property {string} btnGoBackCode - Code của nút quay lại. Ví dụ: `#btn-go-back`, `.btn-go-back`. Mặc định là `#btn-go-back`.
     * @property {string} editCodeOfTable - Code của nút sửa 1 item trong bảng (Nút 3 chấm). Ví dụ: `#action-edit`, `.action-edit`. Mặc đinh là `.action-edit`
     * @property {string} deleteCodeOfTable - Code của nút xóa 1 item trong bảng (Nút 3 chấm). Ví dụ: `#action-delete`, `.action-delete`.
     * @property {string} detailCodeOfTable - Code của nút chi tiết 1 item trong bảng (Nút 3 chấm). Ví dụ: `#action-detail`, `.action-detail`. Mặc định là `.action-detail`.
     * @property {string} saveCodeOfTable - Code của nút lưu 1 item trong bảng (Nút 3 chấm). Ví dụ: `#action-save`, `.action-save`. Mặc định là `.action-save`.
     * @property {string} queryInsert - Query parameters của api insert. Ví dụ: `insert-item`, `save-item`. Mặc định là `insert-item`.
     * @property {string} queryUpdate - Query parameters của api update. Ví dụ: `update-item`. Mặc định là `update-item`.
     * @property {string} queryDelete - Query parameters của api delete. Ví dụ: `delete-item`. Mặc định là `delete-item`.
     * @property {Object} queryParamsDelete - Các tham số động của query cho API delete. Có thể cấu hình trực tiếp trong deleteItem(). Mặc định là một đối tượng rỗng `{}`. Lưu ý: Lần gọi tiếp theo nếu vẫn là các tham số trước đó thì không cần truyền lại. Và ngược lại nếu tham số với các giá trị thay đổi thì phải gán lại, sau đó gọi lại phương thức `deleteItem()` bằng cách `super.deleteItem()`. Ví dụ: `id` của các row trong bảng là khác nhau.
     * @property {boolean} useQueryFormatDelete - Xác định xem có sử dụng định dạng query string (ví dụ: `?param=value`) hay không khi gọi API delete. Có thể cấu hình trực tiếp trong deleteItem(). Mặc định là `false`.
     * @property {string} queryGetItems - Query parameters của api get-items. Ví dụ: `get-items`. Mặc định là `get-items`.
     * @property {Object} queryParamsGetItems - Các tham số động của query cho API get-items. Có thể cấu hình trực tiếp trong getItems(). Mặc định là một đối tượng rỗng `{}`. Lưu ý: Lần gọi tiếp theo nếu vẫn là các tham số trước đó thì không cần truyền lại. Và ngược lại nếu tham số với các giá trị thay đổi thì phải gán lại, sau đó gọi lại phương thức `getItems()` bằng cách `super.getItems()`.
     * @property {boolean} useQueryFormatGetItems - Xác định xem có sử dụng định dạng query string (ví dụ: `?param=value`) hay không khi gọi API get-items. Có thể cấu hình trực tiếp trong getItems(). Mặc định là `false`.
     * @property {string} queryGetList - Query parameters của api get-items, nhưng phục vụ vẽ bảng - tableCode cần được cấu hình bằng phương thức configure(). Ví dụ: `get-items`. Mặc định là `get-items`.
     * @property {Object} queryParamsGetList - Các tham số động của query cho API get-items phục vụ vẽ bảng. Có thể cấu hình trực tiếp trong getList(). Mặc định là một đối tượng rỗng `{}`. Lưu ý: Lần gọi tiếp theo nếu vẫn là các tham số trước đó thì không cần truyền lại. Và ngược lại nếu tham số với các giá trị thay đổi thì phải gán lại, sau đó gọi lại phương thức `getList()` bằng cách `super.getList()`.
     * @property {boolean} useQueryFormatGetList - Xác định xem có sử dụng định dạng query string (ví dụ: `?param=value`) hay không khi gọi API get-items phục vụ vẽ bảng. Có thể cấu hình trực tiếp trong getList(). Mặc định là `false`.
     * @property {string} queryGetItemById - Query parameters của api get-item-by-id. Ví dụ: `get-item-by-id`. Mặc định là `get-item-by-id`.
     * @property {Object} queryParamsGetItemById - Các tham số động của query cho API get-item-by-id. Có thể cấu hình trực tiếp trong getItemById(). Mặc định là một đối tượng rỗng `{}`. Lưu ý: Lần gọi tiếp theo nếu vẫn là các tham số trước đó thì không cần truyền lại. Và ngược lại nếu tham số với các giá trị thay đổi thì phải gán lại, sau đó gọi lại phương thức `getItemById()` bằng cách `super.getItemById()`.
     * @property {boolean} useQueryFormatGetItemById - Xác định xem có sử dụng định dạng query string (ví dụ: `?param=value`) hay không khi gọi API get-item-by-id. Có thể cấu hình trực tiếp trong getItemById(). Mặc định là `false`.
     * @property {boolean} isFormSearch - Xác định xem có phải là form search hay không. Mặc định là `false`.
     * @property {string} querySearch - Query parameters của api tìm kiếm bằng oSearch. Ví dụ: `search`, `search-item`. Mặc định là `search`.
     * @property {boolean} useQueryFormatSearch - Xác định xem có sử dụng định dạng query string (ví dụ: `?param=value`) hay không khi gọi API search. Mặc định là `false`.
     * @property {string} queryAdvanceSearch - Query parameters của api tìm kiếm nâng cao. Ví dụ: `search`, `search-item`. Mặc định giống `querySearch`.
     * @property {string} advanceModalCode - Code của modal tìm kiếm nâng cao. Ví dụ: `#modalAdvancedSearch`, `.modalAdvancedSearch`, ...
     * @property {string} advanceFormCode - Class của form tìm kiếm nâng cao (ví dụ: `advanced_search`). Lưu ý: Không cần `.` hay `#` đằng trước advanceFormCode.
     * @property {string} btnSearchCode - Code của nút tìm kiếm nhanh trong form search. Ví dụ: `#btn-search`, `.btn-search`. Mặc định là `#btn-search`.
     * @property {string} btnAdvanceRefreshCode - Code của nút làm mới trong modal tìm kiếm nâng cao. Ví dụ: `#btn-advanced-refresh`, `.btn-advanced-refresh`. Mặc định là `#btn-advanced-refresh`.
     * @property {string} btnAdvanceSearchCode - Code của nút tìm kiếm trong modal tìm kiếm nâng cao. Ví dụ: `#btn-advanced-search`, `.btn-advanced-search`. Mặc định là `#btn-advanced-search`.
     * @property {boolean} closeAdvanceModalWhenResultFound - Cờ xác định xem có đóng modal tìm kiếm nâng cao khi tìm thấy kết quả hay không. Mặc định là `true`.
     * @property {Array<string>} needToBeDMY - Danh sách các trường cần định dạng ngày tháng năm (dd/MM/yyyy). Ví dụ: [`created_date`, `published_date`, ...]. Phần tử cuối cùng của mảng là kí tự phân cách ngày tháng năm. Mặc định là "/".
     * @property {Array<string>} needToBeISO - Danh sách các trường cần định dạng thời gian theo chuẩn ISO. Ví dụ: [`created_date`, `published_date`, ...]. Phần tử cuối cùng của mảng là kí tự phân cách ngày tháng năm. Mặc định là "/".
     * @property {Array<string>} needToBeInt - Danh sách các trường cần định dạng số nguyên. Ví dụ: [`amount`, `quantity`, ...].
     * @property {Array<string>} needToBeEmpty - Danh sách các trường cần phải gán "". Ví dụ: [`id`, `name`, ...].
     * @property {Array<string>} needToUnformatCurrency - Danh sách các trường cần bỏ định dạng tiền tệ VNĐ. Ví dụ: [`price`, `cost`, ...].
     * @property {Array<DefaultValue>} defaultValueIfNone - Danh sách các object chứa các trường thuộc tính được gán giá trị mặc định nếu không có giá trị. Phục vụ cho việc tạo oValue.
     * @property {Array<SelectOptions>} selectOptions - Danh sách các object chứa các trường thuộc tính để vẽ Select. Có thể vẽ Select qua Ajax hoặc áp dụng Select2 cho Select đã được tạo sẵn. Sử dụng phương thức `onAllSelectsProcessed()` để xử lý khi tất cả Select được tạo xong.
     * @property {Array<PromiseOptions>} promiseOptions - Danh sách các object chứa các thuộc tính để thực hiện các promise api. Sử dụng phương thức `onPromiseSuccessfully()` để xử lý khi tất cả các promise thực thi xong.
     * @property {boolean} enableFixedColumn - Kích hoạt fixed column cho cột action (cuối cùng) của table. Mặc định là `true`.
     * @property {string} deleteMessage - Cấu hình thông báo khi ấn xóa item trên bảng. Mặc là là 'Bạn có muốn xóa ...?'.
     * @property {ToastConfig} infoToastConfig - Cấu hình cho Info Toast (Thông báo thông tin - Màu xanh trời).
     * @property {ToastConfig} successToastConfig - Cấu hình cho Success Toast (Thông báo thành công - Màu xanh lá).
     * @property {ToastConfig} warningToastConfig - Cấu hình cho Warning Toast (Thông báo cảnh báo - Màu vàng).
     * @property {ToastConfig} dangerToastConfig - Cấu hình cho Error Toast (Thông báo lỗi - Màu đỏ).
     */

    /**
     * Đối tượng cấu hình cho form.
     * @param {Config} config
     */

    configure(config) {
        for (const [key, value] of Object.entries(config)) {
            if (value && typeof value === "object" && !Array.isArray(value)) {
                this[key] = { ...value };
            } else if (Array.isArray(value)) {
                this[key] = [...value];
            } else {
                this[key] = value;
            }
        }

        this.formName = config.formName
            ? config.formName.toLowerCase()
            : this.formName;
        this.formCode = config.formCode
            ? config.formCode.replace(".", "")
            : this.formCode;

        this.setupEventHandlers();

        this.#onDomReady(() => {
            if (this.formCode) {
                document.querySelector(`.${this.formCode} input`)?.focus();
            }

            this.queryAdvanceSearch =
                config.queryAdvanceSearch || this.querySearch;
            this.deleteMessage = `Bạn có muốn xóa ${this.formName} này không?`;
        });

        this.#initialize();
    }

    #onDomReady(callback) {
        if (this.#domReadyFired) {
            callback();
        } else {
            this.#domReadyQueue.push(callback);
        }
        return this;
    }

    #configureToastConfig(type, toastConfig) {
        const defaultConfig = {
            animation: true,
            autohide: true,
            delay: Toast().DelayConfig[type],
        };

        return Object.assign({}, defaultConfig, toastConfig);
    }

    #showToast(type, codeContent, ...args) {
        const container = document.getElementById("toast-container");
        const template = document.getElementById("toast-template");

        if (!container || !template) return;

        const toastEl = template.content
            .cloneNode(true)
            .querySelector(".toast");

        const typeClasses = {
            warning: "bg-warning",
            info: "bg-info",
            success: "bg-success",
            danger: "bg-danger-uc",
        };

        const toastConfig = {
            warning: this.#configureToastConfig(
                "warning",
                this.warningToastConfig
            ),
            info: this.#configureToastConfig("info", this.infoToastConfig),
            success: this.#configureToastConfig(
                "success",
                this.successToastConfig
            ),
            danger: this.#configureToastConfig(
                "danger",
                this.dangerToastConfig
            ),
        };

        const bgClass = typeClasses[type] || "bg-success";
        toastEl.classList.add(bgClass);
        toastEl.querySelector(".toast-header").classList.add(bgClass);
        toastEl.querySelector(".toast-body").textContent = Toast().Code2Text(
            codeContent,
            ...args
        );
        toastEl.querySelector(".toast-header strong").textContent =
            Toast().TitleConfig[type];

        container.appendChild(toastEl);

        const toast = new bootstrap.Toast(toastEl, {
            ...toastConfig[type],
        });
        toast.show();

        toastEl.addEventListener("hidden.bs.toast", () => {
            toastEl.remove();
        });
    }

    #initialize() {
        this.processSelectOptions();
        this.processPromises();
    }

    #createCheckbox(status) {
        var html = "";
        if (status) {
            html += '<label class="custom_check me-1 c-pointer">';
            html += '<input type="checkbox" checked disabled>';
        } else {
            html += '<label class="custom_check me-1 c-pointer">';
            html += '<input type="checkbox" disabled>';
        }
        html += '<span class="checkmark"></span>';
        html += "</label>";
        return html;
    }

    /**
     * Xử lý khi json config tải thành công.
     */
    onLoadJsonConfigSuccessfully() {}

    async loadJsonConfig() {
        this.jsonConfig = await Config().FormSearchList(this.jsonConfigPath);
        this.onLoadJsonConfigSuccessfully();
    }

    /**
     * Xử lý khi tất cả promise được xử lý thành công. Các promise này được cấu hình trong `promiseOptions` ở phương thức `configure()`.
     */
    onPromiseSuccessfully() {}

    processPromises() {
        const promises = this.promiseOptions.map((item) => {
            if (!item.base_url || !item.end_point) return Promise.resolve();

            let url = item.base_url + item.end_point;
            const params = [];

            // columnsQuery
            let extraColumns =
                Array.isArray(item.extraColumns) && item.extraColumns.length > 0
                    ? item.extraColumns.join(",")
                    : "";
            if (extraColumns) {
                params.push(`columnsQuery=${extraColumns}`);
            }

            // whereQuery
            if (
                item.extraConditions &&
                typeof item.extraConditions === "object"
            ) {
                let whereParts = [];
                for (let key in item.extraConditions) {
                    if (item.extraConditions.hasOwnProperty(key)) {
                        let value = item.extraConditions[key];
                        whereParts.push(`${key}='${value}'`);
                    }
                }
                if (whereParts.length > 0) {
                    params.push(`whereQuery=${whereParts.join(" AND ")}`);
                }
            }

            // orderQuery
            if (item.extraOrders && typeof item.extraOrders === "object") {
                let orderParts = "";
                for (let key in item.extraOrders) {
                    if (item.extraOrders.hasOwnProperty(key)) {
                        let value = item.extraOrders[key];
                        orderParts += `${key} ${value}, `;
                    }
                }
                if (orderParts) {
                    params.push(`orderQuery=${orderParts.slice(0, -2)}`);
                }
            }

            // Gắn các params vào URL
            if (params.length > 0) {
                url +=
                    (item.end_point.includes("?") ? "&" : "?") +
                    params.join("&");
            }

            return UcAjax.get(url).done((response) => {
                item.assignVariable?.(response.data);
            });
        });

        Promise.all(promises)
            .then(() => this.#onDomReady(() => this.onPromiseSuccessfully()))
            .catch((error) => this.showError(msg.common_error));
    }

    /**
     * Trả về response khi `getImageUrl()` thành công. `response.data` chứa url.
     */
    onGetImageUrlSuccessfully(response) {}

    /**
     * Lấy url của ảnh theo `fileId` (id của ảnh trên database). Sử dụng phương thức `onGetImageUrlSuccessfully()` để xử lý khi thành công.
     */
    getImageUrl() {
        UcAjax.get(base_url_core + `Fms_File/view?fileId=${this.fileId}`).done(
            (response) => this.onGetImageUrlSuccessfully(response)
        );
    }

    /**
     * Trả về response khi `updateImage()` thành bại.
     */
    onUpdateImageFailed(response) {
        this.handleResponseWhenError(response);
    }

    /**
     * Trả về response khi `updateImage()` thành công.
     */
    onUpdateImageSuccessfully(response) {
        if (response.success) {
            this.showSuccess(response.message);
        } else {
            this.onUpdateImageFailed(response);
        }
    }

    /**
     * Phương thức sẽ cập nhật lại tham chiếu của `fileId` với `refId`.
     * @param {string} fileId - Id của file.
     */
    updateImage(fileId) {
        UcAjax.put(
            base_url_core +
                `Fms_File/update-ref-id?fileId=${fileId}&refId=${this.refId}`
        ).done((response) => {
            this.fileId = fileId;
            this.onUpdateImageSuccessfully(response);
        });
    }

    /**
     * Xử lý khi `uploadImage()` thất bại. Phương thức trả về `response` của api.
     */
    onUploadImageFailed(response) {
        this.handleResponseWhenError(response);
    }

    /**
     * Xử lý khi `uploadImage()` thành công. response.data[0] bao gồm `fileId`, `fileName`, `baseUrl`, `path` và `message`.
     */
    onUploadImageSuccessfully(response) {
        this.updateImage(response.data[0].fileId);
    }

    /**
     * Trả về response của `uploadImage()`.
     */
    handleUploadImageResponse(response) {
        if (response.success) {
            this.onUploadImageSuccessfully(response);
        } else {
            this.onUploadImageFailed(response);
        }
    }

    /**
     * Phương thức tải ảnh lên server. Cần cấu hình (`fileId`), `inputFileCode`, `refId`, `refType` và `folderCode` bằng phương thức `configure()`.
     */
    uploadImage(imageFile) {
        var refId = this.refId;
        var fileId = this.fileId;
        var refType = this.refType;
        var folderCode = this.folderCode;

        var formData = new FormData();
        fileId
            ? formData.append("files", imageFile[0])
            : formData.append(`FileId_${fileId}`, imageFile[0]);

        UcAjax.post(
            base_url_core +
                "Fms_File/upload?" +
                (this.refId ? `refId=${refId}&` : "") +
                `refType=${refType}&folderCode=${folderCode}&appCode=${app_code}`,
            formData
        ).done((response) => this.handleUploadImageResponse(response));
    }

    /**
     * Phương thức vẽ bảng theo `data` truyền vào và kèm theo `extraConfig` bổ sung cho DataTable (nếu có). Cũng có thể cấu hình cho `tableConfig` ở phương thức `configure`. *Lưu ý*: `tableCode`, `jsonConfig` cần được cấu hình.
     */
    async drawTable(data, extraConfig) {
        if (!this.jsonConfig) {
            await this.loadJsonConfig(this.jsonConfigPath);
        }

        this.tableData = data || [];
        extraConfig = extraConfig || this.tableConfig;

        if (!this.tableInstance) {
            this.tableInstance = UcFormHelpers().DataTableAjaxClientSide(
                this.tableCode,
                this.jsonConfig.columns,
                this.tableData,
                {
                    autoWidth: false,
                    ...extraConfig,
                    headerCallback: this.#mergeCallbacks(
                        this.headerCallback,
                        extraConfig?.headerCallback
                    ),
                    rowCallback: this.#mergeCallbacks(
                        this.#rowCallback,
                        extraConfig?.rowCallback
                    ),
                    initComplete: this.#mergeCallbacks(
                        (setting, json) => this.initCallback(setting, json),
                        extraConfig?.initComplete
                    ),
                    drawCallback: this.#mergeCallbacks((settings) => {
                        this.#drawCallback(settings);
                    }, extraConfig?.drawCallback),
                },
                this.enableFixedColumn
            );
        } else {
            this.tableInstance.clear();
            this.tableData = data || [];
            this.tableInstance.rows.add(this.tableData);
            Object.assign(this.tableInstance.settings()[0].oInit, extraConfig);
            this.tableInstance.draw(false);
        }
    }

    /**
     * Phương thức vẽ bảng bên phía server theo `oSearch` truyền vào và kèm theo `extraConfig` bổ sung cho DataTable (nếu có). Cũng có thể cấu hình cho `tableConfig` ở phương thức `configure`. *Lưu ý*: `tableCode`, `jsonConfig` cần được cấu hình.
     */
    async drawServerSideTable(oSearch, extraConfig) {
        if (!this.jsonConfig) {
            await this.loadJsonConfig(this.jsonConfigPath);
        }

        this.oSearch = oSearch ? oSearch : this.getOSearch();

        this.onBeforeSearching();

        extraConfig = extraConfig || this.tableConfig;

        if (!this.tableInstance) {
            this.tableInstance = UcFormHelpers().DataTableAjaxServerSide(
                this.tableCode,
                null,
                this.jsonConfig.columns,
                this.oSearch,
                {
                    autoWidth: false,
                    ajax: this.#serverSideTableAjaxConfig(oSearch),
                    ...extraConfig,
                    headerCallback: this.#mergeCallbacks(
                        this.headerCallback,
                        extraConfig?.headerCallback
                    ),
                    rowCallback: this.#mergeCallbacks(
                        this.#rowCallback,
                        extraConfig?.rowCallback
                    ),
                    initComplete: this.#mergeCallbacks(
                        (setting, json) => this.initCallback(setting, json),
                        extraConfig?.initComplete
                    ),
                    drawCallback: this.#mergeCallbacks((settings) => {
                        this.#drawCallback(settings);
                    }, extraConfig?.drawCallback),
                },
                this.enableFixedColumn
            );

            let lastPage = this.tableInstance.page();

            this.tableInstance.on("draw.dt", () => {
                const pageInfo = this.tableInstance.page.info();
                if (
                    pageInfo.page !== lastPage &&
                    this.scrollToTopOnPagination
                ) {
                    document.documentElement.scrollTo({
                        top: 0,
                        left: 0,
                        behavior: this.enableSmoothScrollToTop
                            ? "smooth"
                            : "instant",
                    });
                }
                lastPage = pageInfo.page;
            });
        } else {
            const newAjaxConfig = this.#serverSideTableAjaxConfig(
                this.getOSearch()
            );
            this.tableInstance.ajax.url(newAjaxConfig.url);
            this.tableInstance.settings()[0].ajax.data = newAjaxConfig.data;
            this.tableInstance.settings()[0].ajax.dataSrc =
                newAjaxConfig.dataSrc;
            this.tableInstance.settings()[0].ajax.type =
                newAjaxConfig.type || "POST";
            this.tableInstance.settings()[0].ajax.headers =
                newAjaxConfig.headers;
            this.tableInstance.ajax.reload(null, false);
        }
    }

    #drawSelectByResponse(response, config) {
        if (response.success) {
            config.data = response.data;
            this.drawSelect(config);
        } else {
            this.handleResponseWhenError(response);
        }
    }

    /**
     * Phương thức vẽ Select theo đối tượng `config` truyền vào.
     * @param {SelectOption} config
     */
    drawSelect(config) {
        if (!config.data || !Array.isArray(config.data)) return;

        const selectCodes = Array.isArray(config.selectCode)
            ? config.selectCode
            : [config.selectCode];

        let options = UcFormHelpers().DrawOptionsSelect(
            config.data,
            config.value,
            config.label,
            config.selectType,
            config.dataAttributes
        );

        selectCodes.forEach((code) => {
            const selectElement = $(code);
            selectElement.html(options);

            if (config.isSelect2) {
                const select2Config = config.searching
                    ? select2_config_search(code, config.multiple)
                    : { minimumResultsForSearch: -1 };

                if (!selectElement.hasClass("select2-hidden-accessible")) {
                    selectElement.select2(select2Config);

                    if (!config.searching) {
                        this.createClickEvent(`${code} + span`, () => {
                            const codeEl = document.querySelector(code);
                            if (!codeEl) return;

                            const containers = Array.from(
                                codeEl.parentElement.children
                            ).filter((el) =>
                                el.matches("span.select2-container")
                            );

                            containers.forEach((container) => {
                                const searches = container.querySelectorAll(
                                    "span.select2-search"
                                );
                                searches.forEach((s) => s.remove());
                            });
                        });
                    }
                }
            }
        });
    }

    showError(message) {
        this.#showToast("danger", message);
    }

    showSuccess(message) {
        this.#showToast("success", message);
    }

    showInfo(message) {
        this.#showToast("info", message);
    }

    showWarning(message) {
        this.#showToast("warning", message);
    }

    /**
     * Xử lý khi tất cả select được tạo. Các select này được cấu hình trong `selectOptions` ở phương thức `configure()`.
     */
    onAllSelectsProcessed() {}

    /**
     * Xử lý khi tất cả select được vẽ lại. Các select này cần được cấu hình trước đó trong `selectOptions` ở phương thức `configure()`.
     */
    onAllSelectsProcessedAgain() {}

    /**
     * Vẽ lại các Select đã được cấu hình ở `configure()`. Truyền vào 1 mảng gồm có các selectCode. Ví dụ: [`#select1`, `#select2`, ...]. Mặc định vẽ lại tất cả.
     */
    drawSelectAgain(selectCodes = []) {
        if (selectCodes.length == 0) {
            this.processSelectOptions();
        } else {
            const promises = this.selectOptions
                .filter((item) => selectCodes.includes(item.selectCode))
                .map((item) => {
                    return this.getResolveOfProcessSelect(item);
                });

            Promise.all(promises)
                .then(() => this.onAllSelectsProcessedAgain())
                .catch((error) => this.showError(msg.common_error));
        }
    }

    getResolveOfProcessSelect(config) {
        if (!config.base_url || !config.end_point) {
            this.drawSelect(config);
            return Promise.resolve();
        }

        let hasStartParamSymbol = config.end_point.includes("?");

        let extraColumns =
            Array.isArray(config.extraColumns) && config.extraColumns.length > 0
                ? "," + config.extraColumns.join(",")
                : "";

        let whereQuery = "";

        if (
            config.extraConditions &&
            typeof config.extraConditions === "object"
        ) {
            for (let key in config.extraConditions) {
                if (config.extraConditions.hasOwnProperty(key)) {
                    let value = config.extraConditions[key];
                    whereQuery += `${key}='${value}' AND `;
                }
            }
        }

        if (whereQuery) {
            whereQuery = `&whereQuery=${whereQuery.slice(0, -5)}`;
        }

        let orderQuery = "";
        if (config.extraOrders && typeof config.extraOrders === "object") {
            for (let key in config.extraOrders) {
                if (config.extraOrders.hasOwnProperty(key)) {
                    let value = config.extraOrders[key];
                    orderQuery += `${key} ${value}, `;
                }
            }
        }

        if (orderQuery) {
            orderQuery = `,${orderQuery.slice(0, -2)}`;
        }

        let url =
            config.base_url +
            config.end_point +
            (hasStartParamSymbol ? "&" : "?") +
            `columnsQuery=${config.value},${config.label}${extraColumns}` +
            whereQuery +
            `&orderQuery=${config.label} ${
                config.orderBy || "asc"
            }${orderQuery}`;

        return UcAjax.get(url).done((response) =>
            this.#drawSelectByResponse(response, config)
        );
    }

    processSelectOptions() {
        if (this.selectOptions && this.selectOptions.length > 0) {
            const promises = this.selectOptions.map((item) => {
                return this.getResolveOfProcessSelect(item);
            });

            Promise.all(promises)
                .then(() =>
                    this.#onDomReady(() => this.onAllSelectsProcessed())
                )
                .catch((error) => this.showError(msg.common_error));
        }
    }

    extendData(oValue) {
        var initialOValue = oValue;
        if (oValue.hasOwnProperty("id")) {
            if (!oValue.id) {
                if (this.needToBeUuid) {
                    oValue.id = "00000000-0000-0000-0000-000000000000";
                } else {
                    oValue.id = "";
                }
            }
        } else {
            if (this.needToBeUuid) {
                oValue.id = "00000000-0000-0000-0000-000000000000";
            } else {
                oValue.id = "";
            }
        }

        if (this.needToBeDMY.length > 0) {
            let charSplit = this.needToBeDMY[this.needToBeDMY.length]
                ? this.needToBeDMY[this.needToBeDMY.length]
                : "/";
            for (let key in initialOValue) {
                if (oValue[key] && this.needToBeDMY.includes(key)) {
                    oValue[key] = new DateUtil().FormatDateToDDMMYYYY(
                        oValue[key],
                        charSplit
                    );
                }
            }
        }

        if (this.needToBeISO.length > 0) {
            let charSplit = this.needToBeISO[this.needToBeISO.length]
                ? this.needToBeISO[this.needToBeISO.length]
                : "/";
            for (let key in initialOValue) {
                if (oValue[key] && this.needToBeISO.includes(key)) {
                    oValue[key] = new DateUtil().ParseDDMMYYYYToISO(
                        oValue[key],
                        charSplit
                    );
                }
            }
        }

        if (this.needToBeInt.length > 0) {
            for (let key in initialOValue) {
                if (this.needToBeInt.includes(key)) {
                    if (oValue[key]) {
                        oValue[key] = parseInt(oValue[key]);
                    }
                }
            }
        }

        if (this.needToBeEmpty.length > 0) {
            for (let key of this.needToBeEmpty) {
                oValue[key] = "";
            }
        }

        if (this.needToUnformatCurrency.length > 0) {
            for (let key of this.needToUnformatCurrency) {
                if (oValue[key]) {
                    oValue[key] = CurrencyUtil().unformatCurrencyVND(
                        oValue[key]
                    );
                }
            }
        }

        if (this.defaultValueIfNone.length > 0) {
            this.defaultValueIfNone.forEach((defaultValue) => {
                if (
                    initialOValue[defaultValue.key] === null ||
                    initialOValue[defaultValue.key] === undefined ||
                    initialOValue[defaultValue.key] === ""
                ) {
                    oValue[defaultValue.key] = defaultValue.value;
                }
            });
        }

        return oValue;
    }

    /**
     * Tạo ra chuỗi query parameters.
     * @param {Object} params - Các tham số cần thiết dưới dạng object { key: value }. `value` có thể là mảng. Có thể không cần truyền gì nếu không có param.
     * @param {boolean} useQueryFormat - _true_ nếu muốn dạng query string (?param=value), _false_ nếu dạng URL (/param).
     */
    buildUrlParams(params = {}, useQueryFormat = true) {
        if (!params || Object.keys(params).length === 0) return "";

        if (useQueryFormat) {
            const queryParts = [];

            Object.entries(params).forEach(([key, value]) => {
                if (Array.isArray(value)) {
                    value.forEach((v) => {
                        queryParts.push(
                            `${encodeURIComponent(key)}=${encodeURIComponent(
                                v
                            )}`
                        );
                    });
                } else {
                    queryParts.push(
                        `${encodeURIComponent(key)}=${encodeURIComponent(
                            value
                        )}`
                    );
                }
            });

            return "?" + queryParts.join("&");
        } else {
            // Nếu là dạng path: /value1/value2
            return (
                "/" +
                Object.values(params)
                    .map((value) => encodeURIComponent(value))
                    .join("/")
            );
        }
    }

    /**
     * Xử lý khi `getItems()` thất bại. Phương thức trả về param là response của api.
     */
    onGetItemsFailed(response) {
        this.handleResponseWhenError(response);
    }

    /**
     * Xử lý khi `getItems()` thành công. Phương thức trả về param là response của api.
     */
    onGetItemsSuccessfully(response) {}

    /**
     * Xử lý khi response được trả về từ `getItems()`.
     */
    handleGetItemsResponse(response) {
        if (response.success) {
            this.onGetItemsSuccessfully(response);
        } else {
            this.onGetItemsFailed(response);
        }
    }

    /**
     * Thực hiện logic nào đó trước khi thực hiện lấy danh sách. Có thể dùng cho việc gán lại parameters, các thuộc tính trong oValue,... của API lấy danh sách.
     */
    onBeforeGettingItems() {}

    /**
     * Lấy danh sách trả về từ API.
     * @param {Object} params - Các tham số cần thiết dưới dạng object { key: value }. Có thể không cần truyền gì nếu không có param.
     * @param {boolean} useQueryFormat - `true` nếu muốn dạng query string (?param=value), `false` nếu dạng URL (/param).
     */
    getItems(params = null, useQueryFormat = null) {
        if (params && Object.keys(params).length > 0) {
            this.queryParamsGetItems = params;
        }

        if (useQueryFormat !== null) {
            this.useQueryFormatGetItems = useQueryFormat;
        }

        const queryString = this.buildUrlParams(
            this.queryParamsGetItems,
            this.useQueryFormatGetItems
        );

        const url =
            this.base_url +
            this.endPoint +
            "/" +
            this.queryGetItems +
            queryString;

        this.onBeforeGettingItems();
        UcAjax.get(url).done((response) => {
            this.handleGetItemsResponse(response);
        });
    }

    /**
     * Tạo api theo query linh hoạt từ các tham số đầu vào - Lấy kết quả theo truy vấn SQL. `Lưu ý`: **API phải hỗ trợ chức năng này**. API sẽ callback function được truyền vào ở tham số thứ 5. Phương thức này trả về cho hàm callback gồm 2 tham số lần lượt là `error` (nếu có lỗi xảy ra) và `response` (khi api được thực hiện thành công). Callback bằng cách **createApiWithSqlQueryParams(...,...,...,..., (error, response) => {})**. Nếu `error` có giá trị khác `null` thì có lỗi xảy ra. Ngược lại, nếu `error` = null, `response` sẽ có giá trị.
     *
     * @param {string} apiUrl - URL của API. Ví dụ: https://localhost:5166/api/product/get-items,...
     * @param {Array<string>} extraColumns - Danh sách cột bổ sung.
     * @property {Array<string>} extraColumns - Danh sách các thuộc tính sẽ được api truy vấn và trả về. Mặc định api sẽ trả về thuộc tính được gán cho cấu hình `label` và `value`. Ví dụ ['name','price',...] => Truy vấn **SELECT**.
     * @property {Object} extraConditions - Cấu hình truy vấn theo điều kiện cho api. Object gồm các cặp key - value bên trong, tương ứng sẽ là truy vấn `where key_1 = 'value_1' and ...` Ví dụ: { code: "blib" } sẽ truy vấn theo điều kiện là cột `code` với giá trị là `blib` => Truy vấn **WHERE**.
     * @property {Object} extraOrders - Cấu hình truy vấn theo điều kiện cho api. Object gồm các cặp key - value bên trong, tương ứng sẽ là truy vấn `order by key_1 value_1, ...` Ví dụ: { code: "asc" } sẽ sắp xếp theo điều kiện là cột `code` theo chiều tăng dần => Truy vấn **ORDER BY**.
     */
    createApiWithSqlQueryParams(
        apiUrl,
        extraColumns = [],
        extraConditions = {},
        extraOrders = {},
        callback
    ) {
        let url = apiUrl;
        let queryParams = [];

        // Xử lý extraColumns
        if (Array.isArray(extraColumns) && extraColumns.length > 0) {
            queryParams.push(`columnsQuery=${extraColumns.join(",")}`);
        }

        // Xử lý extraConditions
        let whereQuery = Object.entries(extraConditions)
            .map(([key, value]) => `${key}='${value}'`)
            .join(" AND ");
        if (whereQuery) {
            queryParams.push(`whereQuery=${encodeURIComponent(whereQuery)}`);
        }

        // Xử lý extraOrders
        let orderQuery = Object.entries(extraOrders)
            .map(([key, value]) => `${key} ${value}`)
            .join(", ");
        if (orderQuery) {
            queryParams.push(`orderQuery=${encodeURIComponent(orderQuery)}`);
        }

        // Ghép query parameters vào URL
        if (queryParams.length > 0) {
            url += (url.includes("?") ? "&" : "?") + queryParams.join("&");
        }

        return UcAjax.get(url)
            .done((response) => {
                callback(null, response);
            })
            .fail((xhr, status, error) => {
                callback(error, null);
            });
    }

    handleResponseWhenError(response) {
        this.showError(msg.common_error);
        console.groupCollapsed("Chi tiết lỗi từ response API");
        console.error("Message: ", response.message);
        console.log("Response API: ", response);
        console.groupEnd();
    }

    intersectionObserve() {
        const lazyImages = document.querySelectorAll(
            "img.lazy-load:not([src])"
        );

        const observer = new IntersectionObserver(
            (entries, observer) => {
                entries.forEach((entry) => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        img.dataset.isAssignedSource = "1";
                        img.src = img.dataset.src;
                        img.classList.remove("lazy-load");
                        observer.unobserve(img);
                    }
                });
            },
            { rootMargin: "0px 0px 50px 0px" }
        );

        lazyImages.forEach((img) => observer.observe(img));
    }

    /**
     * Hàm callback để tuỳ chỉnh header của bảng DataTables.
     *
     * @param {HTMLElement} thead - Phần tử `thead` của bảng DataTables.
     * @param {Array} data - Dữ liệu hiển thị trong bảng.
     * @param {number} start - Chỉ mục bắt đầu của dữ liệu đang hiển thị.
     * @param {number} end - Chỉ mục kết thúc của dữ liệu đang hiển thị.
     * @param {Array} display - Mảng chứa các chỉ mục của dữ liệu đang hiển thị (sau khi sắp xếp, lọc).
     *
     * @example
     * headerCallback = (thead, data, start, end, display) => {
     *     const headerCells = thead.querySelectorAll('th');
     *     headerCells.forEach((cell, index) => {
     *         cell.innerHTML = `Cột ${index + 1}`;
     *     });
     * };
     */
    headerCallback = (thead, data, start, end, display) => {};

    /**
     * Phương thức này được sử dụng để tùy chỉnh nội dung hoặc xử lý logic đặc biệt cho từng hàng. Trả về gồm: `row` (hàng hiện tại), `data` (data object của hàng đó) và `index` (chỉ số của hàng). Lưu ý: Phải sử dụng arrow function đối với hàm này.
     */
    rowCallback = (row, data, index) => {};

    #rowCallback = (row, data, index) => {
        this.rowCallback(row, data, index);

        if (Array.isArray(this.checkboxColumnClass)) {
            var td = null;
            for (const columnClass of this.checkboxColumnClass) {
                var status = data[columnClass];
                if (typeof status == "undefined") continue;
                td = row.querySelector("td." + columnClass);
                td.classList.add("text-center");
                td.innerHTML = this.#createCheckbox(status);
            }
        }
    };

    /**
     * Phương thức này được gọi khi bảng được khởi tạo lần đầu tiên. Lưu ý: Phải sử dụng arrow function đối với hàm này.
     */
    initCallback = (settings, json) => {};

    /**
     * Phương thức này được gọi mỗi khi bảng được vẽ. Lưu ý: Phải sử dụng arrow function đối với hàm này.
     */
    drawCallback = (settings) => {};

    #drawCallback(settings) {
        this.drawCallback(settings);

        if (this.enableLazyLoadForTable) {
            const table = document.querySelector(this.tableCode);
            if (!table) return;

            this.lazyLoadColumnClass.forEach((classColumn) => {
                table.querySelectorAll("td." + classColumn).forEach((td) => {
                    const img = td.querySelector("img");
                    if (img) {
                        const isAssignedSource = img.dataset.isAssignedSource;
                        if (isAssignedSource === "1") return;
                        const currentSrc = img.src;
                        if (!currentSrc) return;
                        img.removeAttribute("src");
                        img.classList.add("lazy-load");
                        img.dataset.src = currentSrc;
                        img.dataset.isAssignedSource = "0";
                    }
                });
            });

            this.intersectionObserve();
        }
    }

    /**
     * Xử lý khi `getList()` thất bại. Phương thức trả về param là response của api.
     */
    onGetListFailed(response) {
        this.handleResponseWhenError(response);
        this.drawTable([]);
    }

    /**
     * Xử lý khi `getList()` thành công. Phương thức trả về param là response của api.
     */
    onGetListSuccessfully(response) {
        this.drawTable(response.data);
    }

    /**
     * Xử lý khi response được trả về từ `getList()`.
     */
    handleGetListResponse(response) {
        if (response.success) {
            this.onGetListSuccessfully(response);
        } else {
            this.onGetListFailed(response);
        }
    }

    /**
     * Thực hiện logic nào đó trước khi thực hiện việc lấy danh sách cho bảng. Có thể dùng cho việc gán lại parameters, các thuộc tính trong oValue,... của API lấy danh sách cho bảng.
     */
    onBeforeGettingList() {}

    /**
     * Phương thức này lấy danh sách phục vụ vẽ bảng. Nếu bạn muốn lấy danh sách từ API, hãy dùng `getItems()`.
     * @param {Object} params - Các tham số cần thiết dưới dạng object { key: value }. Có thể không cần truyền gì nếu không có param.
     * @param {boolean} useQueryFormat - _true_ nếu muốn dạng query string (?param=value), _false_ nếu dạng URL (/param).
     */
    getList(params = null, useQueryFormat = null) {
        if (params && Object.keys(params).length > 0) {
            this.queryParamsGetList = params;
        }

        if (useQueryFormat !== null) {
            this.useQueryFormatGetList = useQueryFormat;
        }

        const queryString = this.buildUrlParams(
            this.queryParamsGetList,
            this.useQueryFormatGetList
        );

        const url =
            this.base_url +
            this.endPoint +
            "/" +
            this.queryGetList +
            queryString;

        this.onBeforeGettingList();
        UcAjax.get(url).done(async (response) => {
            if (!this.jsonConfig) {
                await this.loadJsonConfig(this.jsonConfigPath);
            }
            this.handleGetListResponse(response);
        });
    }

    /**
     * Xử lý khi `insertItem()` thành công. Phương thức trả về param là response của api.
     */
    onInsertSuccessfully(response) {
        this.hiddenKeyValue = response.data.id;
        if (!this.isServerSideTable) {
            if (!this.refreshTableWithoutApiInsert) {
                this.jsonConfigPath && this.getList();
            }
        } else {
            this.drawServerSideTable(this.getOSearch());
        }
    }

    /**
     * Xử lý khi `insertItem()` thất bại. Phương thức trả về param là response của api.
     */
    onInsertFailed(response) {
        this.showError(response.message);
        console.error(response);
    }

    /**
     * Xử lý khi response được trả về từ `insertItem()`.
     */
    handleInsertResponse(response) {
        if (response.success) {
            this.showSuccess(msg.common_add_success);
            this.onInsertSuccessfully(response);
        } else {
            this.onInsertFailed(response);
        }
    }

    /**
     * Thực hiện logic nào đó trước khi thực hiện việc thêm. Có thể dùng cho việc gán lại parameters, các thuộc tính trong oValue,... của API thêm.
     */
    onBeforeInserting() {}

    /**
     * Thực hiện việc thêm mới item. Cần truyền vào `oValue`. Các config liên quan cần được cấu hình bằng phương thức `configure()`.
     */
    insertItem(oValue) {
        this.onBeforeInserting(oValue);
        oValue = this.extendData(oValue);
        UcAjax.post(
            this.base_url + this.endPoint + "/" + this.queryInsert,
            oValue
        ).done((response) => {
            this.handleInsertResponse(response);
            if (this.refreshTableWithoutApiInsert) {
                UcFormHelpers().AddRowToTable(this.tableCode, response.data);
            }
        });
    }

    /**
     * Xử lý khi `updateItem()` thành công. Phương thức trả về param là response của api.
     */
    onUpdateSuccessfully(response) {
        if (!this.isServerSideTable) {
            if (!this.refreshTableWithoutApiUpdate) {
                this.jsonConfigPath && this.getList();
            }
        } else {
            this.drawServerSideTable(this.getOSearch());
        }
    }

    /**
     * Xử lý khi `updateItem()` thất bại. Phương thức trả về param là response của api.
     */
    onUpdateFailed(response) {
        this.showError(response.message);
    }

    /**
     * Xử lý khi response được trả về từ `updateItem()`.
     */
    handleUpdateResponse(response) {
        if (response.success) {
            this.showSuccess(msg.common_update_success);
            this.onUpdateSuccessfully(response);
        } else {
            this.onUpdateFailed(response);
        }
    }

    /**
     * Thực hiện logic nào đó trước khi thực hiện việc sửa. Có thể dùng cho việc gán lại parameters, các thuộc tính trong oValue,... của API sửa. Phương thức trả về oValue.
     */
    onBeforeUpdating(oValue) {}

    updateItem(oValue) {
        this.onBeforeUpdating(oValue);
        oValue = this.extendData(oValue);
        UcAjax.put(
            this.base_url + this.endPoint + "/" + this.queryUpdate,
            oValue
        ).done((response) => {
            this.handleUpdateResponse(response);
            if (this.refreshTableWithoutApiUpdate) {
                UcFormHelpers().UpdateRowOfTable(
                    this.tableCode,
                    response.data.id,
                    response.data
                );
            }
        });
    }

    /**
     * Xử lý khi `getItemById()` thất bại. Phương thức trả về param là response của api.
     */
    onGetItemByIdFailed(response) {
        this.handleResponseWhenError(response);
    }

    /**
     * Xử lý khi `getItemById()` thành công. Phương thức trả về param là response của api.
     */
    onGetItemByIdSuccessfully(response) {
        this.hiddenKeyValue = response.data.id;
        UcFormHelpers().SetFormValues(this.formCode, response.data);
        this.onExtraGetItemByIdSucessfully(response);
    }

    /**
     * Phương thức bổ sung khi `getItemById()` thành công. Phương thức trả về param là response của api.
     */
    onExtraGetItemByIdSucessfully(response) {}

    /**
     * Xử lý khi response được trả về từ `getItemById()`.
     */
    handleGetItemByIdResponse(response) {
        if (response.success) {
            this.onGetItemByIdSuccessfully(response);
        } else {
            this.onGetItemByIdFailed(response);
        }
    }

    /**
     * Phương thức lấy item theo id.
     * @param {Object} params - Tham số cần thiết dưới dạng object { key: value }.
     * @param {boolean} useQueryFormat - `true` nếu muốn dạng query string `?param=value`, `false` nếu dạng URL `/param`.
     */
    getItemById(params = null, useQueryFormat = null) {
        if (params && Object.keys(params).length > 0) {
            this.queryParamsGetItemById = params;
        }

        if (useQueryFormat !== null) {
            this.useQueryFormatGetItemById = useQueryFormat;
        }

        const queryString = this.buildUrlParams(
            this.queryParamsGetItemById,
            this.useQueryFormatGetItemById
        );

        const url =
            this.base_url +
            this.endPoint +
            "/" +
            this.queryGetItemById +
            queryString;
        UcAjax.get(url).done((response) =>
            this.handleGetItemByIdResponse(response)
        );
    }

    /**
     * Xử lý khi `deleteItem()` thất bại. Phương thức trả về param là response của api.
     */
    onDeleteFailed(response) {
        this.showError(msg.common_delete_error);
    }

    /**
     * Xử lý khi `deleteItem()` thành công. Phương thức trả về param là response của api.
     */
    onDeleteSuccessfully(response) {
        if (!this.isServerSideTable) {
            if (!this.refreshTableWithoutApiDelete) {
                this.jsonConfigPath && this.getList();
            }
        } else {
            this.drawServerSideTable(this.getOSearch());
        }
    }

    /**
     * Xử lý khi response được trả về từ `deleteItem()`.
     */
    handleDeleteResponse(response) {
        if (response.success) {
            this.showSuccess(msg.common_delete_success);
            this.onDeleteSuccessfully(response);
        } else {
            this.onDeleteFailed(response);
        }
    }

    /**
     * Phương thức xóa item theo id.
     * @param {Object} params - Tham số cần thiết dưới dạng object { key: value }.
     * @param {boolean} useQueryFormat - `true` nếu muốn dạng query string (?param=value), `false` nếu dạng URL (/param).
     * @param {HTMLElement} elementOfTable - Element của hàng trong bảng cần xóa. Ví dụ: Thẻ `<a></a>` bên trong row, .... Mặc định là `null`
     */
    deleteItem(params = null, useQueryFormat = null, elementOfTable = null) {
        if (params && Object.keys(params).length > 0) {
            this.queryParamsDelete = params;
        }

        if (useQueryFormat !== null) {
            this.useQueryFormatDelete = useQueryFormat;
        }

        const queryString = this.buildUrlParams(
            this.queryParamsDelete,
            this.useQueryFormatDelete
        );

        const url =
            this.base_url +
            this.endPoint +
            "/" +
            this.queryDelete +
            queryString;

        UcAjax.delete(url).done((response) => {
            this.handleDeleteResponse(response);
            if (elementOfTable && this.refreshTableWithoutApiDelete) {
                UcFormHelpers().RemoveRowOfTable(
                    this.tableCode,
                    elementOfTable
                );
            }
        });
    }

    /**
     * Xử lý khi `searchItems()` thành công. Phương thức trả về param là response của api.
     */
    onSearchSuccessfully(response) {
        this.drawTable(response.data);
    }

    /**
     * Xử lý khi `searchItems()` không thành công. Phương thức trả về param là response của api.
     */
    onSearchFailed(response) {
        this.handleResponseWhenError(response);
    }

    /**
     * Xử lý khi response được trả về từ `searchItems()`.
     */
    handleSearchResponse(response) {
        if (response.success) {
            this.onSearchSuccessfully(response);
        } else {
            this.onSearchFailed(response);
        }
    }

    /**
     * Xử lý khi response được trả về từ `searchAdvancedItems()`.
     */
    onGottenAdvancedSearchItems(response) {
        if (response.success) {
            this.drawTable(response.data);
        } else {
            this.handleResponseWhenError(response);
        }
    }

    /**
     * Thực hiện logic nào đó trước khi thực hiện việc tìm kiếm. Có thể dùng cho việc gán lại parameters, các thuộc tính trong oValue,... của API tìm kiếm.
     */
    onBeforeSearching(oSearch) {}

    /**
     * Phương thức tìm kiếm dữ liệu theo oSearch. oSearch của form được lấy bằng phương thức `UcFormHelpers().GetFormValues(formCode)`.
     */
    searchItems(oSearch) {
        this.oSearch = oSearch ? oSearch : this.getOSearch();

        this.onBeforeSearching(oSearch);

        const queryString = this.buildUrlParams(
            this.queryParamsSearch,
            this.useQueryFormatSearch
        );

        UcAjax.post(
            this.base_url +
                this.endPoint +
                "/" +
                this.querySearch +
                queryString,
            this.oSearch,
            true
        ).done((response) => {
            this.handleSearchResponse(response);
        });
    }

    /**
     * Phương thức tìm kiếm dữ liệu nâng cao theo oSearch. oSearch của form được lấy bằng phương thức `UcFormHelpers().GetFormValues(advanceFormCode)`.
     */
    searchAdvancedItems(oSearch) {
        this.oSearch = oSearch ? oSearch : this.getOSearch();

        this.onBeforeSearching();

        UcAjax.post(
            this.base_url + this.endPoint + "/" + this.queryAdvanceSearch,
            this.oSearch,
            true
        ).done(async (response) => {
            if (!this.jsonConfig) {
                await this.loadJsonConfig(this.jsonConfigPath);
            }
            this.onGottenAdvancedSearchItems(response);
        });
    }

    refreshForm() {
        UcFormHelpers().RefreshFormValues(this.formCode);
        this.hiddenKeyValue = null;
        document
            .querySelector(
                `.${this.formCode} input:not([type='hidden']):not([type='checkbox']):not([type='radio'])`
            )
            .focus();
    }

    refreshAdvancedForm() {
        UcFormHelpers().RefreshFormValues(this.advanceFormCode);
        document
            .querySelector(
                `.${this.advanceFormCode} input:not([type='hidden']):not([type='checkbox']):not([type='radio'])`
            )
            .focus();
    }

    /**
     * Lấy oValue của form. Lưu ý: Cần cấu hình code của form bằng formCode của phương thức `configure()`.
     */
    getOValue() {
        try {
            var oForm = UcFormHelpers().GetFormValues(this.formCode);
            if (!oForm) return null;
            var oValue = UcFormHelpers().FormFieldsToObject(oForm);
            return oValue;
        } catch (error) {
            console.error(error);
            return {};
        }
    }

    /**
     * Lấy oValue của form nhưng phục vụ form với thuộc tính `isFormSearch` là `true`. Lưu ý: Cần cấu hình code của form bằng `formCode`, thuộc tính `isFormSearch` bằng phương thức `configure()`.
     */
    getOSearch() {
        try {
            var oSearch = UcFormHelpers().GetFormValues(
                !this.isAdvanceForm ? this.formCode : this.advanceFormCode
            ) ?? {
                fields: [],
            };
            return oSearch;
        } catch (error) {
            console.error(error);
            return { fields: [] };
        }
    }

    /**
     * Xử lý sự kiện khi nhấn nút lưu. Code của nút lưu cần được cấu hình trong `btnSaveCode` bằng phương thức `configure()`.
     */
    onClickSave() {
        this.oValue = this.getOValue();
        if (!this.oValue) return;
        var id = this.hiddenKeyValue;

        if (!id) {
            this.insertItem(this.oValue);
        } else {
            this.oValue.id = id;
            this.updateItem(this.oValue);
        }
    }

    /**
     * Xử lý sự kiện khi nhấn nút tìm kiếm nhanh trong form search. Code của nút tìm kiếm cần được cấu hình trong `btnSearchCode` bằng phương thức `configure()`.
     */
    onClickSearch() {
        this.oSearch = this.getOSearch();

        if (this.oSearch.fields.length == 0) {
            this.showWarning("Vui lòng nhập giá trị tìm kiếm!");
            this.refreshForm();
            return;
        }

        if (!this.isServerSideTable) {
            this.searchItems(this.oSearch);
        } else {
            this.drawServerSideTable(this.oSearch);
        }
    }

    /**
     * Xử lý sự kiện khi nhấn nút tìm kiếm nâng cao. Code của nút tìm kiếm cần được cấu hình trong `btnAdvanceSearchCode` bằng phương thức `configure()`.
     */
    onClickAdvancedSearch() {
        this.oSearch = this.getOSearch();

        if (this.oSearch.fields.length == 0) {
            this.showWarning("Vui lòng nhập giá trị tìm kiếm!");
            this.refreshAdvancedForm();
            return;
        }

        if (!this.isServerSideTable) {
            this.searchAdvancedItems(this.oSearch);
        } else {
            this.drawServerSideTable(this.oSearch);
        }
    }

    /**
     * Xử lý sự kiện khi nhấn 3 chấm trong bảng -> Nút chỉnh sửa với class được gán cho `editCodeOfTable`.
     */
    onClickEdit(id, element) {
        this.refreshForm();
        document.querySelector(
            this.labelCode
        ).textContent = `Chỉnh sửa ${this.formName}`;
        this.hiddenKeyValue = id;
        this.getItemById({ id: id });
    }

    /**
     * Xử lý sự kiện khi nhấn nút làm mới với code được gán cho `btnRefreshCode`.
     */
    onClickRefresh() {
        this.refreshForm();
        this.formCode &&
            document
                .querySelector(
                    `.${this.formCode} input:not([type='hidden']):not([type='checkbox']):not([type='radio'])`
                )
                .focus();
        this.extraOnClickRefresh();
    }

    /**
     * Thêm logic khác khi nhấn nút làm mới với code được gán cho `btnRefreshCode`.
     */
    extraOnClickRefresh() {}

    /**
     * Xử lý sự kiện khi nhấn nút làm mới trong modal tìm kiếm nâng cao với code được gán cho `btnAdvanceRefreshCode`.
     */
    onClickAdvancedRefresh() {
        this.refreshAdvancedForm();
        this.advanceFormCode &&
            document
                .querySelector(
                    `.${this.advanceFormCode} input:not([type='hidden']):not([type='checkbox']):not([type='radio'])`
                )
                .focus();
    }

    /**
     * Xử lý sự kiện khi nhấn nút thêm với code được gán cho `btnAddCode`.
     */
    onClickAdd() {
        this.refreshForm();
        this.hiddenKeyValue = null;
        document.querySelector(
            this.labelCode
        ).textContent = `Thêm mới ${this.formName}`;
    }

    /**
     * Xử lý sự kiện khi nhấn 3 chấm trong bảng -> Nút chi tiết với class được gán cho `detailCodeOfTable`. Phương thức trả về 2 param bao gồm:
     * @param {string} id - Id của item.
     * @param {HTMLElement} element - Element của hàng trong bảng cần xóa.
     */
    onClickDetail(id, element) {}

    /**
     * Thực hiện logic nào đó trước khi thực hiện việc xóa. Có thể dùng cho việc gán lại parameters của API xóa. Phương thức trả về 2 param bao gồm:
     * @param {string} id - Id của item cần xóa.
     * @param {HTMLElement} element - Element của hàng trong bảng cần xóa.
     */
    onBeforeDeleting(id, element) {}

    /**
     * Xử lý sự kiện khi nhấn 3 chấm trong bảng -> Nút xóa với class được gán cho `deleteCodeOfTable`. Phương thức trả về 2 param bao gồm:
     * @param {string} id - Id của item cần xóa.
     * @param {HTMLElement} element - Element của hàng trong bảng cần xóa.
     */
    onClickDelete(id, element) {
        this.onBeforeDeleting(id, element);

        Notification().Confirm(
            this.deleteMessage,
            () => {
                let objParams = { ...this.queryParamsDelete, id };
                this.deleteItem(objParams, null, element);
            },
            null
        );
    }

    /**
     * Xử lý sự kiện khi nhấn 3 chấm trong bảng -> Nút lưu với class được gán cho `saveCodeOfTable`. Phương thức trả về 2 param bao gồm:
     * @param {string} id - Id của item cần lưu.
     * @param {HTMLElement} element - Element của hàng trong bảng cần lưu.
     */
    onClickSaveInTable(id, element) {}

    #getModalInstance(modalCode) {
        const modalEl = document.querySelector(modalCode);
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);
        return modal;
    }

    /**
     * Xử lý sự kiện khi modal bắt đầu xuất hiện. Code của modal cần được cấu hình ở phương thức `configure()`.
     */
    onShowModal() {}

    /**
     * Xử lý sự kiện khi modal tìm kiếm nâng cao bắt đầu xuất hiện. Code của modal cần được cấu hình ở phương thức `configure()`.
     */
    onShowAdvancedModal() {}

    /**
     * Xử lý sự kiện khi modal hiển thị xong. Code của modal cần được cấu hình ở phương thức `configure()`.
     */
    onShownModal() {
        this.formCode &&
            document
                .querySelector(
                    `.${this.formCode} input:not([type='hidden']):not([type='checkbox']):not([type='radio'])`
                )
                .focus();
    }

    /**
     * Xử lý sự kiện khi modal tìm kiếm nâng cao hiển thị xong. Code của modal cần được cấu hình ở phương thức `configure()`.
     */
    onShownAdvancedModal() {
        this.advanceModalCode &&
            document
                .querySelector(
                    `.${this.advanceFormCode} input:not([type='hidden']):not([type='checkbox']):not([type='radio'])`
                )
                .focus();
    }

    /**
     * Xử lý sự kiện khi modal bắt đầu đóng. Code của modal cần được cấu hình ở phương thức `configure()`.
     */
    onHideModal() {}

    /**
     * Xử lý sự kiện khi modal tìm kiếm nâng cao bắt đầu đóng. Code của modal cần được cấu hình ở phương thức `configure()`.
     */
    onHideAdvancedModal() {}

    /**
     * Xử lý sự kiện khi modal đóng hoàn toàn. Code của modal cần được cấu hình ở phương thức `configure()`.
     */
    onHiddenModal() {}

    /**
     * Xử lý sự kiện khi modal tìm kiếm nâng cao đóng hoàn toàn. Code của modal cần được cấu hình ở phương thức `configure()`.
     */
    onHiddenAdvancedModal() {
        document
            .querySelector(
                `.${this.formCode} input:not([type='hidden']):not([type='checkbox']):not([type='radio'])`
            )
            .focus();
    }

    /**
     * Xử lý sự kiện khi ấn nút quay lại với code được gán cho `btnGoBackCode`.
     */
    onClickGoBack() {
        window.history.back();
    }

    /**
     * Xử lý sự kiện khi ấn nút đóng modal với code được gán cho `btnCloseCode`.
     */
    onClickCloseModal() {
        var modalInstance = this.#getModalInstance(this.modalCode);
        modalInstance.hide();
    }

    /**
     * Cuộn tới và focus vào element theo id.
     */
    scrollToControl(id) {
        var element = document.getElementById(id);
        element.scrollIntoView({
            behavior: "smooth",
            block: "start",
        });
        element.focus();
    }

    /**
     * Đặt trạng thái `checked` cho một input (checkbox/radio) và kích hoạt sự kiện `change`. **Lưu ý**: `selector` cần được gán sự kiện bằng phương thức `createChangeEvent()` trước đó.
     */
    setCheckedAndTriggerChange(target, checked) {
        let el = null;

        if (typeof target === "string") {
            el = document.querySelector(target);
        } else if (target instanceof Element) {
            el = target;
        }

        if (!el) return;

        if (typeof checked == "boolean") {
            el.checked = checked;
        }
        el.dispatchEvent(new Event("change", { bubbles: true }));
    }

    /**
     * Tìm phần tử tổ tiên (`ancestor`) từ `startEl` đi lên cho đến khi gặp đúng `targetEl`.
     */
    closestElement(startEl, targetEl) {
        let el = startEl;
        while (el) {
            if (el === targetEl) return el;
            el = el.parentElement;
        }
        return null;
    }

    #createEventManager(eventType, target = document) {
        const callbacks = new Set();
        let initialized = false;

        function handler(e) {
            for (const cb of callbacks) {
                try {
                    cb(e);
                } catch (err) {
                    console.error(`${eventType} callback error:`, err);
                }
            }
        }

        return {
            add(cb) {
                if (typeof cb === "function") callbacks.add(cb);
            },
            remove(cb) {
                callbacks.delete(cb);
            },
            clear() {
                callbacks.clear();
            },
            init() {
                if (!initialized) {
                    target.addEventListener(eventType, handler);
                    initialized = true;
                }
            },
        };
    }

    static #eventSelectors = new Map();
    static #domEventTargets = new Map();
    #eventManagers = {};

    #createEvent(eventType, selector, callbackFunction) {
        if (!UcBase.#eventSelectors.has(eventType)) {
            UcBase.#eventSelectors.set(eventType, new Set());
        }

        const selectors = UcBase.#eventSelectors.get(eventType);

        if (selectors.has(selector)) return;
        selectors.add(selector);

        if (!this.#eventManagers[eventType]) {
            this.#eventManagers[eventType] =
                this.#createEventManager(eventType);
            this.#eventManagers[eventType].init();
        }

        this.#eventManagers[eventType].add((e) => {
            if (typeof selector !== "string") return;
            const target = e.target.closest(selector);
            if (target) {
                callbackFunction(target, e);
            }
        });
    }

    /**
     * Tạo sự kiện `click` cho `selector`.
     *
     * @param {string} selector - Bộ chọn phần tử (`#button`, `.checkbox`, v.v.).
     * @param {function(HTMLElement): void} callbackFunction - Hàm callback xử lý sự kiện khi `click` kích hoạt.
     *
     * **Lưu ý**: Gán sự kiện cho các element trùng id có thể khiến chúng kích hoạt cùng nhau!
     *
     * @example
     * createClickEvent("#myButton", (target, event) => {
     *     console.log("Giá trị mới:", target.value);
     * });
     */
    createClickEvent(selector, callbackFunction) {
        const eventType = "click";
        this.#createEvent(eventType, selector, callbackFunction);
    }

    /**
     * Tạo sự kiện `keyup` cho `selector` khi nhấn phím `Enter`.
     *
     * @param {string} selector - Bộ chọn phần tử (`#input`, `.div`, v.v.).
     * @param {function(HTMLElement): void} callbackFunction - Hàm callback xử lý sự kiện khi nhấn `Enter`.
     *
     * **Lưu ý**: Gán sự kiện cho các element trùng id có thể khiến chúng kích hoạt cùng nhau!
     *
     * @example
     * createEnterEvent("#searchBox", (target, event) => {
     *     console.log("Người dùng nhấn Enter trên:", target);
     * });
     */
    createEnterEvent(selector, callbackFunction) {
        const eventType = "keyup";
        this.#createEvent(eventType, selector, (target, e) => {
            if (e.key == "Enter") {
                callbackFunction(target, e);
            }
        });
    }

    /**
     * Tạo sự kiện `input` cho `selector`.
     *
     * @param {string} selector - Bộ chọn phần tử (`#input`, `.div`, v.v.).
     * @param {function(HTMLElement): void} callbackFunction - Hàm callback xử lý sự kiện khi giá trị thay đổi.
     *
     * **Lưu ý**: Gán sự kiện cho các element trùng id có thể khiến chúng kích hoạt cùng nhau!
     *
     * @example
     * createInputEvent("#textBox", (target, event) => {
     *     console.log("Giá trị nhập:", target.value);
     * });
     */
    createInputEvent(selector, callbackFunction) {
        const eventType = "input";
        this.#createEvent(eventType, selector, callbackFunction);
    }

    /**
     * Tạo sự kiện `change` cho `selector`.
     *
     * @param {string} selector - Bộ chọn phần tử (`#checkbox`, `.select`, v.v.).
     * @param {function(HTMLElement): void} callbackFunction - Hàm callback xử lý sự kiện khi `change` kích hoạt.
     *
     * **Lưu ý**: Gán sự kiện cho các element trùng id có thể khiến chúng kích hoạt cùng nhau!
     *
     * @example
     * createChangeEvent("#mySelect", (target, event) => {
     *     console.log("Giá trị mới:", target.value);
     * });
     */
    createChangeEvent(selector, callbackFunction) {
        const eventType = "change";
        this.#createEvent(eventType, selector, callbackFunction);
    }

    /**
     * Tạo sự kiện `focus` cho `selector`.
     *
     * @param {string} selector - Bộ chọn phần tử (`#input`, `.div`, v.v.).
     * @param {function(HTMLElement): void} callbackFunction - Hàm callback xử lý khi phần tử được focus.
     *
     * **Lưu ý**: Gán sự kiện cho các element trùng id có thể khiến chúng kích hoạt cùng nhau!
     *
     * @example
     * createFocusEvent("#textBox", (target, event) => {
     *     console.log("Được focus:", target);
     * });
     */
    createFocusEvent(selector, callbackFunction) {
        const eventType = "focusin";
        this.#createEvent(eventType, selector, callbackFunction);
    }

    /**
     * Tạo sự kiện `blur` cho `selector`.
     *
     * @param {string} selector - Bộ chọn phần tử (`#input`, `.div`, v.v.).
     * @param {function(HTMLElement): void} callbackFunction - Hàm callback xử lý khi phần tử mất focus.
     *
     * **Lưu ý**: Gán sự kiện cho các element trùng id có thể khiến chúng kích hoạt cùng nhau!
     *
     * @example
     * createBlurEvent("#textBox", (target, event) => {
     *     console.log("Mất focus:", target);
     * });
     */
    createBlurEvent(selector, callbackFunction) {
        var eventType = "focusout";
        this.#createEvent(eventType, selector, callbackFunction);
    }

    /**
     * Tạo sự kiện DOM tùy chỉnh cho một phần tử.
     *
     * @param {string|HTMLElement|jQuery} target - Phần tử DOM hoặc selector CSS hoặc jQuery object.
     * @param {string} eventType - Loại sự kiện DOM (ví dụ: "click", "shown.bs.modal", "blur", ...).
     * @param {function(HTMLElement ,Event): void} callbackFunction - Hàm callback được gọi khi event xảy ra.
     *
     * **Lưu ý**: Gán sự kiện cho các element trùng id có thể khiến chúng kích hoạt cùng nhau!
     *
     * @example
     * // Click trên document với delegation
     * this.createCustomDomEvent("click", document, (target, event) => {
     *     const btn = target.closest("#btnAddCode");
     *     if (btn) this.onClickAdd();
     * });
     *
     * @example
     * // Sự kiện Bootstrap modal
     * this.createCustomDomEvent("shown.bs.modal", this.modalCode, (target, event) => {
     *     this.onShownModal();
     * });
     *
     * @example
     * // Blur cho input
     * this.createCustomDomEvent("blur", "#textBox", (target, event) => {
     *     console.log("Mất focus:", target);
     * });
     */
    createCustomDomEvent(eventType, target, callbackFunction) {
        if (typeof target === "string") {
            target = document.querySelector(target);
        }

        if (target && target.jquery) {
            target = target[0];
        }

        if (!target) {
            console.warn(`Target for event "${eventType}" not found`);
            return;
        }

        if (!UcBase.#domEventTargets.has(eventType)) {
            UcBase.#domEventTargets.set(eventType, new WeakSet());
        }

        const registeredTargets = UcBase.#domEventTargets.get(eventType);

        if (registeredTargets.has(target)) return;
        registeredTargets.add(target);

        if (!this.#eventManagers[eventType]) {
            this.#eventManagers[eventType] = this.#createEventManager(
                eventType,
                target
            );
            this.#eventManagers[eventType].init();
        }

        this.#eventManagers[eventType].add((e) => {
            callbackFunction(e);
        });
    }

    /**
     * Khởi tạo các sự kiện bổ sung cho phương thức `setupEventHandlers()`.
     */
    extraEventHandlers() {}

    /**
     * Khởi tạo các sự kiện nền. Nếu gọi lại phương thức này, các sự kiện nền mặc định sẽ bị mất. Gọi lại trước khi thêm các sự kiện mới để không bị mất các sự kiện nền.
     */
    setupEventHandlers() {
        if (this.btnAddCode) {
            this.createClickEvent(this.btnAddCode, () => this.onClickAdd());
        }

        if (this.modalCode) {
            const events = {
                "shown.bs.modal": () => this.onShownModal(),
                "show.bs.modal": () => this.onShowModal(),
                "hidden.bs.modal": () => this.onHiddenModal(),
                "hide.bs.modal": () => this.onHideModal(),
            };

            for (const [event, handler] of Object.entries(events)) {
                this.createCustomDomEvent(event, this.modalCode, handler);
            }
        }

        if (this.isFormSearch && this.advanceModalCode) {
            const events = {
                "shown.bs.modal": () => {
                    this.isAdvanceForm = true;
                    this.onShownAdvancedModal();
                },
                "show.bs.modal": () => this.onShowAdvancedModal(),
                "hidden.bs.modal": () => {
                    this.isAdvanceForm = false;
                    this.onHiddenAdvancedModal();
                },
                "hide.bs.modal": () => this.onHideAdvancedModal(),
            };

            for (const [event, handler] of Object.entries(events)) {
                this.createCustomDomEvent(
                    event,
                    this.advanceModalCode,
                    handler
                );
            }
        }

        if (this.btnRefreshCode) {
            this.createClickEvent(this.btnRefreshCode, () =>
                this.onClickRefresh()
            );
        }

        if (this.isFormSearch && this.btnAdvanceRefreshCode) {
            this.createClickEvent(this.btnAdvanceRefreshCode, () =>
                this.onClickAdvancedRefresh()
            );
        }

        if (this.btnSaveCode) {
            this.createClickEvent(this.btnSaveCode, () => this.onClickSave());
        }

        if (this.isFormSearch && this.btnSearchCode) {
            this.createClickEvent(this.btnSearchCode, (e) =>
                this.onClickSearch()
            );
        }

        if (this.isFormSearch && this.btnAdvanceSearchCode) {
            this.createClickEvent(this.btnAdvanceSearchCode, () =>
                this.onClickAdvancedSearch()
            );
        }

        if (this.isFormSearch) {
            if (this.formCode) {
                this.createEnterEvent(`.${this.formCode} input`, () => {
                    document.querySelector(this.btnSearchCode).click();
                });
            }

            if (this.advanceFormCode) {
                this.createEnterEvent(`.${this.advanceFormCode} input`, () => {
                    document.querySelector(this.btnAdvanceSearchCode).click();
                });
            }
        }

        if (this.btnGoBackCode) {
            this.createClickEvent(this.btnGoBackCode, (e) => {
                this.onClickGoBack();
            });
        }

        if (this.btnCloseCode) {
            this.createClickEvent(this.btnCloseCode, () =>
                this.onClickCloseModal()
            );
        }

        if (this.editCodeOfTable) {
            this.createClickEvent(
                `${this.tableCode} tbody ${this.editCodeOfTable}`,
                (target) => this.onClickEdit(target.dataset.id, target)
            );
        }

        if (this.detailCodeOfTable) {
            this.createClickEvent(
                `${this.tableCode} tbody ${this.detailCodeOfTable}`,
                (target) => this.onClickDetail(target.dataset.id, target)
            );
        }

        if (this.deleteCodeOfTable) {
            this.createClickEvent(
                `${this.tableCode} tbody ${this.deleteCodeOfTable}`,
                (target) => this.onClickDelete(target.dataset.id, target)
            );
        }

        this.createClickEvent(
            `${this.tableCode} tbody ${this.saveCodeOfTable}`,
            (target) => this.onClickSaveInTable(target.dataset.id, target)
        );

        this.extraEventHandlers();
    }
}
