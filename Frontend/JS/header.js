function renderHeader() 
{
    const headerHTML = `
    <nav class="navbar navbar-expand-sm navbar-light bg-white border-bottom box-shadow mb-3 fixed-top">
        <div class="container-fluid">

            <a class="navbar-brand">Watches</a>

            <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                <ul class="navbar-nav flex-grow-1">
                    <li class="nav-item">
                        <a class="nav-link text-dark" href="home.html">Home</a>
                    </li>
                    <li class="nav-item dropdown">
                        <a id="managementDropdown" class="nav-link dropdown-toggle text-dark d-none" href="#"
                           role="button" data-bs-toggle="dropdown" aria-expanded="false">Management</a>
                        <ul class="dropdown-menu" aria-labelledby="managementDropdown">
                            <li><a class="dropdown-item" href="./watch.html">Watches</a></li>
                            <li><a class="dropdown-item" href="./invoice.html">Invoices</a></li>
                        </ul>
                    </li>
                </ul>
            </div>

            <button class="navbar-toggler" type="button" data-bs-toggle="collapse"
                    data-bs-target=".navbar-collapse" aria-expanded="false" aria-label="Toggle navigation">
                <span class="navbar-toggler-icon"></span>
            </button>

            <div class="d-flex ms-auto">
                <button id="btnLogin" class="btn btn-outline-primary me-2">Login</button>
                <button id="btnLogout" class="btn btn-outline-danger d-none">Logout</button>
            </div>

        </div>
    </nav>
    `;

    document.getElementById("header").innerHTML = headerHTML;

    document.getElementById("btnLogin").addEventListener("click", openLoginModal);
    document.getElementById("btnLogout").addEventListener("click", logout);

    const user = JSON.parse(localStorage.getItem("user"));
    updateUIAfterLogin(!!user);

    document.body.style.paddingTop = "70px";
}

document.addEventListener("DOMContentLoaded", renderHeader);