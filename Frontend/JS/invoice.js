const invoiceAPI = {
    getAll: () => api.get("/Invoices"),
    getById: id => api.get(`/Invoices/${id}`),
    create: data => api.post("/Invoices", data),
    update: (id, data) => api.put(`/Invoices/${id}`, data),
    delete: id => api.delete(`/Invoices/${id}`)
};

let invoiceTable = document.getElementById("invoiceTable");
let invoiceModal = null;
let viewInvoiceModal;
let invoiceModalLabel = document.getElementById("invoiceModalLabel");
let invoiceUser = document.getElementById("invoiceUser");
let invoiceDetailsContainer = document.getElementById("invoiceDetailsContainer");
let saveInvoiceBtn = document.getElementById("saveInvoiceBtn");
let isEditingInvoice = false;
let editingInvoiceId = null;
let invoiceCurrentPage = 1;
let invoiceRowsPerPage = 10; 
let invoiceDataAll = []; 
let invoiceData = []; 
let invoiceSortState = {};

function renderInvoicePage(page) {
    const start = (page - 1) * invoiceRowsPerPage;
    const end = start + invoiceRowsPerPage;
    const pageData = invoiceData.slice(start, end);

    invoiceTable.innerHTML = "";

    if (pageData.length === 0) {
        invoiceTable.innerHTML = '<tr><td colspan="4">No invoices found</td></tr>';
    } else {
        pageData.forEach(i => {
            const tr = document.createElement("tr");

            const tdUser = document.createElement("td");
            tdUser.textContent = i.userEmail;

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
            btnDetail.addEventListener("click", () => openDetailInvoiceModal(i));

            // Nút Edit
            const btnEdit = document.createElement("button");
            btnEdit.className = "btn btn-success me-2";
            btnEdit.innerHTML = `<i class="bi bi-pencil"></i> Edit`;
            btnEdit.addEventListener("click", () => openEditInvoiceModal(i));

            // Nút Delete
            const btnDeleteInvoice = document.createElement("button");
            btnDeleteInvoice.className = "btn btn-danger";
            btnDeleteInvoice.innerHTML = `<i class="bi bi-trash"></i> Delete`;
            btnDeleteInvoice.addEventListener("click", () => deleteInvoice(i.id, btnDeleteInvoice));

            tdActions.appendChild(btnDetail);
            tdActions.appendChild(btnEdit);
            tdActions.appendChild(btnDeleteInvoice);

            tr.appendChild(tdUser);
            tr.appendChild(tdCreatedAt);
            tr.appendChild(tdTotal);
            tr.appendChild(tdActions);

            invoiceTable.appendChild(tr);
        });
    }

    createPagination({
        totalItems: invoiceData.length,
        pageSize: invoiceRowsPerPage,
        currentPage: page,
        containerId: "paginationInvoice",
        onPageClick: (newPage) => {
            invoiceCurrentPage = newPage;
            renderInvoicePage(newPage);
        }
    });
}

async function getInvoiceTable() {
    try {
        const response = await invoiceAPI.getAll();
        invoiceDataAll = response.data;
        invoiceData = [...invoiceDataAll];

        renderInvoicePage(invoiceCurrentPage);

        
    } catch (error) {
        invoiceTable.innerHTML = `<tr><td colspan="4">${error.message}</td></tr>`;
        console.error(error);
    }
}

async function loadUsers() {
    try {
        const response = await api.get("/Users");
        const users = response.data;

        const userSelect = document.getElementById("invoiceUser");
        userSelect.innerHTML = '<option value="" disabled selected>Select a user</option>';

        users.forEach(u => {
            const option = document.createElement("option");
            option.value = u.id;
            option.textContent = u.email;
            userSelect.appendChild(option);
        });
    } catch (error) {
        console.error("Failed to load users:", error);
    }
}

async function loadWatchOptions(select) {
    try {
        const response = await watchAPI.getAll();
        const watches = response.data;

        select.innerHTML = '<option disabled selected>Select watch</option>';

        watches.forEach(w => {
            const option = document.createElement("option");
            option.value = w.id;
            option.textContent = w.name;
            option.dataset.imageUrl =  getFullImageUrl(w.imageUrl);
            select.appendChild(option);
        });
    } catch (error) {
        console.error("Failed to load watches:", error);
    }
}


async function addWatchRow(watchId = "", quantity = 1) {
    const template = document.getElementById("invoiceDetailRowTemplate").innerHTML;
    invoiceDetailsContainer.insertAdjacentHTML("beforeend", template);

    const newRow = invoiceDetailsContainer.lastElementChild;
    const select = newRow.querySelector(".watch-select");
    const qtyInput = newRow.querySelector(".watch-quantity");

    newRow.querySelector(".remove-row-btn").addEventListener("click", () => {
        newRow.remove();
    });

    await loadWatchOptions(select);
    qtyInput.value = quantity;
    if (watchId) select.value = watchId;
}

async function openCreateInvoiceModal() {
    isEditingInvoice = false;
    editingInvoiceId = null;

    saveInvoiceBtn.classList.remove("btn-primary", "btn-success");
    saveInvoiceBtn.classList.add(isEditingInvoice ? "btn-success" : "btn-primary");
    invoiceModalLabel.textContent = "Create Invoice";
    saveInvoiceBtn.textContent = "Create";

    invoiceUser.value = "";
    invoiceDetailsContainer.innerHTML = "";

    await loadUsers(); 

    invoiceModal.show();
}

async function openEditInvoiceModal(invoice) {
    isEditingInvoice = true;
    editingInvoiceId = invoice.id;

    saveInvoiceBtn.classList.remove("btn-primary", "btn-success");
    saveInvoiceBtn.classList.add(isEditingInvoice ? "btn-success" : "btn-primary");
    invoiceModalLabel.textContent = "Edit Invoice";
    saveInvoiceBtn.textContent = "Save";

    await loadUsers();
    invoiceUser.value = invoice.userId;

    invoiceDetailsContainer.innerHTML = "";

    invoice.details.forEach(d => {
        addWatchRow(d.watchId, d.quantity);
    });

    invoiceModal.show();
}

function openDetailInvoiceModal(invoice) {
    if (!invoice) {
        viewInvoiceContainer.innerHTML = "<p>No invoice data.</p>";
        return;
    }

    const detailsHTML = invoice.details.map(d => `
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

    viewInvoiceContainer.innerHTML = "";
    const createdAtUTC = new Date(invoice.createdAt.replace(/\.\d+/, ''));
    const vnTime = new Date(createdAtUTC.getTime() + 7 * 60 * 60 * 1000);
    viewInvoiceContainer.innerHTML = `
        <p>Invoice ID: ${invoice.id}</p>
        <p>User: ${invoice.userEmail}</p>
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

        <h5 class="text-end fw-bold">Total amount: ${invoice.details.reduce((sum,d) => sum + d.quantity*d.price, 0).toLocaleString()} VND</h5>
    `;

    viewInvoiceModal.show();
}

saveInvoiceBtn.addEventListener("click", async () => {
    const userId = invoiceUser.value;

    if (!userId) {
        Swal.fire("Error", "Please select a user.", "error");
        return;
    }

    const rows = document.querySelectorAll(".invoice-detail-row");

    if (rows.length === 0) {
        Swal.fire("Error", "Invoice must contain at least one watch.", "error");
        return;
    }

    const details = [];
    const watchIds = new Set();
    let hasError = false;

    rows.forEach(row => {
        const watchId = row.querySelector(".watch-select").value;
        const quantity = row.querySelector(".watch-quantity").value;

        if (!watchId) {
            Swal.fire("Error", "Each row must select a watch.", "error");
            hasError = true;
            return;
        }

        if (!quantity || quantity <= 0 || isNaN(quantity)) {
            Swal.fire("Error", "Quantity must be a positive number.", "error");
            hasError = true;
            return;
        }

        if (watchIds.has(watchId)) {
            Swal.fire("Error", "Duplicate watches are not allowed.", "error");
            hasError = true;
            return;
        }

        watchIds.add(watchId);

        details.push({
            watchId: watchId,
            quantity: Number(quantity)
        });
    });

    if (hasError) return;

    const invoiceData = {
        userId: userId,
        details: details
    };

    try {
        if (!isEditingInvoice) {
            await invoiceAPI.create(invoiceData);
            Swal.fire({
                icon: "success",
                title: "Invoice created successfully!",
                toast: true,
                position: "bottom-end",
                timer: 1500,
                showConfirmButton: false
            });
        } else {
            await invoiceAPI.update(editingInvoiceId, invoiceData);
            Swal.fire({
                icon: "success",
                title: "Invoice updated successfully!",
                toast: true,
                position: "bottom-end",
                timer: 1500,
                showConfirmButton: false
            });
        }

        invoiceModal.hide();
        getInvoiceTable();

    } catch (error) {
        console.error(error);
        Swal.fire("Error", "Invoice operation failed.", "error");
    }
});

async function deleteInvoice(id, btnDeleteInvoice) {
    const result = await Swal.fire({
        title: 'Are you sure?',
        text: "This action cannot be undone!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    });

    if (result.isConfirmed) {
        btnDeleteInvoice.disabled = true;

        try {
            await invoiceAPI.delete(id);

            await getInvoiceTable();

            Swal.fire({
                icon: "success",
                title: "Invoice has been deleted!",
                toast: true,
                position: "bottom-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });
        } 
        catch (error) {
            Swal.fire({
                icon: "error",
                title: "Could not delete invoice!",
                toast: true,
                position: "bottom-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });
            console.error(error);
        } 
        finally {
            btnDeleteInvoice.disabled = false;
        }
    }
}

function searchManagementInvoice()
{
    let keyword = document.getElementById("searchManagementInvoice").value.trim().toLowerCase();
    if (keyword == null) {
        invoiceData = [...invoiceDataAll];
        return;
    }
    else
    {
        const dateParts = keyword.split('/');
        if (dateParts.length === 3) {
            const [dd, mm, yyyy] = dateParts;
            keyword = `${yyyy}-${mm.padStart(2, '0')}-${dd.padStart(2, '0')}`;
        }

        invoiceData = invoiceDataAll.filter(i => {
            const emailMatch = i.userEmail.toLowerCase().includes(keyword);

            const dateStr = i.createdAt.split(' ')[0];
            const dateMatch = dateStr.includes(keyword);

            return emailMatch || dateMatch;
        });
    }

    watchCurrentPage = 1;

    renderInvoicePage(invoiceCurrentPage);
}


function sortInvoiceBy(field)
{
    invoiceSortState[field] = invoiceSortState[field] === "asc" ? "desc" : "asc";
    const direction = invoiceSortState[field];

    invoiceData.sort((a, b) => {
        let x, y;

        switch (field) {
            case "user":
                x = a.userEmail.toLowerCase();
                y = b.userEmail.toLowerCase();
                break;

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

    renderInvoicePage(invoiceCurrentPage);
}

document.addEventListener("DOMContentLoaded", () => {
    const pathName = window.location.pathname;
    if (pathName.endsWith("invoice.html")) {
        getInvoiceTable();
        invoiceModal = new bootstrap.Modal(document.getElementById("invoiceModal"));
        viewInvoiceModal = new bootstrap.Modal(document.getElementById("viewInvoiceModal"));
        document.getElementById("searchManagementInvoice").addEventListener("keyup", e => {
            if (e.key === "Enter") {
                searchManagementInvoice();
            }
        });
        document.querySelectorAll(".sort-icon").forEach(icon => {
            icon.addEventListener("click", () => {
                const field = icon.dataset.sort;
                sortInvoiceBy(field);
            });
        });
    }
});