const watchAPI = {
    getAll: () => api.get("/Watches"),
    getById: id => api.get(`/Watches/${id}`),
    checkName: name => api.get(`/Watches/check-name`, { params: { name } }),
    create: formData => api.post("/Watches", formData),
    update: (id, formData) => api.put(`/Watches/${id}`, formData),
    delete: id => api.delete(`/Watches/${id}`)
};
let watchModal;
let watchTable = document.getElementById("watchTable");
let watchModalLabel = document.getElementById("watchModalLabel");
let watchId = document.getElementById("watchId");
let watchName = document.getElementById("watchName");
let watchPrice = document.getElementById("watchPrice");
let ImageFile = document.getElementById("ImageFile");
let saveBtn = document.getElementById("saveBtn");
let isEditing = false;
let currentPage = 1;
let rowsPerPage = 3; 
let watchesData = []; 

async function getTable() {
    try {
        const response = await watchAPI.getAll();
        watchesData = response.data;

        renderTablePage(currentPage);
        renderPagination();
    } 
    catch (error) 
    {
        watchTable.innerHTML = `<tr><td colspan="4">${error.message}</td></tr>`;
        console.error(error);
    }
}

function renderTablePage(page) {
    const start = (page - 1) * rowsPerPage;
    const end = start + rowsPerPage;
    const pageData = watchesData.slice(start, end);

    watchTable.innerHTML = "";

    if (pageData.length === 0) {
        watchTable.innerHTML = '<tr><td colspan="4">No watches found</td></tr>';
    } else {
        pageData.forEach(w => {
            const tr = document.createElement("tr");

            const tdImage = document.createElement("td");
            const img = document.createElement("img"); 
            img.src = getFullImageUrl(w.imageUrl);
            img.alt = w.name;
            img.style.width = "200px";
            img.style.height = "200px";
            tdImage.appendChild(img);

            const tdName = document.createElement("td");
            tdName.textContent = w.name;

            const tdPrice = document.createElement("td");
            tdPrice.textContent = w.price.toLocaleString() + " VND";

            const tdActions = document.createElement("td");

            const btnEdit = document.createElement("button");
            btnEdit.className = "btn btn-success me-2";
            btnEdit.innerHTML = `<i class="bi bi-pencil"></i> Edit`;
            btnEdit.addEventListener("click", () => openEditModal(w.id, w.name, w.price));

            const btnDelete = document.createElement("button");
            btnDelete.className = "btn btn-danger";
            btnDelete.innerHTML = `<i class="bi bi-trash"></i> Delete`;
            btnDelete.addEventListener("click", () => deleteWatch(w.id, btnDelete));

            tdActions.appendChild(btnEdit);
            tdActions.appendChild(btnDelete);

            tr.appendChild(tdImage);
            tr.appendChild(tdName);
            tr.appendChild(tdPrice);
            tr.appendChild(tdActions);

            watchTable.appendChild(tr);
        });
    }
}

function renderPagination() {
    const pagination = document.getElementById("pagination");
    pagination.innerHTML = "";

    const pageCount = Math.ceil(watchesData.length / rowsPerPage);

    for (let i = 1; i <= pageCount; i++) {
        const btn = document.createElement("button");
        btn.textContent = i;
        btn.className = "btn btn-outline-primary me-1";
        if (i === currentPage) btn.classList.add("active");

        btn.addEventListener("click", () => {
            currentPage = i;
            renderTablePage(currentPage);
            renderPagination();
        });

        pagination.appendChild(btn);
    }
}

function openCreateModal() {
    isEditing = false;
    saveBtn.classList.remove("btn-primary", "btn-success");
    saveBtn.classList.add(isEditing ? "btn-success" : "btn-primary");
    document.getElementById("watchModalLabel").textContent = "Create new watch";
    saveBtn.textContent = "Create";
    watchName.value = "";
    watchPrice.value = "";
    ImageFile.value = "";
    watchId.value = "";
    watchModal.show();
}

function openEditModal(id, name, price) {
    isEditing = true;
    saveBtn.classList.remove("btn-primary", "btn-success");
    saveBtn.classList.add(isEditing ? "btn-success" : "btn-primary");
    document.getElementById("watchModalLabel").textContent = "Edit watch";
    saveBtn.textContent = "Edit";
    watchName.value = name;
    watchPrice.value = price;
    watchId.value = id;
    watchModal.show();
}

saveBtn.addEventListener("click", async () => {
    const name = watchName.value.trim();
    const price = watchPrice.value.trim();
    const id = watchId.value;

    if (!name || !price) 
    {
        Swal.fire('Error', 'Watch name and price are required.', 'error');
        return;
    }
    else if (name.length > 100)
    {
        Swal.fire({
            icon: 'warning',
            title: 'Watch name Requirement',
            html: 'Watch name cannot exceed 100 characters.'
        });
        return;
    }

    let exists = false;
    try {
        const res = await watchAPI.checkName(name);
        if (res.data && res.data.id.toString() !== id.toString()) exists = true;
    } catch(err) {
        if (err.response && err.response.status === 404) exists = false;
        else throw err;
    }

    if (exists) {
        Swal.fire('Error', 'A watch with this name already exists.', 'error');
        return;
    }

    const pricePattern = /^\d+$/;

    if (!pricePattern.test(price)) {
        Swal.fire({
            icon: 'error',
            title: 'Price Requirement',
            html: 'Price must be a valid integer without commas or letters.'
        });
        return;
    }

    const priceNum = Number(price);

    if (isNaN(priceNum)) {
        Swal.fire({
            icon: 'error',
            title: 'Price Requirement',
            html: 'Price must be a valid number without commas or invalid characters.'
        });
        return;
    }

    if (priceNum < 0 || priceNum > 1000000000) {
        Swal.fire({
            icon: 'warning',
            title: 'Price Requirement',
            html: 'Price must be between 0 and 1,000,000,000.'
        });
        return;
    }

    saveBtn.disabled = true;

    const formData = new FormData();
    formData.append("Name", name);
    formData.append("Price", priceNum);
    if (ImageFile.files[0]) {
        formData.append("ImageFile", ImageFile.files[0]);
    }

    try {
        if (!isEditing) {
            await watchAPI.create(formData);

            Swal.fire({
                icon: "success",
                title: "Watch created successfully!",
                toast: true,
                position: "bottom-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });
        } else {
            await watchAPI.update(id, formData);

            Swal.fire({
                icon: "success",
                title: "Watch updated successfully!",
                toast: true,
                position: "bottom-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });
        }
        watchModal.hide();
        getTable();
    } catch (error) {
            Swal.fire({
                icon: "error",
                title: "Operation failed!",
                toast: true,
                position: "bottom-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });
        console.error(error);
    } finally {
        saveBtn.disabled = false;
    }
});

async function deleteWatch(id, btnDelete) {
    const result = await Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, delete it!'
    });
    if (result.isConfirmed)
    {
        btnDelete.disabled = true;

        try {
            await watchAPI.delete(id);

            const row = btnDelete.closest('tr');
            if (row) row.remove();

            Swal.fire({
                icon: "success",
                title: "Your watchh has been deleted!",
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
                title: "Could not delete item!",
                toast: true,
                position: "bottom-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });            
            console.error(error);
        } 
        finally 
        {
            btnDelete.disabled = false;
        }
    }
}

document.addEventListener("DOMContentLoaded", () => {
    const currentPage = window.location.pathname;
    if (currentPage.endsWith("watch.html")) {
        getTable();
    }
    watchModal = new bootstrap.Modal(document.getElementById("watchModal"));
});