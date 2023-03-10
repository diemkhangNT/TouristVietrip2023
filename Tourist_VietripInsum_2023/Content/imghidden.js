ImagerTour.onchange = evt => {
    const [file] = ImagerTour.files
    if (file) {
        preview.src = URL.createObjectURL(file);
        document.getElementById("preview").style.visibility = "visible";
    }
}