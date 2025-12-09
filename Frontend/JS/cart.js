const cartAPI = {
    getCart: () => api.get("/CartItems"),
    updateItem: data => api.put("/CartItems", data),
    deleteItem: watchId => api.delete(`/CartItems/${watchId}`), 
    clearCart: () => api.delete("/CartItems/clear") 
};

const cartTable = document.getElementById("cartTable");
const cartTotalAmount = document.getElementById("cartTotalAmount");
const paginationCart = document.getElementById("paginationCart");
let cartDataAll = []; 
const selectAllCheckbox = document.getElementById("selectAll");
const btnClearCart = document.getElementById("btnClearCart");
const btnCheckout = document.getElementById("btnCheckout");

async function getCart() {
    try {
        const response = await cartAPI.getCart();
        cartDataAll = response.data.map(c => ({
            ...c,
            selected: true
        }));
        renderCart();
    } catch (error) {
        cartTable.innerHTML = `<tr><td colspan="7">${error.message}</td></tr>`;
        console.error(error);
    }
}

function renderCart() {
    cartTable.innerHTML = "";

    if (cartDataAll.length === 0) {
        cartTable.innerHTML = '<tr><td colspan="7">Your cart is empty</td></tr>';
        return;
    }

    cartDataAll.forEach(item => {
        const tr = document.createElement("tr");
        
        const tdSelect = document.createElement("td");
        const checkbox = document.createElement("input");
        checkbox.type = "checkbox";
        checkbox.checked = item.selected;
        checkbox.addEventListener("change", () => {
            item.selected = checkbox.checked;
            updateCartTotal();
            const allSelected = cartDataAll.every(i => i.selected);
            selectAllCheckbox.checked = allSelected;
        });
        tdSelect.appendChild(checkbox);

        const tdImage = document.createElement("td");
        const img = document.createElement("img");
        img.src = getFullImageUrl(item.imageUrl);
        img.alt = item.watchName;
        img.style.width = "80px";
        tdImage.appendChild(img);

        const tdName = document.createElement("td");
        tdName.textContent = item.watchName;

        const tdPrice = document.createElement("td");
        tdPrice.textContent = item.price.toLocaleString() + " VND";

        const tdQuantity = document.createElement("td");
        const group = document.createElement("div");
        group.className = "input-group quantity-group";

        const btnDecrease = document.createElement("button");
        btnDecrease.className = "btn btn-outline-secondary btn-decrease";
        btnDecrease.textContent = "-";
        const input = document.createElement("input");
        input.type = "number";
        input.min = "1";
        input.value = item.quantity || 1;
        input.className = "text-center";
        input.style.width = "50px";
        input.style.border = "1px solid #6c757d";
        const btnIncrease = document.createElement("button");
        btnIncrease.className = "btn btn-outline-secondary btn-increase";
        btnIncrease.textContent = "+";

        function updateRowTotal() {
            tdTotal.textContent = (item.price * item.quantity).toLocaleString() + " VND";
            updateCartTotal();
        }

        async function updateQuantityInDB(item) {
        try {
            await cartAPI.updateItem({
                watchId: item.watchId,
                quantity: item.quantity
            });
        } catch (err) {
            console.error("Update quantity failed:", err);
        }
    }

        btnDecrease.addEventListener("click", () => {
            if (item.quantity > 1) item.quantity--;
            input.value = item.quantity;
            updateRowTotal();
            updateQuantityInDB(item);
        });
        input.addEventListener("input", () => {
            const val = parseInt(input.value);
            if (!isNaN(val) && val >= 1) item.quantity = val;
            else input.value = item.quantity;
            updateRowTotal();
            updateQuantityInDB(item);
        });
        btnIncrease.addEventListener("click", () => {
            item.quantity++;
            input.value = item.quantity;
            updateRowTotal();
            updateQuantityInDB(item);
        });

        group.appendChild(btnDecrease);
        group.appendChild(input);
        group.appendChild(btnIncrease);
        tdQuantity.appendChild(group);

        const tdTotal = document.createElement("td");
        tdTotal.textContent = (item.price * item.quantity).toLocaleString() + " VND";

        const tdRemove = document.createElement("td");
        const btnRemove = document.createElement("button");
        btnRemove.className = "btn btn-danger btn-sm";
        btnRemove.innerHTML = `<i class="bi bi-trash"></i>`;
        btnRemove.addEventListener("click", async () => {
            await cartAPI.deleteItem(item.watchId);
            cartDataAll = cartDataAll.filter(w => w.id !== item.id);
            renderCart();
            updateCartTotal();
            loadCartCount();
        });
        tdRemove.appendChild(btnRemove);

        tr.appendChild(tdSelect);
        tr.appendChild(tdImage);
        tr.appendChild(tdName);
        tr.appendChild(tdPrice);
        tr.appendChild(tdQuantity);
        tr.appendChild(tdTotal);
        tr.appendChild(tdRemove);

        cartTable.appendChild(tr);
    });

    updateCartTotal();
}

function updateCartTotal() {
    const totalAmount = cartDataAll
        .filter(item => item.selected)
        .reduce((sum, item) => sum + item.price * item.quantity, 0);
    cartTotalAmount.textContent = totalAmount.toLocaleString();
}

async function clearCart() {
    const result = await Swal.fire({
        title: 'Are you sure?',
        text: "This will clear all items in your cart!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, clear it!'
    });

    if (result.isConfirmed) {
        try {
            await cartAPI.clearCart(); 
            cartDataAll = [];           
            renderCart();              
            updateCartTotal();          
            loadCartCount();
            Swal.fire('Cleared!', 'Your cart has been cleared.', 'success');
        } catch (err) {
            console.error(err);
            Swal.fire('Error', 'Failed to clear cart.', 'error');
        }
    }
}

async function checkout() {
    const selectedItems = cartDataAll.filter(item => item.selected);

    if (selectedItems.length === 0) {
        Swal.fire('No items selected', 'Please select items to checkout.', 'info');
        return;
    }

    try {
        const result = await Swal.fire({
            title: 'Confirm Checkout',
            text: `You are about to checkout ${selectedItems.length} items.`,
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes, checkout!',
            cancelButtonText: 'Cancel'
        });

        if (!result.isConfirmed) return;

        const user = JSON.parse(localStorage.getItem("user"));
        const userId = user?.id;

        const invoiceData = {
            userId: userId,
            details: selectedItems.map(item => ({
                watchId: item.watchId,
                quantity: item.quantity
            }))
        };

        await api.post("/Invoices", invoiceData);

        await Promise.all(selectedItems.map(item => cartAPI.deleteItem(item.watchId)));

        cartDataAll = cartDataAll.filter(item => !item.selected);
        renderCart();
        updateCartTotal();
        loadCartCount();
        Swal.fire('Success', 'Order created and cart updated successfully!', 'success');
    } catch (err) {
        console.error(err);
        Swal.fire('Error', 'Checkout failed.', 'error');
    }
}


document.addEventListener("DOMContentLoaded", () => {
    const pathName = window.location.pathname;
    if (pathName.endsWith("cart.html")) {
        getCart();

        selectAllCheckbox.addEventListener("change", () => {
            const isChecked = selectAllCheckbox.checked;
            cartDataAll.forEach(item => item.selected = isChecked);
            renderCart();
        });

        btnClearCart.addEventListener("click", clearCart);

        btnCheckout.addEventListener("click", checkout);
    }
});