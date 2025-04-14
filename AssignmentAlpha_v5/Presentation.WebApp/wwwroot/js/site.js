// Make the address look normal after a Facebook login.
if (window.location.hash && window.location.hash === "#_=_") {
    if (history.replaceState) {
        history.replaceState(null, document.title, window.location.pathname + window.location.search);
    } else {
        window.location.hash = "";
    }
}

// Dropdown menu's
document.addEventListener('DOMContentLoaded', () => {
    initializeDropdowns();
    updateRelativeTimes();
    setInterval(updateRelativeTimes, 60000);
});

function closeAllDropdowns(exceptDropdown, dropDropElements) {
    dropDropElements.forEach(dropdown => {
        if (dropdown !== exceptDropdown) {
            dropdown.classList.remove('show');
        }
    })
}

function initializeDropdowns() {
    const dropdownTriggers = document.querySelectorAll('[data-type="dropdown"]')

    const dropdownElements = new Set()
    dropdownTriggers.forEach(trigger => {
        const targetSelector = trigger.getAttribute('data-target')
        if (targetSelector) {
            const dropdown = document.querySelector(targetSelector);
            if (dropdown) {
                dropdownElements.add(dropdown);
            }
        }
    })

    dropdownTriggers.forEach(trigger => {
        trigger.addEventListener('click', (e) => {
            e.stopPropagation()
            const targetSelector = trigger.getAttribute('data-target')
            if (!targetSelector) return
            const dropdown = document.querySelector(targetSelector)
            if (!dropdown) return

            closeAllDropdowns(dropdown, dropdownElements)
            dropdown.classList.toggle('show')
        })
    })

    dropdownElements.forEach(dropdown => {
        dropdown.addEventListener('click', (e) => {
            e.stopPropagation()
        })
    })

    document.addEventListener('click', () => {
        closeAllDropdowns(null, dropdownElements)
    })
}

function updateRelativeTimes() {
    const elements = document.querySelectorAll('.notification-item .time');
    const now = new Date();
    
    elements.forEach(el => {
        const created = new Date(el.getAttribute('data-created'));
        const diff = now - created;
        const diffSeconds = Math.floor(diff / 1000);
        const diffMinutes = Math.floor(diffSeconds / 60);
        const diffHours = Math.floor(diffMinutes / 60);
        const diffDays = Math.floor(diffHours / 24);
        const diffWeeks = Math.floor(diffDays / 7);
        
        let relativeTime = '';
        
        if (diffMinutes < 1) {
            relativeTime = '0 min ago';
        } else if (diffMinutes < 60) {
            relativeTime = diffMinutes + ' min ago';
        } else if (diffHours < 2) {
            relativeTime = diffHours + ' hour ago';
        } else if (diffHours < 24) {
            relativeTime = diffHours + ' hours ago';
        } else if (diffDays < 2) {
            relativeTime = diffDays + ' day ago';
        } else if (diffDays < 7) {
            relativeTime = diffDays + ' days ago';
        } else if (diffWeeks < 2) {
            relativeTime = diffWeeks + ' week ago';
        } else {
            relativeTime = diffWeeks + ' weeks ago';
        }
        el.textContent = relativeTime;
    });
}

// Modal popups (Does not work well with external logins)
// document.addEventListener("DOMContentLoaded", () => {
//     const previewSize = 150
//
//     // Open Modal
//     const modalButtons = document.querySelectorAll('[data-modal="true"]')
//     modalButtons.forEach(button => {
//         button.addEventListener('click', () => {
//             const modalTarget = button.getAttribute('data-target');
//             const modal = document.querySelector(modalTarget);
//
//             if (modal) {
//                 modal.style.display = 'flex';
//             }
//         });
//     });
//
//     // Close Modal
//     const closeButtons = document.querySelectorAll('[data-close="true"]')
//     closeButtons.forEach(button => {
//         button.addEventListener('click', () => {
//             const modal = button.closest('.the-local-modal')
//             if (modal) {
//                 modal.style.display = 'none';
//
//                 // Clear Form
//                 modal.querySelectorAll('form').forEach(form => {
//                     form.reset()
//
//                     const imagePreview = form.querySelector('.image-preview')
//                     if (imagePreview)
//                         imagePreview.src = ''
//
//                     const imagePreviewer = form.querySelector('.image-previewer')
//                     if (imagePreviewer)
//                         imagePreviewer.classList.remove('selected')
//                 })
//             }
//         })
//     })
//
//     // Handle image-previewer
//     document.querySelectorAll('.image-previewer').forEach(previewer => {
//         const fileInput = previewer.querySelector('input[type="file"]')
//         const imagePreview = document.querySelector('.image-preview')
//
//         previewer.addEventListener('click', () => fileInput.click())
//
//         fileInput.addEventListener('change', ({ target: { files} }) => {
//             const file = files[0];
//             if (file)
//                 processImage(file, imagePreview, previewer, previewSize)
//         })
//     })
//
//     // Handle submit forms
//     const forms = document.querySelectorAll('form')
//     forms.forEach(form => {
//         form.addEventListener('submit', async (e) => {
//             e.preventDefault()
//
//             clearErrorMessages(form)
//
//             const formData = new FormData(form)
//
//             try {
//                 const res = await fetch(form.action, {
//                     method: 'POST',
//                     body: formData,
//                 })
//
//                 if (res.ok) {
//                     const modal = form.closest('.the-local-modal')
//                     modal.style.display = 'none'
//                     // form.reset()
//                     window.location.reload()
//                 }
//                 else if (res.status === 400) {
//                     const data = await res.json()
//
//                     if (data.errors) {
//                         Object.keys(data.errors).forEach(key => {
//                             addErrorMessages(form, key, data.errors[key].join('\n'))
//
//                             // let input = form.querySelector(`input[name="${key}"]`)
//                             // if (input){
//                             //     input.classList.add('input-validation-error')
//                             // }
//                             //
//                             // let span = form.querySelector(`[data-valmsg-for="${key}"]`)
//                             // if (span) {
//                             //     span.innerText = data.errors[key].join('\n')
//                             //     span.classList.add('field-validation-error')
//                             // }
//                         })
//                     }
//                 }
//             }
//             catch {
//                 console.log('error submitting the form')
//             }
//         })
//     })
// })
//
// function clearErrorMessages(form) {
//     form.querySelectorAll('[data-val="true"]').forEach(input => {
//         input.classList.remove('input-validation-error')
//     })
//
//     form.querySelectorAll('[data-valmsg-for]').forEach(span => {
//         span.innerText = ''
//         span.classList.remove('field-validation-error')
//     })
// }
//
// function addErrorMessages(form, key, errorMessage) {
//     let input = form.querySelector(`input[name="${key}"]`)
//     if (input){
//         input.classList.add('input-validation-error')
//     }
//
//     let span = form.querySelector(`[data-valmsg-for="${key}"]`)
//     if (span) {
//         span.innerText = errorMessage
//         span.classList.add('field-validation-error')
//     }
// }
//
// async function loadImage(file) {
//     return new Promise((resolve, reject) => {
//         const reader = new FileReader();
//
//         reader.onerror = () => reject(new Error('Failed to load file'))
//         reader.onload = (e) => {
//             const img = new Image()
//             img.onerror = () => reject(new Error('Failed to load image'))
//             img.onload = () => resolve(img)
//             img.src = e.target.result
//         }
//
//         reader.readAsDataURL(file)
//     })
// }
//
// async function processImage(file, imagePreview, previewer, previewSize = 150) {
//     try {
//         const img = await loadImage(file)
//         const canvas = document.createElement('canvas')
//         canvas.width = previewSize
//         canvas.height = previewSize
//
//         const ctx = canvas.getContext('2d')
//         ctx.drawImage(img, 0, 0, previewSize, previewSize)
//         imagePreview.src = canvas.toDataURL('image/jpeg')
//         previewer.classList.add('selected')
//     }
//     catch (error) {
//         console.error('Failed on image-processing: ', error)
//     }
// }