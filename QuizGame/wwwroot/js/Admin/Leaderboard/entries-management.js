
/* ── Table search ── */
function filterTable() {
    const search = document.getElementById('searchInput').value.toLowerCase().trim();
    const rows = document.querySelectorAll('#entriesTable tbody tr');
    let visible = 0;

    rows.forEach(row => {
        const email = row.dataset.email || '';
        const show = !search || email.includes(search);
        row.style.display = show ? '' : 'none';
        if (show) visible++;
    });

    const countEl = document.getElementById('resultCount');
    if (countEl) countEl.textContent = `Showing ${visible} entr(ies)`;
}

/* ── Edit mode ── */
function openEdit(id, score, rank) {
    document.getElementById('entryId').value = id;
    document.getElementById('scoreInput').value = score;
    document.getElementById('rankInput').value = rank;

    // Switch form to edit action
    document.getElementById('entryForm').action = '@Url.Action("EditEntry", "Leaderboard", new {area = "Admin"})';

    document.getElementById('formCardTitle').textContent = 'Edit entry';
    document.getElementById('submitLabel').textContent = 'Save changes';
    document.getElementById('cancelEditBtn').style.display = '';
    document.getElementById('userFieldWrap').style.display = 'none';

    document.getElementById('addCard').scrollIntoView({ behavior: 'smooth', block: 'start' });
}

function resetForm() {
    document.getElementById('entryId').value = '';
    document.getElementById('scoreInput').value = '';
    document.getElementById('rankInput').value = '';
    document.getElementById('userSelect').value = '';

    document.getElementById('entryForm').action = '@Url.Action("AddEntry", "Leaderboard", new {area = "Admin"})';

    document.getElementById('formCardTitle').textContent = 'Add entry';
    document.getElementById('submitLabel').textContent = 'Add entry';
    document.getElementById('cancelEditBtn').style.display = 'none';
    document.getElementById('userFieldWrap').style.display = '';
}

function promptForScore(form, userName, currentScore) {
    const input = prompt(`Enter new score for ${userName} (current: ${currentScore})`);
    if (input === null) return false;
    const score = parseInt(input);
    if (isNaN(score)) { alert("Invalid score."); return false; }
    form.querySelector('[name="newScore"]').value = score;
    return true;
}
