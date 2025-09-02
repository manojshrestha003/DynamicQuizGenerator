document.addEventListener("DOMContentLoaded", async function () {
    const profileDiv = document.getElementById("profileInfo");
    const token = localStorage.getItem("token");

    
    
        document.getElementById("logoutBtn").addEventListener("click", function () {
        localStorage.removeItem("token");
        window.location.href = "/Account/AdminLogin";
    });
});
