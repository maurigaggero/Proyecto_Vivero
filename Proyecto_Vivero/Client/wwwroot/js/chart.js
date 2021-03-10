google.charts.load('current', { packages: ['corechart', 'line'] });
google.charts.setOnLoadCallback(chart_line);

function chart_line(ejercicio, compras) {
    var data = new google.visualization.DataTable();
    data.addColumn('string', 'Mes');
    data.addColumn('number', 'Ingresos');
    data.addColumn('number', 'Gastos');

    data.addRows([
        ['Enero', ejercicio[0], compras[0]],
        ['Febrero', ejercicio[1], compras[1]],
        ['Marzo', ejercicio[2], compras[2]],
        ['Abril', ejercicio[3], compras[3]],
        ['Mayo', ejercicio[4], compras[4]],
        ['Junio', ejercicio[5], compras[5]],
        ['Julio', ejercicio[6], compras[6]],
        ['Agosto', ejercicio[7], compras[7]],
        ['Septiembre', ejercicio[8], compras[8]],
        ['Octubre', ejercicio[9], compras[9]],
        ['Noviembre', ejercicio[10], compras[10]],
        ['Diciembre', ejercicio[11], compras[11]],
    ]);

    var options = {
        title: 'Ejercicio comercial del año corriente',
        hAxis: {
            title: 'Mes',
        },
        vAxis: {
            title: '$ (pesos)',
        },
        colors: ['#097138', '#a52714']
    };

    var chart = new google.visualization.LineChart(document.getElementById('ej_chart'));
    chart.draw(data, options);
}

google.charts.load('current', { packages: ['corechart', 'bar'] });
google.charts.setOnLoadCallback(chart_column);

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