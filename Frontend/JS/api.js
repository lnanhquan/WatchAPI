const base = "https://localhost:7123/api";
window.api = axios.create({
    baseURL: base,
    timeout: 10000
});

// Thêm token vào header trước khi gửi request
api.interceptors.request.use(config => {
    const user = JSON.parse(localStorage.getItem("user"));
    if (user) {
        config.headers.Authorization = `Bearer ${user.token}`;
    }
    return config;
}, error => {
    return Promise.reject(error);
});

// Xử lý lỗi response
api.interceptors.response.use(
    response => response,
    error => {
        if (error.response?.status === 401) {
            console.warn("Unauthorized — Token might be expired");
        }
        return Promise.reject(error);
    }
);