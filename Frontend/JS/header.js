let loginModal;
let registerModal;

function renderHeader() {
    const headerHTML = `
        <nav class="navbar navbar-expand-sm navbar-light bg-white border-bottom box-shadow mb-3 fixed-top">
            <div class="container-fluid d-flex align-items-center">

                <ul class="navbar-nav me-auto mb-2 mb-sm-0">
                    <li class="nav-item">
                        <a class="nav-link text-dark" href="home.html">Home</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link text-dark" href="home.html?category=men">Men</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link text-dark" href="home.html?category=women">Women</a>
                    </li>
                    <li class="nav-item">
                        <a class="nav-link text-dark" href="home.html?category=couple">Couple</a>
                    </li>
                </ul>

                <form id="searchForm" class="d-flex mx-auto position-relative">
                    <input id="searchInput" class="form-control pe-5" type="search" placeholder="Search watches..." aria-label="Search" style="width: 450px;">
                    <i class="bi bi-search position-absolute" style="right: 10px; top: 50%; transform: translateY(-50%); color: #6c757d;"></i>
                </form>

                <div class="d-flex align-items-center ms-auto">
                    <span id="userGreeting" class="me-2 d-none"></span>

                    <button id="btnOrder" class="btn btn-outline-secondary position-relative me-2">
                        <i class="bi bi-receipt"></i>
                    </button>

                    <button id="btnCart" class="btn btn-outline-success position-relative me-2">
                        <i class="bi bi-cart3"></i>
                        <span id="cartCount" class="position-absolute top-0 start-100 translate-middle badge rounded-pill bg-danger">0</span>
                    </button>

                    <button id="btnLogin" class="btn btn-outline-primary me-2">Login</button>
                    <button id="btnLogout" class="btn btn-outline-danger d-none">Logout</button>
                </div>

            </div>
        </nav>

        <div class="modal fade" id="loginModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header justify-content-center">
                        <h2 class="modal-title">Login</h2>
                    </div>
                    <div class="modal-body">
                        <form id="loginForm">
                            <input type="email" id="loginEmail" class="form-control mb-2" placeholder="Email" autocomplete="email"></input>
                            <input type="password" id="loginPassword" class="form-control mb-2" placeholder="Password" autocomplete="current-password">
                        </form>
                    </div>
                    <div class="modal-footer d-flex justify-content-center">
                        <button type="button" class="btn btn-primary me-2" onclick="login()">Login</button>
                        <button type="button" class="btn btn-success" onclick="switchToRegister()">Register</button>
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="registerModal" tabindex="-1">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header justify-content-center">
                        <h2 class="modal-title">Register</h2>
                    </div>
                    <div class="modal-body">
                        <form id="registerForm">
                            <input type="email" id="registerEmail" class="form-control mb-2" placeholder="Email">
                            <input type="text" id="registerUsername" class="form-control mb-2" placeholder="Username" autocomplete="username">
                            <input type="password" id="registerPassword" class="form-control mb-2" placeholder="Password" autocomplete="current-password">
                        </form>
                    </div>
                    <div class="modal-footer d-flex justify-content-center">
                        <button type="button" class="btn btn-success me-2" onclick="register()">Register</button>
                        <button type="button" class="btn btn-secondary" onclick="switchToLogin()">Back to Login</button>
                    </div>
                </div>
            </div>
        </div>
    `;

    document.getElementById("header").insertAdjacentHTML("beforeend", headerHTML);
    document.getElementById("btnLogin").addEventListener("click", openLoginModal);
    document.getElementById("btnLogout").addEventListener("click", logout);

    const user = JSON.parse(localStorage.getItem("user"));

    document.getElementById("btnCart").addEventListener("click", () => {
        const user = JSON.parse(localStorage.getItem("user"));
        if (!user) {
            Swal.fire({
                icon: 'info',
                title: 'Please login',
                text: 'You must be logged in to view your cart.'
            });
            return;
        }
        window.location.href = "cart.html";
    });

    document.getElementById("btnOrder").addEventListener("click", () => {
        const user = JSON.parse(localStorage.getItem("user"));
        if (!user) {
            Swal.fire({
                icon: 'info',
                title: 'Please login',
                text: 'You must be logged in to view your orders.'
            });
            return;
        }
        window.location.href = "order.html";
    });

    updateUIAfterLogin(!!user);
    loadCartCount();
}

function renderSidebar() {
    if (document.getElementById("adminSidebar")) return;
    const sidebarHTML = `
    <div id="adminSidebar" class="admin-sidebar">
        <div class="sidebar-header">
            <button id="toggleSidebar" class="toggle-btn border-0"><i class="bi bi-list"></i></button>
            <h5>Management</h5>
        </div>
        <ul>
            <li><a href="userManagement.html"><i class="bi bi-person me-2"></i><span>Users</span></a></li>
            <li><a href="watch.html"><i class="bi bi-watch me-2"></i><span>Watches</span></a></li>
            <li><a href="invoice.html"><i class="bi bi-receipt me-2"></i><span>Invoices</span></a></li>
        </ul>
    </div>
    `;

    document.body.insertAdjacentHTML("afterbegin", sidebarHTML);
    document.body.classList.add("has-sidebar");

    const toggleBtn = document.getElementById("toggleSidebar");
    const sidebar = document.getElementById("adminSidebar");

    const isCollapsed = localStorage.getItem("sidebarCollapsed") === "true";
    if (isCollapsed) {
        sidebar.classList.add("collapsed");
    }

    toggleBtn.addEventListener("click", () => {
        sidebar.classList.toggle("collapsed");
        localStorage.setItem("sidebarCollapsed", sidebar.classList.contains("collapsed"));
    });
}

function removeSidebar() {
    const sidebar = document.getElementById("adminSidebar");
    if (sidebar) {
        sidebar.remove();
    }
    document.body.classList.remove("has-sidebar");
}

async function loadCartCount() {
    const user = JSON.parse(localStorage.getItem("user"));
    if (!user) {
        document.getElementById("cartCount").textContent = 0;
        return;
    }

    try {
        const response = await api.get("/CartItems");
        const items = response.data;
        const totalItems = items.length;
        document.getElementById("cartCount").textContent = totalItems;
    } catch (err) {
        console.error("Failed to load cart count:", err);
        document.getElementById("cartCount").textContent = 0;
    }
}

function setupSearch() {
    const searchForm = document.getElementById("searchForm");
    const searchInput = document.getElementById("searchInput");

    searchForm.addEventListener("submit", function (e) {
        e.preventDefault(); 

        const query = searchInput.value.trim();
        if (!query) return;

        if (window.location.pathname.endsWith("home.html")) {
            renderSearchResults(query);
        }
        else {
            window.location.href = `home.html?search=${encodeURIComponent(query)}`;
        }
    });
}

document.addEventListener("DOMContentLoaded", () => {
    const headerContainer = document.getElementById("header");

    if (headerContainer && !headerContainer.querySelector("nav")) {
        renderHeader();
        setupSearch();
        loginModal = new bootstrap.Modal(document.getElementById("loginModal"));
        registerModal = new bootstrap.Modal(document.getElementById("registerModal"));
    }
});