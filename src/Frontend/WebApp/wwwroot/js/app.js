// Funções JavaScript chamadas pelos componentes Blazor (IJSRuntime)
window.webApp = {
    // Baixa um texto como arquivo (exportação CSV da tela Executar Querys)
    download: function (fileName, content, contentType) {
        const blob = new Blob([content], { type: contentType });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = fileName;
        document.body.appendChild(link);
        link.click();
        link.remove();
        URL.revokeObjectURL(url);
    },

    // Copia um texto (botão CopyButton). navigator.clipboard só existe em HTTPS e localhost; por HTTP na rede interna,
    // cai no textarea com execCommand
    copy: async function (text) {
        if (navigator.clipboard && window.isSecureContext) {
            await navigator.clipboard.writeText(text);
            return;
        }

        const area = document.createElement('textarea');
        area.value = text;
        area.style.position = 'fixed';
        area.style.opacity = '0';
        document.body.appendChild(area);
        area.select();
        const copied = document.execCommand('copy');
        area.remove();
        if (!copied)
            throw new Error('copy failed');
    }
};
