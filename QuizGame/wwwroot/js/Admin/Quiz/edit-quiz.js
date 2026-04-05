function updateCount() {
    const checked = document.querySelectorAll(".question-checkbox:checked").length;
    document.getElementById("selectedCount").innerText = `${checked} selected`;
}
document.addEventListener("change", (e) => {
    if (e.target.classList.contains("question-checkbox")) updateCount();
});
updateCount();
