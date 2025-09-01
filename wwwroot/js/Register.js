document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form"); 

    form.addEventListener("submit", async function (e) {
        e.preventDefault(); 

        // collect values from form inputs
        const data = {
            username: document.getElementById("username").value,
            email: document.getElementById("Email").value,
            password: document.getElementById("Password").value
        };

        try {
            const response = await fetch("/api/StudentAuth/register", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(data)
            });

            const result = await response.json();

            if (response.ok) {
                alert(result.message); 
                window.location.href = "/Account/Login"; 
            } else {
                alert(result.message);
            }
        } catch (err) {
            console.error(err);
            alert("An error occurred while registering.");
        }
    });
});
