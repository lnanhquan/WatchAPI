const authAPI = {
    register: (data) => {return api.post("/Auth/register", data);},
    login: (data) => {return api.post("/Auth/login", data);},
    logout: () => {return api.post("/Auth/logout");},
    checkEmail: (email) => {return api.get("/Auth/check-email", { params: { email } });},
    checkUsername: (userName) => {return api.get("/Auth/check-username", { params: { userName } });}
};

function openLoginModal() {
    registerModal.hide();

    loginModal.show();

    document.getElementById("loginEmail").value = "";
    document.getElementById("loginPassword").value = "";
}

function switchToRegister() {
    loginModal.hide();
    registerModal.show();
}

function switchToLogin() {
    registerModal.hide();
    loginModal.show();
}

function validatePassword(password) {
    const minLength = 6;
    const requireUpper = true;
    const requireLower = true;
    const requireDigit = true;

    if (password.length < minLength) return false;
    if (requireUpper && !/[A-Z]/.test(password)) return false;
    if (requireLower && !/[a-z]/.test(password)) return false;
    if (requireDigit && !/\d/.test(password)) return false;

    return true;
}

async function isEmailTaken(email) {
    const response = await authAPI.checkEmail(email);
    return response.data;
}

async function isUsernameTaken(userName) {
    const response = await authAPI.checkUsername(userName);
    return response.data;
}

async function register() {
    const email = document.getElementById("registerEmail").value;
    const username = document.getElementById("registerUsername").value;
    const password = document.getElementById("registerPassword").value;

    try 
    {
        if (!email || !password || !username) 
        {
            Swal.fire('Error', 'Please fill in every field', 'error');
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

        if (await isEmailTaken(email)) 
        {
            Swal.fire("Error", "Email is already registered.", "error");
            return;
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

        if (await isUsernameTaken(username)) 
        {
            Swal.fire("Error", "Username is already registered.", "error");
            return;
        }

        if (!validatePassword(password)) 
        {
            Swal.fire({
                icon: 'warning',
                title: 'Password Requirement',
                html: 'Password must be at least 6 characters, include uppercase, lowercase and a number.'
            });
            return;
        }
        await authAPI.register({ username, email, password });
        Swal.fire({
            icon: "success",
            title: "Registered successfully!",
            toast: true,
            position: "bottom-end",
            showConfirmButton: false,
            timer: 1500,
            timerProgressBar: true,
        });
        switchToLogin();
    } 
    catch (error) 
    {
        Swal.fire("Error", "Registration failed", "error");
        console.error(error);
    }
}

async function login() {
    const email = document.getElementById("loginEmail").value;
    const password = document.getElementById("loginPassword").value;
    
    try 
    {
        if (!email || !password) 
            {
            Swal.fire('Error', 'Please enter both email and password!', 'error');
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

        const response = await authAPI.login({ email, password });
        localStorage.setItem("user", JSON.stringify(response.data));
        updateUIAfterLogin(true);
        loadCartCount();
        loginModal.hide();

        Swal.fire({
            icon: "success",
            title: "Logged in successfully!",
            toast: true,
            position: "bottom-end",
            showConfirmButton: false,
            timer: 1500,
            timerProgressBar: true,
        });
    }
    catch (error) {
        if (error.response) 
        {
            if (error.response.status === 401) 
            {
                Swal.fire("Login Failed", "Invalid email or password.", "error");
            } 
            else 
            {
                Swal.fire("Error", error.response.data || "An error occurred.", "error");
            }
        } else {
            Swal.fire("Error", "Login failed. Check your network.", "error");
        }
        console.error(error);
    }
}

async function logout() {
    const result = await Swal.fire({
        title: "Are you sure?",
        text: "Do you really want to log out?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, log out",
        cancelButtonText: "Cancel",
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6"
    });

    if (result.isConfirmed) {
        await authAPI.logout();
        localStorage.removeItem("user");
        updateUIAfterLogin(false);
        removeSidebar();
        window.location.href = "home.html";
    }
}