export function focusMainContent() {
    const mainContent = document.getElementById("main-content");

    if (!(mainContent instanceof HTMLElement)) {
        return;
    }

    if (window.location.hash !== "#main-content") {
        window.location.hash = "main-content";
    }

    mainContent.focus({ preventScroll: true });
}
