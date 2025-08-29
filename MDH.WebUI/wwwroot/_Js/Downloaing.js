function downloadFile(fileName, urlData) {
    const link = document.createElement('a');
    link.href = urlData;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
