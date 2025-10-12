/**
 * Chứa các phương thức API tĩnh. Sử dụng Fetch API thay vì jQuery AJAX.
 */
class UcAjax {
    static defaultHeaders = {};
    static defaultOptions = {};

    static initialize() {
        const tokenWeb = document.querySelector('input[name="__RequestVerificationToken"]').value;
        $.ajaxSetup({
            headers: {
                Authorization: "Bearer " + UcHelpers().GetAccessToken(),
                UcSite: client_site,
                AppCode: app_code,
                RequestVerificationToken: tokenWeb
            },
            beforeSend: () => openLoader(),
            error: (jqXHR, textStatus, errorThrown) =>
                UcAjax.handleAjaxError(jqXHR, textStatus, errorThrown),
            complete: () => closeLoader(),
        });

        // Thiết lập headers mặc định
        UcAjax.defaultHeaders = {
            Authorization: "Bearer " + UcHelpers().GetAccessToken(),
            UcSite: client_site,
            AppCode: app_code,
            RequestVerificationToken: tokenWeb
        };

        UcAjax.defaultOptions = {
            headers: UcAjax.defaultHeaders,
        };
    }

    /**
     * Tạo fetch request với các options cơ bản và trả về jQuery-like object
     * @param {string} url - URL endpoint
     * @param {object} options - Fetch options
     * @param {boolean} hasToken - Có gắn token không
     * @param {boolean} loading - Hiển thị loader không
     * @returns {Promise<any> & {done: Function, fail: Function, always: Function}}
     */
    static _fetch(url, options = {}, hasToken = true, loading = true) {
        if (loading) openLoader();

        const fetchPromise = (async () => {
            try {
                // Merge headers
                const headers = { ...UcAjax.defaultHeaders };

                if (!hasToken) {
                    delete headers.Authorization;
                    headers.UcSite = client_site;
                }

                // Merge với headers từ options
                if (options.headers) {
                    Object.assign(headers, options.headers);
                }

                const fetchOptions = {
                    ...options,
                    headers,
                };

                const response = await fetch(url, fetchOptions);

                // Xử lý lỗi HTTP
                if (!response.ok) {
                    await UcAjax._handleFetchError(response, url);
                    return;
                }

                // Parse response dựa trên Content-Type
                const contentType = response.headers.get("content-type");
                let data;

                if (contentType && contentType.includes("application/json")) {
                    data = await response.json();
                } else if (contentType && contentType.includes("text/")) {
                    data = await response.text();
                } else {
                    data = await response.blob();
                }

                return data;
            } catch (error) {
                UcAjax._handleNetworkError(error, url);
                throw error;
            } finally {
                if (loading) closeLoader();
            }
        })();

        // Thêm jQuery-like methods để backward compatibility
        const jqueryLikePromise = fetchPromise.then(
            (data) => data,
            (error) => {
                throw error;
            }
        );

        // Thêm .done(), .fail(), .always() methods
        jqueryLikePromise.done = function (callback) {
            this.then(callback);
            return this;
        };

        jqueryLikePromise.fail = function (callback) {
            this.catch(callback);
            return this;
        };

        jqueryLikePromise.always = function (callback) {
            this.finally(callback);
            return this;
        };

        return jqueryLikePromise;
    }

    /**
     * Gọi API bằng phương thức GET.
     * @param {string} url - Đường dẫn API.
     * @param {boolean} hasToken - `true` để gắn token vào headers, `false` để loại bỏ token khỏi headers. Mặc định là `true`.
     * @returns {Promise<any>} Trả về Promise được resolve hoặc reject.
     */
    static get(url, hasToken = true) {
        return UcAjax._fetch(
            url,
            {
                method: "GET",
            },
            hasToken,
            true
        );
    }

    /**
     * Gọi API bằng phương thức POST.
     * @param {string} url - Đường dẫn API.
     * @param {object} data - Dữ liệu gửi lên server.
     * @param {boolean} hasToken - `true` để gắn token vào headers, `false` để loại bỏ token khỏi headers. Mặc định là `true`.
     * @param {boolean} loading - Hiển thị loader. Mặc định là `true`.
     * @returns {Promise<any>} Trả về Promise được resolve hoặc reject.
     */
    static post(url, data = {}, hasToken = true, loading = true) {
        const isFormData = data instanceof FormData;

        const options = {
            method: "POST",
            body: isFormData ? data : JSON.stringify(data),
        };

        // Chỉ set Content-Type nếu không phải FormData
        if (!isFormData) {
            options.headers = {
                "Content-Type": "application/json",
            };
        }

        return UcAjax._fetch(url, options, hasToken, loading);
    }

    /**
     * Gọi API bằng phương thức PUT.
     * @param {string} url - Đường dẫn API.
     * @param {object} data - Dữ liệu cập nhật gửi lên server.
     * @param {boolean} hasToken - `true` để gắn token vào headers, `false` để loại bỏ token khỏi headers. Mặc định là `true`.
     * @param {boolean} loading - Hiển thị loader. Mặc định là `true`.
     * @returns {Promise<any>} Trả về Promise được resolve hoặc reject.
     */
    static put(url, data = {}, hasToken = true, loading = true) {
        const isFormData = data instanceof FormData;

        const options = {
            method: "PUT",
            body: isFormData ? data : JSON.stringify(data),
        };

        // Chỉ set Content-Type nếu không phải FormData
        if (!isFormData) {
            options.headers = {
                "Content-Type": "application/json",
            };
        }

        return UcAjax._fetch(url, options, hasToken, loading);
    }

    /**
     * Gọi API bằng phương thức DELETE.
     * @param {string} url - Đường dẫn API.
     * @param {boolean} hasToken - `true` để gắn token vào headers, `false` để loại bỏ token khỏi headers. Mặc định là `true`.
     * @param {boolean} loading - Hiển thị loader. Mặc định là `true`.
     * @returns {Promise<any>} Trả về Promise được resolve hoặc reject.
     */
    static delete(url, hasToken = true, loading = true) {
        return UcAjax._fetch(
            url,
            {
                method: "DELETE",
            },
            hasToken,
            loading
        );
    }

    /**
     * Gọi API bằng phương thức PATCH.
     * @param {string} url - Đường dẫn API.
     * @param {object} data - Dữ liệu cập nhật gửi lên server.
     * @param {boolean} hasToken - `true` để gắn token vào headers, `false` để loại bỏ token khỏi headers. Mặc định là `true`.
     * @param {boolean} loading - Hiển thị loader. Mặc định là `true`.
     * @returns {Promise<any>} Trả về Promise được resolve hoặc reject.
     */
    static patch(url, data = {}, hasToken = true, loading = true) {
        return UcAjax._fetch(
            url,
            {
                method: "PATCH",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(data),
            },
            hasToken,
            loading
        );
    }

    /**
     * Hỗ trợ gửi lại yêu cầu khi gặp lỗi, với số lần thử lại tối đa và thời gian chờ giữa các lần.
     * @param {Function} requestFn - Hàm gọi api và trả về Promise.
     * @param {number} retries - Số lần thử. Mặc định là 3.
     * @param {number} delay - Thời gian giữa các lần thử (miliseconds). Mặc định là 1000ms.
     * @returns {Promise<any>} Trả về Promise được resolve hoặc reject.
     * @example
     * // Example usage with a GET request
     * UcAjax.retryRequest(() => UcAjax.get('/api/resource'), 3, 1000)
     *   .then((data) => console.log('Success:', data))
     *   .catch((error) => console.error('Failed after retries:', error));
     */
    static retryRequest(requestFn, retries = 3, delay = 1000) {
        return new Promise((resolve, reject) => {
            const attempt = () => {
                requestFn()
                    .then(resolve)
                    .catch((error) => {
                        if (retries > 0) {
                            retries--;
                            setTimeout(attempt, delay);
                        } else {
                            reject(error);
                        }
                    });
            };
            attempt();
        });
    }

    /**
     * @typedef {Object} RequestConfig
     * @property {Function} requestFn - Hàm khởi tạo yêu cầu và trả về Promise.
     * @property {Function} onSuccess - Hàm callback để xử lý khi thành công.
     * @property {Function} onError - Hàm callback để xử lý khi thất bại.
     */

    /**
     * Thực hiện một loạt yêu cầu với logic thử lại, hỗ trợ các lệnh gọi lại riêng lẻ để xử lý phản hồi và lỗi.
     * @param {Array<RequestConfig>} requestConfigs - Một mảng các đối tượng, mỗi đối tượng chứa:
     * - requestFn: {Function} Hàm khởi tạo yêu cầu và trả về Promise.
     * - onSuccess: {Function} (Tùy chọn) Hàm gọi lại để xử lý các phản hồi thành công (response.data).
     * - onError: {Function} (Tùy chọn) Hàm gọi lại để xử lý lỗi.
     * @param {number} [retries=3] - Số lần thử lại tối đa cho mỗi yêu cầu.
     * @param {number} [delay=1000] - Độ trễ (tính bằng mili giây) giữa các lần thử lại đối với các yêu cầu không thành công.
     * @returns {Promise<any[]>} Lời hứa được giải quyết bằng một loạt phản hồi thành công nếu tất cả yêu cầu đều thành công, hoặc từ chối với một loạt thành công và thất bại.
     *
     * @example
     * const requestConfigs = [
     *   {
     *     requestFn: () => UcAjax.get('/api/resource1'),
     *     onSuccess: (data) => console.log('Resource 1 data:', data),
     *     onError: (error) => console.error('Error fetching resource 1:', error),
     *   },
     *   {
     *     requestFn: () => UcAjax.post('/api/resource2', { payload: 'example' }),
     *     onSuccess: (data) => console.log('Resource 2 created:', data),
     *     onError: (error) => console.error('Error creating resource 2:', error),
     *   },
     * ];
     *
     * UcAjax.batchRequestsWithRetry(requestConfigs, 3, 1000)
     *   .then((successes) => {
     *     console.log('All requests completed successfully:', successes);
     *   })
     *   .catch(({ successes, failures }) => {
     *     console.warn('Some requests failed.');
     *     console.log('Successful responses:', successes);
     *     console.error('Failed requests:', failures);
     *   });
     */
    static batchRequestsWithRetry(
        requestConfigs = [],
        retries = 3,
        delay = 1000
    ) {
        if (requestConfigs.length == 0) return Promise.resolve([]);

        const promises = requestConfigs.map(
            ({ requestFn, onSuccess, onError }) =>
                this.retryRequest(requestFn, retries, delay)
                    .then((response) => {
                        if (onSuccess) {
                            onSuccess(response);
                        }
                        return response;
                    })
                    .catch((error) => {
                        if (onError) {
                            onError(error);
                        }
                        throw error;
                    })
        );

        return Promise.allSettled(promises).then((results) => {
            const successes = results
                .filter((result) => result.status === "fulfilled")
                .map((result) => result.value);
            const failures = results
                .filter((result) => result.status === "rejected")
                .map((result) => result.reason);

            if (failures.length > 0) {
                return Promise.reject({
                    successes,
                    failures,
                });
            }

            return successes;
        });
    }

    /**
     * Xử lý lỗi HTTP từ fetch response
     * @param {Response} response - Fetch Response object
     * @param {string} url - URL của request
     */
    static async _handleFetchError(response, url) {
        let errorMessage =
            msg.common_error || "Đã xảy ra lỗi. Vui lòng thử lại.";

        try {
            const contentType = response.headers.get("content-type");
            let errorResponse;

            if (contentType && contentType.includes("application/json")) {
                errorResponse = await response.json();
                errorMessage = errorResponse.message || msg.common_error;
            } else {
                const textResponse = await response.text();
                if (textResponse) {
                    errorMessage = textResponse;
                }
            }
        } catch (e) {
            console.error("Lỗi phân tích response:", e);
        }

        // Xử lý theo status code
        switch (response.status) {
            case 400:
                errorMessage = errorMessage || "Yêu cầu không hợp lệ (400).";
                break;
            case 401:
                errorMessage =
                    errorMessage ||
                    "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
                break;
            case 403:
                errorMessage =
                    errorMessage ||
                    "Bạn không có quyền truy cập tài nguyên này (403).";
                break;
            case 404:
                errorMessage =
                    errorMessage || "Không tìm thấy tài nguyên yêu cầu (404).";
                break;
            case 408:
                errorMessage =
                    errorMessage ||
                    "Yêu cầu quá lâu (408). Kiểm tra kết nối mạng.";
                break;
            case 429:
                errorMessage =
                    errorMessage || "Quá nhiều yêu cầu. Vui lòng thử lại sau.";
                break;
            case 500:
                errorMessage =
                    errorMessage || "Lỗi máy chủ (500). Vui lòng thử lại sau.";
                break;
            case 502:
                errorMessage =
                    errorMessage || "Lỗi gateway (502). Máy chủ gặp sự cố.";
                break;
            case 503:
                errorMessage =
                    errorMessage || "Máy chủ hiện không khả dụng (503).";
                break;
            case 504:
                errorMessage =
                    errorMessage || "Máy chủ không phản hồi kịp thời (504).";
                break;
            default:
                errorMessage = `${msg.common_error} (Mã: ${response.status}).`;
                break;
        }

        // Hiển thị lỗi cho người dùng
        Toast().ShowToastError(errorMessage);

        // Log lỗi chi tiết
        console.groupCollapsed("Chi tiết lỗi Fetch");
        console.error("URL:", url);
        console.error("Mã trạng thái:", response.status);
        console.error("Status Text:", response.statusText);
        console.error("Headers:", [...response.headers.entries()]);
        console.groupEnd();

        const error = new Error(errorMessage);
        error.status = response.status;
        error.response = response;
        throw error;
    }

    /**
     * Xử lý lỗi network (connection errors, timeouts, etc.)
     * @param {Error} error - Network error
     * @param {string} url - URL của request
     */
    static _handleNetworkError(error, url) {
        let errorMessage;

        if (error.name === "AbortError") {
            errorMessage = "Yêu cầu đã bị hủy.";
        } else if (
            error.message.includes("NetworkError") ||
            error.message.includes("Failed to fetch")
        ) {
            errorMessage = "Lỗi kết nối. Vui lòng kiểm tra internet.";
        } else if (error.message.includes("timeout")) {
            errorMessage =
                "Yêu cầu đã hết thời gian chờ. Vui lòng kiểm tra kết nối mạng.";
        } else {
            errorMessage =
                error.message ||
                msg.common_error ||
                "Đã xảy ra lỗi. Vui lòng thử lại.";
        }

        // Hiển thị lỗi cho người dùng
        Toast().ShowToastError(errorMessage);

        // Log lỗi chi tiết
        console.groupCollapsed("Chi tiết lỗi Network");
        console.error("URL:", url);
        console.error("Error name:", error.name);
        console.error("Error message:", error.message);
        console.error("Error stack:", error.stack);
        console.groupEnd();
    }

    /**
     * Xử lý lỗi toàn cục - Giữ lại để backward compatibility
     * @deprecated Sử dụng _handleFetchError và _handleNetworkError thay thế
     */
    static handleAjaxError(jqXHR, textStatus, errorThrown) {
        console.warn(
            "handleAjaxError is deprecated. Use _handleFetchError and _handleNetworkError instead."
        );

        let errorMessage =
            msg.common_error || "Đã xảy ra lỗi. Vui lòng thử lại.";

        if (jqXHR.responseText) {
            try {
                let errorResponse = JSON.parse(jqXHR.responseText);
                errorMessage = errorResponse.message || msg.common_error;
            } catch (e) {
                errorMessage = "Phản hồi từ máy chủ không hợp lệ.";
                console.error("Lỗi phân tích JSON:", e);
            }
        } else if (textStatus === "timeout") {
            errorMessage =
                "Yêu cầu đã hết thời gian chờ. Vui lòng kiểm tra kết nối mạng.";
        } else if (textStatus === "abort") {
            errorMessage = "Yêu cầu đã bị hủy.";
        } else {
            switch (jqXHR.status) {
                case 0:
                    errorMessage = "Lỗi kết nối. Vui lòng kiểm tra internet.";
                    break;
                case 400:
                    errorMessage = "Yêu cầu không hợp lệ (400).";
                    break;
                case 401:
                    errorMessage =
                        "Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.";
                    break;
                case 403:
                    errorMessage =
                        "Bạn không có quyền truy cập tài nguyên này (403).";
                    break;
                case 404:
                    errorMessage = "Không tìm thấy tài nguyên yêu cầu (404).";
                    break;
                case 408:
                    errorMessage =
                        "Yêu cầu quá lâu (408). Kiểm tra kết nối mạng.";
                    break;
                case 429:
                    errorMessage = "Quá nhiều yêu cầu. Vui lòng thử lại sau.";
                    break;
                case 500:
                    errorMessage = "Lỗi máy chủ (500). Vui lòng thử lại sau.";
                    break;
                case 502:
                    errorMessage = "Lỗi gateway (502). Máy chủ gặp sự cố.";
                    break;
                case 503:
                    errorMessage = "Máy chủ hiện không khả dụng (503).";
                    break;
                case 504:
                    errorMessage = "Máy chủ không phản hồi kịp thời (504).";
                    break;
                default:
                    errorMessage = `${msg.common_error} (Mã: ${jqXHR.status}).`;
                    break;
            }
        }

        Toast().ShowToastError(errorMessage);

        console.groupCollapsed("Chi tiết lỗi AJAX");
        console.error("URL:", jqXHR.responseURL || "Không xác định");
        console.error("Mã trạng thái:", jqXHR.status);
        console.error("Trạng thái:", textStatus);
        console.error("Lỗi:", errorThrown);
        console.error("Phản hồi:", jqXHR.responseText);
        console.groupEnd();
    }
}
