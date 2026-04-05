
function filterTable() {
    const search = document.getElementById('searchInput').value.toLowerCase().trim();
    const rows = document.querySelectorAll('#lbTable tbody tr');
    let visible = 0;

    rows.forEach(row => {
        const title = row.dataset.title || '';
        const id = row.dataset.id || '';
        const quiz = row.dataset.quiz || '';
        const show = !search || title.includes(search) || id.includes(search) || quiz.includes(search);
        row.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    const countEl = document.getElementById('resultCount');
    if (countEl) countEl.textContent = `Showing ${visible} leaderboard(s)`;
}
