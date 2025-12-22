// Users JavaScript
(function() {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initEditUserModal();
        initDeleteUserModal();
    });

    // Edit User Modal - Load data when opened
    function initEditUserModal() {
        const editUserModal = document.getElementById('editUserModal');
        if (editUserModal) {
            editUserModal.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget;
                const userId = button.getAttribute('data-user-id');
                const userName = button.getAttribute('data-user-name');
                const userEmail = button.getAttribute('data-user-email');
                const userRole = button.getAttribute('data-user-role');
                const userStatus = button.getAttribute('data-user-status');
                
                document.getElementById('editUserId').value = userId;
                document.getElementById('editUserName').value = userName;
                document.getElementById('editUserEmail').value = userEmail;
                document.getElementById('editUserRole').value = userRole;
                document.getElementById('editUserStatus').value = userStatus;
            });
        }
    }

    // Delete User Modal - Load data when opened
    function initDeleteUserModal() {
        const deleteUserModal = document.getElementById('deleteUserModal');
        if (deleteUserModal) {
            deleteUserModal.addEventListener('show.bs.modal', function (event) {
                const button = event.relatedTarget;
                const userId = button.getAttribute('data-user-id');
                const userName = button.getAttribute('data-user-name');
                
                document.getElementById('deleteUserId').value = userId;
                document.getElementById('deleteUserName').textContent = userName;
            });
        }
    }

    // Make functions globally available for onclick handlers
    window.handleAddUser = function() {
        const form = document.getElementById('addUserForm');
        if (form.checkValidity()) {
            // TODO: Implement add user logic
            console.log('Add user:', {
                name: document.getElementById('addUserName').value,
                email: document.getElementById('addUserEmail').value,
                username: document.getElementById('addUserUsername').value,
                role: document.getElementById('addUserRole').value,
                status: document.getElementById('addUserStatus').value
            });
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('addUserModal'));
            modal.hide();
            form.reset();
            alert('Đã thêm người dùng thành công!');
        } else {
            form.reportValidity();
        }
    };

    window.handleEditUser = function() {
        const form = document.getElementById('editUserForm');
        if (form.checkValidity()) {
            const userId = document.getElementById('editUserId').value;
            // TODO: Implement edit user logic
            console.log('Edit user:', {
                id: userId,
                name: document.getElementById('editUserName').value,
                email: document.getElementById('editUserEmail').value,
                username: document.getElementById('editUserUsername').value,
                role: document.getElementById('editUserRole').value,
                status: document.getElementById('editUserStatus').value
            });
            
            const modal = bootstrap.Modal.getInstance(document.getElementById('editUserModal'));
            modal.hide();
            alert('Đã cập nhật người dùng thành công!');
        } else {
            form.reportValidity();
        }
    };

    window.handleDeleteUser = function() {
        const userId = document.getElementById('deleteUserId').value;
        // TODO: Implement delete user logic
        console.log('Delete user:', userId);
        
        const modal = bootstrap.Modal.getInstance(document.getElementById('deleteUserModal'));
        modal.hide();
        alert('Đã xóa người dùng thành công!');
    };
})();

