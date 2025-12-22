// Settings JavaScript
(function() {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initSaveButton();
        initThemeSelector();
    });

    // Save Settings Button
    function initSaveButton() {
        const saveButton = document.querySelector('.btn-primary');
        if (saveButton && saveButton.textContent.includes('Save Changes')) {
            saveButton.addEventListener('click', function() {
                // TODO: Implement save settings logic
                const settings = {
                    siteName: document.getElementById('siteName').value,
                    siteEmail: document.getElementById('siteEmail').value,
                    sitePhone: document.getElementById('sitePhone').value,
                    timezone: document.getElementById('timezone').value,
                    emailNotifications: document.getElementById('emailNotifications').checked,
                    smsNotifications: document.getElementById('smsNotifications').checked,
                    pushNotifications: document.getElementById('pushNotifications').checked,
                    orderNotifications: document.getElementById('orderNotifications').checked,
                    sessionTimeout: document.getElementById('sessionTimeout').value,
                    twoFactorAuth: document.getElementById('twoFactorAuth').checked,
                    passwordExpiry: document.getElementById('passwordExpiry').checked,
                    theme: document.getElementById('theme').value,
                    language: document.getElementById('language').value,
                    dateFormat: document.getElementById('dateFormat').value
                };
                
                console.log('Save settings:', settings);
                alert('Đã lưu cài đặt thành công!');
            });
        }
    }

    // Theme Selector
    function initThemeSelector() {
        const themeSelect = document.getElementById('theme');
        if (themeSelect) {
            themeSelect.addEventListener('change', function() {
                const theme = this.value;
                // Update theme immediately if not auto
                if (theme !== 'auto') {
                    document.documentElement.setAttribute('data-bs-theme', theme);
                    localStorage.setItem('theme', theme);
                }
            });
        }
    }
})();

