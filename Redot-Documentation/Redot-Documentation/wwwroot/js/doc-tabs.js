(() => {
    function activateTab(container, buttonToActivate) {
        const buttons = container.querySelectorAll('.doc-tab-button');
        const panels = container.querySelectorAll('.doc-tab-panel');
        const targetSelector = buttonToActivate.getAttribute('data-tab-target');

        buttons.forEach(button => {
            const isActive = button === buttonToActivate;
            button.classList.toggle('active', isActive);
            button.setAttribute('aria-selected', isActive ? 'true' : 'false');
        });

        panels.forEach(panel => {
            const isActive = targetSelector !== null && `#${panel.id}` === targetSelector;
            panel.classList.toggle('active', isActive);
        });
    }

    document.addEventListener('click', event => {
        const button = event.target.closest('.doc-tab-button');
        if (!button) {
            return;
        }

        const tabsContainer = button.closest('.doc-tabs');
        if (!tabsContainer) {
            return;
        }

        activateTab(tabsContainer, button);
    });
})();