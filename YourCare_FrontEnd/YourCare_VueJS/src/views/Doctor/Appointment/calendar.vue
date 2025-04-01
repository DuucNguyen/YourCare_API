<script setup>
    import DoctorSideBar from "@/shared/DoctorSideBar.vue";

    import ApiAppointment from "@/api/ApiAppointment";
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import AppointmentStatus from "@/constants/AppointmentStatus";

    import { ref, onMounted } from "vue";
    import { useAuthStore } from "@/stores/auth-store";
    import { useRouteStore } from "@/stores/route-store";
    import { useRouter } from "vue-router";

    import dayjs from "dayjs";
    import isSameOrAfter from "dayjs/plugin/isSameOrAfter";
    dayjs.extend(isSameOrAfter);

    const authStore = useAuthStore();
    const routeStore = useRouteStore();
    const router = useRouter();

    var data = ref([]);
    var doctorProfileId = ref({});

    const chosenDate = ref(dayjs());
    const watchChosenDate = ref(dayjs());
    const appointments = ref([]);

    const getListData = (value) => {
        return data.value
            .filter((x) => dayjs(x.date).isSame(dayjs(value), "day") && x.appointmentCount)
            .map((x) => ({
                type: "success",
                content: `${x.appointmentCount.toString().padStart(2, "0")} lịch khám`,
            }));
    };
    const getMonthData = (value) => {
        if (value.month() === 8) {
            return 1394;
        }
    };

    const InitDoctor = async () => {
        var user = await authStore.getUserInfo();
        var result = await ApiDoctorProfile.GetByUserID(user.id);
        if (result.data.isSucceeded) {
            doctorProfileId.value = result.data.data;
        }
    };

    const GetData = async () => {
        if (doctorProfileId.value) {
            var result = await ApiAppointment.CountAppointmentInMonthByDate(
                doctorProfileId.value,
                chosenDate.value,
            );
            if (result.data.isSucceeded) {
                data.value = result.data.data;
                watchChosenDate.value = chosenDate.value;
            }
        }
    };

    const GetCalendarData = async () => {
        if (
            !dayjs(chosenDate.value).isSame(dayjs(watchChosenDate.value), "month") ||
            !dayjs(chosenDate.value).isSame(dayjs(watchChosenDate.value), "year")
        ) {
            await GetData();
        }
        var find = data.value.find(
            (x) => x.appointmentCount > 0 && dayjs(chosenDate.value).isSame(dayjs(x.date), "day"),
        );

        GetAppointmentByDate();
    };

    const GetAppointmentByDate = async () => {
        var result_appointment = await ApiAppointment.GetDoctorAppointmentByDate(
            doctorProfileId.value,
            dayjs(chosenDate.value).format("MM/DD/YYYY"),
        );
        if (result_appointment.data.isSucceeded) {
            appointments.value = result_appointment.data.data;
        }
    };

    const RedirectToAppointmentDetail = (item) => {
        routeStore.setData(item);
        router.push({ name: "Doctor_Appointment_View" });
    };

    const CheckIsValidToLeave = () => {
        return (
            appointments.value.filter((x) => x.status !== AppointmentStatus.WAITING).length <= 0 &&
            dayjs(chosenDate.value, "DD/MM/YYYY").isSameOrAfter(dayjs().startOf("day"))
        );
    };

    onMounted(async () => {
        await InitDoctor();
        await GetData();
        await GetAppointmentByDate();
    });
</script>
<template>
    <div class="doctor_dashboard_body">
        <DoctorSideBar active-item="calendar" />
        <div class="doctor_dashboard_section">
            <div class="doctor_dashboard_body">
                <div class="col-md-8 doctor_calendar_container">
                    <a-calendar v-model:value="chosenDate" @change="GetCalendarData">
                        <template #dateCellRender="{ current }">
                            <ul class="events">
                                <li v-for="item in getListData(current)" :key="item.content">
                                    <a-badge :status="item.type" :text="item.content" />
                                </li>
                            </ul>
                        </template>
                        <template #monthCellRender="{ current }">
                            <div v-if="getMonthData(current)" class="notes-month">
                                <section>{{ getMonthData(current) }}</section>
                                <span>Backlog number</span>
                            </div>
                        </template>
                    </a-calendar>
                </div>
                <div class="col-md-4 doctor_calendar_detail">
                    <div class="doctor_dashboard_title">
                        <h3>Lịch khám: {{ dayjs(chosenDate).format("DD/MM/YYYY") }}</h3>
                    </div>
                    <!-- <a-row v-if="CheckIsValidToLeave()">
                        <a-col :span="24">
                            <a-button type="primary">Không nhận lịch ngày này</a-button>
                        </a-col>
                    </a-row> -->
                    <div v-show="appointments.length > 0" class="calendar_appointment_container">
                        <template v-for="item in appointments">
                            <div
                                class="calendar_appointment_item"
                                @click="RedirectToAppointmentDetail(item)">
                                <a-row>
                                    <a-col :span="1" class="d-flex align-items-center">
                                        <a-badge
                                            :status="
                                                item.status !== AppointmentStatus.WAITING
                                                    ? 'success'
                                                    : 'error'
                                            " />
                                    </a-col>
                                    <a-col :span="6" class="calendar_appointment_timeslot">
                                        <span>
                                            {{
                                                dayjs(item.timetableStartTime, "HH:mm:ss").format(
                                                    "HH:mm",
                                                )
                                            }}
                                            -
                                            {{
                                                dayjs(item.timetableEndTime, "HH:mm:ss").format(
                                                    "HH:mm",
                                                )
                                            }}
                                        </span>
                                    </a-col>
                                    <a-col :span="16" class="ms-2 d-flex align-items-center">
                                        <div>
                                            Người đặt lịch:
                                            <span style="font-weight: 500">
                                                {{ item.patientName }}
                                            </span>
                                        </div>
                                    </a-col>
                                </a-row>
                            </div>
                        </template>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
    .events {
        list-style: none;
        margin: 0;
        padding: 0;
    }
    .events .ant-badge-status {
        overflow: hidden;
        white-space: nowrap;
        width: 100%;
        text-overflow: ellipsis;
        font-size: 14px;
    }
    .notes-month {
        text-align: center;
        font-size: 28px;
    }
    .notes-month section {
        font-size: 28px;
    }

    .doctor_calendar_container {
        height: 670px;
        max-height: 670px;
        overflow-y: scroll;
        border-radius: 8px;
        margin: 0 20px 0 0;
    }
    .doctor_calendar_detail {
        padding: 10px;
        margin-left: 15px;
        border-radius: 8px;
        background: #fff;
        overflow-y: scroll;
    }
    .calendar_appointment_item {
        margin: 10px 0px;
        cursor: pointer;
        border-radius: 3px;
        padding: 5px 0px;
    }

    .calendar_appointment_item:hover {
        background: #d6effe;
    }

    .calendar_appointment_timeslot {
        padding: 5px 0px;
        border: 1px solid #1975dc;
        border-radius: 3px;
        font-weight: 500;
        color: #1975dc;
        text-align: center;
    }
</style>
