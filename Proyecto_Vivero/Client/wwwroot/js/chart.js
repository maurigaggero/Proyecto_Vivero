google.charts.load('current', { packages: ['corechart', 'line'] });
google.charts.setOnLoadCallback(drawLineColors);

function chart_line(ejercicio) {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Mes');
    data.addColumn('number', 'Ingresos');

    data.addRows([
        ['Enero', ejercicio[0]],
        ['Febrero', ejercicio[1]],
        ['Marzo', ejercicio[2]],
        ['Abril', ejercicio[3]],
        ['Mayo', ejercicio[4]],
        ['Junio', ejercicio[5]],
        ['Julio', ejercicio[6]],
        ['Agosto', ejercicio[7]],
        ['Septiembre', ejercicio[8]],
        ['Octubre', ejercicio[9]],
        ['Noviembre', ejercicio[10]],
        ['Diciembre', ejercicio[11]],
    ]);

    var options = {
        title: 'Ejercicio comercial del año corriente',
        hAxis: {
            title: 'Mes',
        },
        vAxis: {
            title: '$ (pesos)'
        },
        colors: ['#097138', '#a52714']
    };

    var chart = new google.visualization.LineChart(document.getElementById('ejercicio'));
    chart.draw(data, options);
}

function chart_column(ventas) {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Mes');
    data.addColumn('number', 'Ventas');

    data.addRows([
        ['Enero', ventas[0]],
        ['Febrero', ventas[1]],
        ['Marzo', ventas[2]],
        ['Abril', ventas[3]],
        ['Mayo', ventas[4]],
        ['Junio', ventas[5]],
        ['Julio', ventas[6]],
        ['Agosto', ventas[7]],
        ['Septiembre', ventas[8]],
        ['Octubre', ventas[9]],
        ['Noviembre', ventas[10]],
        ['Diciembre', ventas[11]],
    ]);

    var options = {
        title: 'Ventas del año corriente',
        hAxis: {
            title: 'Mes',
        },
        vAxis: {
            title: '# Cantidad'
        },
        colors: ['#097138']
    };

    var chart = new google.visualization.ColumnChart(
        document.getElementById('ventas'));

    chart.draw(data, options);
}