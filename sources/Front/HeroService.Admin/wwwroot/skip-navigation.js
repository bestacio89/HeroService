document.addEventListener("click", event => {
    if (!(event.target instanceof Element)) {
        return;
    }

    const skipLink = event.target.closest('a.skip-link[href="#main-content"]');

    if (!skipLink) {
        return;
    }

    const mainContent = document.getElementById("main-content");

    if (!(mainContent instanceof HTMLElement)) {
        return;
    }

    window.requestAnimationFrame(() => {
        mainContent.focus({ preventScroll: true });
    });
}, true);
