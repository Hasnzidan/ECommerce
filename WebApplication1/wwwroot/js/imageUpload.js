document.addEventListener('DOMContentLoaded', function () {
    const dropZone = document.getElementById('drop-zone');
    const fileInput = document.getElementById('file-input');
    const previewContainer = document.getElementById('preview-container');
    const uploadForm = document.getElementById('uploadForm');
    const maxFileSize = 5 * 1024 * 1024; // 5MB
    const allowedTypes = ['image/jpeg', 'image/png', 'image/gif'];

    // Drag and drop handlers
    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        dropZone.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    ['dragenter', 'dragover'].forEach(eventName => {
        dropZone.addEventListener(eventName, highlight, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        dropZone.addEventListener(eventName, unhighlight, false);
    });

    function highlight() {
        dropZone.classList.add('drag-over');
    }

    function unhighlight() {
        dropZone.classList.remove('drag-over');
    }

    // Handle dropped files
    dropZone.addEventListener('drop', handleDrop, false);
    fileInput.addEventListener('change', function(e) {
        handleFiles(e.target.files);
    });

    function handleDrop(e) {
        const dt = e.dataTransfer;
        const files = dt.files;
        fileInput.files = files; // Update the file input with dropped files
        handleFiles(files);
    }

    function handleFiles(files) {
        previewContainer.innerHTML = ''; // Clear previous previews
        const validFiles = [...files].filter(file => validateFile(file));
        validFiles.forEach(previewFile);
    }

    function validateFile(file) {
        // Check file type
        if (!allowedTypes.includes(file.type)) {
            alert(`File type ${file.type} is not allowed. Please use JPEG, PNG or GIF.`);
            return false;
        }

        // Check file size
        if (file.size > maxFileSize) {
            alert(`File ${file.name} is too large. Maximum size is 5MB.`);
            return false;
        }

        return true;
    }

    function previewFile(file) {
        const reader = new FileReader();
        reader.readAsDataURL(file);
        reader.onloadend = function() {
            const img = document.createElement('div');
            img.className = 'preview-item';
            img.innerHTML = `
                <img src="${reader.result}" />
                <div class="preview-name">${file.name}</div>
                <button type="button" class="remove-btn" data-filename="${file.name}">&times;</button>
            `;
            previewContainer.appendChild(img);

            // Add remove button functionality
            img.querySelector('.remove-btn').addEventListener('click', function() {
                img.remove();
                updateFilesCollection();
            });
        }
    }

    function updateFilesCollection() {
        const previewItems = previewContainer.querySelectorAll('.preview-item');
        if (previewItems.length === 0) {
            fileInput.value = ''; // Clear the file input if no previews remain
        }
    }

    // Form submission handler
    uploadForm.addEventListener('submit', function(e) {
        if (fileInput.files.length === 0) {
            e.preventDefault();
            alert('Please select at least one image file.');
            return false;
        }
    });
});
