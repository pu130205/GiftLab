// Admin Layout Main JavaScript
(function() {
    'use strict';

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initSidebar();
        initThemeToggle();
        initFullscreenToggle();
        initTooltips();
        hideLoadingScreen();
    });

    // Sidebar Toggle
    function initSidebar() {
        const toggleButtons = document.querySelectorAll('[data-sidebar-toggle]');
        const wrapper = document.getElementById('admin-wrapper');
        const isMobile = window.innerWidth <= 768;

        if (toggleButtons.length > 0 && wrapper) {
            // Set initial state from localStorage (chỉ trên desktop)
            if (!isMobile) {
                const isCollapsed = localStorage.getItem('sidebar-collapsed') === 'true';
                if (isCollapsed) {
                    wrapper.classList.add('sidebar-collapsed');
                    toggleButtons.forEach(btn => btn.classList.add('is-active'));
                }
            } else {
                // Trên mobile, mặc định ẩn sidebar
                wrapper.classList.add('sidebar-collapsed');
            }

            // Attach click listener cho tất cả toggle buttons
            toggleButtons.forEach(toggleButton => {
                toggleButton.addEventListener('click', function(e) {
                    e.preventDefault();
                    e.stopPropagation();
                    
                    const isCurrentlyCollapsed = wrapper.classList.contains('sidebar-collapsed');
                    
                    if (isCurrentlyCollapsed) {
                        // Mở sidebar
                        wrapper.classList.remove('sidebar-collapsed');
                        toggleButtons.forEach(btn => btn.classList.add('is-active'));
                        if (!isMobile) {
                            localStorage.setItem('sidebar-collapsed', 'false');
                        }
                    } else {
                        // Đóng sidebar
                        wrapper.classList.add('sidebar-collapsed');
                        toggleButtons.forEach(btn => btn.classList.remove('is-active'));
                        if (!isMobile) {
                            localStorage.setItem('sidebar-collapsed', 'true');
                        }
                    }
                });
            });
        }
    }

    // Theme Toggle
    function initThemeToggle() {
        const themeToggle = document.getElementById('theme-toggle');
        const themeIcon = document.getElementById('theme-icon');
        
        if (themeToggle) {
            // Get initial theme
            const currentTheme = localStorage.getItem('theme') || 'light';
            document.documentElement.setAttribute('data-bs-theme', currentTheme);
            updateThemeIcon(currentTheme, themeIcon);

            themeToggle.addEventListener('click', function() {
                const currentTheme = document.documentElement.getAttribute('data-bs-theme');
                const newTheme = currentTheme === 'light' ? 'dark' : 'light';
                
                document.documentElement.setAttribute('data-bs-theme', newTheme);
                localStorage.setItem('theme', newTheme);
                updateThemeIcon(newTheme, themeIcon);
            });
        }
    }

    function updateThemeIcon(theme, iconElement) {
        if (iconElement) {
            iconElement.className = theme === 'light' 
                ? 'bi bi-sun-fill' 
                : 'bi bi-moon-fill';
        }
    }

    // Fullscreen Toggle
    function initFullscreenToggle() {
        const fullscreenButton = document.querySelector('[data-fullscreen-toggle]');
        
        if (fullscreenButton) {
            fullscreenButton.addEventListener('click', async function() {
                try {
                    if (!document.fullscreenElement) {
                        await document.documentElement.requestFullscreen();
                    } else {
                        await document.exitFullscreen();
                    }
                } catch (error) {
                    console.error('Fullscreen toggle failed:', error);
                }
            });
        }
    }

    // Initialize Bootstrap Tooltips
    function initTooltips() {
        const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
        tooltipTriggerList.map(function (tooltipTriggerEl) {
            return new bootstrap.Tooltip(tooltipTriggerEl);
        });
    }

    // Hide Loading Screen
    function hideLoadingScreen() {
        const loadingScreen = document.getElementById('loading-screen');
        if (loadingScreen) {
            setTimeout(function() {
                loadingScreen.style.opacity = '0';
                setTimeout(function() {
                    loadingScreen.style.display = 'none';
                }, 300);
            }, 500);
        }
    }

    // Keyboard shortcuts
    document.addEventListener('keydown', function(e) {
        // Ctrl/Cmd + K for search
        if ((e.ctrlKey || e.metaKey) && e.key === 'k') {
            e.preventDefault();
            const searchInput = document.querySelector('[data-search-input]');
            if (searchInput) {
                searchInput.focus();
            }
        }
    });
})();

