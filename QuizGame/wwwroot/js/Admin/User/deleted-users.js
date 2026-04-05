
function filterTable() {
    const search = document.getElementById('searchInput').value.toLowerCase().trim();
    const rows = document.querySelectorAll('#bannedTable tbody tr');
    let visible = 0;

    rows.forEach(row => {
        const email = row.dataset.email || '';
        const show = !search || email.includes(search);
        row.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    const countEl = document.getElementById('resultCount');
    if (countEl) countEl.textContent = `Showing ${visible} banned account(s)`;
}
