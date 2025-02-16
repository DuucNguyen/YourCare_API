<script setup>
    import { useRouteStore } from "@/stores/route-store";
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiTimetable from "@/api/ApiTimetable";
    import { ref, reactive, onMounted } from "vue";
    import { useRoute, useRouter } from "vue-router";

    import dayjs from "dayjs";

    const routeStore = useRouteStore();
    const route = useRoute();
    const router = useRouter();

    const doctorID = route.params.doctorID;
    const doctor = ref({});
    const doctor_timetables = ref([]); //doctor schedule for the next 10 days + today
    const doctor_timetables_day = ref([]); //unique days in the list
    const day_timeTable = ref([]); //time slots of a day - dynamic
    const chosenDate = ref();

    const getNumberOfTimeSlots = (date) => {
        // return [...new Map(doctor_timetables.value.map((item) => [item.startTime, item])).values()]
        //     .length;

        return [...new Set(doctor_timetables.value.map((item) => item.startTime))].length;
    };

    const getTimeSlotByDate = (date) => {
        chosenDate.value = date;
        day_timeTable.value = doctor_timetables.value.filter((x) => x.date === date);
    };

    const redirect = (timetable) => {
        routeStore.setData(timetable);
        router.push("/make-appointment");
    };

    onMounted(async () => {
        var doctor_result = await ApiDoctorProfile.GetByID(doctorID ?? 0);
        if (doctor_result.data.isSucceeded) {
            doctor.value = doctor_result.data.data;
        }
        var doctor_timetables_result = await ApiTimetable.GetTimetableByDoctorID(doctorID ?? 0);
        if (doctor_timetables_result.data.isSucceeded) {
            doctor_timetables.value = doctor_timetables_result.data.data;
            doctor_timetables_day.value = [
                ...new Map(doctor_timetables.value.map((item) => [item.date, item])).values(),
            ];
            getTimeSlotByDate(doctor_timetables_day.value[0].date);
        }
    });
</script>

<template>
    <div class="mt-3 d-flex justify-content-center align-items-center">
        <div
            class="col-md-8 p-4 pt-5 doctor-search-item d-flex flex-column justify-content-between">
            <div style="cursor: pointer" class="d-flex">
                <div style="width: 180px; height: 180px" class="me-3 doctor-img rounded-circle">
                    <img :src="doctor.imageString" alt="doctorImg" />
                </div>

                <div class="me-3 doctor-search-info">
                    <h5 class="mb-2 fs-4">{{ doctor.doctorTitle }}. {{ doctor.fullName }}</h5>
                    <div class="d-flex align-items-center">
                        <h6>
                            <i class="bx bxs-badge-check"></i> Bác sĩ |
                            <span class="text-black fw-normal"
                                ><span style="font-weight: 500">{{
                                    dayjs().year() - doctor.startCareerYear
                                }}</span>
                                năm kinh nghiệm.</span
                            >
                        </h6>
                    </div>
                    <div class="d-flex align-items-center">
                        <p class="text-secondary m-0 fs-6 me-3">Chuyên khoa</p>
                        <div class="mb-1 d-flex flex-wrap">
                            <label
                                v-for="spe in doctor.specialties"
                                class="specialization-item-capsule"
                                >{{ spe.title }}</label
                            >
                        </div>
                    </div>
                    <div class="text-secondary me-3 d-flex align-items-center">
                        <p style="width: 130px" class="m-0 fs-6">Trình độ</p>
                        <span style="font-weight: 500" class="text-dark m-0 fs-6">{{
                            doctor.doctorTitle
                        }}</span>
                    </div>
                    <div class="text-secondary me-3 d-flex align-items-center">
                        <p style="width: 130px" class="m-0 fs-6">Nơi công tác</p>
                        <span style="font-weight: 500" class="text-dark m-0 fs-6">{{
                            doctor.address
                        }}</span>
                    </div>
                </div>
            </div>

            <div class="mt-3 d-flex flex-column">
                <h4>Đặt khám nhanh</h4>
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
                        <p>{{ dayjs(item.date).date() }} - {{ dayjs(item.date).month() + 1 }}</p>
                        <span v-if="getNumberOfTimeSlots(item.date) <= 0" class="text-danger"
                            >Đã đầy lịch</span
                        >
                        <span v-else>{{ getNumberOfTimeSlots(item.date) }} khung giờ</span>
                    </div>
                </div>
                <div class="mt-4 d-flex flex-column">
                    <h4>Khung giờ</h4>
                    <p v-if="day_timeTable.length <= 0" class="w-100 alert alert-warning m-0">
                        <span class="d-flex align-items-center"
                            ><span class="me-2"
                                ><i class="fs-2 text-danger bx bx-error-circle"></i
                            ></span>
                            Lịch làm việc trong ngày này đã hết. Vui lòng chọn ngày khác. Xin cảm ơn
                            !</span
                        >
                    </p>
                    <div v-else class="timeSlot-slot-container">
                        <template v-for="item in day_timeTable">
                            <div
                                @click="redirect(item)"
                                v-if="item.isAvailable"
                                class="timeSlot-slot">
                                {{ dayjs(item.startTime, "HH:mm:ss").format("HH:mm") }} -
                                {{ dayjs(item.endTime, "HH:mm:ss").format("HH:mm") }}
                            </div>
                            <div v-else="item.isAvailable" class="timeSlot-slot slot-unavailable">
                                {{ dayjs(item.startTime, "HH:mm:ss").format("HH:mm") }} -
                                {{ dayjs(item.endTime, "HH:mm:ss").format("HH:mm") }}
                            </div>
                        </template>
                    </div>
                </div>
            </div>
            <div class="mt-3 alert alert-warning d-flex flex-column">
                <span><i class="text-danger bx bx-error-alt"></i> Lưu ý</span>
                <span style="font-size: 15px"
                    >* Nếu bệnh nhân bận việc không đến khám được vui lòng hủy lịch khám đã đặt và
                    đặt lại ngày khác. Xin cảm ơn!</span
                >
            </div>
            <div class="mt-5">
                <h4>Giới thiệu</h4>
                <p>
                    {{ doctor.doctorDescription }}
                </p>
            </div>
            <div class="mt-5">
                <h4>Kinh nghiệm làm việc</h4>
                <div>
                    {{ doctor.doctorTitle }}. {{ doctor.fullName }} hiện đang công tác tại
                    {{ doctor.address }}. Bác sĩ có
                    {{ dayjs().year() - doctor.startCareerYear }} năm kinh nghiệm trong lĩnh vực
                    <div v-for="spe in doctor.specialties">
                        <a class="text-decoration-none">{{ spe.title }}</a>
                        <span>,</span>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
