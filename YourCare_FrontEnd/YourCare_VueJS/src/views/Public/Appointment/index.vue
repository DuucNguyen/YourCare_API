<script setup>
    import ApiAppointment from "@/api/ApiAppointment";
    import { useAuthStore } from "@/stores/auth-store";
    import { ref, reactive, onMounted } from "vue";
    import dayjs from "dayjs";
    //
    import UserSideBar from "@/shared/UserSideBar.vue";
    import { valHooks } from "jquery";

    const authStore = useAuthStore();

    const appointments = ref([]);
    const appointmentDetail = ref({});
    const chosenAppointment = ref({});

    const InitAppointments = async () => {
        var user = await authStore.getUserInfo();
        var result = await ApiAppointment.GetAllByUserID(user.id);
        if (result.data.isSucceeded) {
            appointments.value = result.data.data;
        }
    };

    const searchAppointment = () => {
        //do it here
    };

    const getAppointmentDetail = async (item) => {
        chosenAppointment.value = item;
        var result = await ApiAppointment.GetDetailById(chosenAppointment.value.id);
        if (result.data.isSucceeded) {
            appointmentDetail.value = result.data.data;
        }
    };

    onMounted(async () => {
        try {
            await InitAppointments();
        } catch (error) {
            console.log(error);
        }
    });
</script>

<template>
    <div class="d-flex">
        <UserSideBar page="history" />
        <div class="profile_container">
            <a-row class="profile_head">
                <a-col :span="24">
                    <span class="profile_head_title"> Lịch khám </span>
                </a-col>
            </a-row>
            <a-divider class="m-0"></a-divider>
            <a-row class="profile_body">
                <a-col :span="8" class="appointment_container">
                    <div class="appointment_search">
                        <a-input
                            @input="searchAppointment"
                            style="height: 40px"
                            placeholder="Mã phiếu khám, tên bệnh nhân,...">
                        </a-input>
                    </div>
                    <div>
                        <template v-for="item in appointments">
                            <div
                                :class="
                                    'appointment_item ' +
                                    (chosenAppointment.id == item.id
                                        ? 'appointment_item_chosen'
                                        : '')
                                "
                                @click="getAppointmentDetail(item)">
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
                                                dayjs(item.timetableStartTime, "HH:mm:ss").format(
                                                    "HH:mm",
                                                )
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
                                        <span class="text-secondary ms-3" style="font-size: 13px">
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
                </a-col>
                <a-col :span="15" class="appointment_detail_container">
                    <div v-if="chosenAppointment.id">
                        <a-row>
                            <a-col
                                :span="12"
                                class="text-start"
                                style="color: #22c55e; font-weight: 500; font-size: 20px">
                                STT: {{ chosenAppointment.timeTableOrder }}
                            </a-col>
                            <a-col :span="12" class="text-secondary d-flex justify-content-end">
                                <span class="fs-5 d-flex align-items-center">
                                    <i class="bx bx-error-alt fs-4 me-1"></i>
                                    {{ chosenAppointment.status }}
                                </span>
                            </a-col>
                        </a-row>
                        <a-row class="mt-3">
                            <a-col :span="12">
                                <img
                                    style="width: 75px; height: 100%; object-fit: contain"
                                    :src="appointmentDetail.doctorImage" />
                                <span class="ms-2" style="font-weight: 500; font-size: 18px">
                                    {{ appointmentDetail.doctorName }}
                                </span>
                            </a-col>
                            <a-col :span="12" class="d-flex justify-content-end">
                                <a-qrcode
                                    :size="100"
                                    :value="
                                        appointmentDetail.appointmentCode +
                                        ' / ' +
                                        appointmentDetail.doctorName +
                                        ' / ' +
                                        appointmentDetail.patientName +
                                        ' / ' +
                                        dayjs(appointmentDetail.timetableDate).format(
                                            'DD/MM/YYYY',
                                        ) +
                                        ' / STT: ' +
                                        appointmentDetail.timeTableOrder +
                                        ' / ' +
                                        appointmentDetail.timetableStartTime +
                                        '-' +
                                        appointmentDetail.timetableEndTime
                                    "></a-qrcode>
                            </a-col>
                        </a-row>
                        <a-row>
                            <a-col :span="24" class="appointment_detail_title">
                                <span> Thông tin lịch khám </span>
                            </a-col>
                        </a-row>
                        <div>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span>Mã phiếu khám</span>
                                </a-col>
                                <a-col :span="12">
                                    <span style="color: #1975dc; font-weight: 500">
                                        {{ appointmentDetail.appointmentCode }}
                                    </span>
                                </a-col>
                            </a-row>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span> Ngày khám </span>
                                </a-col>
                                <a-col :span="12">
                                    <span>
                                        {{
                                            dayjs(appointmentDetail.timetableDate).format(
                                                "DD/MM/YYYY",
                                            )
                                        }}
                                    </span>
                                </a-col>
                            </a-row>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span>Ca khám</span>
                                </a-col>
                                <a-col :span="12">
                                    <span>
                                        {{
                                            dayjs(
                                                appointmentDetail.timetableStartTime,
                                                "HH:mm:ss",
                                            ).format("HH:mm")
                                        }}
                                        -
                                        {{
                                            dayjs(
                                                appointmentDetail.timetableEndTime,
                                                "HH:mm:ss",
                                            ).format("HH:mm")
                                        }}
                                    </span>
                                </a-col>
                            </a-row>
                        </div>
                        <a-row>
                            <a-col :span="24" class="appointment_detail_title">
                                <span> Thông tin bệnh nhân </span>
                            </a-col>
                        </a-row>
                        <div>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span> Số thứ tự trong ca </span>
                                </a-col>
                                <a-col :span="12">
                                    <span>
                                        {{ appointmentDetail.timeTableOrder }}
                                    </span>
                                </a-col>
                            </a-row>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span> Họ và tên </span>
                                </a-col>
                                <a-col :span="12">
                                    <span>
                                        {{ appointmentDetail.patientName }}
                                    </span>
                                </a-col>
                            </a-row>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span> Ngày sinh </span>
                                </a-col>
                                <a-col :span="12">
                                    <span>
                                        {{
                                            appointmentDetail.patientDob != null
                                                ? dayjs(appointmentDetail.patientDob).format(
                                                      "DD/MM/YYYY",
                                                  )
                                                : "Chưa cập nhật"
                                        }}
                                    </span>
                                </a-col>
                            </a-row>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span> Số điện thoại </span>
                                </a-col>
                                <a-col :span="12">
                                    <span>
                                        {{
                                            appointmentDetail.patientPhoneNumber != null
                                                ? appointmentDetail.patientPhoneNumber
                                                : "Chưa cập nhật"
                                        }}
                                    </span>
                                </a-col>
                            </a-row>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span> Giới tính </span>
                                </a-col>
                                <a-col :span="12">
                                    <span>
                                        {{ appointmentDetail.patientGender == true ? "Nam" : "Nữ" }}
                                    </span>
                                </a-col>
                            </a-row>
                            <a-row class="appointment_detail_item">
                                <a-col :span="12">
                                    <span> Địa chỉ </span>
                                </a-col>
                                <a-col :span="12">
                                    <span>
                                        {{
                                            appointmentDetail.patientAddress != null &&
                                            appointmentDetail.patientAddress != ""
                                                ? appointmentDetail.patientAddress
                                                : "Chưa cập nhật"
                                        }}
                                    </span>
                                </a-col>
                            </a-row>
                        </div>
                    </div>
                </a-col>
            </a-row>
            <a-divider class="m-0"></a-divider>
            <a-row class="profile_footer">
                <a-col :span="24">
                    <span
                        >Tổng đài hỗ trợ và chăm sóc khách hàng:
                        <span class="text-primary">0865493798</span></span
                    >
                </a-col>
            </a-row>
        </div>
    </div>
</template>
<style scoped>
    .appointment_container {
        margin-left: 10px;
        padding: 5px;
        height: 500px;
        max-height: 500px;
        overflow-y: scroll;
        background: #f3f4f6;
        border-bottom: 1px solid #ddd;
    }

    .appointment_item {
        background-color: #fff;
        padding: 5px 8px;
        margin: 5px;
        border: 1px solid #ddd;
        border-radius: 5px;
        cursor: pointer;
    }
    .appointment_item:hover {
        border: 1px solid #1975dc;
        background: #f9fafb;
    }
    .appointment_item_chosen {
        border: 1px solid #1975dc;
        background: #f9fafb;
    }

    .appointment_search {
        position: sticky;
        top: 0;
        z-index: 1;
    }

    .appointment_detail_container {
        padding: 15px;
    }
    .appointment_detail_title {
        color: #1975dc;
        margin-top: 20px;
        background-color: #f1f5f9;
        padding: 5px;
        border-radius: 3px;
    }
    .appointment_detail_title span {
        font-size: 16px;
        font-weight: 500;
    }
    .appointment_detail_item {
        margin-top: 10px;
    }
    .appointment_detail_item span {
        font-size: 16px;
    }
    .appointment_detail_item :nth-child(2) {
        display: flex;
        justify-content: end;
    }
</style>
