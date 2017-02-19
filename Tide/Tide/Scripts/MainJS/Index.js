/// <reference path="china.js" />
/// <reference path="echarts.js" />
/// <reference path="~/Scripts/jquery-1.8.2.intellisense.js" />
//主页面JS


$(function () {
    $.ajax({
        type: "post",
        async: false,
        url: "/Main/GetData",
        contentType: "application/json; charset=utf-8",
        datatype: "json",
        beforeSend: function () {

        },
        success: LoadChart
    });
});

function LoadAreaItem(id,provicename) {
    $("#detail").load("/Main/GetAreaData", { exchangeID: id, provice: provicename }, function () {
    });
}


//加载图表
function LoadChart(obj) {

    var panel = document.getElementById('main');
    $("#main").css({ "width": $(window).width()-20, "height": ($(window).height())/2 });
    
    var myChart = echarts.init(panel);
    myChart.on("click", function (obj) {
        if (obj.data)
            LoadAreaItem(obj.data.id, "");
        else LoadAreaItem("", obj.name);
    });
    //myChart.on("geoselectchanged", function (obj) {
    //    alert(obj.name);
    //    //LoadAreaItem(obj.data.id);
    //});
    
    var data = [];
    var geoCoordMap = {};

    for (var i = 0; i < obj.length; i++) {
        data[i] = { name: obj[i].City, value: obj[i].Value, id: obj[i].ID };
    }

    for (var i = 0; i < obj.length; i++) {
        var loc = new Array();
        loc[0] = obj[i].Loc.E;
        loc[1] = obj[i].Loc.W;
        geoCoordMap[data[i].name] = loc;
    }
    
    var convertData = function (data) {
        var res = [];
        for (var i = 0; i < data.length; i++) {
            var geoCoord = geoCoordMap[data[i].name];
            if (geoCoord) {
                res.push({
                    id:data[i].id,
                    name: data[i].name,
                    value: geoCoord.concat(data[i].value)
                });
            }
        }
        return res;
    };

    option = {
        backgroundColor: '#404a59',
        title: {
            text: '全国主要交易所分部地图',
            //subtext: 'data from PM25.in',
            //sublink: 'http://www.pm25.in',
            left: 'center',
            textStyle: {
                color: '#fff'
            }
        },
        tooltip: {
            trigger: 'item'
        },
        legend: {
            orient: 'vertical',
            y: 'bottom',
            x: 'right',
            data: ['交易所'],
            textStyle: {
                color: '#fff'
            }
        },
        geo: {
            map: 'china',
            label: {
                emphasis: {
                    show: false
                }
            },
            roam: true,
            itemStyle: {
                normal: {
                    areaColor: '#323c48',
                    borderColor: '#111'
                },
                emphasis: {
                    areaColor: '#2a333d'
                }
            }
        },
        series: [
            {
                name: '交易所',
                type: 'scatter',
                coordinateSystem: 'geo',
                data: convertData(data),
                symbolSize: function (val) {
                    return val[2] / 10;
                },
                label: {
                    normal: {
                        formatter: '{b}',
                        position: 'right',
                        show: false
                    },
                    emphasis: {
                        show: true
                    }
                },
                itemStyle: {
                    normal: {
                        color: '#ddb926'
                    }
                }
            }
            ,
            {
                name: 'Top 5',
                type: 'effectScatter',
                coordinateSystem: 'geo',
                data: convertData(data.sort(function (a, b) {
                    return b.value - a.value;
                }).slice(0, 5)),
                symbolSize: function (val) {
                    return val[2] / 10;
                },
                showEffectOn: 'render',
                rippleEffect: {
                    brushType: 'stroke'
                },
                hoverAnimation: true,
                label: {
                    normal: {
                        formatter: '{b}',
                        position: 'right',
                        show: true
                    }
                },
                itemStyle: {
                    normal: {
                        color: '#f4e925',
                        shadowBlur: 10,
                        shadowColor: '#333'
                    }
                },
                zlevel: 1
            }
        ]
    };

    // 为echarts对象加载数据
    myChart.setOption(option);
}