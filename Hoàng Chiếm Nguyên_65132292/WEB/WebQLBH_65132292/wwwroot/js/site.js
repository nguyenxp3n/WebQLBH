(function () {
	function normalizeText(value) {
		return (value || "")
			.toString()
			.toLowerCase()
			.normalize("NFD")
			.replace(/[\u0300-\u036f]/g, "")
			.trim();
	}

	function initInventorySearch(page) {
		var searchInput = page.querySelector(".inventory-search");
		var rows = Array.prototype.slice.call(page.querySelectorAll("tbody tr[data-search-row]"));
		var currentCountElement = page.querySelector("[data-inventory-current]");
		var totalCountElement = page.querySelector("[data-inventory-total]");
		var emptyRow = page.querySelector("[data-empty-row]");
		var emptyMessage = page.querySelector("[data-empty-message]");

		if (!rows.length) {
			return;
		}

		var total = rows.length;
		if (totalCountElement) {
			totalCountElement.textContent = String(total);
		}

		function applyFilter() {
			var query = normalizeText(searchInput ? searchInput.value : "");
			var rawQuery = searchInput ? searchInput.value.trim() : "";
			var visible = 0;

			rows.forEach(function (row) {
				var rowText = normalizeText(row.getAttribute("data-search-row") || row.textContent);
				var isMatch = !query || rowText.indexOf(query) !== -1;
				row.hidden = !isMatch;
				if (isMatch) {
					visible += 1;
				}
			});

			if (currentCountElement) {
				currentCountElement.textContent = String(visible);
			}

			if (emptyRow) {
				emptyRow.hidden = visible !== 0;
			}

			if (emptyMessage && visible === 0) {
				emptyMessage.textContent = rawQuery
					? "Không tìm thấy kết quả cho \"" + rawQuery + "\"."
					: "Không có dữ liệu để hiển thị.";
			}
		}

		if (searchInput) {
			searchInput.addEventListener("input", applyFilter);
			searchInput.addEventListener("search", applyFilter);
		}

		applyFilter();
	}

	function setActiveNavLink() {
		var currentPath = window.location.pathname.toLowerCase();
		var navLinks = document.querySelectorAll(".app-navbar .nav-link");

		navLinks.forEach(function (link) {
			var href = (link.getAttribute("href") || "").toLowerCase();
			if (!href) {
				return;
			}

			var isActive = href !== "/" && currentPath.startsWith(href);
			link.classList.toggle("active", isActive);
			if (isActive) {
				link.setAttribute("aria-current", "page");
			} else {
				link.removeAttribute("aria-current");
			}
		});
	}

	document.addEventListener("DOMContentLoaded", function () {
		var pages = document.querySelectorAll(".inventory-page");
		pages.forEach(initInventorySearch);
		setActiveNavLink();
	});
})();
