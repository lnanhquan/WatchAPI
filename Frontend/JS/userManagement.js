const userManagementAPI = {
    getAll: () => api.get("/Users"),
    update: (id, data) => api.put(`/Users/${id}`, data),
    delete: id => api.delete(`/Users/${id}`)
};

let userManagementTable = document.getElementById("userManagementTable");
let userManagementModal = null;
let userEmail = document.getElementById("userEmail");
let userName = document.getElementById("userName");
let userRole = document.getElementById("userRole");
let existingEmail;
let existingUsername;
let editingUserId;
let viewUserManagementModal;
let viewUserManagementContainer = document.getElementById("viewUserManagementContainer");
let updateBtn = document.getElementById("updateBtn");
let userManagementCurrentPage = 1;
let userManagementRowsPerPage = 10; 
let userManagementDataAll = []; 
let userManagementData = []; 

function renderUserManagementPage(page) {
    const start = (page - 1) * userManagementRowsPerPage;
    const end = start + userManagementRowsPerPage;
    const pageData = userManagementData.slice(start, end);

    userManagementTable.innerHTML = "";

    if (pageData.length === 0) {
        userManagementTable.innerHTML = '<tr><td colspan="5">No users found</td></tr>';
    } else {
        pageData.forEach(i => {
            const tr = document.createElement("tr");

            const tdId = document.createElement("td");
            tdId.textContent = i.id;

            const tdEmail = document.createElement("td");
            tdEmail.textContent = i.email;

            const tdUsername = document.createElement("td");
            tdUsername.textContent = i.userName;

            const tdRole = document.createElement("td");
            tdRole.textContent = i.role;

            const tdActions = document.createElement("td");

            const btnDetail = document.createElement("button");
            btnDetail.className = "btn btn-secondary me-2";
            btnDetail.innerHTML = `<i class="bi bi-info-circle"></i> Detail`;
            btnDetail.addEventListener("click", () => openDetailModal(i));

            const btnEdit = document.createElement("button");
            btnEdit.className = "btn btn-success me-2";
            btnEdit.innerHTML = `<i class="bi bi-pencil"></i> Edit`;
            btnEdit.addEventListener("click", () => openEditModal(i));

            const btnDelete = document.createElement("button");
            btnDelete.className = "btn btn-danger";
            btnDelete.innerHTML = `<i class="bi bi-trash"></i> Delete`;
            btnDelete.addEventListener("click", () => deleteUser(i.id, btnDelete));

            tdActions.appendChild(btnDetail);
            tdActions.appendChild(btnEdit);
            tdActions.appendChild(btnDelete);

            tr.appendChild(tdId);
            tr.appendChild(tdEmail);
            tr.appendChild(tdUsername);
            tr.appendChild(tdRole);
            tr.appendChild(tdActions);

            userManagementTable.appendChild(tr);
        });
    }

    createPagination({
        totalItems: userManagementData.length,
        pageSize: userManagementRowsPerPage,
        currentPage: page,
        containerId: "paginationUserManagement",
        onPageClick: (newPage) => {
            userManagementCurrentPage = newPage;
            renderUserManagementPage(newPage);
        }
    });
}

async function getUserManagementTable() {
    try {
        const response = await userManagementAPI.getAll();
        userManagementDataAll = response.data;
        userManagementData = [...userManagementDataAll];

        renderUserManagementPage(userManagementCurrentPage);
    } catch (error) {
        userManagementTable.innerHTML = `<tr><td colspan="5">${error.message}</td></tr>`;
        console.error(error);
    }
}

async function openEditModal(user) {
    userEmail.value = user.email;
    userName.value = user.userName;
    userRole.value = user.role;
    existingEmail = user.email;
    editingUserId = user.id;
    existingUsername = user.userName;
    userManagementModal.show();
}

async function editUser() 
{
    const email = userEmail.value.trim();
    const username = userName.value.trim();
    const role = userRole.value.trim();

    if (!email || !username || !role) {
        Swal.fire('Error', 'Please fill in all fields.', 'error');
        return;
    }

    const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailPattern.test(email)) 
    {
        Swal.fire({
            icon: 'warning',
            title: 'Email Requirement',
            html: 'Please enter a valid email.'
        });
        return;
    }

    if (email !== existingEmail) {
        if (await isEmailTaken(email)) {
            Swal.fire("Error", "Email is already registered.", "error");
            return;
        }
    }

    if (username.length < 3 || username.length > 20)
    {
        Swal.fire({
            icon: 'warning',
            title: 'Username Requirement',
            html: 'Username must be 3-20 characters.'
        });
        return;
    }

    const usernamePattern = /^[a-zA-Z0-9_]+$/;
    if (!usernamePattern.test(username)) 
    {
        Swal.fire({
            icon: 'warning',
            title: 'Username Requirement',
            html: 'Username can only contain letters, numbers, and underscores.'
        });
        return;
    }

    if (username !== existingUsername) {
        if (await isUsernameTaken(username)) {
            Swal.fire("Error", "Username is already registered.", "error");
            return;
        }
    }

    data = {
        email: email,
        userName: username,
        role: role
    }

    try 
    {
        await userManagementAPI.update(editingUserId, data);
        userManagementModal.hide();
        getUserManagementTable();
        Swal.fire({
            icon: "success",
            title: "User updated successfully!",
            toast: true,
            position: "bottom-end",
            timer: 1500,
            showConfirmButton: false
        });
    }
    catch (error) 
    {
        console.error(error);
        Swal.fire("Error", "User operation failed.", "error");
    }
}

function openDetailModal(user) {
    if (!user) {
        viewUserManagementContainer.innerHTML = "<p>No user data.</p>";
        return;
    }

    viewUserManagementContainer.innerHTML = "";
    viewUserManagementContainer.innerHTML = `
        <p>ID: ${user.id}</p>
        <p>Email: ${user.email}</p>
        <p>Username: ${user.userName}</p>
        <p>Role: ${user.role}</p>
    `;

    viewUserManagementModal.show();
}

async function deleteUser(id, btnDelete) {
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
        btnDelete.disabled = true;

        try {
            await userManagementAPI.delete(id);

            await getUserManagementTable();

            Swal.fire({
                icon: "success",
                title: "User has been deleted!",
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
                title: "Could not delete user!",
                toast: true,
                position: "bottom-end",
                showConfirmButton: false,
                timer: 1500,
                timerProgressBar: true,
            });
            console.error(error);
        } 
        finally {
            btnDelete.disabled = false;
        }
    }
}

function searchUserManagement()
{
    let keyword = document.getElementById("searchUserManagement").value.trim().toLowerCase();
    if (keyword == null) {
        userManagementData = [...userManagementDataAll];
        return;
    }
    else
    {
        userManagementData = userManagementDataAll.filter(i => {
            const idMatch = i.id.toLowerCase().includes(keyword);
            const emailMatch = i.email.toLowerCase().includes(keyword);
            const usernameMatch = i.userName.toLowerCase().includes(keyword);
            return idMatch || emailMatch || usernameMatch;
        });
    }

    userManagementCurrentPage = 1;

    renderUserManagementPage(userManagementCurrentPage);
}

document.addEventListener("DOMContentLoaded", () => {
    const pathName = window.location.pathname;
    if (pathName.endsWith("userManagement.html")) {
        getUserManagementTable();

        userManagementModal = new bootstrap.Modal(document.getElementById("userManagementModal"));

        updateBtn.addEventListener("click", editUser);

        viewUserManagementModal = new bootstrap.Modal(document.getElementById("viewUserManagementModal"));
        
        document.getElementById("searchUserManagement").addEventListener("keyup", e => {
            if (e.key === "Enter") {
                searchUserManagement();
            }
        });
    }
});