<script setup>
    import UserSideBar from "@/shared/UserSideBar.vue";
    import ApiPatientProfile from "@/api/ApiPatientProfile";
    import { useAuthStore } from "@/stores/auth-store";
    import { ref, onMounted, nextTick, reactive, createVNode } from "vue";
    import dayjs from "dayjs";

    //
    import { Modal, notification } from "ant-design-vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";

    const authStore = useAuthStore();

    const spinning = ref(false);
    const patientProfiles = ref([]);
    const chosenPatientProfile = ref({});

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

    const initPatientProfileData = async () => {
        spinning.value = true;
        var user = await authStore.getUserInfo();
        var patientProfile_data = await ApiPatientProfile.GetAllByUserID(user.id);
        if (patientProfile_data.data.isSucceeded) {
            patientProfiles.value = patientProfile_data.data.data;
            chosenPatientProfile.value = patientProfiles.value[0];
            selectProfile();
        }
        spinning.value = false;
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

    const actions = {
        create: "Tạo mới",
        update: "Lưu thay đổi",
        delete: "Xóa hồ sơ",
    };

    const createModalInvisible = ref(false);

    const selectProfile = () => {
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
                var result = null;

                if (action === actions.delete) {
                    result = await ApiPatientProfile.Delete(chosenPatientProfile.value.id);

                    var type = result.data.isSucceeded ? "success" : "error";
                    var description = result.data.message;

                    showNotification(type, "Trạng thái hành động", description);
                    await initPatientProfileData();
                    return;
                }

                var user = await authStore.getUserInfo();
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

                if (action === actions.create) {
                    result = await ApiPatientProfile.Create(formData);
                } else {
                    formData.append("id", chosenPatientProfile.value.id);
                    result = await ApiPatientProfile.Update(formData);
                }

                var type = result.data.isSucceeded ? "success" : "error";
                var description = result.data.message;

                showNotification(type, "Trạng thái hành động", description);

                createModalInvisible.value = false;
                await initPatientProfileData();
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
        await initPatientProfileData();

        nextTick(() => {
            //run after rendered and mounted in DOM
            $(".profile-item-body").each((index, element) => {
                $(element).hide();
            });
        });
    });
</script>

<template>
    <div class="d-flex">
        <UserSideBar page="patientProfile" />
        <div class="profile_container">
            <a-row class="profile_head">
                <a-col :span="24">
                    <span class="profile_head_title"> Hồ sơ bệnh nhân </span>
                </a-col>
            </a-row>
            <a-divider class="m-0"></a-divider>
            <a-row class="profile_body">
                <a-col :span="8">
                    <a-spin :spinning="spinning">
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
                                                @change="selectProfile()"
                                                v-model="chosenPatientProfile"
                                                :value="profile"
                                                name="patient-profile"
                                                type="radio"
                                                class="form-check-input"
                                                :id="'profile-' + profile.id" />
                                        </div>

                                        <label class="d-flex w-100" :for="'profile-' + profile.id">
                                            <div class="profile-item-img">
                                                <img src="\src\assets\profile.png" />
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
                                </div>
                            </div>
                        </div>
                    </a-spin>
                    <div class="d-flex justify-content-center">
                        <button
                            @click="
                                createModalInvisible = true;
                                modalTitle = actions.create;
                            "
                            type="button"
                            title="Tạo hồ sơ mới"
                            class="w-75 p-2 btn-slideToggle d-flex justify-content-center">
                            Thêm hồ sơ mới <i class="bx bx-user-plus"></i>
                        </button>
                    </div>
                </a-col>
                <a-col :span="16" class="p-2 d-flex flex-column justify-content-center">
                    <div class="d-flex w-100">
                        <div style="width: 80px; height: 80px" class="profile-item-img mb-3">
                            <img src="\src\assets\profile.png" />
                        </div>
                        <div class="profile-item-info">
                            <span class="fs-4">{{ chosenPatientProfile.name }}</span>
                            <span class="text-secondary">{{
                                chosenPatientProfile.dob != null
                                    ? dayjs(chosenPatientProfile.dob).format("DD/MM/YYYY")
                                    : ""
                            }}</span>
                        </div>
                    </div>
                    <a-alert
                        message="Hoàn thiện thông tin để đặt khám và quản lý hồ sơ y tế được tốt hơn."
                        type="warning"
                        show-icon />
                    <a-divider class="m-0"></a-divider>
                    <a-row class="mb-2">
                        <a-col :span="24" class="appointment_detail_title">
                            <span> Điều chỉnh thông tin </span>
                        </a-col>
                    </a-row>
                    <a-spin :spinning="spinning">
                        <a-form
                            layout="vertical"
                            :model="formState"
                            :rules="rules"
                            ref="createForm">
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
                                        <a-input
                                            placeholder="SĐT"
                                            v-model:value="formState.phoneNumber"></a-input>
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
                                        <a-input
                                            placeholder="Dân tộc"
                                            v-model:value="formState.ethnic"></a-input>
                                    </a-form-item>
                                </a-col>
                            </a-row>
                        </a-form>
                    </a-spin>
                    <a-button key="submit" type="primary" @click="onFinish(actions.update)">
                        {{ actions.update }}
                    </a-button>
                    <a-button class="mt-2" key="submit" danger @click="onFinish(actions.delete)">
                        {{ actions.delete }}
                    </a-button>
                </a-col>
            </a-row>
        </div>
    </div>

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
            <a-button key="submit" type="primary" @click="onFinish(actions.create)">
                {{ modalTitle }}
            </a-button>
        </template>
    </a-modal>
</template>
