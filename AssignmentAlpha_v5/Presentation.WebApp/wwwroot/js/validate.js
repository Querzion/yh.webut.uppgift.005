const validateField = (field) => {
    const errorSpan = document.querySelector(`span[data-valmsg-for='${field.name}']`);
    if (!errorSpan) return;

    let errorMessage = "";

    const isCheckbox = field.type === "checkbox";
    const value = isCheckbox ? field.checked : field.value.trim();

    // Required (text fields only)
    if (field.hasAttribute("data-val-required") && !isCheckbox && value === "") {
        errorMessage = field.getAttribute("data-val-required");
    }

    // Checkbox must be checked (for [Range(typeof(bool), "true", "true")])
    if (field.hasAttribute("data-val-range") && isCheckbox && !value) {
        errorMessage = field.getAttribute("data-val-range");
    }

    // Regex
    if (!errorMessage && field.hasAttribute("data-val-regex") && !isCheckbox && value !== "") {
        const pattern = new RegExp(field.getAttribute("data-val-regex-pattern"));
        if (!pattern.test(value)) {
            errorMessage = field.getAttribute("data-val-regex");
        }
    }

    // Compare (e.g. ConfirmPassword == Password)
    if (!errorMessage && field.hasAttribute("data-val-equalto") && !isCheckbox) {
        const otherFieldId = field.getAttribute("data-val-equalto-other").replace("*.", "");
        const otherField = document.querySelector(`[name='${otherFieldId}']`);
        if (otherField && otherField.value !== field.value) {
            errorMessage = field.getAttribute("data-val-equalto");
        }
    }

    // MinLength
    if (!errorMessage && field.hasAttribute("data-val-minlength") && !isCheckbox) {
        const minLength = parseInt(field.getAttribute("data-val-minlength-min"), 10);
        if (value.length < minLength) {
            errorMessage = field.getAttribute("data-val-minlength");
        }
    }

    // Show or clear error
    if (errorMessage) {
        field.classList.add("input-validation-error");
        errorSpan.classList.remove("field-validation-valid");
        errorSpan.classList.add("field-validation-error");
        errorSpan.textContent = errorMessage;
    } else {
        field.classList.remove("input-validation-error");
        errorSpan.classList.remove("field-validation-error");
        errorSpan.classList.add("field-validation-valid");
        errorSpan.textContent = "";
    }
};

document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    if (!form) return;

    const fields = form.querySelectorAll('input[data-val="true"]');

    // Validate on input/change
    fields.forEach((field) => {
        const eventType = field.type === "checkbox" ? "change" : "input";
        field.addEventListener(eventType, function () {
            validateField(field);
        });
    });

    // Validate on submit
    form.addEventListener("submit", function (e) {
        let hasErrors = false;

        fields.forEach((field) => {
            validateField(field);
            if (field.classList.contains("input-validation-error")) {
                hasErrors = true;
            }
        });

        if (hasErrors) {
            e.preventDefault();
        }
    });
});
