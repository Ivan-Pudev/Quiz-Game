
function filterTable() {
    const search = document.getElementById('searchInput').value.toLowerCase().trim();
    const role = document.getElementById('roleFilter').value.toLowerCase();
    const rows = document.querySelectorAll('#usersTable tbody tr');
    let visible = 0;

    rows.forEach(row => {
        const email = row.dataset.email || '';
        const roles = (row.dataset.roles || '').toLowerCase();
        const matchSearch = !search || email.includes(search);
        const matchRole = !role || roles.includes(role);
        const show = matchSearch && matchRole;
        row.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    const countEl = document.getElementById('resultCount');
    if (countEl) countEl.textContent = `Showing ${visible} user(s)`;
}

