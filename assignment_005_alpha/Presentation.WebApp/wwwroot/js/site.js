document.addEventListener("DOMContentLoaded", () => {
    const previewSize = 150

    // Open Modal
    const modalButtons = document.querySelectorAll('[data-modal="true"]')
    modalButtons.forEach(button => {
        button.addEventListener('click', () => {
            const modalTarget = button.getAttribute('data-target');
            const modal = document.querySelector(modalTarget);

            if (modal) {
                modal.style.display = 'flex';
            }
        });
    });

    // Close Modal
    const closeButtons = document.querySelectorAll('[data-close="true"]')
    closeButtons.forEach(button => {
        button.addEventListener('click', () => {
            const modal = button.closest('.the-local-modal')
            if (modal) {
                modal.style.display = 'none';

                // Clear Form
                modal.querySelectorAll('form').forEach(form => {
                    form.reset()

                    const imagePreview = form.querySelector('.image-preview')
                    if (imagePreview)
                        imagePreview.src = ''

                    const imagePreviewer = form.querySelector('.image-previewer')
                    if (imagePreviewer)
                        imagePreviewer.classList.remove('selected')
                })
            }
        })
    })

    // Handle image-previewer
    document.querySelectorAll('.image-previewer').forEach(previewer => {
        const fileInput = previewer.querySelector('input[type="file"]')
        const imagePreview = document.querySelector('.image-preview')

        previewer.addEventListener('click', () => fileInput.click())

        fileInput.addEventListener('change', ({ target: { files} }) => {
            const file = files[0];
            if (file)
                processImage(file, imagePreview, previewer, previewSize)
        })
    })

    // Handle submit forms
    const forms = document.querySelectorAll('form')
    forms.forEach(form => {
        form.addEventListener('submit', async (e) => {
            e.preventDefault()

            clearErrorMessages(form)

            const formData = new FormData(form)

            try {
                const res = await fetch(form.action, {
                    method: 'POST',
                    body: formData,
                })

                if (res.ok) {
                    const modal = form.closest('.the-local-modal')
                    modal.style.display = 'none'
                    // form.reset()
                    window.location.reload()
                }
                else if (res.status === 400) {
                    const data = await res.json()

                    if (data.errors) {
                        Object.keys(data.errors).forEach(key => {
                            addErrorMessages(form, key, data.errors[key].join('\n'))

                            // let input = form.querySelector(`input[name="${key}"]`)
                            // if (input){
                            //     input.classList.add('input-validation-error')
                            // }
                            //
                            // let span = form.querySelector(`[data-valmsg-for="${key}"]`)
                            // if (span) {
                            //     span.innerText = data.errors[key].join('\n')
                            //     span.classList.add('field-validation-error')
                            // }
                        })
                    }
                }
            }
            catch {
                console.log('error submitting the form')
            }
        })
    })
})

function clearErrorMessages(form) {
    form.querySelectorAll('[data-val="true"]').forEach(input => {
        input.classList.remove('input-validation-error')
    })

    form.querySelectorAll('[data-valmsg-for]').forEach(span => {
        span.innerText = ''
        span.classList.remove('field-validation-error')
    })
}

function addErrorMessages(form, key, errorMessage) {
    let input = form.querySelector(`input[name="${key}"]`)
    if (input){
        input.classList.add('input-validation-error')
    }

    let span = form.querySelector(`[data-valmsg-for="${key}"]`)
    if (span) {
        span.innerText = errorMessage
        span.classList.add('field-validation-error')
    }
}

async function loadImage(file) {
    return new Promise((resolve, reject) => {
        const reader = new FileReader();

        reader.onerror = () => reject(new Error('Failed to load file'))
        reader.onload = (e) => {
            const img = new Image()
            img.onerror = () => reject(new Error('Failed to load image'))
            img.onload = () => resolve(img)
            img.src = e.target.result
        }

        reader.readAsDataURL(file)
    })
}

async function processImage(file, imagePreview, previewer, previewSize = 150) {
    try {
        const img = await loadImage(file)
        const canvas = document.createElement('canvas')
        canvas.width = previewSize
        canvas.height = previewSize

        const ctx = canvas.getContext('2d')
        ctx.drawImage(img, 0, 0, previewSize, previewSize)
        imagePreview.src = canvas.toDataURL('image/jpeg')
        previewer.classList.add('selected')
    }
    catch (error) {
        console.error('Failed on image-processing: ', error)
    }
}