// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

(function () {
    const toggle = document.querySelector('.library-nav-toggle');
    const nav = document.getElementById('primary-nav');
    if (!toggle || !nav) return;

    toggle.addEventListener('click', function () {
        const isOpen = nav.classList.toggle('is-open');
        toggle.setAttribute('aria-expanded', String(isOpen));
    });

    nav.addEventListener('click', function (event) {
        if (event.target.closest('a, button') && nav.classList.contains('is-open')) {
            nav.classList.remove('is-open');
            toggle.setAttribute('aria-expanded', 'false');
        }
    });
})();
