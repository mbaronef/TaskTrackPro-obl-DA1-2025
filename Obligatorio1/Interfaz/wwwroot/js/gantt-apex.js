window.dibujarGanttConApex = (selector, tareas) => {
    const series = [
        {
            data: tareas.map(t => ({
                x: t.titulo,
                y: [new Date(t.fechaInicio).getTime(), new Date(t.fechaFin).getTime()],
                fillColor: t.holgura === 0 ? '#FF4560' : '#0cd2fd'
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
        colors: ['#a4f7ff']
    };

    const chart = new ApexCharts(document.querySelector(selector), { series, ...options });
    chart.render();
};