<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";

    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiTimeSlot from "@/api/ApiTimeSlot";
    import ApiTimetable from "@/api/ApiTimetable";
    import { ref, onMounted, onBeforeMount, reactive, watch } from "vue";
    import { useRoute } from "vue-router";
    import dayjs from "dayjs";
    import isSameOrAfter from "dayjs/plugin/isSameOrAfter";
    import isSameOrBefore from "dayjs/plugin/isSameOrBefore";

    import { PlusOutlined, ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { createVNode } from "vue";
    import { Modal } from "ant-design-vue";
    import { notification } from "ant-design-vue";

    dayjs.extend(isSameOrAfter);
    dayjs.extend(isSameOrBefore);

    const route = useRoute();
    const doctor = ref({});
    const doctorID = route.params.id || 0;

    const timeSlots = ref([]);
    const timeSlots_morning = ref([]);
    const timeSlots_afternoon = ref([]);

    const morningStart = dayjs().hour(6).minute(0).second(0);
    const morningEnd = dayjs().hour(11).minute(59).second(59);
    const afternoonStart = dayjs().hour(12).minute(0).second(0);
    const afternoonEnd = dayjs().hour(18).minute(59).second(59);

    const checkBox_state = reactive({
        indeterminate: false,
        checkAll: false,
        checkedList_morning: [],
        checkedList_afternoon: [],
        checkedList_All: [],
        range: [],
    });

    const onCheckAllChange = (e) => {
        Object.assign(checkBox_state, {
            checkedList_All: e.target.checked ? timeSlots.value.map((x) => x.id) : [],
            checkedList_morning: e.target.checked ? timeSlots_morning.value.map((x) => x.id) : [],
            checkedList_afternoon: e.target.checked
                ? timeSlots_afternoon.value.map((x) => x.id)
                : [],
            indeterminate: e.target.checked,
        });
    };

    watch(
        () => [...checkBox_state.checkedList_morning, ...checkBox_state.checkedList_afternoon],
        () => {
            var totalLength =
                checkBox_state.checkedList_morning.length +
                checkBox_state.checkedList_afternoon.length;
            checkBox_state.indeterminate = totalLength && totalLength < timeSlots.value.length;
            checkBox_state.checkAll = totalLength === timeSlots.value.length;
            checkBox_state.checkedList_All = [
                ...checkBox_state.checkedList_morning,
                ...checkBox_state.checkedList_afternoon,
            ];
        },
    );

    const convertTime = (timeStr) => {
        const [h, m, s] = timeStr.split(":").map(Number);
        return dayjs().hour(h).minute(m).second(s);
    };

    onBeforeMount(async () => {
        var doc_result = await ApiDoctorProfile.GetByID(doctorID);
        doctor.value = doc_result.data.data;

        var timeSlots_result = await ApiTimeSlot.GetAll();
        timeSlots.value = timeSlots_result.data.data;

        timeSlots.value.forEach((item) => {
            const start = convertTime(item.startTime);
            const end = convertTime(item.endTime);

            if (start.isSameOrAfter(morningStart) && end.isSameOrBefore(morningEnd)) {
                timeSlots_morning.value.push(item);
            }
            if (start.isSameOrAfter(afternoonStart) && end.isSameOrBefore(afternoonEnd)) {
                timeSlots_afternoon.value.push(item);
            }
        });
    });

    const formRef = ref();
    const rules = {
        timeSlots: [
            {
                validator: () => validateTimeSlot(),
                message: "Choose at leat one item before continue.",
                trigger: "change",
            },
        ],
        range: [
            {
                required: true,
                message: "Input cannot be null",
                trigger: "change",
            },
        ],
    };

    const validateTimeSlot = () => {
        return checkBox_state.checkedList_afternoon.length +
            checkBox_state.checkedList_morning.length <=
            0
            ? Promise.reject("Choose at least one item before continue.")
            : Promise.resolve();
    };

    const onFinish = () => {
        formRef.value
            .validate()
            .then(() => {
                showModalConfirmation();
            })
            .catch((error) => {
                console.log("INvalidate: " + error);
            });
    };

    const showModalConfirmation = () => {
        Modal.confirm({
            title: "Are you sure to perform this action ?",
            icon: createVNode(ExclamationCircleOutlined),
            content: "Schedule timetable for the doctor. ",
            okText: "Yes",
            cancelText: "No",
            onOk: async () => {
                var formData = new FormData();
                formData.append("id", doctorID);
                checkBox_state.checkedList_All.forEach((item, index) => {
                    formData.append(`timetableIDs[${index}]`, item);
                });
                formData.append("startDate", checkBox_state.range[0]);
                formData.append("endDate", checkBox_state.range[1]);

                var result = await ApiTimetable.CreateRange(formData);
                var description = result.data.message;
                var type = result.data.isSucceeded ? "success" : "error";

                showNotification(type, "Create status", description);
            },
            onCancel: () => {},
        });
    };

    const showNotification = (type, message, description) => {
        notification[type]({
            message: message,
            description: description,
        });
    };
</script>

<template>
    <div class="admin_dashboard">
        <AdminSideBar active-item="doctor" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_body">
                <div v-if="doctor" class="d-flex justify-content-evenly">
                    <div class="col-md-4">
                        <div
                            class="p-3 shadow rounded-3 d-flex flex-column align-items-center"
                            style="background-color: #fff">
                            <div class="doctor-img">
                                <img :src="doctor.imageString" alt="avatar" />
                            </div>
                            <div class="doctor-title">
                                <span> {{ doctor.doctorTitle }}. </span>
                                <span> {{ doctor.fullName }}</span>
                            </div>
                            <div class="mb-3 d-flex flex-wrap align-items-center">
                                <label
                                    v-for="specialty in doctor.specialties"
                                    class="specialization-item-capsule"
                                    >{{ specialty.title }}</label
                                >
                            </div>
                            <div class="d-flex flex-column">
                                <div>
                                    <label style="width: 100px" class="">Phone: </label>
                                    <span class="text-decoration-underline">{{
                                        doctor.phoneNumber
                                    }}</span>
                                </div>
                                <div>
                                    <label style="width: 100px" class="">Email: </label>
                                    <span class="text-decoration-underline">{{
                                        doctor.email
                                    }}</span>
                                </div>
                                <div>
                                    <label style="width: 100px" class="">Address: </label>
                                    <span class="text-decoration-underline">{{
                                        doctor.address
                                    }}</span>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-md-7" style="background-color: #fff">
                        <div class="p-3 shadow rounded-3">
                            <h4 class="text-center">Doctor schedule</h4>
                            <div class="mb-4">
                                <a-checkbox
                                    class="timeTable-dayTime-item"
                                    v-model:checked="checkBox_state.checkAll"
                                    :indeterminate="checkBox_state.indeterminate"
                                    @change="onCheckAllChange"
                                    >Check all</a-checkbox
                                >
                            </div>
                            <div class="d-flex flex-column">
                                <a-form :model="checkBox_state" ref="formRef" :rules="rules">
                                    <a-form-item name="timeSlots">
                                        <div class="timeTable-dayTime-title">
                                            <span> <i class="bx bx-sun"></i> Morning </span>
                                            <div class="timeTable-dayTime">
                                                <a-checkbox-group
                                                    v-model:value="
                                                        checkBox_state.checkedList_morning
                                                    ">
                                                    <a-checkbox
                                                        v-for="item in timeSlots_morning"
                                                        :key="item.id"
                                                        class="timeTable-dayTime-item"
                                                        :value="item.id">
                                                        {{ item.startTime }} - {{ item.endTime }}
                                                    </a-checkbox>
                                                </a-checkbox-group>
                                            </div>
                                        </div>
                                        <div class="timeTable-dayTime-title">
                                            <span> <i class="bx bxs-sun"></i> Afternoon </span>
                                            <div class="timeTable-dayTime">
                                                <a-checkbox-group
                                                    v-model:value="
                                                        checkBox_state.checkedList_afternoon
                                                    ">
                                                    <a-checkbox
                                                        v-for="item in timeSlots_afternoon"
                                                        :key="item.id"
                                                        class="timeTable-dayTime-item"
                                                        :value="item.id">
                                                        {{ item.startTime }} - {{ item.endTime }}
                                                    </a-checkbox>
                                                </a-checkbox-group>
                                            </div>
                                        </div>
                                    </a-form-item>
                                    <a-form-item
                                        style="width: 100%"
                                        class="w-100 mt-5 timeTable-dayTime-title"
                                        name="range">
                                        <span> <i class="bx bx-calendar"></i> Range </span>
                                        <div class="timeTable-dayTime">
                                            <a-range-picker
                                                style="width: 100%"
                                                v-model:value="checkBox_state.range" />
                                        </div>
                                    </a-form-item>
                                    <a-form-item class="d-flex justify-content-end">
                                        <a-button @click="onFinish" type="primary">Save</a-button>
                                    </a-form-item>
                                </a-form>
                            </div>
                        </div>
                    </div>
                </div>
                <div v-else>Doctor not found.</div>
            </div>
        </div>
    </div>
</template>

<style scoped>
    /*.doctor-profile-vertical{
    border-radius: 25px;
    padding: 20px;
    box-shadow: 1px 1px 10px #ddd;
}*/
    .doctor-img {
        width: 96px;
        height: 96px;
        border-radius: 50px;
        overflow: hidden;
    }

    .doctor-img img {
        max-width: 100%;
        object-fit: cover;
    }
    .timeTable-dayTime-title {
        border-top: 1px solid #ddd;
        display: flex;
        position: relative;
        width: 100%;
    }

    .timeTable-dayTime-title span {
        display: flex;
        align-items: center;
        text-align: center;
        font-weight: 500;
        background: #fff;
        position: absolute;
        left: 0px;
        top: -15px;
        color: #1975dc;
    }

    .timeTable-dayTime {
        padding: 30px 10px;
        display: flex;
        flex-wrap: wrap;
        align-items: flex-start;
    }

    .timeTable-dayTime input {
        padding: 5px;
        border: 1px solid #1975dc;
        border-radius: 5px;
    }

    .timeTable-dayTime label {
        font-weight: 500;
        color: #1975dc;
    }

    .timeTable-dayTime input:focus {
        outline: none;
    }

    .timeTable-dayTime-item {
        height: 45px;
        display: flex;
        align-items: center;
        width: fit-content;
        margin: 5px;
        padding: 8px;
        border: 1px solid #1975dc;
        border-radius: 5px;
        color: #1975dc;
        font-weight: 500;
        cursor: pointer;
    }

    .timeTable-dayTime-item:hover {
        color: #fff;
        background: #1975dc;
    }
</style>
