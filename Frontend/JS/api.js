const base = "https://localhost:7123/api";
window.api = axios.create({
    baseURL: base,
    timeout: 10000,
    withCredentials: true
});


// Response interceptor
api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.data?.error === "RefreshTokenExpired") {
      Swal.fire({
        icon: "warning",
        title: "Session Expired",
        text: "Your session has expired. Please log in again.",
        confirmButtonText: "OK"
      }).then(() => {
        localStorage.removeItem("user");
        window.location.href = "home.html";
      });

      return Promise.reject(error);
    }

    if (originalRequest.url === "/Auth/refresh-token") {
      return Promise.reject(error);
    }

    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        await api.post("/Auth/refresh-token");
        return api(originalRequest);
      } catch (err) {
        return Promise.reject(err);
      }
    }

    return Promise.reject(error);
  }
);

