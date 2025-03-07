<script setup>
    import ApiAppointment from "@/api/ApiAppointment";
    import ApiTimetable from "@/api/ApiTimetable";
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";

    import { ref, onMounted } from "vue";
    import { useAuthStore } from "@/stores/auth-store";
    import { useRouteStore } from "@/stores/route-store";
    import dayjs from "dayjs";
    import { createVNode } from "vue";
    import { useRouter } from "vue-router";

    const authStore = useAuthStore();
    const routeStore = useRouteStore();
    const router = useRouter();

    const appointment = ref(routeStore.data);

    const doctor_timetables = ref([]); //doctor schedule for the next 10 days + today
    const doctor_timetables_day = ref([]); //unique days in the list
    const day_timeTable = ref([]); //time slots of a day - dynamic
    const chosenDate = ref(); //chosen date item in doctor_timetables

    const timetable = ref({});

    const getNumberOfTimeSlots = () => {
        return [...new Set(doctor_timetables.value.map((item) => item.startTime))].length;
    };

    const getTimeSlotByDate = (date) => {
        chosenDate.value = date;
        day_timeTable.value = doctor_timetables.value.filter((x) => x.date === date);
    };

    const selectTimetable = (item) => {
        timetable.value = item;
    };

    const getTimetableOrder = (availableSlot) => {
        switch (availableSlot) {
            case 1:
                return 3;
            case 2:
                return 2;
            case 3:
                return 1;
        }
    };

    //init functions
    const initTimetableData = async () => {
        var doctor_timetables_result = await ApiTimetable.GetTimetableByDoctorID(
            appointment.value.doctorID ?? 0,
        );
        if (doctor_timetables_result.data.isSucceeded) {
            doctor_timetables.value = doctor_timetables_result.data.data;
            doctor_timetables_day.value = [
                ...new Map(doctor_timetables.value.map((item) => [item.date, item])).values(),
            ];
            getTimeSlotByDate(doctor_timetables_day.value[0].date);
        }
    };

    onMounted(async () => {
        initTimetableData();
    });
</script>

<template>
    <div class="container vh-100" >
        <div class="mt-5 d-flex justify-content-between">
            <div class="col-md-8" style="max-height: 350px">
                <div class="profile-patient">
                    <div class="profile-patient-title">
                        <div>
                            <span>1</span>
                            <span> - Ngày và giờ khám</span>
                        </div>
                        <!-- <button
                        @click="toggleSlide($event, '#profile-patient-timeSlot')"
                        type="button"
                        class="btn-slideToggle"
                        id="profile-patient-timeSlot-btn">
                        <i class="bx bx-chevron-down"></i>
                    </button> -->
                    </div>
                    <div class="w-100" id="profile-patient-timeSlot">
                        <div class="timeSlot-date-container">
                            <div
                                v-for="item in doctor_timetables_day"
                                :id="'timetb-' + item.id"
                                :class="
                                    'timeSlot-date ' +
                                    (chosenDate === item.date ? 'timeSlot-date-chosen' : '')
                                "
                                @click="getTimeSlotByDate(item.date)">
                                <p>{{ dayjs(item.date).format("dddd") }}</p>
                                <p>
                                    {{ dayjs(item.date).date() }} -
                                    {{ dayjs(item.date).month() + 1 }}
                                </p>
                                <span
                                    v-if="getNumberOfTimeSlots(item.date) <= 0"
                                    class="text-danger"
                                    >Đã đầy lịch</span
                                >
                                <span v-else>{{ getNumberOfTimeSlots(item.date) }} khung giờ</span>
                            </div>
                        </div>
                        <div class="timeSlot-slot-instructions">
                            <div class="timeSlot-slot-instruction-item">
                                <div class="timeSlot-available-chosen">#</div>
                                <span>-Đang chọn</span>
                            </div>
                            <div class="timeSlot-slot-instruction-item">
                                <div class="timeSlot-available-1">1</div>
                                <span>-Còn 01 chỗ trống</span>
                            </div>
                            <div class="timeSlot-slot-instruction-item">
                                <div class="timeSlot-available-2">2</div>
                                <span>-Còn 02 chỗ trống</span>
                            </div>
                            <div class="timeSlot-slot-instruction-item">
                                <div class="slot-unavailable">0</div>
                                <span>-Hết chỗ trống</span>
                            </div>
                        </div>
                        <div>
                            <p
                                v-if="day_timeTable.length <= 0"
                                class="w-100 alert alert-warning m-0">
                                <span class="d-flex align-items-center"
                                    ><span class="me-2"
                                        ><i class="fs-2 text-danger bx bx-error-circle"></i
                                    ></span>
                                    Lịch làm việc trong ngày này đã hết. Vui lòng chọn ngày khác.
                                    Xin cảm ơn !</span
                                >
                            </p>
                            <div v-else class="timeSlot-slot-container">
                                <template v-for="item in day_timeTable">
                                    <a-tooltip v-if="item.isAvailable" placement="top">
                                        <template #title>
                                            <span>Chỗ trống: {{ item.availableSlots }}</span>
                                        </template>
                                        <div
                                            @click="selectTimetable(item)"
                                            :class="
                                                item.id === timetable.id
                                                    ? 'timeSlot-slot timeSlot-chosen' +
                                                      (item.availableSlots === 1
                                                          ? ' timeSlot-available-1'
                                                          : '') +
                                                      (item.availableSlots === 2
                                                          ? ' timeSlot-available-2'
                                                          : '')
                                                    : 'timeSlot-slot' +
                                                      (item.availableSlots === 1
                                                          ? ' timeSlot-available-1'
                                                          : '') +
                                                      (item.availableSlots === 2
                                                          ? ' timeSlot-available-2'
                                                          : '')
                                            ">
                                            {{ dayjs(item.startTime, "HH:mm:ss").format("HH:mm") }}
                                            -
                                            {{ dayjs(item.endTime, "HH:mm:ss").format("HH:mm") }}
                                        </div>
                                    </a-tooltip>
                                    <div v-else class="timeSlot-slot slot-unavailable">
                                        {{ dayjs(item.startTime, "HH:mm:ss").format("HH:mm") }}
                                        -
                                        {{ dayjs(item.endTime, "HH:mm:ss").format("HH:mm") }}
                                    </div>
                                </template>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4 d-flex flex-column"></div>
        </div>
    </div>
</template>
