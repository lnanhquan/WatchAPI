// Base URL API (tự động lấy host + port)
const getBaseUrl = () => {
    const url = new URL("https://localhost:7123");
    return `${url.protocol}//${url.host}`;
};

const fullServerUrl = getBaseUrl();

// Convert ảnh relative path → full URL
const getFullImageUrl = (relativeUrl) => {
    if (!relativeUrl) return '';
    if (relativeUrl.startsWith('http://') || relativeUrl.startsWith('https://')) {
        return relativeUrl;
    }
    return `${fullServerUrl}${relativeUrl.startsWith('/') ? '' : '/'}${relativeUrl.trimStart('/')}`;
};

function updateUIAfterLogin(isLoggedIn) {
    const btnLogin = document.getElementById("btnLogin");
    const btnLogout = document.getElementById("btnLogout");
    const user = JSON.parse(localStorage.getItem("user"));

    if (isLoggedIn) {
        btnLogin.classList.add("d-none");
        btnLogout.classList.remove("d-none");
        userGreeting.textContent = `Welcome, ${user.userName}!`;
        userGreeting.classList.remove("d-none");
    } else {
        btnLogin.classList.remove("d-none");
        btnLogout.classList.add("d-none");
        userGreeting.textContent = ``;
        userGreeting.classList.add("d-none");
    }

    if (user && user.roles && user.roles.includes("Admin")) {
        renderSidebar();
    }
}

function createPagination({ totalItems, pageSize, currentPage, containerId, onPageClick }) {
    const container = document.getElementById(containerId);
    if (!container) return;

    container.innerHTML = "";

    const pageCount = Math.ceil(totalItems / pageSize);
    if (pageCount <= 1) return; 

    for (let i = 1; i <= pageCount; i++) {
        const btn = document.createElement("button");
        btn.textContent = i;
        btn.className = "btn btn-outline-primary me-1";
        if (i === Number(currentPage)) btn.classList.add("active");

        btn.addEventListener("click", () => {
            if (i === currentPage) return; 
            onPageClick(i);
        });

        container.appendChild(btn);
    }
}