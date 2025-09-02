document.addEventListener("DOMContentLoaded", async function () {
    const profileDiv = document.getElementById("profileInfo");
    const token = localStorage.getItem("token");

    if (!token) {
        alert("You must be logged in to view this page.");
        window.location.href = "/Account/Login";
        return;
    }

    try {
        const response = await fetch("/api/StudentAuth/profile", {
            method: "GET",
            headers: {
                "Authorization": `Bearer ${token}`
            }
        });

        if (response.ok) {
            const student = await response.json();
            profileDiv.innerHTML = `
                <p><strong>Username:</strong> ${student.username}</p>
                <p><strong>Email:</strong> ${student.email}</p>
                <p><strong>Student ID:</strong> ${student.studentId}</p>
            `;
        } else {
            const err = await response.json();
            alert(err.message);
            window.location.href = "/Account/Login";
        }
    } catch (err) {
        console.error(err);
        alert("Error loading profile.");
    }

    document.getElementById('participate').addEventListener("click", function () {
        window.location.href = "/TakeQuiz";
    })

    document.getElementById('history').addEventListener("click", function () {
        window.location.href = "/TakeQuiz/History";
    })

    // Logout button
    document.getElementById("logoutBtn").addEventListener("click", function () {
        localStorage.removeItem("token");
        window.location.href = "/Account/Login";
    });
});
