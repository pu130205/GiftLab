 //Products JavaScript
(function() {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initEditProductModal();
        initDeleteProductModal();
        initViewProductModal();
        initImagePreview();
        initFilterModal();
    });

    // Edit Product Modal - Load data when opened
    function initEditProductModal() {
        const modal = document.getElementById('editProductModal');
        if (!modal) return;

        modal.addEventListener('show.bs.modal', function (event) {
            const btn = event.relatedTarget;

            document.getElementById('editProductId').value = btn.dataset.id;
            document.getElementById('editProductName').value = btn.dataset.name;

            document.getElementById('editProductShortDesc').value = btn.dataset.shortdesc;
            document.getElementById('editProductDescription').value = btn.dataset.description;

            document.getElementById('editProductPrice').value = btn.dataset.price;
            document.getElementById('editProductStock').value = btn.dataset.stock;
            document.getElementById('editProductStatus').value = btn.dataset.active === "True" ? "In Stock" : "Out of Stock";
            document.getElementById('editProductCurrentImage').value = btn.dataset.image;

            document.getElementById('editProductImagePreview').src = btn.dataset.image;
            document.getElementById('editProductImagePreview').style.display = 'block';
        });
    }

    // Delete Product Modal - Load data when opened
    function initDeleteProductModal() {
        const modal = document.getElementById('deleteProductModal');
        if (!modal) return;

        modal.addEventListener('show.bs.modal', function (event) {
            const btn = event.relatedTarget;

            document.getElementById('deleteProductId').value = btn.dataset.id;
            document.getElementById('deleteProductName').innerText = btn.dataset.name;
        });
    }

    // Make functions globally available for onclick handlers
    window.handleAddProduct = function () {

        const formData = new FormData();

        formData.append("ProductName", document.getElementById("addProductName").value);
        formData.append("ShortDesc", document.getElementById("addProductShortDesc").value);
        formData.append("Description", document.getElementById("addProductDescription").value);
        formData.append("Price", document.getElementById("addProductPrice").value);
        formData.append("UnitsInStock", document.getElementById("addProductStock").value);
        formData.append("CatID", document.getElementById("addProductCategory").value);
        formData.append("Active", true);

        const imageInput = document.getElementById("addProductImage");
        if (imageInput.files.length > 0) {
            formData.append("Image", imageInput.files[0]);
        }

        fetch("/admin/products/create", {
            method: "POST",
            body: formData
        })
            .then(res => {
                if (!res.ok) throw new Error("Thêm sản phẩm thất bại");
                return res.json();
            })
            .then(data => {
                alert("✅ Thêm sản phẩm thành công!");
                location.reload();
            })
            .catch(err => {
                alert("❌ " + err.message);
            });

    };

    window.handleEditProduct = function () {
        const formData = new FormData();

        formData.append("ProductID", document.getElementById("editProductId").value);
        formData.append("ProductName", document.getElementById("editProductName").value);
        formData.append("Price", document.getElementById("editProductPrice").value);
        formData.append("UnitsInStock", document.getElementById("editProductStock").value);
        formData.append("CatID", document.getElementById("editProductCategory").value);
        formData.append("Active", document.getElementById("editProductStatus").value === "In Stock");

        formData.append("ShortDesc", document.getElementById("editProductShortDesc").value);
        formData.append("Description", document.getElementById("editProductDescription").value);

        const imageInput = document.getElementById("editProductImage");
        if (imageInput.files.length > 0) {
            formData.append("Image", imageInput.files[0]);
        }

        fetch("/admin/products/edit", {
            method: "POST",
            body: formData
        })
            .then(async res => {
                const data = await res.json().catch(() => null);
                if (!res.ok) throw new Error(data?.message || "Cập nhật thất bại");
                return data;
            })

            .then(() => {
                alert("✅ Cập nhật thành công");
                location.reload();
            })
            .catch(err => alert("❌ " + err.message));
    };

    window.handleDeleteProduct = function () {
        const id = document.getElementById('deleteProductId').value;

        fetch('/admin/products/delete', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: `id=${id}`
        })
            .then(res => {
                if (!res.ok) throw new Error('Xóa thất bại');
                return res.json();
            })
            .then(() => {
                alert('🗑️ Đã xóa sản phẩm');
                location.reload();
            })
            .catch(err => alert('❌ ' + err.message));
    };


    // Preview Image Function
    function initImagePreview() {
        const addProductImage = document.getElementById('addProductImage');
        const editProductImage = document.getElementById('editProductImage');
        
        if (addProductImage) {
            addProductImage.addEventListener('change', function() {
                previewImage(this, 'addProductImagePreview');
            });
        }
        
        if (editProductImage) {
            editProductImage.addEventListener('change', function() {
                previewImage(this, 'editProductImagePreview');
            });
        }
    }

    window.previewImage = function(input, previewId) {
        const preview = document.getElementById(previewId);
        if (input.files && input.files[0]) {
            const reader = new FileReader();
            reader.onload = function(e) {
                preview.src = e.target.result;
                preview.style.display = 'block';
            };
            reader.readAsDataURL(input.files[0]);
        } else {
            if (preview) {
                preview.style.display = 'none';
            }
        }
    };

    // View Product Modal - Load data when opened
    function initViewProductModal() {
        const viewProductModal = document.getElementById('viewProductModal');
        if (viewProductModal) {
            viewProductModal.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget;
                const productId = button.getAttribute('data-product-id');
                const productName = button.getAttribute('data-product-name');
                const productCategory = button.getAttribute('data-product-category');
                const productPrice = button.getAttribute('data-product-price');
                const productStock = button.getAttribute('data-product-stock');
                const productStatus = button.getAttribute('data-product-status');
                const productImage = button.getAttribute('data-product-image');
                const productDescription = button.getAttribute('data-product-description');
                
                document.getElementById('viewProductId').textContent = '#' + productId;
                document.getElementById('viewProductName').textContent = productName;
                document.getElementById('viewProductCategory').textContent = productCategory;
                document.getElementById('viewProductPrice').textContent = '₫' + parseInt(productPrice).toLocaleString('vi-VN');
                document.getElementById('viewProductStock').textContent = productStock;
                document.getElementById('viewProductDescription').textContent = productDescription || 'Chưa có mô tả.';
                
                // Set status badge
                const statusElement = document.getElementById('viewProductStatus');
                if (productStatus === 'In Stock' || productStatus === 'Còn hàng') {
                    statusElement.innerHTML = '<span class="badge bg-success">Còn hàng</span>';
                } else if (productStatus === 'Out of Stock' || productStatus === 'Hết hàng') {
                    statusElement.innerHTML = '<span class="badge bg-danger">Hết hàng</span>';
                } else {
                    statusElement.innerHTML = '<span class="badge bg-warning">Sắp hết hàng</span>';
                }
                
                // Set image
                //document.getElementById('viewProductImage').src = productImage;
                document.getElementById('viewProductImage').src =
                    productImage || '/images/no-image.png';

                
                // Set edit button to open edit modal
                const editFromViewBtn = document.getElementById('editFromViewBtn');
                if (editFromViewBtn) {
                    editFromViewBtn.onclick = function() {
                        const modal = bootstrap.Modal.getInstance(viewProductModal);
                        modal.hide();
                        // Trigger edit modal with same data
                        setTimeout(() => {
                            button.click();
                        }, 300);
                    };
                }
            });
        }
    }

    // Filter Modal
function initFilterModal() {
    const clearFiltersBtn = document.getElementById('clearProductFiltersBtn');
    const applyFiltersBtn = document.getElementById('applyProductFiltersBtn');

    if (clearFiltersBtn) {
        clearFiltersBtn.addEventListener('click', clearProductFilters);
    }

    if (applyFiltersBtn) {
        applyFiltersBtn.addEventListener('click', applyProductFilters);
    }

    // ✅ Auto-fill filter inputs from current URL query string
    const modalEl = document.getElementById('filterProductModal');
    if (!modalEl) return;

    modalEl.addEventListener('show.bs.modal', function () {
        const params = new URLSearchParams(window.location.search);

        const setIfExists = (id, value) => {
            const el = document.getElementById(id);
            if (!el) return;
            el.value = value ?? '';
        };

        setIfExists('filterProductCategory', params.get('category'));
        setIfExists('filterProductStatus', params.get('status'));
        setIfExists('filterProductPriceFrom', params.get('minPrice'));
        setIfExists('filterProductPriceTo', params.get('maxPrice'));
        setIfExists('filterProductStockFrom', params.get('minStock'));
        setIfExists('filterProductStockTo', params.get('maxStock'));
    });
}


    window.applyProductFilters = function () {
        const category = document.getElementById('filterProductCategory')?.value?.trim() || '';
        const status = document.getElementById('filterProductStatus')?.value?.trim() || '';
        const minPrice = document.getElementById('filterProductPriceFrom')?.value?.trim() || '';
        const maxPrice = document.getElementById('filterProductPriceTo')?.value?.trim() || '';
        const minStock = document.getElementById('filterProductStockFrom')?.value?.trim() || '';
        const maxStock = document.getElementById('filterProductStockTo')?.value?.trim() || '';

        // Build query string đúng với AdminController.Products(category, status, minPrice...)
        const params = new URLSearchParams();

        if (category) params.set('category', category);
        if (status) params.set('status', status);
        if (minPrice) params.set('minPrice', minPrice);
        if (maxPrice) params.set('maxPrice', maxPrice);
        if (minStock) params.set('minStock', minStock);
        if (maxStock) params.set('maxStock', maxStock);

        // (Tuỳ chọn) nếu bạn muốn giữ search hiện tại khi filter:
        const q = document.getElementById('productSearch')?.value?.trim();
        if (q) params.set('q', q);

        const url = '/admin/products' + (params.toString() ? `?${params.toString()}` : '');

        // đóng modal (để UI mượt) rồi redirect
        const modalEl = document.getElementById('filterProductModal');
        const modal = modalEl ? bootstrap.Modal.getInstance(modalEl) : null;
        if (modal) modal.hide();

        // redirect để server lọc DB
        window.location.href = url;
    };


    window.clearProductFilters = function () {
        // reset UI input
        const setVal = (id, v) => {
            const el = document.getElementById(id);
            if (el) el.value = v;
        };

        setVal('filterProductCategory', '');
        setVal('filterProductStatus', '');
        setVal('filterProductPriceFrom', '');
        setVal('filterProductPriceTo', '');
        setVal('filterProductStockFrom', '');
        setVal('filterProductStockTo', '');

        // đóng modal rồi quay về trang không filter (server trả full list)
        const modalEl = document.getElementById('filterProductModal');
        const modal = modalEl ? bootstrap.Modal.getInstance(modalEl) : null;
        if (modal) modal.hide();

        window.location.href = '/admin/products';
    };


    const rowsPerPage = 5;
    let currentPage = 1;

    function paginate() {
        const allRows = Array.from(document.querySelectorAll('.product-row'));

        // 👉 chỉ lấy những row KHÔNG bị filter
        const visibleRows = allRows.filter(row =>
            !row.classList.contains('filtered-out')
        );

        const totalPages = Math.ceil(visibleRows.length / rowsPerPage);
        const pagination = document.getElementById('pagination');
        pagination.innerHTML = '';

        // Ẩn toàn bộ trước
        allRows.forEach(row => row.style.display = 'none');

        // Hiện row theo trang
        visibleRows.forEach((row, index) => {
            if (
                index >= (currentPage - 1) * rowsPerPage &&
                index < currentPage * rowsPerPage
            ) {
                row.style.display = '';
            }
        });

        // Render pagination
        for (let i = 1; i <= totalPages; i++) {
            const li = document.createElement('li');
            li.className = `page-item ${i === currentPage ? 'active' : ''}`;
            li.innerHTML = `<a class="page-link" href="#">${i}</a>`;
            li.onclick = function (e) {
                e.preventDefault();
                currentPage = i;
                paginate();
            };
            pagination.appendChild(li);
        }
    }

    document.addEventListener('DOMContentLoaded', paginate);

    document.getElementById('productSearch')?.addEventListener('keyup', function () {
        const keyword = this.value.toLowerCase();
        const rows = document.querySelectorAll('.product-row');

        rows.forEach(row => {
            const name = row.dataset.name;
            const category = row.dataset.category;

            if (name.includes(keyword) || category.includes(keyword)) {
                row.classList.remove('filtered-out');
            } else {
                row.classList.add('filtered-out');
            }
        });

        currentPage = 1; // 🔥 reset về trang 1
        paginate();
    });
    document.getElementById('exportProductsBtn')?.addEventListener('click', function () {
        // lấy query hiện tại để export đúng dữ liệu đang lọc
        const query = window.location.search || '';
        window.location.href = '/admin/products/export' + query;
    });
})();

