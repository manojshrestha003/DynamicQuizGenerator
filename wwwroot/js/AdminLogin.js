document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");

    form.addEventListener("submit", async function (e) {
        e.preventDefault();
        const data = {
            email: document.getElementById("Email").value,
            password: document.getElementById("Password").value
        };

        try {
            const response = await fetch("/api/AdminAuth/login", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();

            if (response.ok) {
                localStorage.setItem("token", result.token);
                alert("Login successful!");
                window.location.href = "/Quizzes";
            } else {
                alert(result.message);
            }
        } catch (err) {
            console.error(err);
            alert("An error occurred while logging in.");
        }
    });
});
