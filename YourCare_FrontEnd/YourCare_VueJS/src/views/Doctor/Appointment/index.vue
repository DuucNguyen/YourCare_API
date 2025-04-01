<script setup>
    import ApiAppointment from "@/api/ApiAppointment";
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import DoctorSideBar from "@/shared/DoctorSideBar.vue";
    import { ref, onMounted, reactive } from "vue";
    import dayjs from "dayjs";
    import { useRouter } from "vue-router";

    import AppointmentStatus from "@/constants/AppointmentStatus";

    import { useAuthStore } from "@/stores/auth-store";
    import { useRouteStore } from "@/stores/route-store";

    //
    import { createVNode } from "vue";
    import {
        InboxOutlined,
        ExclamationCircleOutlined,
        VerticalAlignBottomOutlined,
    } from "@ant-design/icons-vue";
    import { message, Modal } from "ant-design-vue";

    const router = useRouter();
    const authStore = useAuthStore();
    const routeStore = useRouteStore();
    const appointment = ref(routeStore.data); //get from route

    const user = ref({});
    const doctorProfileId = ref({});
    const appointments = ref([]); //list
    const appointmentDetail = ref(); //detail get from appointment.value
    const date = ref(dayjs());

    const formRef = ref();
    const formState = reactive({
        id: appointment.value.id ?? "",
        doctorDiagnosis: "",
        doctorNote: "",
        files: [],
    });

    const rules = {
        doctorDiagnosis: [
            {
                required: true,
                message: "Không được bỏ trống mục này",
                trigger: "change",
            },
        ],
        doctorNote: [
            {
                required: true,
                message: "Không được bỏ trống mục này",
                trigger: "change",
            },
        ],
        files: [
            {
                type: "array",
                max: 5,
                message: "Tối đa 5 tệp tin",
            },
        ],
    };

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

    const GetAppointmentDetail = async (item) => {
        appointment.value = item;
        formState.id = appointment.value.id;
        formState.files = [];
        var result = await ApiAppointment.GetDetailById(appointment.value.id);
        if (result.data.isSucceeded) {
            appointmentDetail.value = result.data.data;
            formState.doctorDiagnosis = appointmentDetail.value.doctorDiagnosis;
            formState.doctorNote = appointmentDetail.value.doctorNote;
        }
    };

    const handleChange = (event) => {
        var status = event.file.status;
        if (status === "done") {
            message.success(`${event.file.name} file uploaded successfully.`);
        } else if (status === "error") {
            message.error(`${event.file.name} file upload failed.`);
        }
    };

    const toggleSlide = (event) => {
        var currentClasses = event.currentTarget.classList;
        event.currentTarget.classList.toggle("up");

        if (currentClasses.contains("up")) {
            event.currentTarget.innerHTML = "Thu gọn <i class='bx bx-chevron-up fs-4'></i>";
        } else {
            event.currentTarget.innerHTML = "Chi tiết <i class='bx bx-chevron-down fs-4'></i>";
        }

        $("#shrinkable_appointment_detail").slideToggle();
    };

    onMounted(async () => {
        await InitDoctor();
        await InitAppointment();
        if (appointment.value.id) {
            await GetAppointmentDetail(appointment.value);
        }
    });

    const onFinish = () => {
        formRef.value
            .validate()
            .then(() => {
                showModalConfirmation();
            })
            .catch((error) => {
                console.log(error);
            });
    };

    const showModalConfirmation = () => {
        Modal.confirm({
            title: "Hoàn thành lịch khám ?",
            icon: createVNode(ExclamationCircleOutlined),
            content:
                "Xác nhận hoàn thành lịch khám cho bệnh nhân: " +
                appointmentDetail.value.patientName,
            okText: "Xác nhận",
            cancelText: "Hủy",
            async onOk() {
                try {
                    var formData = new FormData();
                    formData.append("id", formState.id);
                    formData.append("doctorDiagnosis", formState.doctorDiagnosis);
                    formData.append("doctorNote", formState.doctorNote);

                    if (formState.files.length > 0) {
                        formState.files.forEach((fileObj) => {
                            formData.append("files", fileObj.originFileObj || fileObj);
                        });
                    }

                    var result = await ApiAppointment.CompleteAppointment(formData);

                    if (result.data.isSucceeded) {
                        message.success("Hoàn thành lịch khám");
                        GetAppointmentDetail(appointment.value);
                    } else {
                        message.error("Lỗi, vui lòng thử lại");
                    }
                } catch (error) {
                    console.log(error);
                }
            },
        });
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
                    appointment.value.status = AppointmentStatus.CANCELLED;
                    await GetAppointmentDetail(appointmentDetail.value);
                } else {
                    message.error("Lỗi, vui lòng thử lại");
                }
            },
            onCancel() {},
        });
    };

    const RedirectToFollowUp = () => {
        Modal.confirm({
            title: "Xác nhận đặt lịch tái khám",
            icon: createVNode(ExclamationCircleOutlined),
            content:
                "Xác nhận đi tới trang đặt lịch tái khám cho bệnh nhân: " +
                appointment.value.patientName,
            okText: "Xác nhận",
            cancelText: "Hủy",
            onOk() {
                // routeStore.setData(appointment.value);
                router.push({
                    name: "Doctor_Appointment_Create",
                    params: { id: appointment.value.id },
                });
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
</script>

<template>
    <div class="doctor_dashboard_body">
        <DoctorSideBar active-item="appointment" />
        <div class="doctor_dashboard_section">
            <div class="doctor_dashboard_body">
                <div class="col-md-4 doctor_appointment_list">
                    <div class="doctor_dashboard_title">
                        <h3 class="mb-0 ms-3 mt-1">Lịch khám hôm nay</h3>
                    </div>
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
                    <div
                        v-if="appointments.length <= 0"
                        class="h-75 mt-5 d-flex align-items-center justify-content-center">
                        <a-empty class="">
                            <template #description>
                                <span>Không có lịch khám trong hôm nay</span>
                            </template>
                        </a-empty>
                    </div>
                </div>
                <div class="col-md-8">
                    <a-col :span="23" class="appointment_detail_container">
                        <div v-if="appointmentDetail">
                            <a-row
                                v-if="
                                    appointmentDetail.status === AppointmentStatus.WAITING ||
                                    appointmentDetail.status === AppointmentStatus.PROCESSING
                                "
                                class="appointment_detail_buttons">
                                <a-button @click="onFinish" type="primary">
                                    Xác nhận đã khám xong
                                </a-button>
                                <a-button @click="onFinish" type="primary"> Đợi kết quả </a-button>
                                <a-button
                                    @click="onCancelAppointment(appointmentDetail)"
                                    type="primary"
                                    danger>
                                    Hủy
                                </a-button>
                            </a-row>
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
                                <a-row
                                    v-if="appointment.status != AppointmentStatus.WAITING"
                                    class="mt-1">
                                    <a-col :span="24" class="d-flex justify-content-end">
                                        <a-button
                                            type="primary"
                                            class="d-flex align-items-center"
                                            @click="RedirectToFollowUp">
                                            Hẹn tái khám <i class="bx bx-right-arrow-alt fs-5"></i>
                                        </a-button>
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
                                <a-form ref="formRef" :model="formState" :rules="rules">
                                    <a-form-item name="doctorDiagnosis">
                                        <a-row class="appointment_detail_item">
                                            <a-col :span="24">
                                                <span>Chuẩn đoán</span>
                                            </a-col>
                                            <a-col :span="24">
                                                <a-textarea
                                                    :readonly="
                                                        appointmentDetail.status ===
                                                        AppointmentStatus.COMPLETED
                                                    "
                                                    v-model:value="formState.doctorDiagnosis"
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
                                                    :readonly="
                                                        appointmentDetail.status ===
                                                        AppointmentStatus.COMPLETED
                                                    "
                                                    v-model:value="formState.doctorNote"
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
                                                        Hỗ trợ tải lên nhiều file. Nghiêm cấm tải
                                                        lên các tài liệu không liên quan đến quá
                                                        trình khám bệnh hoặc tài liệu phòng khám
                                                    </p>
                                                </a-upload-dragger>
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
    .appointment_container {
        margin: 10px;
        padding: 5px;
        height: 630px;
        max-height: 630px;
        overflow-y: scroll;
        background: #f3f4f6;
        border-bottom: 1px solid #ddd;
    }
    .doctor_appointment_list {
        background: #fff;
        margin-right: 20px;
        border-radius: 8px;
    }
    .appointment_detail_container {
        padding: 10px;
        width: 100%;
        border-radius: 8px;
        height: 670px;
        max-height: 670px;
        overflow-y: scroll;
    }

    .appointment_detail_buttons {
        display: flex;
        justify-content: end;
        padding: 3px 0px;
        background-color: #f3f4f6;
        position: sticky;
        top: 0;
        z-index: 1;
    }

    .appointment_detail_buttons button {
        margin: 0 10px;
    }
</style>
