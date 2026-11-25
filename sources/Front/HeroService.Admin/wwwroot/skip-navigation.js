document.addEventListener(
    "click",
    event => {
        const eventTarget = event.target;

        if (!(eventTarget instanceof Element)) {
            return;
        }

        const skipLink = eventTarget.closest("a.skip-link");

        if (!(skipLink instanceof HTMLAnchorElement)) {
            return;
        }

        const targetHash = new URL(skipLink.href, window.location.href).hash;

        if (targetHash !== "#main-content") {
            return;
        }

        const mainContent = document.getElementById("main-content");

        if (!(mainContent instanceof HTMLElement)) {
            return;
        }

        event.preventDefault();

        if (window.location.hash !== targetHash) {
            window.history.pushState(null, "", targetHash);
        }

        mainContent.focus();
    },
    true
);
