document.addEventListener("DOMContentLoaded", () => {

    // ---------- helpers ----------
    const fmtVND = (n) => {
        try {
            return (Number(n || 0)).toLocaleString("vi-VN") + " ₫";
        } catch {
            return (n || 0) + " ₫";
        }
    };

    const setText = (id, value) => {
        const el = document.getElementById(id);
        if (el) el.textContent = value ?? "";
    };

    const safeGet = (obj, path, fallback = "") => {
        try {
            return path.split(".").reduce((a, b) => a?.[b], obj) ?? fallback;
        } catch {
            return fallback;
        }
    };
    // ---------- status badge class helper ----------
    function getStatusBadgeClass(status) {
        const s = (status || "").trim().toLowerCase();

        if (s === "pending") return "bg-warning text-dark";
        if (s === "cancelled" || s === "canceled") return "bg-secondary text-white";
        if (s === "processing") return "bg-primary text-white";
        if (s === "shipped") return "bg-info text-dark";   // muốn trắng thì đổi text-white
        if (s === "completed") return "bg-success text-white";

        return "bg-light text-dark";
    }

    // ---------- STATUS LIST (ensure 5 statuses) ----------
    async function loadStatusesIntoSelect() {
        const select = document.getElementById("newStatus");
        if (!select) return;

        // server render đã có rồi
        if (select.options.length > 1) return;

        try {
            const res = await fetch(window.ADMIN_ORDER_STATUS_LIST_URL, { method: "GET" });
            const list = await res.json(); // expected: [{id, name}]

            select.innerHTML = `<option value="">Chọn trạng thái...</option>`;
            (list || []).forEach(s => {
                const opt = document.createElement("option");
                opt.value = s.id;
                opt.textContent = s.name;
                select.appendChild(opt);
            });
        } catch (e) {
            console.error("Load status list failed:", e);
        }
    }

    const changeStatusModal = document.getElementById("changeStatusModal");
    if (changeStatusModal) {
        changeStatusModal.addEventListener("show.bs.modal", async () => {
            await loadStatusesIntoSelect();
        });
    }

    // ---------- ORDER DETAIL (fill modal) ----------
    async function fetchOrderDetail(orderId) {
        // URL đã set ở view: window.ADMIN_ORDER_DETAIL_URL
        // => gọi dạng ?id=...
        const url = `${window.ADMIN_ORDER_DETAIL_URL}?id=${encodeURIComponent(orderId)}`;
        const res = await fetch(url, { method: "GET" });
        return await res.json();
    }

    function renderOrderDetailToModal(detailJson) {
        // Controller của bạn đang trả: { ok: true, data: payload }
        // nhưng cũng có thể có dạng trả thẳng payload -> nên normalize:
        const data = (detailJson && typeof detailJson === "object" && "data" in detailJson)
            ? detailJson.data
            : detailJson;

        // header id
        const orderId = data?.orderId ?? data?.OrderID ?? "";
        setText("viewOrderId", orderId);
        setText("viewOrderId2", orderId);

        // customer info
        setText("viewOrderCustomer", safeGet(data, "customer.name", safeGet(data, "CustomerName", "")));
        setText("viewOrderEmail", safeGet(data, "customer.email", safeGet(data, "Email", "")));
        setText("viewOrderPhone", safeGet(data, "customer.phone", safeGet(data, "Phone", "")));
        setText("viewOrderAddress", safeGet(data, "customer.address", safeGet(data, "Address", "")));

        // order info
        setText("viewOrderDate", safeGet(data, "date", safeGet(data, "OrderDate", "")));
        setText("viewOrderStatus", safeGet(data, "status", safeGet(data, "Status", "")));

        // items
        const tbody = document.getElementById("viewOrderItems");
        if (tbody) {
            tbody.innerHTML = "";

            const items = data?.items ?? data?.Items ?? [];
            if (items && items.length) {
                items.forEach(it => {
                    const name = it.productName ?? it.ProductName ?? "Sản phẩm";
                    const qty = Number(it.quantity ?? it.Quantity ?? 0);
                    const unit = Number(it.unitPrice ?? it.UnitPrice ?? 0);
                    const total = Number(it.total ?? it.Total ?? (qty * unit));

                    const tr = document.createElement("tr");
                    tr.innerHTML = `
            <td>${name}</td>
            <td>${qty}</td>
            <td>${fmtVND(unit)}</td>
            <td>${fmtVND(total)}</td>
          `;
                    tbody.appendChild(tr);
                });
            } else {
                const tr = document.createElement("tr");
                tr.innerHTML = `<td colspan="4" class="text-center text-muted py-3">Không có sản phẩm.</td>`;
                tbody.appendChild(tr);
            }
        }

        // totals
        const subtotal = Number(data?.subtotal ?? data?.SubTotal ?? 0);
        const shipping = Number(data?.shippingFee ?? data?.ShippingFee ?? 0);
        const grand = Number(data?.grandTotal ?? data?.GrandTotal ?? (subtotal + shipping));

        setText("viewOrderSubtotal", fmtVND(subtotal));
        setText("viewOrderShipping", fmtVND(shipping));
        setText("viewOrderTotal", fmtVND(grand));
    }

    // Click "view order" button
    document.addEventListener("click", async (e) => {
        const btn = e.target.closest(".view-order");
        if (!btn) return;

        const id = btn.getAttribute("data-id");
        if (!id) return;

        try {
            const json = await fetchOrderDetail(id);

            // nếu controller trả {ok:false,...}
            if (json && json.ok === false) {
                console.error("Detail error:", json);
                return;
            }

            renderOrderDetailToModal(json);
        } catch (err) {
            console.error("Fetch order detail failed:", err);
        }
    });

    // ---------- CHANGE STATUS (fill modal current values) ----------
    document.addEventListener("click", (e) => {
        const btn = e.target.closest(".change-status");
        if (!btn) return;

        const id = btn.getAttribute("data-id") || "";
        const currentStatus = btn.getAttribute("data-status") || "";

        const hiddenId = document.getElementById("changeStatusOrderId");
        const current = document.getElementById("currentStatusDisplay");

        if (hiddenId) hiddenId.value = id;
        if (current) current.value = currentStatus;
    });

    // Submit update status
    const changeBtn = document.getElementById("changeStatusBtn");
    if (changeBtn) {
        changeBtn.addEventListener("click", async () => {
            const orderId = document.getElementById("changeStatusOrderId")?.value;
            const statusId = document.getElementById("newStatus")?.value;
            const note = document.getElementById("statusNote")?.value || "";

            if (!orderId || !statusId) return;

            try {
                const form = new URLSearchParams();
                form.append("orderId", orderId);
                form.append("statusId", statusId);
                form.append("note", note);

                const res = await fetch(window.ADMIN_ORDER_UPDATE_STATUS_URL, {
                    method: "POST",
                    headers: { "Content-Type": "application/x-www-form-urlencoded; charset=UTF-8" },
                    body: form.toString()
                });

                const json = await res.json();

                if (json && json.ok) {
                    // update badge in table
                    const row = document.querySelector(`tr[data-order-id="${orderId}"]`);
                    if (row) {
                        const badge = row.querySelector(".order-status");
                        if (badge) {
                            const newStatus = json.status || "";
                            badge.textContent = newStatus;

                            // reset class + set màu theo status
                            badge.className = "badge order-status " + getStatusBadgeClass(newStatus);
                        }
                        const changeBtn = row.querySelector(".change-status");
                        if (changeBtn) changeBtn.setAttribute("data-status", json.status || "");
                    }

                    // close modal
                    const modalEl = document.getElementById("changeStatusModal");
                    if (modalEl && window.bootstrap) {
                        const modal = window.bootstrap.Modal.getInstance(modalEl);
                        if (modal) modal.hide();
                    }
                } else {
                    console.error("UpdateStatus failed:", json);
                }
            } catch (err) {
                console.error("UpdateStatus error:", err);
            }
        });
    }
    // ================== FILTER + QUICK FILTER ==================
    (function () {
        const btnApply = document.getElementById("applyOrderFiltersBtn");
        const btnClear = document.getElementById("clearOrderFiltersBtn");

        const elStatus = document.getElementById("filterOrderStatus");
        const elFromDate = document.getElementById("filterOrderDateFrom");
        const elToDate = document.getElementById("filterOrderDateTo");
        const elMin = document.getElementById("filterOrderAmountFrom");
        const elMax = document.getElementById("filterOrderAmountTo");

        function buildUrl(options) {
            const url = new URL(window.location.href);

            // giữ các param khác nếu có, chỉ xóa param filter
            ["statusId", "fromDate", "toDate", "minTotal", "maxTotal"].forEach(k => url.searchParams.delete(k));

            if (options && options.statusId) url.searchParams.set("statusId", options.statusId);
            if (options && options.fromDate) url.searchParams.set("fromDate", options.fromDate);
            if (options && options.toDate) url.searchParams.set("toDate", options.toDate);
            if (options && options.minTotal) url.searchParams.set("minTotal", options.minTotal);
            if (options && options.maxTotal) url.searchParams.set("maxTotal", options.maxTotal);

            // ✅ đổi filter thì quay về trang 1
            url.searchParams.set("page", "1");
            return url.toString();
        }

        // Apply filters (modal)
        if (btnApply) {
            btnApply.addEventListener("click", () => {
                const statusId = (elStatus?.value || "").trim();
                const fromDate = (elFromDate?.value || "").trim(); // yyyy-MM-dd
                const toDate = (elToDate?.value || "").trim();
                const minTotal = (elMin?.value || "").trim();
                const maxTotal = (elMax?.value || "").trim();

                window.location.href = buildUrl({
                    statusId: statusId || "",
                    fromDate: fromDate || "",
                    toDate: toDate || "",
                    minTotal: minTotal || "",
                    maxTotal: maxTotal || ""
                });
            });
        }

        // Clear filters
        if (btnClear) {
            btnClear.addEventListener("click", () => {
                if (elStatus) elStatus.value = "";
                if (elFromDate) elFromDate.value = "";
                if (elToDate) elToDate.value = "";
                if (elMin) elMin.value = "";
                if (elMax) elMax.value = "";

                window.location.href = buildUrl(null);
            });
        }

        // Quick status buttons (Tất cả/Đang chờ/Hoàn thành)
        document.querySelectorAll(".js-quick-filter").forEach(btn => {
            btn.addEventListener("click", (e) => {
                e.preventDefault();

                const sid = (btn.getAttribute("data-status-id") || "").trim();

                const url = new URL(window.location.href);

                if (sid && sid !== "0") url.searchParams.set("statusId", sid);
                else url.searchParams.delete("statusId");
                // ✅ đổi status thì quay về trang 1
                url.searchParams.set("page", "1");

                // bỏ hash để khỏi hiện "#"
                url.hash = "";

                window.location.href = url.toString();
            });
        });


        // Highlight active quick button theo statusId trên URL
        const cur = new URLSearchParams(window.location.search).get("statusId") || "";
        document.querySelectorAll(".js-quick-filter").forEach(btn => {
            const sid = (btn.getAttribute("data-status-id") || "").trim();
            const isAll = !cur;
            const isMatch = cur && sid === cur;

            btn.classList.remove("active");
            if ((isAll && !sid) || isMatch) btn.classList.add("active");
        });

        // Optional: Load current filter params -> set lại lên UI modal
        const sp = new URLSearchParams(window.location.search);
        if (elStatus && sp.get("statusId")) elStatus.value = sp.get("statusId");
        if (elFromDate && sp.get("fromDate")) elFromDate.value = sp.get("fromDate");
        if (elToDate && sp.get("toDate")) elToDate.value = sp.get("toDate");
        if (elMin && sp.get("minTotal")) elMin.value = sp.get("minTotal");
        if (elMax && sp.get("maxTotal")) elMax.value = sp.get("maxTotal");
    })();
});
