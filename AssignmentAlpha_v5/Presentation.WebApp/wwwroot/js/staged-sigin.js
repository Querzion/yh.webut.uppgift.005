// console.log("🟢 Script section is running");

document.addEventListener("DOMContentLoaded", function () {
    // console.log("✅ DOM is ready");

    const continueBtn = document.querySelector('#continueBtn');
    const emailInput = document.getElementById('emailInput');
    const form = document.getElementById('email-staged');
    const errorMessage = document.getElementById('error-message');
    const partialContainer = document.getElementById('partial-container');

    if (!continueBtn) {
        // console.warn("⚠️ Continue button NOT found");
        return;
    }

    // console.log("🟡 Button found");

    continueBtn.addEventListener('click', async function () {
        // console.log("🔵 Continue button clicked");

        errorMessage.classList.add('hidden');
        errorMessage.textContent = "";

        if (!form.checkValidity()) {
            // console.warn("❌ Form not valid");
            form.reportValidity();
            return;
        }

        continueBtn.disabled = true;

        try {
            const email = encodeURIComponent(emailInput.value);
            const url = `/Auth/LocalSignInPartial?email=${email}`;
            // console.log(`📡 Fetching: ${url}`);

            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (response.ok) {
                const html = await response.text();
                // console.log("✅ Response OK, injecting partial HTML");
                form.style.display = 'none';
                partialContainer.innerHTML = html;
            } else if (response.status === 404) {
                showError("No account found with that email.");
            } else if (response.status === 400) {
                const errorText = await response.text();
                showError(errorText || "Invalid request.");
            } else {
                showError("An error occurred. Please try again.");
            }
        } catch (error) {
            console.error("❗️Fetch error:", error);
            showError("Unable to connect. Try again later.");
        }

        setTimeout(() => {
            continueBtn.disabled = false;
        }, 5000);
    });

    function showError(message) {
        errorMessage.textContent = message;
        errorMessage.classList.remove('hidden');
    }
});
