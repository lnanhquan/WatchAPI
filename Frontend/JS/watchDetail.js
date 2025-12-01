const watchDetailContainer = document.getElementById("watchDetailContainer");
const urlParams = new URLSearchParams(window.location.search);
const watchId = urlParams.get("id");

async function loadWatchDetail() {
    if (!watchId) {
        watchDetailContainer.innerHTML = "<p>Invalid watch ID</p>";
        return;
    }

    try {
        const response = await api.get(`/Watches/${watchId}`);
        const watch = response.data;

        watchDetailContainer.innerHTML = `
            <div class="card shadow-lg watch-card">

                <div class="card-header text-center align-items-center">
                    <h2 class="text-center fw-semibold align-items-center m-0">Watch details</h2>
                </div>


                <div class="card-body row">

                    <div class="col-md-4">
                        <img src="${getFullImageUrl(watch.imageUrl)}"
                            class="img-fluid rounded"
                            alt="${watch.name}">
                    </div>

                    <div class="col-md-8">
                        <h2 class="fw-bold mb-2">${watch.name}</h2>

                        <div class="d-flex align-items-center gap-3 mb-3">
                            <span class="badge bg-dark px-3 py-2">${watch.brand}</span>
                            <span class="badge bg-info px-3 py-2">${watch.category}</span>
                            <span class="badge bg-success px-3 py-2">In Stock</span>
                        </div>

                        <div class="mb-4">
                            <h3 class="text-danger fw-bold mb-1">
                                ${watch.price.toLocaleString()} VND
                            </h3>

                            ${watch.oldPrice ? `
                            <div class="d-flex align-items-center gap-2">
                                <span class="text-muted text-decoration-line-through">
                                    ${watch.oldPrice.toLocaleString()} VND
                                </span>
                                <span class="badge bg-danger">-${Math.round((1 - watch.price/watch.oldPrice) * 100)}%</span>
                            </div>` : ""}
                        </div>

                        <hr>

                        <div class="mb-4">
                            <h5 class="fw-semibold">Description</h5>
                            <p class="text-muted" style="text-align: justify;">
                                ${watch.description ?? "No description available."}
                            </p>
                        </div>

                        <div>
                            <label class="fw-semibold mb-2 d-block">Quantity</label>

                            <div class="input-group quantity-group" style="width:100px;">
                                <button class="btn btn-outline-secondary btn-decrease" id="btnDecrease">-</button>

                                <input
                                    type="number"
                                    id="quantityInput"
                                    value="1"
                                    min="1"
                                    class="form-control text-center"
                                    style="border: 1px solid #6c757d; width: 70px;"
                                >

                                <button class="btn btn-outline-secondary btn-increase" id="btnIncrease">+</button>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="card-footer d-flex gap-2 text-center align-items-center">
                    <button id="btnAddToCart" class="btn btn-success w-100 py-1 fs-6 btn-sm">
                        <i class="bi bi-cart3"></i> Add to Cart
                    </button>

                    <button id="btnBack" class="btn btn-secondary w-100 py-1 fs-6 btn-sm"> 
                        Back to home
                    </button>
                </div>

            </div>
        `;

        const quantityInput = document.getElementById("quantityInput");
        const btnDecrease = document.getElementById("btnDecrease");
        const btnIncrease = document.getElementById("btnIncrease");
        const btnAddToCart = document.getElementById("btnAddToCart");
        const btnBack = document.getElementById("btnBack");

        btnDecrease.addEventListener("click", () => {
            if (parseInt(quantityInput.value) > 1) quantityInput.value = parseInt(quantityInput.value) - 1;
        });
        btnIncrease.addEventListener("click", () => {
            quantityInput.value = parseInt(quantityInput.value) + 1;
        });

        btnAddToCart.addEventListener("click", async () => {
            const user = JSON.parse(localStorage.getItem("user"));
            if (!user) {
                Swal.fire({
                    icon: 'info',
                    title: 'Please login',
                    text: 'You must be logged in to add this watch to your cart.'
                });
                return;
            }

            try 
            {
                const data = {
                    watchId: watchId,
                    quantity: quantityInput.value
                }

                await api.post("/CartItems", data);
                loadCartCount();
                Swal.fire("Added to cart", "", "success");
            } 
            catch (err) 
            {
                console.error(err);
                Swal.fire("Error adding to cart", "", "error");
            }
        });

        btnBack.addEventListener("click", () => {
            window.location.href = "home.html";
        });
    } 
    catch (err) 
    {
        console.error(err);
        watchDetailContainer.innerHTML = "<p>Failed to load watch details</p>";
    }
}

document.addEventListener("DOMContentLoaded", () => {
    loadWatchDetail();
});
