<script setup>
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiTimetable from "@/api/ApiTimetable";
    import ApiPatientProfile from "@/api/ApiPatientProfile";

    //
    import { useRouteStore } from "@/stores/route-store";
    import { useAuthStore } from "@/stores/auth-store";

    import { useRoute } from "vue-router";
    import { ref, reactive, onMounted, nextTick } from "vue";
    import dayjs from "dayjs";

    //
    import { Modal, notification } from "ant-design-vue";
    import { createVNode } from "vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { icons } from "ant-design-vue/es/image/PreviewGroup";
    import { Item } from "ant-design-vue/es/menu";

    const actions = {
        create: "Tạo mới",
        update: "Chỉnh sửa",
    };

    const createModalInvisible = ref(false);
    const modalTitle = ref(actions.create);

    //
    const createForm = ref();
    const formState = reactive({
        name: "",
        gender: true,
        email: "",
        phoneNumber: "",
        address: "",
        dob: "",
        identityNumber: "",
        insuranceNumber: "",
        career: "",
        ethnic: "",
    });

    const rules = {
        name: [
            {
                required: true,
                message: "Vui lòng điền Họ và tên",
                trigger: "change",
            },
        ],
        gender: [
            {
                required: true,
                message: "Vui lòng chọn giới tính",
            },
        ],
        phoneNumber: [
            {
                required: true,
                message: "Vui lòng điền số điện thoại",
                trigger: "change",
            },
        ],
        dob: [
            {
                required: true,
                message: "Vui lòng chọn ngày tháng năm sinh",
                trigger: "change",
            },
        ],
        address: [
            {
                required: true,
                message: "Vui lòng điền địa chỉ",
                trigger: "change",
            },
        ],
    };

    const routeStore = useRouteStore();
    const authstore = useAuthStore();
    const route = useRoute();

    //patientProfile
    const patientProfiles = ref([]);
    const chosenPatientProfile = ref({});

    const timetable = ref({});
    const doctor = ref({});
    //doctor-timtable
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

    const toggleSlide = (event, slideID) => {
        //toggle section 1||2
        var classes = event.currentTarget.classList;
        event.currentTarget.classList.toggle("up");

        if (classes.contains("up")) {
            event.currentTarget.innerHTML = "<i class='bx bx-chevron-up'></i>";
        } else {
            event.currentTarget.innerHTML = "<i class='bx bx-chevron-down'></i>";
        }
        $(slideID).slideToggle();
    };

    const selectTimetable = (item) => {
        timetable.value = item;
    };

    const togglePatientInfo = (id, button) => {
        var $button = $(button);
        var $body = $("#profileBody-" + id);

        if ($button.hasClass("up")) {
            $button.innerHTML = "<i class='bx bx-chevron-up'></i>";
        } else {
            $button.innerHTML = "<i class='bx bx-chevron-down'></i>";
        }
        $body.slideToggle();
    };

    //init functions
    const initTimetableData = async () => {
        if (timetable.value) {
            var doctor_result = await ApiDoctorProfile.GetByID(timetable.value.doctorID);
            if (doctor_result.data.isSucceeded) {
                doctor.value = doctor_result.data.data;
            }
        }
    };

    const initDoctorData = async () => {
        var doctor_timetables_result = await ApiTimetable.GetTimetableByDoctorID(
            timetable.value.doctorID ?? 0,
        );
        if (doctor_timetables_result.data.isSucceeded) {
            doctor_timetables.value = doctor_timetables_result.data.data;
            doctor_timetables_day.value = [
                ...new Map(doctor_timetables.value.map((item) => [item.date, item])).values(),
            ];
            getTimeSlotByDate(doctor_timetables_day.value[0].date);
        }
    };

    const initPatientProfileData = async () => {
        var user = await authstore.getUserInfo();
        var patientProfile_data = await ApiPatientProfile.GetAllByUserID(user.id);
        if (patientProfile_data.data.isSucceeded) {
            patientProfiles.value = patientProfile_data.data.data;
            chosenPatientProfile.value = patientProfiles.value[0];
        }
    };

    const openUpdateModal = (profile) => {
        chosenPatientProfile.value = profile;
        modalTitle.value = actions.update;
        createModalInvisible.value = true;
        formState.name = chosenPatientProfile.value.name ?? "";
        formState.gender = chosenPatientProfile.value.gender ?? true;
        formState.phoneNumber = chosenPatientProfile.value.phoneNumber ?? "";
        formState.address = chosenPatientProfile.value.address ?? "";
        formState.dob = dayjs(chosenPatientProfile.value.dob) ?? "";
        formState.email = chosenPatientProfile.value.email ?? "";
        formState.identityNumber = chosenPatientProfile.value.identityNumber ?? "";
        formState.insuranceNumber = chosenPatientProfile.value.insuranceNumber ?? "";
        formState.ethnic = chosenPatientProfile.value.ethnic ?? "";
        formState.career = chosenPatientProfile.value.career ?? "";
    };

    const onFinish = (action) => {
        createForm.value
            .validate()
            .then(() => {
                showModalConfirmation(action);
            })
            .catch((error) => {
                console.log(error);
            });
    };

    const showModalConfirmation = (action) => {
        Modal.confirm({
            title: "Bạn có chắc chắn muốn thực hiện hành động này không ?",
            icons: createVNode(ExclamationCircleOutlined),
            okText: "Đồng ý",
            cancelText: "Hủy",
            async onOk() {
                var user = await authstore.getUserInfo();
                var formData = new FormData();

                formData.append("gender", formState.gender);
                formData.append("name", formState.name);
                formData.append("address", formState.address);
                formData.append("applicationUserID", user.id);
                formData.append("dob", formState.dob);
                formData.append("phoneNumber", formState.phoneNumber);
                formData.append("email", formState.email);
                formData.append("identityNumber", formState.identityNumber);
                formData.append("insuranceNumber", formState.insuranceNumber);
                formData.append("career", formState.career);
                formData.append("ethnic", formState.ethnic);

                var result = null;
                if (action === actions.create) {
                    result = await ApiPatientProfile.Create(formData);
                } else {
                    formData.append("id", chosenPatientProfile.value.id);
                    result = await ApiPatientProfile.Update(formData);
                }

                var type = result.data.isSucceeded ? "success" : "error";
                var description = result.data.message;

                showNotification(type, "Trạng thái hành động", description);
            },
        });
    };

    const showNotification = (type, message, description) => {
        notification[type]({
            message: message,
            description: description,
        });
    };

    onMounted(async () => {
        timetable.value = routeStore.data;
        await initTimetableData();
        await initDoctorData();
        await initPatientProfileData();

        $("#profile-patient-timeSlot").hide();
        nextTick(() => {
            //run after rendered and mounted in DOM
            $(".profile-item-body").each((index, element) => {
                $(element).hide();
            });
        });
    });
</script>

<template>
    <form method="POST" id="form-appointment">
        <div class="d-flex justify-content-between">
            <div class="col-md-8" style="max-height: 350px">
                <div class="profile-patient">
                    <div class="profile-patient-title">
                        <div>
                            <span>1</span>
                            <span> - Ngày và giờ khám</span>
                        </div>
                        <button
                            @click="toggleSlide($event, '#profile-patient-timeSlot')"
                            type="button"
                            class="btn-slideToggle"
                            id="profile-patient-timeSlot-btn">
                            <i class="bx bx-chevron-down"></i>
                        </button>
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
                <div class="mt-3 profile-patient">
                    <div class="profile-patient-title">
                        <div>
                            <span>2</span>
                            <span> - Hồ sơ bệnh nhân</span>
                        </div>
                        <button
                            @click="toggleSlide($event, '#profile-patient-profile')"
                            type="button"
                            id="profile-patient-profile-btn"
                            class="btn-slideToggle up">
                            <i class="bx bx-chevron-up"></i>
                        </button>
                    </div>
                    <div class="w-100" id="profile-patient-profile">
                        <div class="profile-container">
                            <div v-for="profile in patientProfiles" class="d-flex flex-column">
                                <div
                                    :id="'profileHead-' + profile.id"
                                    :class="
                                        'profile-item-head ' +
                                        (chosenPatientProfile.id === profile.id
                                            ? 'profile-item-head-chosen'
                                            : '')
                                    ">
                                    <div class="d-flex w-100">
                                        <div class="me-2">
                                            <input
                                                v-model="chosenPatientProfile"
                                                :value="profile"
                                                name="patient-profile"
                                                type="radio"
                                                class="form-check-input"
                                                :id="'profile-' + profile.id" />
                                        </div>

                                        <label class="d-flex w-100" :for="'profile-' + profile.id">
                                            <div class="profile-item-img">
                                                <img
                                                    src="https://upload.wikimedia.org/wikipedia/commons/thumb/5/59/User-avatar.svg/2048px-User-avatar.svg.png" />
                                            </div>
                                            <div class="profile-item-info">
                                                <span>{{ profile.name }}</span>
                                                <span class="text-secondary">{{
                                                    profile.dob != null
                                                        ? dayjs(profile.dob).format("DD/MM/YYYY")
                                                        : ""
                                                }}</span>
                                            </div>
                                        </label>
                                    </div>
                                    <div>
                                        <button
                                            type="button"
                                            @click="togglePatientInfo(profile.id, this)"
                                            class="btn-slideToggle-outline">
                                            <i class="bx bx-chevron-down"></i>
                                        </button>
                                    </div>
                                </div>
                                <div :id="'profileBody-' + profile.id" class="profile-item-body">
                                    <div class="profile-item-body-info">
                                        <span>Họ và tên</span>
                                        <span>{{
                                            profile.name != null ? profile.name : "--"
                                        }}</span>
                                    </div>
                                    <div class="profile-item-body-info">
                                        <span>Giới tính</span>
                                        <span>{{ profile.gender == true ? "Nam" : "Nữ" }}</span>
                                    </div>
                                    <div class="profile-item-body-info">
                                        <span>Ngày sinh</span>
                                        <span>{{
                                            profile.dob != null
                                                ? dayjs(profile.dob).format("DD/MM/YYYY")
                                                : ""
                                        }}</span>
                                    </div>
                                    <div class="profile-item-body-info">
                                        <span>Số điện thoại</span>
                                        <span>{{
                                            profile.phoneNumber != null ? profile.phoneNumber : "--"
                                        }}</span>
                                    </div>
                                    <button
                                        type="button"
                                        class="btn-slideToggle-outline"
                                        id="btn-update-profile"
                                        @click="openUpdateModal(profile)">
                                        Chỉnh sửa hồ sơ
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="mt-3 appointment-info-item">
                        <label for="patientNote">Ghi chú của bệnh nhân: </label>
                        <textarea
                            placeholder="Thông tin bổ sung - VD: triệu chứng, bệnh lý, thuốc đang xử dung,..."></textarea>
                    </div>
                    <div>
                        <button
                            @click="
                                createModalInvisible = true;
                                modalTitle = actions.create;
                            "
                            type="button"
                            title="Tạo hồ sơ mới"
                            class="p-2 btn-slideToggle">
                            Thêm hồ sơ mới <i class="bx bx-user-plus"></i>
                        </button>
                    </div>
                </div>
            </div>
            <div class="col-md-4 d-flex flex-column">
                <div class="profile-doctor">
                    <div class="d-flex align-items-start">
                        <h5>Thông tin đặt khám</h5>
                    </div>
                    <div class="profile-doctor-info">
                        <div style="width: 80px; height: 80px" class="doctor-img">
                            <img :src="doctor.imageString" alt="avatar" />
                        </div>
                        <div class="ms-3">
                            <p class="fs-5 m-0">
                                Bác sĩ <span style="font-weight: 500">{{ doctor.fullName }}</span>
                            </p>
                            <p class="fs-6 m-0">{{ doctor.doctorTitle }}</p>
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
                        <div class="info-row">
                            <span>Bệnh nhân</span>
                            <span id="appointmentPatientName">{{ chosenPatientProfile.name }}</span>
                        </div>
                    </div>
                    <button type="submit" class="w-100 btn btn-primary fw-bold pt-2 pb-2">
                        Xác nhận đặt khám
                    </button>
                </div>
            </div>
        </div>
    </form>

    <a-modal
        width="800px"
        :title="modalTitle + ' hồ sơ bệnh nhân '"
        v-model:open="createModalInvisible"
        centered>
        <a-divider></a-divider>
        <a-form layout="vertical" :model="formState" :rules="rules" ref="createForm">
            <a-row :gutter="24">
                <a-col :span="12">
                    <a-form-item label="Họ và tên" name="name">
                        <a-input
                            placeholder="Họ và tên bệnh nhân"
                            v-model:value="formState.name"></a-input>
                    </a-form-item>
                </a-col>
                <a-col :span="12">
                    <a-form-item label="Số điện thoại" name="phoneNumber">
                        <a-input placeholder="SĐT" v-model:value="formState.phoneNumber"></a-input>
                    </a-form-item>
                </a-col>
            </a-row>
            <a-row :gutter="24">
                <a-col :span="12">
                    <a-form-item label="Ngày sinh" name="dob">
                        <a-date-picker
                            placeholder="Ngày tháng năm sinh"
                            style="width: 100%"
                            v-model:value="formState.dob"></a-date-picker>
                    </a-form-item>
                </a-col>
                <a-col :span="12">
                    <a-form-item label="Giới tính" name="gender">
                        <a-radio-group v-model:value="formState.gender">
                            <a-radio :value="true">Nam</a-radio>
                            <a-radio :value="false">Nữ</a-radio>
                        </a-radio-group>
                    </a-form-item>
                </a-col>
            </a-row>
            <a-row :gutter="24">
                <a-col :span="8">
                    <a-form-item label="Email" name="email">
                        <a-input
                            placeholder="Đại chỉ email bệnh nhân"
                            v-model:value="formState.email"></a-input>
                    </a-form-item>
                </a-col>
                <a-col :span="8">
                    <a-form-item label="Địa chỉ" name="address">
                        <a-input
                            placeholder="Địa chỉ nơi ở cụ thể"
                            v-model:value="formState.address"></a-input>
                    </a-form-item>
                </a-col>
                <a-col :span="8">
                    <a-form-item label="Nghề nghiệp" name="career">
                        <a-input
                            placeholder="Nghề nghiệp hiện tại"
                            v-model:value="formState.career"></a-input>
                    </a-form-item>
                </a-col>
            </a-row>
            <a-row :gutter="24">
                <a-col :span="8">
                    <a-form-item label="Số CMND/CCCD" name="identityNumber">
                        <a-input
                            placeholder="Số CMND hoặc CCCD"
                            v-model:value="formState.identityNumber"></a-input>
                    </a-form-item>
                </a-col>
                <a-col :span="8">
                    <a-form-item label="Số BHYT" name="insuranceNumber">
                        <a-input
                            placeholder="Mã số trên thẻ bảo hiểm y tế"
                            v-model:value="formState.insuranceNumber"></a-input>
                    </a-form-item>
                </a-col>
                <a-col :span="8">
                    <a-form-item label="Dân tộc" name="ethnic">
                        <a-input placeholder="Dân tộc" v-model:value="formState.ethnic"></a-input>
                    </a-form-item>
                </a-col>
            </a-row>
        </a-form>
        <template #footer>
            <a-button key="cancel" @click="createModalInvisible = false">Hủy</a-button>
            <a-button key="submit" type="primary" @click="onFinish(modalTitle)">{{
                modalTitle
            }}</a-button>
        </template>
    </a-modal>
</template>
