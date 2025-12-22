// Analytics JavaScript
(function() {
    'use strict';

    let trafficChart, trafficSourceChart;

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        if (typeof Chart !== 'undefined') {
            initTrafficChart();
            initTrafficSourceChart();
        }
    });

    // Traffic Chart
    function initTrafficChart() {
        const ctx = document.getElementById('trafficChart');
        if (!ctx) return;

        trafficChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'Chủ nhật'],
                datasets: [{
                    label: 'Khách truy cập',
                    data: [1200, 1900, 1500, 2100, 1800, 2400, 2200],
                    borderColor: 'rgb(99, 102, 241)',
                    backgroundColor: 'rgba(99, 102, 241, 0.1)',
                    fill: true,
                    tension: 0.4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });
    }

    // Traffic Source Chart
    function initTrafficSourceChart() {
        const ctx = document.getElementById('trafficSourceChart');
        if (!ctx) return;

        trafficSourceChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Trực tiếp', 'Mạng xã hội', 'Tìm kiếm', 'Giới thiệu'],
                datasets: [{
                    data: [40, 25, 20, 15],
                    backgroundColor: [
                        'rgba(99, 102, 241, 0.8)',
                        'rgba(16, 185, 129, 0.8)',
                        'rgba(245, 158, 11, 0.8)',
                        'rgba(239, 68, 68, 0.8)'
                    ]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });
    }
})();

