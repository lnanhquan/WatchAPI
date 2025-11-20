async function loadWatches() {
    try {
        const response = await watchAPI.getAll();
        const watches = response.data;

        const watchList = document.getElementById("watchList");

        watchList.innerHTML = "";

        watches.forEach(w => {
            const imageUrl = getFullImageUrl(w.imageUrl);
            watchList.innerHTML += `
                <div class="col-md-3 mb-4">
                    <div class="card shadow-lg">
                        <img src="${imageUrl}" class="card-img-top" alt="${w.name}" style="object-fit: cover;">
                        <div class="card-body">
                            <h5 class="card-title text-center">${w.name}</h5>
                            <p class="card-text text-danger fs-6 fw-bold text-center">${w.price.toLocaleString()} VND</p>
                        </div>
                    </div>
                </div>
            `;
        });

    } catch (error) {
        console.error(error);
    }
}

let loginModal;
let registerModal;

function updateUIAfterLogin(isLoggedIn) {
    const btnLogin = document.getElementById("btnLogin");
    const btnLogout = document.getElementById("btnLogout");
    const managementDropdown = document.getElementById("managementDropdown");
    const user = JSON.parse(localStorage.getItem("user"));

    if (isLoggedIn) {
        btnLogin.classList.add("d-none");
        btnLogout.classList.remove("d-none");
        
    } else {
        btnLogin.classList.remove("d-none");
        btnLogout.classList.add("d-none");
    }

    if (user && user.roles && user.roles.includes("Admin"))
    {
        managementDropdown.classList.remove("d-none");
    }
    else
    {
        managementDropdown.classList.add("d-none");
    }
}

document.addEventListener("DOMContentLoaded", () => {
    const currentPage = window.location.pathname;
    if (currentPage.endsWith("home.html")) {
        loadWatches();
        loginModal = new bootstrap.Modal(document.getElementById("loginModal"));
        registerModal = new bootstrap.Modal(document.getElementById("registerModal"));
    }
    const user = JSON.parse(localStorage.getItem("user"));
    updateUIAfterLogin(!!user);
});

