document.addEventListener("DOMContentLoaded", function() {
    const darkModeToggle = document.getElementById("darkModeToggle");

    // Check if dark mode was previously enabled
    if (localStorage.getItem("darkMode") === "enabled") {
        document.body.setAttribute("data-theme", "dark");
        darkModeToggle.checked = true;
    }

    // Toggle dark mode on checkbox change
    darkModeToggle.addEventListener("change", function() {
        if (darkModeToggle.checked) {
            document.body.setAttribute("data-theme", "dark");
            localStorage.setItem("darkMode", "enabled");
        } else {
            document.body.removeAttribute("data-theme");
            localStorage.setItem("darkMode", "disabled");
        }
    });
});