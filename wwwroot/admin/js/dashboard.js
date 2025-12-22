// Dashboard JavaScript
(function() {
    'use strict';

    let revenueChart, userGrowthChart, orderStatusChart, storageChart;

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        if (typeof Chart !== 'undefined') {
            initRevenueChart();
            initUserGrowthChart();
            initOrderStatusChart();
        }
        
        if (typeof ApexCharts !== 'undefined') {
            initStorageChart();
        }

        initChartPeriodButtons();
    });

    // Revenue Chart
    function initRevenueChart() {
        const ctx = document.getElementById('revenueChart');
        if (!ctx) return;

        const months = ['Tháng 1', 'Tháng 2', 'Tháng 3', 'Tháng 4', 'Tháng 5', 'Tháng 6', 
                       'Tháng 7', 'Tháng 8', 'Tháng 9', 'Tháng 10', 'Tháng 11', 'Tháng 12'];
        
        const revenueData = [45000, 52000, 48000, 61000, 55000, 67000, 63000, 72000, 68000, 75000, 71000, 80000];
        const profitData = [18000, 21000, 19000, 24000, 22000, 27000, 25000, 29000, 27000, 30000, 28000, 32000];

        revenueChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: months,
                datasets: [
                    {
                        label: 'Doanh thu',
                        data: revenueData,
                        borderColor: 'rgb(99, 102, 241)',
                        backgroundColor: 'rgba(99, 102, 241, 0.1)',
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: 'rgb(99, 102, 241)',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2,
                        pointRadius: 6,
                        pointHoverRadius: 8
                    },
                    {
                        label: 'Lợi nhuận',
                        data: profitData,
                        borderColor: 'rgb(16, 185, 129)',
                        backgroundColor: 'rgba(16, 185, 129, 0.1)',
                        fill: true,
                        tension: 0.4,
                        pointBackgroundColor: 'rgb(16, 185, 129)',
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2,
                        pointRadius: 6,
                        pointHoverRadius: 8
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                interaction: {
                    intersect: false,
                    mode: 'index'
                },
                plugins: {
                    legend: {
                        position: 'top',
                        labels: {
                            usePointStyle: true,
                            padding: 20
                        }
                    },
                    tooltip: {
                        backgroundColor: 'rgba(0, 0, 0, 0.8)',
                        titleColor: '#fff',
                        bodyColor: '#fff',
                        borderColor: 'rgba(255, 255, 255, 0.1)',
                        borderWidth: 1,
                        cornerRadius: 8,
                        callbacks: {
                            label: function(context) {
                                return context.dataset.label + ': ₫' + context.parsed.y.toLocaleString('vi-VN');
                            }
                        }
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false
                        },
                        border: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.1)'
                        },
                        border: {
                            display: false
                        },
                        ticks: {
                            callback: function(value) {
                                return '₫' + value.toLocaleString('vi-VN');
                            }
                        }
                    }
                }
            }
        });
    }

    // User Growth Chart
    function initUserGrowthChart() {
        const ctx = document.getElementById('userGrowthChart');
        if (!ctx) return;

        const days = ['Ngày 1', 'Ngày 2', 'Ngày 3', 'Ngày 4', 'Ngày 5', 'Ngày 6', 'Ngày 7'];
        const newUsers = [45, 52, 48, 61, 55, 67, 63];

        userGrowthChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: days,
                datasets: [
                    {
                        label: 'Người dùng mới',
                        data: newUsers,
                        backgroundColor: 'rgba(99, 102, 241, 0.8)',
                        borderColor: 'rgb(99, 102, 241)',
                        borderWidth: 1,
                        borderRadius: 6,
                        borderSkipped: false
                    }
                ]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        display: false
                    }
                },
                scales: {
                    x: {
                        grid: {
                            display: false
                        }
                    },
                    y: {
                        beginAtZero: true,
                        grid: {
                            color: 'rgba(0, 0, 0, 0.1)'
                        }
                    }
                }
            }
        });
    }

    // Order Status Chart
    function initOrderStatusChart() {
        const ctx = document.getElementById('orderStatusChart');
        if (!ctx) return;

        orderStatusChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Hoàn thành', 'Đang xử lý', 'Đang chờ', 'Đã hủy'],
                datasets: [{
                    data: [1245, 156, 87, 23],
                    backgroundColor: [
                        'rgba(16, 185, 129, 0.8)',
                        'rgba(99, 102, 241, 0.8)',
                        'rgba(245, 158, 11, 0.8)',
                        'rgba(239, 68, 68, 0.8)'
                    ],
                    borderWidth: 0,
                    cutout: '60%'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: {
                        position: 'bottom',
                        labels: {
                            padding: 20,
                            usePointStyle: true
                        }
                    }
                }
            }
        });
    }

    // Storage Chart (ApexCharts)
    function initStorageChart() {
        const chartElement = document.getElementById('storageStatusChart');
        if (!chartElement) return;

        const options = {
            chart: {
                height: 280,
                type: "radialBar",
            },
            series: [76],
            colors: ["#20E647"],
            plotOptions: {
                radialBar: {
                    hollow: {
                        margin: 0,
                        size: "70%",
                        background: "#293450"
                    },
                    track: {
                        dropShadow: {
                            enabled: true,
                            top: 2,
                            left: 0,
                            blur: 4,
                            opacity: 0.15
                        }
                    },
                    dataLabels: {
                        name: {
                            offsetY: -10,
                            color: "#333",
                            fontSize: "13px"
                        },
                        value: {
                            color: "#333",
                            fontSize: "30px",
                            show: true
                        }
                    }
                }
            },
            fill: {
                type: "gradient",
                gradient: {
                    shade: "dark",
                    type: "vertical",
                    gradientToColors: ["#87D4F9"],
                    stops: [0, 100]
                }
            },
            stroke: {
                lineCap: "round"
            },
            labels: ["Dung lượng đã dùng"]
        };

        storageChart = new ApexCharts(chartElement, options);
        storageChart.render();
    }

    // Chart Period Buttons
    function initChartPeriodButtons() {
        const periodButtons = document.querySelectorAll('[data-chart-period]');
        
        periodButtons.forEach(function(button) {
            button.addEventListener('click', function() {
                // Remove active class from all buttons
                periodButtons.forEach(function(btn) {
                    btn.classList.remove('active');
                });
                
                // Add active class to clicked button
                button.classList.add('active');
                
                const period = button.getAttribute('data-chart-period');
                updateChartPeriod(period);
            });
        });
    }

    function updateChartPeriod(period) {
        // This would typically fetch new data based on the period
        console.log('Updating chart for period:', period);
        // For now, we'll just log it. In a real app, you'd fetch new data and update the charts.
    }
})();

