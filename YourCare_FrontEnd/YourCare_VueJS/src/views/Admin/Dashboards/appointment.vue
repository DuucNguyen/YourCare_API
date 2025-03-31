<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";
    import ApiAppointment from "@/api/ApiAppointment";
    import { ref, onMounted, reactive } from "vue";
    import AppointmentStatus from "@/constants/AppointmentStatus";
    import dayjs from "dayjs";

    const appointments = ref([]); //list
    const appointment = ref({}); //chosen
    const appointmentDetail = ref(); //detail get from appointment.value

    const date = ref(dayjs());

    const InitAppointment = async () => {
        var result_appointment = await ApiAppointment.GetAllAppointmentByDate(
            dayjs(date.value).format("MM/DD/YYYY"),
        );
        if (result_appointment.data.isSucceeded) {
            appointments.value = result_appointment.data.data;
        }
    };
    const GetAppointmentDetail = async (item) => {
        appointment.value = item;
        var result = await ApiAppointment.GetDetailById(appointment.value.id);
        if (result.data.isSucceeded) {
            appointmentDetail.value = result.data.data;
        }
    };
    onMounted(async () => {
        await InitAppointment();
        console.log(appointments.value);
    });
</script>
<template>
    <div class="admin_dashboard">
        <AdminSideBar active-item="appointment" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_body">
                <div class="col-md-4 admin_appointment_list">
                    <div v-show="appointments.length > 0" class="appointment_container">
                        <div>
                            <template v-for="item in appointments">
                                <div
                                    :class="
                                        'appointment_item ' +
                                        (appointment.id == item.id ? 'appointment_item_chosen' : '')
                                    "
                                    @click="GetAppointmentDetail(item)">
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
                                                :class="
                                                    (item.status === AppointmentStatus.COMPLETED
                                                        ? 'text-primary'
                                                        : 'text-secondary') + ' ms-3'
                                                "
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
                    </div>
                </div>
                <div class="col-md-8">
                    <a-col :span="23" class="appointment_detail_container">
                        <div v-if="appointmentDetail">
                            <div id="shrinkable_appointment_detail">
                                <a-row>
                                    <a-col
                                        :span="12"
                                        class="text-start"
                                        style="color: #22c55e; font-weight: 500; font-size: 20px">
                                        STT: {{ appointment.timeTableOrder }}
                                    </a-col>
                                    <a-col
                                        :span="12"
                                        class="text-secondary d-flex justify-content-end">
                                        <span class="fs-5 d-flex align-items-center">
                                            <i class="bx bx-error-alt fs-4 me-1"></i>
                                            {{ appointment.status }}
                                        </span>
                                    </a-col>
                                </a-row>
                                <a-row class="mt-1">
                                    <a-col :span="12">
                                        <img
                                            style="width: 75px; height: 100%; object-fit: contain"
                                            :src="appointmentDetail.doctorImage" />
                                        <span
                                            class="ms-2"
                                            style="font-weight: 500; font-size: 18px">
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
                                                        ? dayjs(
                                                              appointmentDetail.patientDob,
                                                          ).format("DD/MM/YYYY")
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
                                                {{
                                                    appointmentDetail.patientGender == true
                                                        ? "Nam"
                                                        : "Nữ"
                                                }}
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
                                    <a-row class="appointment_detail_item">
                                        <a-col :span="12">
                                            <span>Lời nhắn</span>
                                        </a-col>
                                        <a-col :span="12">
                                            <a-textarea
                                                :value="appointmentDetail.patientNote"
                                                placeholder="Không có lời nhắn"
                                                readonly>
                                                {{ appointmentDetail.patientNote }}
                                            </a-textarea>
                                        </a-col>
                                    </a-row>
                                </div>
                            </div>
                            <div
                                style="cursor: pointer"
                                class="text-primary fs-6 d-flex align-items-center justify-content-center m-2 mt-4 up"
                                @click="toggleSlide">
                                Thu gọn <i class="bx bx-chevron-up fs-4"></i>
                            </div>
                            <a-row>
                                <a-col :span="24" class="appointment_detail_title">
                                    <span> Kết quả </span>
                                </a-col>
                            </a-row>
                            <div>
                                <a-form ref="formRef">
                                    <a-form-item name="doctorDiagnosis">
                                        <a-row class="appointment_detail_item">
                                            <a-col :span="24">
                                                <span>Chuẩn đoán</span>
                                            </a-col>
                                            <a-col :span="24">
                                                <a-textarea
                                                    readonly
                                                    placeholder="Chuẩn đoán, kết luận của bác sĩ,..."></a-textarea>
                                            </a-col>
                                        </a-row>
                                    </a-form-item>
                                    <a-form-item name="doctorNote">
                                        <a-row class="appointment_detail_item">
                                            <a-col :span="24">
                                                <span>Ghi chú của Bác sĩ</span>
                                            </a-col>
                                            <a-col :span="24">
                                                <a-textarea
                                                    readonly
                                                    placeholder="VD: Lịch tái khám, thuốc, lời khuyên, ..."></a-textarea>
                                            </a-col>
                                        </a-row>
                                    </a-form-item>
                                    <a-form-item name="files">
                                        <a-row class="appointment_detail_item">
                                            <a-col :span="24">
                                                <span>Tệp đính kèm</span>
                                            </a-col>
                                            <a-col
                                                :span="24"
                                                v-if="
                                                    !appointmentDetail.files ||
                                                    appointmentDetail.status !==
                                                        AppointmentStatus.COMPLETED
                                                ">
                                            </a-col>
                                            <a-col v-else :span="24">
                                                <div
                                                    v-for="(path, index) in appointmentDetail.files"
                                                    class="d-flex align-items-center">
                                                    <span>
                                                        {{
                                                            (index + 1).toString().padStart(2, "0")
                                                        }}
                                                        -
                                                        {{ path.split("\\")[1] }}
                                                    </span>
                                                    <a-button
                                                        @click="DownloadFile(path)"
                                                        type="primary"
                                                        class="d-flex align-items-center">
                                                        <VerticalAlignBottomOutlined />
                                                    </a-button>
                                                </div>
                                            </a-col>
                                        </a-row>
                                    </a-form-item>
                                </a-form>
                            </div>
                        </div>
                        <div v-else class="h-100 d-flex align-items-center justify-content-center">
                            <a-empty>
                                <template #description>
                                    <span>Không có thông tin</span>
                                </template>
                            </a-empty>
                        </div>
                    </a-col>
                </div>
            </div>
        </div>
    </div>
</template>
<style scoped>
    .admin_dashboard {
        display: flex;
    }
    .admin_dashboard_body {
        display: flex;
        padding-left: 30px;
    }
    .admin_dashboard_section {
        flex-grow: 1;
    }
    .admin_appointment_list {
        background-color: #fff;
        border-radius: 8px;
    }
    .admin_appointment_container {
        background: #ddd;
    }

    .appointment_container {
        margin: 10px;
        padding: 5px;
        height: 630px;
        max-height: 630px;
        overflow-y: scroll;
        background: #f3f4f6;
        border-bottom: 1px solid #ddd;
    }
    .appointment_detail_container {
        padding: 10px;
        width: 100%;
        border-radius: 8px;
        height: 670px;
        max-height: 670px;
        overflow-y: scroll;
    }
</style>
