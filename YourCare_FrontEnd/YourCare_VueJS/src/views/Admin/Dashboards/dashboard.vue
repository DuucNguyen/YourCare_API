<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";
    import dayjs from "dayjs";
    import ApiAppointment from "@/api/ApiAppointment";
    import { ref, onMounted, reactive, watch, computed } from "vue";
    import AppointmentStatus from "@/constants/AppointmentStatus";
    import Highcharts from "highcharts";
    Highcharts.setOptions({
        lang: {
            decimalPoint: ".",
            thousandsSep: ",",
        },
    });

    const appointments = ref([]);
    const date = ref(dayjs());
    const monthChartData = ref({});

    const InitAppointment = async () => {
        var result_appointment = await ApiAppointment.GetAllAppointmentByDate(
            dayjs(date.value).format("MM/DD/YYYY"),
        );
        if (result_appointment.data.isSucceeded) {
            appointments.value = result_appointment.data.data;
        }
    };

    const InitMonthChartData = async () => {
        var result = await ApiAppointment.GetMonthChartDataByYear(dayjs().year());
        if (result.data.isSucceeded) {
            monthChartData.value = result.data.data;
        }
    };
    const GetDataTotalCount = (data) => {
        return Object.keys(data).map((month) =>
            data[month].reduce((sum, item) => sum + item.count, 0),
        );
    };

    const GetMonthDataTotalCount = (data) => {
        var month = dayjs().month().toString();
        var result = Object.keys(data).map((month) =>
            data[month].reduce((sum, item) => sum + item.count, 0),
        );
        return result[month] ? result[month] : 0;
    };

    const GetDataByStatusCount = (data, status) => {
        return Object.keys(data).map((month) =>
            data[month]
                .filter((item) => item.status == status)
                .reduce((sum, item) => sum + item.count, 0),
        );
    };

    const currentMonthStatus = computed(() => {
        //handle reactivity
        const month = dayjs().month() + 1;
        const monthData = monthChartData.value[month];
        return monthData || [];
    });

    const CountStatusAppointment = (status) => {
        const monthData = currentMonthStatus.value;
        if (!monthData.length) return 0;
        const statusObj = monthData.find((item) => item.status === status);
        return statusObj ? statusObj.count : 0;
    };

    //for pie chart
    const onDateChange = async () => {
        await InitAppointment();
    };

    const GetChartDataFromAppointments = () => {
        var listStatus = AppointmentStatus.values();
        var result = listStatus.map((x) => {
            return {
                name: x,
                y: GetPercentageByStatus(x),
            };
        });

        return result;
    };

    const GetPercentageByStatus = (status) => {
        var total = appointments.value.length;
        var count = appointments.value.filter((x) => x.status === status).length;

        return (count * 100) / total;
    };

    let dayChartInstance = null;
    const GenerateDayChart = () => {
        if (dayChartInstance) {
            dayChartInstance.destroy();
        }

        Highcharts.chart("day_chart_container", {
            chart: {
                type: "pie",
                custom: {},
                events: {
                    render() {
                        const chart = this,
                            series = chart.series[0];
                        let customLabel = chart.options.chart.custom.label;

                        if (!customLabel) {
                            customLabel = chart.options.chart.custom.label = chart.renderer
                                .label(
                                    "Total<br/>" +
                                        "<strong>" +
                                        appointments.value.length +
                                        "</strong>",
                                )
                                .css({
                                    color: "#000",
                                    textAnchor: "middle",
                                })
                                .add();
                        }

                        const x = series.center[0] + chart.plotLeft,
                            y = series.center[1] + chart.plotTop - customLabel.attr("height") / 2;

                        customLabel.attr({
                            x,
                            y,
                        });
                        // Set font size based on chart diameter
                        customLabel.css({
                            fontSize: `${series.center[2] / 12}px`,
                        });
                    },
                },
            },
            lang: {
                locale: "en",
            },
            accessibility: {
                point: {
                    valueSuffix: "%",
                },
            },
            credits: {
                enabled: false,
            },
            title: {
                text: "Tiến độ " + dayjs(date.value).format("DD/MM/YYYY"),
            },
            tooltip: {
                pointFormat: "{series.name}: <b>{point.percentage:.0f}%</b>",
            },
            legend: {
                enabled: false,
            },
            plotOptions: {
                series: {
                    allowPointSelect: true,
                    cursor: "pointer",
                    borderRadius: 5,
                    dataLabels: [
                        {
                            enabled: true,
                            distance: 20,
                            format: "{point.name}",
                        },
                        {
                            enabled: true,
                            distance: -16,
                            format: "{point.percentage:.0f}%",
                            style: {
                                fontSize: "0.9em",
                            },
                        },
                    ],
                    showInLegend: true,
                },
            },
            series: [
                {
                    name: "Phần trăm",
                    colorByPoint: true,
                    innerSize: "70%",
                    data: GetChartDataFromAppointments(),
                },
            ],
        });
    };

    let monthChartInstance = null;
    const GenerateMonthChart = () => {
        if (monthChartInstance) {
            monthChartInstance.destroy();
        }
        Highcharts.chart("month_chart_container", {
            chart: {
                zooming: {
                    type: "xy",
                },
            },
            credits: false,
            title: {
                text: "Thống kê lịch khám trong tháng " + (dayjs().month() + 1),
            },
            subtitle: {
                text: "Tính tới ngày " + dayjs().format("DD/MM/YYYY"),
            },
            xAxis: [
                {
                    categories: [
                        "Tháng 1",
                        "Tháng 2",
                        "Tháng 3",
                        "Tháng 4",
                        "Tháng 5",
                        "Tháng 6",
                        "Tháng 7",
                        "Tháng 8",
                        "Tháng 9",
                        "Tháng 10",
                        "Tháng 11",
                        "Tháng 12",
                    ],
                    crosshair: true,
                },
            ],
            yAxis: [
                {
                    // Primary yAxis
                    labels: {
                        format: "{value}",
                        style: {
                            color: Highcharts.getOptions().colors[0],
                        },
                    },
                    title: {
                        text: "Tổng",
                        style: {
                            color: Highcharts.getOptions().colors[0],
                        },
                    },
                },
            ],
            tooltip: {
                shared: true,
            },
            legend: {
                layout: "horizontal",
                align: "center",
                verticalAlign: "bottom",
                backgroundColor:
                    Highcharts.defaultOptions.legend.backgroundColor || // theme
                    "rgba(255,255,255,0.25)",
            },
            series: [
                {
                    name: "Tổng",
                    type: "column",
                    yAxis: 0,
                    data: GetDataTotalCount(monthChartData.value),
                    tooltip: {
                        valueSuffix: " lịch khám",
                    },
                },
                {
                    name: AppointmentStatus.COMPLETED,
                    type: "spline",
                    yAxis: 0,
                    data: GetDataByStatusCount(monthChartData.value, AppointmentStatus.COMPLETED),
                    marker: {
                        enabled: false,
                    },
                    tooltip: {
                        valueSuffix: " lịch khám",
                    },
                    color: Highcharts.getOptions().colors[2],
                },
                {
                    name: AppointmentStatus.CANCELLED,
                    type: "spline",
                    yAxis: 0,
                    data: GetDataByStatusCount(monthChartData.value, AppointmentStatus.CANCELLED),
                    dashStyle: "shortdot",
                    tooltip: {
                        valueSuffix: " lịch khám",
                    },
                    color: Highcharts.getOptions().colors[3],
                },
                {
                    name: AppointmentStatus.PROCESSING,
                    type: "spline",
                    yAxis: 0,
                    data: GetDataByStatusCount(monthChartData.value, AppointmentStatus.PROCESSING),
                    dashStyle: "dot",
                    tooltip: {
                        valueSuffix: " lịch khám",
                    },
                    color: Highcharts.getOptions().colors[1],
                },
            ],
            responsive: {
                rules: [
                    {
                        condition: {
                            maxWidth: 500,
                        },
                        chartOptions: {
                            legend: {
                                floating: false,
                                layout: "horizontal",
                                align: "center",
                                verticalAlign: "bottom",
                                x: 0,
                                y: 0,
                            },
                            yAxis: [
                                {
                                    labels: {
                                        align: "right",
                                        x: 0,
                                        y: -6,
                                    },
                                    showLastLabel: false,
                                },
                                {
                                    labels: {
                                        align: "left",
                                        x: 0,
                                        y: -6,
                                    },
                                    showLastLabel: false,
                                },
                                {
                                    visible: false,
                                },
                            ],
                        },
                    },
                ],
            },
        });
    };

    onMounted(async () => {
        await InitAppointment();
        await InitMonthChartData();

        GenerateDayChart();
        GenerateMonthChart();

        watch(
            () => appointments.value,
            () => {
                if (appointments.value.length > 0) {
                    GenerateDayChart();
                }
            },
            { deep: true },
        );
    });
</script>

<template>
    <div class="admin_dashboard">
        <AdminSideBar active-item="dashboard" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_title">
                <h3>Dashboards</h3>
            </div>
            <div class="admin_dashboard_body">
                <div class="col-md-8">
                    <div class="row admin_dashboard_statistic">
                        <div class="col statistic_item">
                            <div
                                class="statistic_item_icon"
                                style="color: #2caffe; background-color: #d6effe">
                                <i class="bx bx-calendar"></i>
                            </div>
                            <div class="statistic_item_data">
                                <span style="color: #00cccc">
                                    {{ GetMonthDataTotalCount(monthChartData) }}
                                </span>
                                <span>Tổng</span>
                            </div>
                        </div>
                        <div class="col statistic_item">
                            <div
                                class="statistic_item_icon"
                                style="color: #eb991b; background-color: #f6e6cc">
                                <i class="bx bx-calendar-exclamation"></i>
                            </div>
                            <div class="statistic_item_data">
                                <span style="color: #eb991b">
                                    {{ CountStatusAppointment(AppointmentStatus.WAITING) }}
                                </span>
                                <span>Đang đợi</span>
                            </div>
                        </div>
                        <div class="col statistic_item">
                            <div
                                class="statistic_item_icon"
                                style="color: #00cccc; background-color: #bceff2">
                                <i class="bx bx-align-left"></i>
                            </div>
                            <div class="statistic_item_data">
                                <span style="color: #00cccc">
                                    {{ CountStatusAppointment(AppointmentStatus.PROCESSING) }}
                                </span>
                                <span>Đang xử lí</span>
                            </div>
                        </div>
                        <div class="col statistic_item">
                            <div
                                class="statistic_item_icon"
                                style="color: #1aa053; background-color: #d1ecdd">
                                <i class="bx bx-calendar-check"></i>
                            </div>
                            <div class="statistic_item_data">
                                <span style="color: #1aa053">
                                    {{ CountStatusAppointment(AppointmentStatus.COMPLETED) }}
                                </span>
                                <span>Đã xử lí</span>
                            </div>
                        </div>
                        <div class="col statistic_item">
                            <div
                                class="statistic_item_icon"
                                style="color: #d32f2f; background-color: #ffebee">
                                <i class="bx bx-calendar-check"></i>
                            </div>
                            <div class="statistic_item_data">
                                <span style="color: #d32f2f">
                                    {{ CountStatusAppointment(AppointmentStatus.CANCELLED) }}
                                </span>
                                <span>Đã hủy</span>
                            </div>
                        </div>
                    </div>
                    <div class="admin_dashboard_month_chart">
                        <div v-if="true" id="month_chart_container"></div>
                        <div v-else>
                            <a-empty>
                                <template #description>
                                    <span>Không có lịch khám trong hôm nay</span>
                                </template>
                            </a-empty>
                        </div>
                    </div>
                </div>
                <div class="col-md-4">
                    <div
                        :style="{
                            width: '400px',
                            border: '1px solid #fff',
                            borderRadius: '8px',
                        }">
                        <a-calendar
                            v-model:value="date"
                            :fullscreen="false"
                            @change="onDateChange" />
                    </div>
                    <div v-show="appointments.length > 0" id="day_chart_container"></div>
                    <div v-show="appointments.length <= 0" class="mt-5">
                        <a-empty>
                            <template #description>
                                <span>Không có dữ liệu cho biểu đồ</span>
                            </template>
                        </a-empty>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<style>
    .admin_dashboard {
        display: flex;
    }
    .admin_dashboard_section {
        margin-top: 10px;
        margin-left: 50px;
        width: 100%;
    }

    .admin_dashboard_statistic {
        width: 100%;
    }
    .admin_dashboard_body {
        display: flex;
    }

    .admin_dashboard_month_chart {
        height: 500px;
        background: #fff;
        margin: 20px 20px 0px 0;
        border-radius: 5px;
        padding: 15px;
        display: flex;
        align-items: center;
        justify-content: center;
    }
    .month_chart_container {
        width: 100%;
        max-width: 100%;
    }

    #month_chart_container {
        width: 100%;
    }

    #day_chart_container {
        width: 400px;
        height: 300px;
        margin-top: 10px;
        border-radius: 8px;
        overflow: hidden;
    }
</style>
