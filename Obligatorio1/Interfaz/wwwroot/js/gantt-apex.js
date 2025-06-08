window.dibujarGanttConApex = (selector, tareas) => {
    const series = [
        {
            data: tareas.map(t => ({
                x: t.titulo,
                y: [new Date(t.fechaInicio).getTime(), new Date(t.fechaFin).getTime()],
                fillColor: t.holgura === 0 ? '#FF4560' : '#0cd2fd',
                customData: {
                    titulo: t.titulo,
                    fechaInicio: t.fechaInicio,
                    fechaFinReal: t.fechaFinReal,
                    holgura: t.holgura
                }
            }))
        }
    ];

    const options = {
        chart: {
            type: 'rangeBar',
            height: 450
        },
        plotOptions: {
            bar: {
                horizontal: true,
                rangeBarGroupRows: true
            }
        },
        xaxis: {
            type: 'datetime'
        },
        fill: {
            type: 'solid'
        },
        colors: ['#a4f7ff'],
        tooltip: {
            enabled: true,
            custom: function ({series, seriesIndex, dataPointIndex, w}) {
                const data = w.config.series[seriesIndex].data[dataPointIndex].customData;
                const formatFecha = (fechaStr) => {
                    const [aaaa, mm, dd] = fechaStr.split('-').map(Number);
                    const fecha = new Date(aaaa, mm - 1, dd);
                    const dia = String(fecha.getDate()).padStart(2, '0');
                    const mes = String(fecha.getMonth() + 1).padStart(2, '0');
                    const año = fecha.getFullYear();
                    return `${dia}/${mes}/${año}`;
                };
                return `<div style="padding: 6px;">
                    <strong>${data.titulo}</strong><br/>
                    ${formatFecha(data.fechaInicio)} - ${formatFecha(data.fechaFinReal)}<br/>
                    Holgura: ${data.holgura}
                </div>`;
            }
        }
    };

    const chart = new ApexCharts(document.querySelector(selector), {series, ...options});
    chart.render();
};