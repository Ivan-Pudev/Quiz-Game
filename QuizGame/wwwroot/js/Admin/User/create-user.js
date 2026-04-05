function toggleChip(checkbox, activeClass) {
    const chip = checkbox.closest('.role-chip');
    if (checkbox.checked) {
        chip.classList.add(activeClass);
    } else {
        chip.classList.remove(activeClass);
    }
}

