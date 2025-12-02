const watchAPI = {
    getAll: () => api.get("/Watches"),
    getById: id => api.get(`/Watches/${id}`),
    checkName: name => api.get(`/Watches/check-name`, { params: { name } }),
    create: formData => api.post("/Watches", formData),
    update: (id, formData) => api.put(`/Watches/${id}`, formData),
    delete: id => api.delete(`/Watches/${id}`)
};

let watchTable = document.getElementById("watchTable");
let watchModal = null;
let watchModalLabel = document.getElementById("watchModalLabel");
let watchId = document.getElementById("watchId");
let watchName = document.getElementById("watchName");
let watchPrice = document.getElementById("watchPrice");
let watchCategory = document.getElementById("watchCategory");
let watchBrand = document.getElementById("watchBrand");
let watchDescription = document.getElementById("watchDescription");
let ImageFile = document.getElementById("ImageFile");
let saveBtn = document.getElementById("saveBtn");
let isEditing = false;
let watchCurrentPage = 1;
let watchRowsPerPage = 3; 
let watchesDataAll = [];
let watchesData = []; 
let watchSortState = {};

async function getTable() {
    try {
        const response = await watchAPI.getAll();
        watchesDataAll = response.data;
        watchesData = [...watchesDataAll];

        renderWatchPage(watchCurrentPage);
    } 
    catch (error) 
    {
        watchTable.innerHTML = `<tr><td colspan="4">${error.message}</td></tr>`;
        console.error(error);
    }
}

function renderWatchPage(page) {
    const start = (page - 1) * watchRowsPerPage;
    const end = start + watchRowsPerPage;
    const pageData = watchesData.slice(start, end);

    watchTable.innerHTML = "";

    if (pageData.length === 0) {
        watchTable.innerHTML = '<tr><td colspan="6">No watches found</td></tr>';
    } else {
        pageData.forEach(w => {
            const tr = document.createElement("tr");

            const tdImage = document.createElement("td");
            const img = document.createElement("img"); 
            img.src = getFullImageUrl(w.imageUrl);
            img.alt = w.name;
            img.style.width = "180px";
            img.style.height = "180px";
            tdImage.appendChild(img);

            const tdName = document.createElement("td");
            tdName.textContent = w.name;

            const tdPrice = document.createElement("td");
            tdPrice.textContent = w.price.toLocaleString() + " VND";

            const tdCategory = document.createElement("td");
            tdCategory.textContent = w.category;

            const tdBrand = document.createElement("td");
            tdBrand.textContent = w.brand;

            const tdActions = document.createElement("td");

            const btnEdit = document.createElement("button");
            btnEdit.className = "btn btn-success me-2";
            btnEdit.innerHTML = `<i class="bi bi-pencil"></i> Edit`;
            btnEdit.addEventListener("click", () => openEditModal(w));

            const btnDelete = document.createElement("button");
            btnDelete.className = "btn btn-danger";
            btnDelete.innerHTML = `<i class="bi bi-trash"></i> Delete`;
            btnDelete.addEventListener("click", () => deleteWatch(w.id, btnDelete));

            tdActions.appendChild(btnEdit);
            tdActions.appendChild(btnDelete);

            tr.appendChild(tdImage);
            tr.appendChild(tdName);
            tr.appendChild(tdPrice);
            tr.appendChild(tdCategory);
            tr.appendChild(tdBrand);
            tr.appendChild(tdActions);

            watchTable.appendChild(tr);
        });
    }

    createPagination({
        totalItems: watchesData.length,
        pageSize: watchRowsPerPage,
        currentPage: page,
        containerId: "paginationWatch",
        onPageClick: (newPage) => {
            watchCurrentPage = newPage;
            renderWatchPage(newPage);
        }
    });
}

function openCreateModal() {
    isEditing = false;
    saveBtn.classList.remove("btn-primary", "btn-success");
    saveBtn.classList.add("btn-primary");
    watchModalLabel.textContent = "Create new watch";
    saveBtn.textContent = "Create";

    watchId.value = "";
    watchName.value = "";
    watchPrice.value = "";
    watchCategory.value = "";
    watchBrand.value = "";
    watchDescription.value = "";
    ImageFile.value = "";

    watchModal.show();
}

function openEditModal(watch) {
    isEditing = true;
    saveBtn.classList.remove("btn-primary", "btn-success");
    saveBtn.classList.add("btn-success");
    watchModalLabel.textContent = "Edit watch";
    saveBtn.textContent = "Edit";

    watchId.value = watch.id;
    watchName.value = watch.name;
    watchPrice.value = watch.price;
    watchCategory.value = watch.category;
    watchBrand.value = watch.brand;
    watchDescription.value = watch.description || "";
    ImageFile.value = "";

    watchModal.show();
}

async function handleSaveWatch() {
    const name = watchName.value.trim();
    const price = watchPrice.value.trim();
    const category = watchCategory.value.trim();
    const brand = watchBrand.value.trim();
    const description = watchDescription.value.trim();
    const id = watchId.value;

    if (!name || !price || !category || !brand) {
        Swal.fire('Error', 'Please fill in all fields.', 'error');
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

    if (priceNum < 0 || priceNum > 1000000000) {
        Swal.fire({
            icon: 'warning',
            title: 'Price Requirement',
            html: 'Price must be between 0 and 1,000,000,000.'
        });
        return;
    }

    if (!isEditing) {
        if (!ImageFile.files || ImageFile.files.length === 0) {
            Swal.fire('Error', 'Image is required when creating.', 'error');
            return;
        }
    }

    const formData = new FormData();
    formData.append("Name", name);
    formData.append("Price", priceNum);
    formData.append("Category", category);
    formData.append("Brand", brand);
    formData.append("Description", description);

    if (ImageFile.files && ImageFile.files.length > 0) {
        formData.append("ImageFile", ImageFile.files[0]);
    }

    for (let [key, value] of formData.entries()) {
        console.log(key, value);
    }
    saveBtn.disabled = true;

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
        await getTable();
        watchModal.hide();
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
}

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

            await getTable();

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

function searchManagementWatch()
{
    const keyword = document.getElementById("searchManagementWatch").value.trim().toLowerCase();
    if (keyword == null) {
        watchesData = [...watchesDataAll];
        return;
    }
    else
    {
        watchesData = watchesDataAll.filter(w => 
            w.name.toLowerCase().includes(keyword)
        );
    }

    watchCurrentPage = 1;

    renderWatchPage(watchCurrentPage);
}

function sortWatchBy(field)
{
    watchSortState[field] = watchSortState[field] === "asc" ? "desc" : "asc";
    const direction = watchSortState[field];

    watchesData.sort((a, b) => {
        let x, y;

        switch (field) {
            case "watchName":
                x = a.name.toLowerCase();
                y = b.name.toLowerCase();
                break;

            case "price":
                x = a.price;
                y = b.price;
                break;

            case "brand":
                x = a.brand.toLowerCase();
                y = b.brand.toLowerCase();
                break;

            case "category":
                x = a.category.toLowerCase();
                y = b.category.toLowerCase();
                break;
        }

        if (x < y) return direction === "asc" ? -1 : 1;
        if (x > y) return direction === "asc" ? 1 : -1;
        return 0;
    });

    renderWatchPage(watchCurrentPage);
}

document.addEventListener("DOMContentLoaded", () => {
    const pathName = window.location.pathname;
    if (pathName.endsWith("watch.html")) {
        getTable();

        watchModal = new bootstrap.Modal(document.getElementById("watchModal"));

        document.getElementById("watchForm").addEventListener("submit", function(e) {
            e.preventDefault();
        });

        if (!saveBtn.dataset.listenerAttached) {
            saveBtn.addEventListener("click", handleSaveWatch);
            saveBtn.dataset.listenerAttached = "true";
        }

        document.getElementById("searchManagementWatch").addEventListener("keyup", e => {
            if (e.key === "Enter") {
                searchManagementWatch();
            }
        });
        document.querySelectorAll(".sort-icon").forEach(icon => {
            icon.addEventListener("click", () => {
                const field = icon.dataset.sort;
                sortWatchBy(field);
            });
        });
    }
});