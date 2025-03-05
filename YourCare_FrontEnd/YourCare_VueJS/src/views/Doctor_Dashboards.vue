<script setup>
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiAppointment from "@/api/ApiAppointment";
    import DoctorSideBar from "@/shared/DoctorSideBar.vue";
    import dayjs from "dayjs";
    import { ref, onMounted, nextTick } from "vue";
    import { useRouter } from "vue-router";

    import { useAuthStore } from "@/stores/auth-store";
    import { useRouteStore } from "@/stores/route-store";

    import Highcharts from "highcharts";

    const authStore = useAuthStore();
    const routeStore = useRouteStore();

    const router = useRouter();
    const user = ref({});
    const doctorProfileId = ref({});
    const appointments = ref([]);
    const date = ref(dayjs());

    const InitDoctor = async () => {
        user.value = await authStore.getUserInfo();
        if (user.value) {
            var result_doctor = await ApiDoctorProfile.GetByUserID(user.value.id);
            doctorProfileId.value = result_doctor.data.data;
        }
    };

    const InitAppointment = async () => {
        var result_appointment = await ApiAppointment.GetDoctorAppointmentByDate(
            doctorProfileId.value,
            dayjs(date.value).format("MM/DD/YYYY"),
        );
        if (result_appointment.data.isSucceeded) {
            appointments.value = result_appointment.data.data;
        }
    };

    const onDateChange = async () => {
        await InitAppointment();
    };

    const RedirectToAppointmentDetail = (item) => {
        routeStore.setData(item);
        router.push({ name: "Doctor_Appointment_View" });
    };

    onMounted(async () => {
        await InitDoctor();
        await InitAppointment();
        await nextTick();

        Highcharts.chart("chart_container", {
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
                                .label("Total<br/>" + "<strong>2 877 820</strong>")
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
                text: "Lịch khám hôm nay",
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
                    name: "Registrations",
                    colorByPoint: true,
                    innerSize: "70%",
                    data: [
                        {
                            name: "EV",
                            y: 23.9,
                        },
                        {
                            name: "Hybrids",
                            y: 12.6,
                        },
                        {
                            name: "Diesel",
                            y: 37.0,
                        },
                        {
                            name: "Petrol",
                            y: 26.4,
                        },
                    ],
                },
            ],
        });
    });
</script>

<template>
    <div class="doctor_dashboard">
        <DoctorSideBar active-item="dashboard" />
        <div class="doctor_dashboard_section">
            <div class="doctor_dashboard_title">
                <h3>Dashboard</h3>
            </div>
            <div class="doctor_dashboard_body">
                <div class="col-md-8">
                    <div class="row doctor_dashboard_statistic">
                        <div class="col statistic_item">
                            <div
                                class="statistic_item_icon"
                                style="color: #3a57e8; background-color: #d8ddfa">
                                <i class="bx bx-calendar"></i>
                            </div>
                            <div class="statistic_item_data">
                                <span style="color: #3a57e8">
                                    {{ dayjs(date).format("DD/MM/YYYY") }}
                                </span>
                                <span>Today</span>
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
                                    {{ appointments.length.toString().padStart(2, "0") }}
                                </span>
                                <span>Total</span>
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
                                    {{ appointments.length.toString().padStart(2, "0") }}
                                </span>
                                <span>Appointment</span>
                            </div>
                        </div>
                        <div class="col statistic_item">
                            <div
                                class="statistic_item_icon"
                                style="color: #1aa053; background-color: #d1ecdd">
                                <i class="bx bx-calendar-check"></i>
                            </div>
                            <div class="statistic_item_data">
                                <span style="color: #1aa053">04</span>
                                <span>Processed</span>
                            </div>
                        </div>
                    </div>
                    <div class="doctor_dashboard_appointment">
                        <div v-if="appointments.length > 0" class="appointment_container">
                            <template v-for="item in appointments">
                                <div
                                    :class="'appointment_item'"
                                    @click="RedirectToAppointmentDetail(item)">
                                    <a-row>
                                        <a-col :span="24">
                                            <span style="font-weight: 500; font-size: 17px">{{
                                                item.doctorName
                                            }}</span>
                                        </a-col>
                                    </a-row>
                                    <a-row>
                                        <a-col :span="20">
                                            <span>
                                                {{
                                                    dayjs(
                                                        item.timetableStartTime,
                                                        "HH:mm:ss",
                                                    ).format("HH:mm")
                                                }}
                                                -
                                            </span>
                                            <span style="color: #1975dc; font-weight: 500">
                                                {{ dayjs(item.timetableDate).format("DD-MM-YYYY") }}
                                            </span>
                                            <br />
                                            <span>
                                                {{ item.patientName }}
                                            </span>
                                            <br />
                                            <span
                                                class="text-secondary ms-3"
                                                style="font-size: 13px">
                                                {{ item.status }}
                                            </span>
                                        </a-col>
                                        <a-col :span="4" class="text-center">
                                            <span style="font-size: 12px">STT</span>
                                            <br />
                                            <span
                                                style="
                                                    font-weight: 500;
                                                    font-size: 20px;
                                                    color: #22c55e;
                                                "
                                                >{{ item.timeTableOrder }}</span
                                            >
                                        </a-col>
                                    </a-row>
                                </div>
                            </template>
                        </div>
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
                    <div id="chart_container"></div>
                </div>
            </div>
        </div>
    </div>
</template>
<style scoped>
    .appointment_container {
        width: 100%;
        padding: 5px;
        height: 470px;
        max-height: 470px;
        overflow-y: scroll;
        background: #f3f4f6;
        border-bottom: 1px solid #ddd;
    }
    #chart_container {
        width: 400px;
        height: 300px;
        margin-top: 10px;
        border-radius: 8px;
    }
</style>
