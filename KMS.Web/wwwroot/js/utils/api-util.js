export async function fetchRestful({
    url,
    method = "GET",
    headers = {},
    data = null,
    contentType = null,
    responseType = "auto", // auto | json | text | blob
    apiKey = null,
    beforeSend = null,
    finallyCallback = null,
    retries = 0,
    retryDelay = 1000, // ms
    timeout = 0, // ms, 0 = no timeout
}) {
    // Gọi trước khi send
    beforeSend?.();

    // Setup abort controller nếu có timeout
    const controller = new AbortController();
    const signal = controller.signal;
    if (timeout > 0) {
        setTimeout(() => controller.abort(), timeout);
    }

    // Headers mặc định
    const defaultHeaders = {
        "X-API-KEY": apiKey,
        ...(contentType ? { "Content-Type": contentType } : {}),
    };

    const buildFetchOptions = () => {
        const opts = {
            method,
            headers: { ...defaultHeaders, ...headers },
            signal,
        };

        if (data) {
            if (data instanceof FormData || data instanceof URLSearchParams) {
                opts.body = data; // để fetch tự set content-type
                delete opts.headers["Content-Type"];
            } else if (
                contentType &&
                contentType.includes("application/json")
            ) {
                opts.body = JSON.stringify(data);
            } else {
                opts.body = data;
            }
        }

        return opts;
    };

    // Hàm xử lý response
    const handleResponse = async (response) => {
        if (!response.ok) {
            const errBody = await response.text().catch(() => "");
            const error = new Error(response.statusText || "Fetch error");
            error.status = response.status;
            error.body = errBody;
            throw error;
        }

        const contentTypeHeader = response.headers.get("content-type") || "";

        switch (responseType.toLowerCase()) {
            case "blob":
                return await response.blob();
            case "text":
                return await response.text();
            case "json":
                return await response.json();
            case "auto":
            default:
                if (contentTypeHeader.includes("application/json")) {
                    const text = await response.text();
                    return text ? JSON.parse(text) : null;
                }
                if (contentTypeHeader.includes("text/"))
                    return await response.text();
                return await response.blob();
        }
    };

    let attempt = 0;
    try {
        while (true) {
            try {
                const response = await fetch(url, buildFetchOptions());
                return await handleResponse(response);
            } catch (err) {
                if (attempt < retries) {
                    attempt++;
                    await new Promise((r) => setTimeout(r, retryDelay));
                } else {
                    throw err;
                }
            }
        }
    } finally {
        finallyCallback?.();
    }
}
