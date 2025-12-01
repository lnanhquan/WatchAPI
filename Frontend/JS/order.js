const orderAPI = {
    getAll: () => api.get("/Invoices"),
    getById: id => api.get(`/Invoices/${id}`)
};

let orderTable = document.getElementById("orderTable");
let viewOrderModal;
let orderCurrentPage = 1;
let orderRowsPerPage = 10; 
let orderDataAll = []; 
let orderData = []; 
let orderSortState = {};

function renderOrderPage(page) {
    const start = (page - 1) * orderRowsPerPage;
    const end = start + orderRowsPerPage;
    const pageData = orderData.slice(start, end);

    orderTable.innerHTML = "";

    if (pageData.length === 0) {
        orderTable.innerHTML = '<tr><td colspan="4">No orders found</td></tr>';
    } else {
        pageData.forEach(i => {
            const tr = document.createElement("tr");

            const tdOrderID = document.createElement("td");
            tdOrderID.textContent = i.id;

            const tdCreatedAt = document.createElement("td");
            const createdAtUTC = new Date(i.createdAt.replace(/\.\d+/, ''));
            const vnTime = new Date(createdAtUTC.getTime() + 7 * 60 * 60 * 1000);
            tdCreatedAt.textContent = vnTime.toLocaleString("vi-VN", { hour12: false });

            const tdTotal = document.createElement("td");
            tdTotal.textContent = i.totalAmount.toLocaleString() + " VND";

            const tdActions = document.createElement("td");

            // Nút Detail
            const btnDetail = document.createElement("button");
            btnDetail.className = "btn btn-secondary me-2";
            btnDetail.innerHTML = `<i class="bi bi-info-circle"></i> Detail`;
            btnDetail.addEventListener("click", () => openDetailOrderModal(i));

            tdActions.appendChild(btnDetail);

            tr.appendChild(tdOrderID);
            tr.appendChild(tdCreatedAt);
            tr.appendChild(tdTotal);
            tr.appendChild(tdActions);

            orderTable.appendChild(tr);
        });
    }

    createPagination({
        totalItems: orderData.length,
        pageSize: orderRowsPerPage,
        currentPage: page,
        containerId: "paginationOrder",
        onPageClick: (newPage) => {
            orderCurrentPage = newPage;
            renderOrderPage(newPage);
        }
    });
}

async function getOrderTable() {
    try {
        const response = await orderAPI.getAll();
        orderDataAll = response.data;
        const user = JSON.parse(localStorage.getItem("user"));
        orderDataAll = orderDataAll.filter(i => (i.userId && i.userId === user.id) );
        orderData = [...orderDataAll];
        renderOrderPage(orderCurrentPage);
    } catch (error) {
        orderTable.innerHTML = `<tr><td colspan="4">${error.message}</td></tr>`;
        console.error(error);
    }
}


function openDetailOrderModal(order) {
    if (!order) {
        viewOrderContainer.innerHTML = "<p>No order data.</p>";
        return;
    }

    const detailsHTML = order.details.map(d => `
        <tr>
            <td>
                <img src="${getFullImageUrl(d.imageUrl)}" style="height:50px; margin-right:5px;" alt="Watch Image">
            </td>
            <td>${d.watchName}</td>
            <td>${d.quantity}</td>
            <td>${d.price.toLocaleString()} VND</td>
            <td>${(d.quantity * d.price).toLocaleString()} VND</td>
        </tr>
    `).join("");

    viewOrderContainer.innerHTML = "";
    const createdAtUTC = new Date(order.createdAt.replace(/\.\d+/, ''));
    const vnTime = new Date(createdAtUTC.getTime() + 7 * 60 * 60 * 1000);
    viewOrderContainer.innerHTML = `
        <p>Order ID: ${order.id}</p>
        <p>User: ${order.userEmail}</p>
        <p>Created at: ${vnTime.toLocaleString("vi-VN", { hour12: false })}</p>

        <table class="table table-bordered">
            <thead class="text-center align-middle">
                <tr>
                    <th>Image</th>
                    <th>Watch</th>
                    <th>Quantity</th>
                    <th>Price</th>
                    <th>Total</th>
                </tr>
            </thead>
            <tbody class="text-center align-middle">
                ${detailsHTML}
            </tbody>
        </table>

        <h5 class="text-end fw-bold">Total amount: ${order.details.reduce((sum,d) => sum + d.quantity*d.price, 0).toLocaleString()} VND</h5>
    `;

    viewOrderModal.show();
}

function searchUserOrder()
{
    let keyword = document.getElementById("searchUserOrder").value.trim().toLowerCase();
    if (keyword == null) {
        orderData = [...orderDataAll];
        return;
    }
    else
    {
        const dateParts = keyword.split('/');
        if (dateParts.length === 3) {
            const [dd, mm, yyyy] = dateParts;
            keyword = `${yyyy}-${mm.padStart(2, '0')}-${dd.padStart(2, '0')}`;
        }

        orderData = orderDataAll.filter(i => {
            const idMatch = i.id.toLowerCase().includes(keyword);

            const dateStr = i.createdAt.split(' ')[0];
            const dateMatch = dateStr.includes(keyword);

            return idMatch || dateMatch;
        });
    }

    watchCurrentPage = 1;

    renderOrderPage(orderCurrentPage);
}


function sortOrderBy(field)
{
    orderSortState[field] = orderSortState[field] === "asc" ? "desc" : "asc";
    const direction = orderSortState[field];

    orderData.sort((a, b) => {
        let x, y;

        switch (field) {
            case "createdAt":
                x = new Date(a.createdAt);
                y = new Date(b.createdAt);
                break;

            case "total":
                x = a.totalAmount;
                y = b.totalAmount;
                break;
        }

        if (x < y) return direction === "asc" ? -1 : 1;
        if (x > y) return direction === "asc" ? 1 : -1;
        return 0;
    });

    renderOrderPage(orderCurrentPage);
}

document.addEventListener("DOMContentLoaded", () => {
    const pathName = window.location.pathname;
    if (pathName.endsWith("order.html")) {
        getOrderTable();

        viewOrderModal = new bootstrap.Modal(document.getElementById("viewOrderModal"));

        document.getElementById("searchUserOrder").addEventListener("keyup", e => {
            if (e.key === "Enter") {
                searchUserOrder();
            }
        });

        document.querySelectorAll(".sort-icon").forEach(icon => {
            icon.addEventListener("click", () => {
                const field = icon.dataset.sort;
                sortOrderBy(field);
            });
        });
    }
});