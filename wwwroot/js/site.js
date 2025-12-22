// ================== ANIMATION CHO CATEGORY + ARRIVAL ==================
document.addEventListener('DOMContentLoaded', function () {
    const options = { threshold: 0.2 };

    function makeObserver(selector, visibleClass) {
        const elements = document.querySelectorAll(selector);
        if (!elements.length) return;

        const observer = new IntersectionObserver((entries, obs) => {
            entries.forEach(entry => {
                if (!entry.isIntersecting) return;

                entry.target.classList.add(visibleClass);
                obs.unobserve(entry.target); // chỉ chạy 1 lần
            });
        }, options);

        elements.forEach(el => observer.observe(el));
    }

    // DANH MỤC SẢN PHẨM
    makeObserver('.cat-animate-left', 'cat-visible-left');
    makeObserver('.cat-animate-up', 'cat-visible-up');

    // NEWEST ARRIVAL
    makeObserver('.arr-animate-left', 'arr-visible-left');
    makeObserver('.arr-animate-right', 'arr-visible-right');

    // COLLECTION SLIDER
    initCollectionSlider();
});

// ================= SLIDER COLLECTION - INFINITE LOOP =================
document.addEventListener('DOMContentLoaded', function () {
    const slider = document.querySelector('.collection-slider');
    if (!slider) return;

    const track = slider.querySelector('.col-track');
    const slides = slider.querySelectorAll('.col-slide');
    const btnPrev = slider.querySelector('.col-prev');
    const btnNext = slider.querySelector('.col-next');

    const visibleSlides = 4;
    const totalSlides = slides.length;

    // Clone slide để tạo vòng lặp
    for (let i = 0; i < visibleSlides; i++) {
        const clone = slides[i].cloneNode(true);
        track.appendChild(clone);
    }

    let index = 0;
    let autoplayId = null;
    let isTransitioning = false;

    function updateSlider(withTransition = true) {
        if (withTransition) {
            track.style.transition = "transform 0.5s ease";
        } else {
            track.style.transition = "none";
        }

        const translatePercent = -(100 / visibleSlides) * index;
        track.style.transform = `translateX(${translatePercent}%)`;
    }

    function nextSlide() {
        if (isTransitioning) return;
        isTransitioning = true;

        index++;
        updateSlider(true);

        // Khi chạy đến clone → nhảy về thật mà không bị giật
        if (index === totalSlides) {
            setTimeout(() => {
                index = 0;
                updateSlider(false);
            }, 510);
        }

        setTimeout(() => isTransitioning = false, 520);
    }

    function prevSlide() {
        if (isTransitioning) return;
        isTransitioning = true;

        if (index === 0) {
            index = totalSlides;
            updateSlider(false);
        }

        setTimeout(() => {
            index--;
            updateSlider(true);
        }, 20);

        setTimeout(() => isTransitioning = false, 520);
    }

    btnNext.addEventListener('click', () => {
        nextSlide();
        resetAutoplay();
    });

    btnPrev.addEventListener('click', () => {
        prevSlide();
        resetAutoplay();
    });

    function startAutoplay() {
        autoplayId = setInterval(nextSlide, 2000);
    }

    function resetAutoplay() {
        clearInterval(autoplayId);
        startAutoplay();
    }

    slider.addEventListener('mouseenter', () => clearInterval(autoplayId));
    slider.addEventListener('mouseleave', startAutoplay);

    updateSlider(false);
    startAutoplay();
});

// BEST SELLER
document.addEventListener("DOMContentLoaded", function () {
    initBestSellerSlider();
});

function initBestSellerSlider() {
    const viewport = document.querySelector(".gl-best-viewport");
    const track = document.querySelector(".gl-best-track");
    const prevBtn = document.querySelector(".gl-best-prev");
    const nextBtn = document.querySelector(".gl-best-next");

    // nếu thiếu phần tử nào thì thôi, khỏi chạy
    if (!viewport || !track || !prevBtn || !nextBtn) return;

    // lấy danh sách item gốc
    const originalItems = Array.from(track.children);
    const originalCount = originalItems.length;
    if (!originalCount) return;

    // tính kích thước 1 item + gap
    const style = getComputedStyle(track);
    const gap = parseFloat(style.columnGap || style.gap || "0");
    const firstRect = originalItems[0].getBoundingClientRect();
    const itemWidth = firstRect.width + gap;

    // số item hiển thị trong viewport (từ 1 đến originalCount)
    const visibleCount = Math.max(
        1,
        Math.min(originalCount, Math.floor(viewport.clientWidth / itemWidth) || 1)
    );

    // xoá track, clone đầu/cuối để tạo loop
    track.innerHTML = "";

    const leadingClones = originalItems
        .slice(-visibleCount)
        .map(n => n.cloneNode(true));
    const trailingClones = originalItems
        .slice(0, visibleCount)
        .map(n => n.cloneNode(true));

    const allItems = [...leadingClones, ...originalItems, ...trailingClones];
    allItems.forEach(n => track.appendChild(n));

    let currentIndex = visibleCount;                    // bắt đầu ở item thật đầu tiên
    const firstRealIndex = visibleCount;
    const lastRealIndex = visibleCount + originalCount - 1;

    function applyTransform(noTransition = false) {
        if (noTransition) {
            track.style.transition = "none";
        } else {
            track.style.transition = "transform 0.45s ease";
        }

        const offset = -currentIndex * itemWidth;
        track.style.transform = `translateX(${offset}px)`;
    }

    // đặt vị trí ban đầu (không animation)
    applyTransform(true);
    void track.offsetWidth;                             // force reflow
    track.style.transition = "transform 0.45s ease";

    function next() {
        currentIndex++;
        applyTransform();

        track.addEventListener("transitionend", function handleNext() {
            if (currentIndex > lastRealIndex) {
                // đã trượt sang vùng clone sau → nhảy kín về vùng thật
                currentIndex -= originalCount;
                applyTransform(true);
                void track.offsetWidth;
                track.style.transition = "transform 0.45s ease";
            }
            track.removeEventListener("transitionend", handleNext);
        });
    }

    function prev() {
        currentIndex--;
        applyTransform();

        track.addEventListener("transitionend", function handlePrev() {
            if (currentIndex < firstRealIndex) {
                // đã trượt sang vùng clone trước → nhảy kín về vùng thật
                currentIndex += originalCount;
                applyTransform(true);
                void track.offsetWidth;
                track.style.transition = "transform 0.45s ease";
            }
            track.removeEventListener("transitionend", handlePrev);
        });
    }
    // gán sự kiện nút
    prevBtn.addEventListener("click", prev);
    nextBtn.addEventListener("click", next);
}

document.addEventListener("DOMContentLoaded", () => {
    const whyItems = document.querySelectorAll(".fade-why");
    if (!whyItems.length) return;

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add("visible");
                observer.unobserve(entry.target); // chỉ chạy 1 lần
            }
        });
    }, { threshold: 0.2 });

    whyItems.forEach(item => observer.observe(item));
});

document.addEventListener("DOMContentLoaded", () => {
    const track = document.querySelector(".gl-review-track");
    const viewport = document.querySelector(".gl-review-viewport");
    const prevBtn = document.querySelector(".gl-review-prev");
    const nextBtn = document.querySelector(".gl-review-next");

    if (!track || !viewport || !prevBtn || !nextBtn) {
        console.warn("Review slider missing elements", { track, viewport, prevBtn, nextBtn });
        return;
    }

    const original = Array.from(track.children);
    const count = original.length;
    if (!count) return;

    const gap = parseFloat(getComputedStyle(track).gap || "0");
    const cardW = original[0].getBoundingClientRect().width + gap;
    const visible = Math.max(1, Math.floor(viewport.clientWidth / cardW));

    const head = original.slice(0, visible).map(n => n.cloneNode(true));
    const tail = original.slice(-visible).map(n => n.cloneNode(true));

    track.innerHTML = "";
    [...tail, ...original, ...head].forEach(n => track.appendChild(n));

    let index = visible;
    const firstReal = visible;
    const lastReal = visible + count - 1;
    let lock = false;

    function setX(noAnim = false) {
        track.style.transition = noAnim ? "none" : "transform .5s ease";
        track.style.transform = `translateX(${-index * cardW}px)`;
    }

    function next() {
        if (lock) return;
        lock = true;

        index++;
        setX(false);

        track.addEventListener("transitionend", function h() {
            if (index > lastReal) {
                index -= count;
                setX(true);
                void track.offsetWidth;
                setX(false);
            }
            lock = false;
            track.removeEventListener("transitionend", h);
        });
    }

    function prev() {
        if (lock) return;
        lock = true;

        index--;
        setX(false);

        track.addEventListener("transitionend", function h() {
            if (index < firstReal) {
                index += count;
                setX(true);
                void track.offsetWidth;
                setX(false);
            }
            lock = false;
            track.removeEventListener("transitionend", h);
        });
    }

    prevBtn.addEventListener("click", prev);
    nextBtn.addEventListener("click", next);

    setX(true);
    void track.offsetWidth;
    setX(false);
});

document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById("glSearchInlineInput");
    const clearBtn = document.getElementById("glSearchInlineClear");
    const form = document.getElementById("glSearchInlineForm");

    if (!input || !clearBtn || !form) return;

    const updateClear = () => {
        if (input.value && input.value.trim().length > 0) clearBtn.classList.remove("is-hidden");
        else clearBtn.classList.add("is-hidden");
    };

    updateClear();
    input.addEventListener("input", updateClear);

    clearBtn.addEventListener("click", () => {
        input.value = "";
        updateClear();

        // Nếu đang ở /Shop có q -> bấm X sẽ bỏ lọc và reload về /Shop (giữ các filter khác nếu bạn muốn thì nói mình)
        const url = new URL(window.location.href);
        url.searchParams.delete("q");
        url.searchParams.delete("page"); // xoá page để về trang 1 cho hợp lý
        window.location.href = url.pathname + url.search;
    });

    // Enter submit bình thường -> /Shop?q=...
    form.addEventListener("submit", (e) => {
        if (!input.value.trim()) {
            e.preventDefault();
            input.focus();
        }
    });
});

