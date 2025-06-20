<script setup>
    import ApiAppointment from "@/api/ApiAppointment";
    import { useAuthStore } from "@/stores/auth-store";
    import { ref, reactive, onMounted, createVNode } from "vue";
    import dayjs from "dayjs";
    //
    import UserSideBar from "@/shared/UserSideBar.vue";
    import { valHooks } from "jquery";
    import AppointmentStatus from "@/constants/AppointmentStatus";
    import { message, Modal } from "ant-design-vue";
    import { ExclamationCircleOutlined, VerticalAlignBottomOutlined } from "@ant-design/icons-vue";

    const authStore = useAuthStore();

    const appointments = ref([]);
    const appointmentDetail = ref({});
    const chosenAppointment = ref({});
    const spinning = ref(false);

    const formState = reactive({
        files: [],
    });

    const InitAppointments = async () => {
        spinning.value = true;
        var user = await authStore.getUserInfo();
        var result = await ApiAppointment.GetAllByUserID(user.id);
        if (result.data.isSucceeded) {
            appointments.value = result.data.data;
        }
        spinning.value = false;
    };

    const searchAppointment = () => {
        //do it here
    };

    const GetAppointmentDetail = async (item) => {
        chosenAppointment.value = item;
        var result = await ApiAppointment.GetDetailById(chosenAppointment.value.id);
        if (result.data.isSucceeded) {
            appointmentDetail.value = result.data.data;
        }
    };

    const onCancelAppointment = () => {
        Modal.confirm({
            title: "Xác nhận hủy lịch khám.",
            icon: createVNode(ExclamationCircleOutlined),
            content:
                "Hành động này không thể hoàn tác, Vui lòng chắc chắn trước khi hủy lịch khám.",
            okText: "Xác nhận",
            cancelText: "Hủy",
            async onOk() {
                var patchDoc = [
                    {
                        op: "replace",
                        path: "/status",
                        value: AppointmentStatus.CANCELLED,
                    },
                ];
                var result = await ApiAppointment.UpdateAppointmentStatus(
                    appointmentDetail.value.id,
                    patchDoc,
                );
                if (result.data.isSucceeded) {
                    message.success("Hủy lịch khám thành công");
                    await GetAppointmentDetail(appointmentDetail.value);
                } else {
                    message.error("Lỗi, vui lòng thử lại");
                }
            },
            onCancel() {},
        });
    };

    const DownloadFile = async (path) => {
        try {
            const response = await ApiAppointment.DownloadFile(path.split("\\")[1]);

            if (!response || !response.data) {
                throw new Error("Invalid response");
            }

            // Create a Blob from the response data
            const fileBlob = new Blob([response.data], { type: response.headers["content-type"] });

            // Extract filename from response headers or fallback to the path
            let fileName = path.split("/").pop();
            const contentDisposition = response.headers["content-disposition"];

            if (contentDisposition) {
                const match = contentDisposition.match(/filename="?(.+?)"?$/);
                if (match) {
                    fileName = match[1];
                }
            }

            // Force download the file
            const url = window.URL.createObjectURL(fileBlob);
            const link = document.createElement("a");
            link.href = url;
            link.setAttribute("download", fileName); // Set download filename
            document.body.appendChild(link);
            link.click();

            // Clean up
            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
        } catch (error) {
            console.error("Download file error:", error);
            message.error("Download file error.");
            message.error("Lỗi, tải file thất bại");
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
                    <a-spin :spinning="spinning"> </a-spin>
                    <div>
                        <template v-for="item in appointments">
                            <div
                                :class="
                                    'appointment_item ' +
                                    (chosenAppointment.id == item.id
                                        ? 'appointment_item_chosen'
                                        : '')
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
                                v-if="appointmentDetail.status === AppointmentStatus.WAITING"
                                :span="5"
                                class="text-secondary d-flex justify-content-start">
                                <a-button
                                    type="primary"
                                    danger
                                    @click="onCancelAppointment(appointmentDetail)"
                                    >Hủy lịch khám</a-button
                                >
                            </a-col>
                            <a-col
                                :span="
                                    appointmentDetail.status === AppointmentStatus.WAITING ? 9 : 12
                                "
                                class="ms-2 text-start"
                                style="color: #22c55e; font-weight: 500; font-size: 20px">
                                STT: {{ chosenAppointment.timeTableOrder }}
                            </a-col>
                            <a-col
                                :span="
                                    appointmentDetail.status === AppointmentStatus.WAITING ? 9 : 11
                                "
                                class="text-secondary d-flex justify-content-end">
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
                            <a-form-item name="files">
                                <a-row class="appointment_detail_item">
                                    <a-col :span="24">
                                        <span>Tệp đính kèm</span>
                                    </a-col>
                                    <a-col
                                        :span="24"
                                        v-if="
                                            !appointmentDetail.files ||
                                            appointmentDetail.status !== AppointmentStatus.COMPLETED
                                        ">
                                        <a-upload-dragger
                                            class="p-1 w-100"
                                            style="max-height: 200px"
                                            list-type="picture"
                                            v-model:fileList="formState.files"
                                            :multiple="true"
                                            action="https://run.mocky.io/v3/784b8599-f03a-40cd-8a8d-5b6f1094d4c2"
                                            @change="handleChange">
                                            <p
                                                class="ant-upload-drag-icon d-flex justify-content-center">
                                                <inbox-outlined></inbox-outlined>
                                            </p>
                                            <p class="ant-upload-text">
                                                Tải lên kết quả khám bệnh, bệnh án, ...
                                            </p>
                                            <p class="ant-upload-hint">
                                                Hỗ trợ tải lên nhiều file. Nghiêm cấm tải lên các
                                                tài liệu không liên quan đến quá trình khám bệnh
                                                hoặc tài liệu phòng khám
                                            </p>
                                        </a-upload-dragger>
                                    </a-col>
                                    <a-col v-else :span="24">
                                        <div
                                            v-for="(path, index) in appointmentDetail.files"
                                            class="d-flex align-items-center">
                                            <span>
                                                {{ (index + 1).toString().padStart(2, "0") }}
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
</style>
