$(document).ready(function () {
    document.getElementById("foter").style.display = 'none';
    getData();
})

function getData() {
    $.ajax({
        url: "/Admin/DashBoard/GetData",
        method: "GET",
        success: function (data) {

            Highcharts.chart('container', {
                chart: {
                    type: 'line'
                },
                title: {
                    text: 'Monthly Average Sales'
                },
                
                xAxis: {
                    title: {
                        text:"Months"
                    },
                    categories: data.date
                },
                yAxis: {
                    title: {
                        text: 'Amount'
                    }
                },
                plotOptions: {
                    line: {
                        dataLabels: {
                            enabled: true
                        },
                        enableMouseTracking: true
                    }
                },
                series: [{
                    name: 'Sales',
                    data: data.amount
                }]
            });
            var avg=0;
            for (let i = 0; i < data.amount.length; i++) {
                avg += data.amount[i]
            }

            avg = (avg / data.date.length)/100;
            
            document.getElementById("sales_average").innerHTML = "%"+avg;


        }
    });

   
}