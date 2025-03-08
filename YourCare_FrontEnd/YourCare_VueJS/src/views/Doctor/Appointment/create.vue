<script setup>
    import DoctorSideBar from "@/shared/DoctorSideBar.vue";

    import ApiAppointment from "@/api/ApiAppointment";
    import ApiTimetable from "@/api/ApiTimetable";
    import ApiPatientProfile from "@/api/ApiPatientProfile";

    import { message, notification } from "ant-design-vue";

    import { ref, onMounted, computed } from "vue";
    import { useAuthStore } from "@/stores/auth-store";
    import { useRouteStore } from "@/stores/route-store";
    import dayjs from "dayjs";
    import { useRouter } from "vue-router";
    import { useRoute } from "vue-router";

    const authStore = useAuthStore();
    const routeStore = useRouteStore();
    const router = useRouter();
    const route = useRoute();

    const appointmentID = ref(0);
    const appointment = ref({});

    const doctor_timetables = ref([]); //doctor schedule for the next 10 days + today
    const doctor_timetables_day = ref([]); //unique days in the list
    const day_timeTable = ref([]); //time slots of a day - dynamic
    const chosenDate = ref(); //chosen date item in doctor_timetables

    const timetable = ref({}); // chosen timetable
    const patientProfile = ref({});

    const appointmentFormState = computed(() => ({
        doctorID: appointment.value.doctorID,
        patientProfileID: appointment.value.patientProfileID,
        timetableID: timetable.value.id,
        timetableOrder: getTimetableOrder(timetable.value.availableSlots),
        isFollowUp: true,
        previousAppointmentID: appointment.value.id,
    }));

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

    const InitAppointment = async () => {
        appointmentID.value = route.params.id;
        if (!appointmentID.value) {
            message.error("Failed to get appointment info");
        }
        var result = await ApiAppointment.GetDetailById(appointmentID.value);
        if (result.data.isSucceeded) {
            appointment.value = result.data.data;
        }
    };

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

    const initPatientProfile = async () => {
        var result = await ApiPatientProfile.GetByID(appointment.value.patientProfileID);
        if (result.data.isSucceeded) {
            patientProfile.value = result.data.data;
        }
    };

    const generateCode = () => {
        var date = dayjs(chosenDate.value);

        var code =
            "YCA" +
            date.date().toString().padStart(2, "0") + //atmost 2 chars otherwise fill with "0"
            (date.month() + 1).toString().padStart(2, "0") + //dayjs return month from 0
            (date.year() % 100) +
            dayjs().minute().toString().padStart(2, "0") +
            dayjs().second().toString().padStart(2, "0");
        return code;
    };

    const onFinishAppointment = async () => {
        
        if (!appointmentFormState.value.timetableID) {
            message.error("Vui lòng chọn lịch khám");
            return;
        }

        var formData = new FormData();
        formData.append("doctorID", appointmentFormState.value.doctorID);
        formData.append("patientProfileID", appointmentFormState.value.patientProfileID);
        formData.append("timetableID", appointmentFormState.value.timetableID);
        formData.append("timetableOrder", appointmentFormState.value.timetableOrder);
        formData.append("isFollowUp", appointmentFormState.value.isFollowUp);
        formData.append("previousAppointmentID", appointmentFormState.value.previousAppointmentID);

        var code = generateCode();
        formData.append("appointmentCode", code);

        var result = await ApiAppointment.Create(formData);

        var type = result.data.isSucceeded ? "success" : "error";
        var description = result.data.message;

        showNotification(type, "Trạng thái đặt khám", description);

        // return new Promise((resolve) => {
        //     setTimeout(() => resolve(true), 5000);
        // });
        //redirect to appointment detail
    };

    const showNotification = (type, message, description) => {
        notification[type]({
            message: message,
            description: description,
        });
    };

    onMounted(async () => {
        await InitAppointment();
        await initTimetableData();
        await initPatientProfile();
    });
</script>

<template>
    <div class="doctor_dashboard_body">
        <DoctorSideBar active-item="appointment" />
        <div class="doctor_dashboard_section">
            <div class="doctor_dashboard_title">
                <h3 class="mb-3 mt-1">Đặt lịch tái khám</h3>
            </div>
            <div class="d-flex justify-content-between">
                <div class="col-md-8" style="max-height: 350px">
                    <div class="profile-patient">
                        <div class="profile-patient-title">
                            <div>
                                <span>#</span>
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
                                    <span v-else
                                        >{{ getNumberOfTimeSlots(item.date) }} khung giờ</span
                                    >
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
                                    <span class="d-flex align-items-center">
                                        <span class="me-2">
                                            <i class="fs-2 text-danger bx bx-error-circle"></i>
                                        </span>
                                        Lịch làm việc trong ngày này đã hết. Vui lòng chọn ngày
                                        khác. Xin cảm ơn !
                                    </span>
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
                                                {{
                                                    dayjs(item.startTime, "HH:mm:ss").format(
                                                        "HH:mm",
                                                    )
                                                }}
                                                -
                                                {{
                                                    dayjs(item.endTime, "HH:mm:ss").format("HH:mm")
                                                }}
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
                <div class="col-md-4 d-flex flex-column">
                    <div class="profile-doctor">
                        <div class="d-flex align-items-start">
                            <h5>Thông tin tái khám</h5>
                        </div>
                        <div class="profile-timetable-info">
                            <div class="info-row">
                                <span>Bệnh nhân</span>
                                <span id="appointmentPatientName">
                                    {{ patientProfile.name }}
                                </span>
                            </div>
                            <div class="info-row">
                                <span>Giới tính</span>
                                <span id="appointmentPatientName">
                                    {{ patientProfile.gender ? "Nam" : "Nữ" }}
                                </span>
                            </div>
                            <div class="info-row">
                                <span>Ngày sinh</span>
                                <span id="appointmentPatientName">
                                    {{
                                        patientProfile.dob
                                            ? dayjs(patientProfile.dob).format("DD/MM/YYYY")
                                            : "Chưa cập nhật"
                                    }}
                                </span>
                            </div>
                            <div class="info-row">
                                <span>SĐT</span>
                                <span id="appointmentPatientName">
                                    {{ patientProfile.phoneNumber ?? "Chưa cập nhật" }}
                                </span>
                            </div>
                        </div>
                        <div class="profile-timetable-info">
                            <div class="info-row">
                                <span>Ngày khám</span>
                                <span>{{ dayjs(timetable.date).format("DD/MM/YYYY") }}</span>
                            </div>
                            <div class="info-row">
                                <span>Ca khám</span>
                                <span>{{ timetable.startTime }} - {{ timetable.endTime }}</span>
                            </div>
                        </div>

                        <a-popconfirm
                            placement="bottom"
                            ok-text="Xác nhận"
                            :okButtonProps="{
                                type: 'primary',
                                style: {
                                    height: '30px',
                                    width: '150px',
                                },
                            }"
                            :cancelButtonProps="{
                                style: {
                                    height: '30px',
                                    width: '80px',
                                },
                            }"
                            cancel-text="Hủy"
                            @confirm="onFinishAppointment">
                            <template #title>
                                <span class="fs-6 text-primary">Xác nhận đặt lịch tái khám </span>
                                <br />
                                <span class="text-danger">
                                    Vui lòng kiểm tra lại thông tin đặt khám phía trên.
                                </span>
                            </template>
                            <a-button
                                type="primary"
                                style="height: 40px; font-weight: 500"
                                class="w-100 fs-6"
                                >Đặt lịch tái khám
                            </a-button>
                        </a-popconfirm>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
