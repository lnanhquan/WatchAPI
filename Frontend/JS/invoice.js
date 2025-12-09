const invoiceAPI = {
    getAll: () => api.get("/Invoices"),
    getById: id => api.get(`/Invoices/${id}`),
    create: data => api.post("/Invoices", data)
};

let invoiceTable = document.getElementById("invoiceTable");
let viewInvoiceModal;
let invoiceDetailsContainer = document.getElementById("invoiceDetailsContainer");
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

            const btnDetail = document.createElement("button");
            btnDetail.className = "btn btn-secondary me-2";
            btnDetail.innerHTML = `<i class="bi bi-info-circle"></i> Detail`;
            btnDetail.addEventListener("click", () => openDetailInvoiceModal(i));

            tdActions.appendChild(btnDetail);

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
                    <th style="width:150px;">Price</th>
                    <th style="width:150px;">Total</th>
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