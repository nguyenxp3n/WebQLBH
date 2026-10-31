(() => {
  const COLLAPSE_KEY = "sidebarCollapsed";
  const DESKTOP_BREAKPOINT = 1024;

  const body = document.body;
  const toggleButton = document.querySelector("[data-sidebar-toggle]");
  const mobileButton = document.querySelector("[data-sidebar-mobile-toggle]");
  const overlay = document.querySelector("[data-sidebar-overlay]");

  const isDesktop = () => window.innerWidth >= DESKTOP_BREAKPOINT;

  const setCollapsed = (collapsed) => {
    body.classList.toggle("collapsed", collapsed);
  };

  const setMobileOpen = (open) => {
    body.classList.toggle("mobile-sidebar-open", open);
  };

  const restoreState = () => {
    const saved = localStorage.getItem(COLLAPSE_KEY) === "true";
    if (isDesktop()) {
      setMobileOpen(false);
      setCollapsed(saved);
      return;
    }

    setCollapsed(false);
  };

  restoreState();

  if (toggleButton) {
    toggleButton.addEventListener("click", () => {
      if (isDesktop()) {
        const nextCollapsed = !body.classList.contains("collapsed");
        setCollapsed(nextCollapsed);
        localStorage.setItem(COLLAPSE_KEY, String(nextCollapsed));
        return;
      }

      setMobileOpen(!body.classList.contains("mobile-sidebar-open"));
    });
  }

  if (mobileButton) {
    mobileButton.addEventListener("click", () => {
      setMobileOpen(!body.classList.contains("mobile-sidebar-open"));
    });
  }

  if (overlay) {
    overlay.addEventListener("click", () => {
      setMobileOpen(false);
    });
  }

  window.addEventListener("resize", () => {
    if (isDesktop()) {
      setMobileOpen(false);
      setCollapsed(localStorage.getItem(COLLAPSE_KEY) === "true");
      return;
    }

    setCollapsed(false);
  });

  document.addEventListener("keydown", (event) => {
    if (event.key === "Escape") {
      setMobileOpen(false);
    }
  });
})();
