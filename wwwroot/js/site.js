// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Adnin menu - Sidebar
(function () {
	// Normaliza path para comparación (sin querystring/hash)
	function cleanPath(url) {
		try {
			const u = new URL(url, window.location.origin);
			return u.pathname.replace(/\/+$/, '');
		} catch {
			return url.split(/[?#]/)[0].replace(/\/+$/, '');
		}
	}

	const currentPath = cleanPath(window.location.pathname);
	const links = document.querySelectorAll('.admin-sidebar .admin-link');

	let activeLink = null;
	links.forEach(link => {
		const linkPath = cleanPath(link.getAttribute('href') || '');
		// Marca activo si coincide exactamente o si es prefijo útil (p.ej. /Admin/Usuarios/Editar/1)
		if (linkPath && (currentPath === linkPath || currentPath.startsWith(linkPath + '/'))) {
			activeLink = link;
		}
	});

	if (activeLink) {
		// resaltar link
		activeLink.classList.add('active');
		// resaltar contenedor del item
		const li = activeLink.closest('.list-group-item');
		if (li) li.classList.add('active-wrap');

		// abrir acordeón del módulo correspondiente
		const collapse = activeLink.closest('.accordion-collapse');
		if (collapse && !collapse.classList.contains('show')) {
			// Usa Collapse de Bootstrap para abrirlo
			const bsCollapse = bootstrap.Collapse.getOrCreateInstance(collapse, { toggle: false });
			bsCollapse.show();

			// Asegurar que el botón del header no quede "collapsed"
			const btn = collapse.previousElementSibling?.querySelector('.accordion-button');
			if (btn) btn.classList.remove('collapsed');
		}
	}
})();