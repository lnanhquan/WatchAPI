const base = "https://localhost:7123/api";
window.api = axios.create({
    baseURL: base,
    timeout: 10000
});

async function getAccessToken() {
    let user = JSON.parse(localStorage.getItem("user") || "{}");
    if (!user?.accessToken || !user?.refreshToken) return null;

    const tokenParts = user.accessToken.split('.');
    if (tokenParts.length !== 3) return null;

    const payload = JSON.parse(atob(tokenParts[1]));
    const exp = payload.exp * 1000;
    const now = Date.now();

    if (exp - now > 60000) return user.accessToken; // Token còn >1 phút

    // Access token gần hết hạn, refresh
    try {
        const response = await axios.post(`${base}/Auth/refresh-token`, {
            accessToken: user.accessToken,
            refreshToken: user.refreshToken
        });
        user.accessToken = response.data.accessToken;
        user.refreshToken = response.data.refreshToken;
        localStorage.setItem("user", JSON.stringify(user));
        return user.accessToken;
    } catch (err) {
        localStorage.removeItem("user");
        updateUIAfterLogin(false); 
        Swal.fire({
            icon: "warning",
            title: "Session expired",
            text: "Please log in again.",
            toast: true,
            position: "bottom-end",
            timer: 3000,
            timerProgressBar: true,
            showConfirmButton: false
        });
        return null;
    }
}

// Thêm token vào header trước khi gửi request
api.interceptors.request.use(async (config) => {
    const accessToken = await getAccessToken();
    if (accessToken) {
        config.headers.Authorization = `Bearer ${accessToken}`;
    }
    return config;
}, error => Promise.reject(error));

// Tự động retry request nếu nhận 401 (token hết hạn)
api.interceptors.response.use(
    response => response,
    async (error) => {
        const originalRequest = error.config;
        if (error.response?.status === 401 && !originalRequest._retry) {
            originalRequest._retry = true;
            const newAccessToken = await getAccessToken();
            if (newAccessToken) {
                originalRequest.headers['Authorization'] = `Bearer ${newAccessToken}`;
                return api(originalRequest);
            }
        }
        return Promise.reject(error);
    }
);