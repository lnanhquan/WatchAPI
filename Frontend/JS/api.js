const base = "https://localhost:7123/api";
window.api = axios.create({
    baseURL: base,
    timeout: 10000
});

// Parse JWT payload
function parseJwt(token) {
    try {
        return JSON.parse(atob(token.split('.')[1]));
    } catch {
        return null;
    }
}

// Check if token is about to expire (threshold in seconds)
function isTokenExpiring(token, threshold = 60) {
    const payload = parseJwt(token);
    if (!payload || !payload.exp) return true;
    const now = Date.now() / 1000;
    return payload.exp - now < threshold;
}

let isRefreshing = false;
let refreshSubscribers = [];

// Add requests to a waiting list
function subscribeTokenRefresh(cb) {
    refreshSubscribers.push(cb);
}

// Add the new refreshed token to the waiting requests and reset the list
function onRefreshed(token) {
    refreshSubscribers.forEach(cb => cb(token));
    refreshSubscribers = [];
}

// Request interceptor
api.interceptors.request.use(async (config) => {
    const user = JSON.parse(localStorage.getItem("user") || "{}");

    if (user?.accessToken) {
        if (!config.url.includes("/refresh-token") && isTokenExpiring(user.accessToken)) 
        {
            if (!isRefreshing) {
                isRefreshing = true;
                try {
                    const response = await api.post("/Auth/refresh-token", {
                        AccessToken: user.accessToken,
                        RefreshToken: user.refreshToken
                    });

                    user.accessToken = response.data.accessToken;
                    user.refreshToken = response.data.refreshToken;
                    localStorage.setItem("user", JSON.stringify(user));
                    isRefreshing = false;
                    onRefreshed(user.accessToken);
                } catch (err) {
                    isRefreshing = false;
                    onRefreshed(null);
                    localStorage.removeItem("user");
                    return Promise.reject(err);
                }
            }

            // Wait for token to be refreshed
            await new Promise((resolve, reject) => {
                subscribeTokenRefresh((newToken) => {
                    if (!newToken) return reject("Refresh token failed");
                    config.headers.Authorization = `Bearer ${newToken}`;
                    resolve();
                });
            });
        } 
        else 
        {
            config.headers.Authorization = `Bearer ${user.accessToken}`;
        }
    }
    return config;
}, (error) => Promise.reject(error));

// Response interceptor
api.interceptors.response.use(
    (response) => {
        return response;
    },
    async (error) => {
        const originalRequest = error.config;
        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;

            const user = JSON.parse(localStorage.getItem("user") || "{}");

            if (user?.accessToken && !originalRequest.url.includes("/refresh-token")) {
                if (!isRefreshing) {
                    isRefreshing = true;
                    try {
                        const response = await api.post("/Auth/refresh-token", {
                            AccessToken: user.accessToken,
                            RefreshToken: user.refreshToken
                        });

                        user.accessToken = response.data.accessToken;
                        user.refreshToken = response.data.refreshToken;
                        localStorage.setItem("user", JSON.stringify(user));

                        isRefreshing = false;
                        onRefreshed(user.accessToken);
                    } catch (err) {
                        isRefreshing = false;
                        onRefreshed(null);
                        localStorage.removeItem("user");
                        return Promise.reject(err);
                    }
                }

                return new Promise((resolve, reject) => {
                    subscribeTokenRefresh((newToken) => {
                        if (!newToken) return reject("Refresh token failed");
                        originalRequest.headers.Authorization = `Bearer ${newToken}`;
                        resolve(api(originalRequest));
                    });
                });
            }
        }
        return Promise.reject(error);
    }
);