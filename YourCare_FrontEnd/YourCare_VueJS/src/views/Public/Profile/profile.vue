<script setup>
    import UserSideBar from "@/shared/UserSideBar.vue";
    import { ref, onMounted, computed, reactive } from "vue";
    import { useAuthStore } from "@/stores/auth-store";
    import dayjs from "dayjs";
    import { message } from "ant-design-vue";
    import { createVNode } from "vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { Modal } from "ant-design-vue";
    import { useRouter } from "vue-router";

    const router = useRouter();
    const authStore = useAuthStore();

    const user = ref({});

    const changePasswordFormState = reactive({
        currentPassword: "",
        newPassword: "",
    });

    const changePasswordForm = ref();
    const changePasswordRules = {
        currentPassword: [
            {
                required: "true",
                message: "Vui lòng điền đầy đủ thông tin",
                trigger: "change",
            },
            {
                min: 6,
                message: "Vui lòng nhập ít nhất 6 ký tự",
                trigger: "change",
            },
        ],
        newPassword: [
            {
                required: "true",
                message: "Vui lòng điền đầy đủ thông tin",
                trigger: "change",
            },
            {
                min: 6,
                message: "Vui lòng nhập ít nhất 6 ký tự",
                trigger: "change",
            },
            {
                validator: (rule, value, callback) => {
                    if (!/[A-Z]/.test(value)) {
                        callback(new Error("Password must include at least one upper case."));
                    } else if (!/[!@#$%^&*(),.?":{}|<>]/.test(value)) {
                        callback(
                            new Error("Password must include at least one special character."),
                        );
                    } else {
                        callback();
                    }
                },
                trigger: "change",
            },
        ],
    };

    onMounted(async () => {
        user.value = await authStore.getUserInfo();
    });

    const onFinishChangePassword = () => {
        changePasswordForm.value
            .validate()
            .then(async () => {
                showModalConfirmation();
            })
            .catch((error) => {
                console.log(error);
            });
    };

    const showModalConfirmation = () => {
        Modal.confirm({
            title: "Xác nhận thay đổi mật khẩu",
            icon: createVNode(ExclamationCircleOutlined),
            content: "Bạn có chắc chắn thay đổi mật khẩu không ?",
            okText: "Đồng ý",
            cancelText: "Hủy",
            async onOk() {
                var formData = new FormData();

                formData.append("userID", user.value.id);
                formData.append("currentPassword", changePasswordFormState.currentPassword);
                formData.append("newPassword", changePasswordFormState.newPassword);

                var result = await authStore.changePassword(formData);
                if (result.data.isSucceeded) {
                    message.success("Thay đổi mật khẩu thành công.");
                    return new Promise((resolve) => {
                        setTimeout(() => {
                            resolve("Timeout done.");
                        }, 2000); // 2000ms = 2 seconds
                    }).then(() => {
                        authStore.logOut();
                    });
                } else {
                    message.error("Lỗi, vui lòng thử lại sau.");
                }
            },
            onCancel() {},
        });
    };
</script>

<template>
    <div class="d-flex">
        <UserSideBar page="profile" />
        <div style="" class="profile_container">
            <a-row class="profile_head">
                <a-col :span="24">
                    <span class="profile_head_title"> Tài khoản </span>
                </a-col>
            </a-row>
            <a-divider class="m-0"></a-divider>
            <a-row class="profile_body">
                <a-col :span="12">
                    <a-row class="profile_body_title">
                        <a-col :span="24">
                            <span>Thông tin tài khoản</span>
                        </a-col>
                    </a-row>
                    <div class="profile_body_container">
                        <a-row class="profile_body_item">
                            <a-col :span="12"><span>Họ và tên</span></a-col>
                            <a-col :span="12">
                                <span>
                                    {{ user.fullName }}
                                </span>
                            </a-col>
                        </a-row>
                        <a-row class="profile_body_item">
                            <a-col :span="12">
                                <span> Email </span>
                            </a-col>
                            <a-col :span="12">
                                <span>
                                    {{ user.email }}
                                </span>
                            </a-col>
                        </a-row>
                        <a-row class="profile_body_item">
                            <a-col :span="12">
                                <span> SĐT </span>
                            </a-col>
                            <a-col :span="12">
                                <span>
                                    {{ user.phoneNumber ? user.phoneNumber : "chưa cập nhật" }}
                                </span>
                            </a-col>
                        </a-row>
                        <a-row class="profile_body_item">
                            <a-col :span="12">
                                <span> Giới tính </span>
                            </a-col>
                            <a-col :span="12">
                                <span>
                                    {{ user.gender == true ? "Nam" : "Nữ" }}
                                </span>
                            </a-col>
                        </a-row>
                        <a-row class="profile_body_item">
                            <a-col :span="12">
                                <span> Địa chỉ </span>
                            </a-col>
                            <a-col :span="12">
                                <span>
                                    {{ user.address ? user.address : "chưa cập nhật" }}
                                </span>
                            </a-col>
                        </a-row>
                        <a-row class="profile_body_item">
                            <a-col :span="12">
                                <span> Ngày sinh </span>
                            </a-col>
                            <a-col :span="12">
                                <span>
                                    {{
                                        user.dob
                                            ? dayjs(user.dob).format("DD/MM/YYYY")
                                            : "chưa cập nhật"
                                    }}
                                </span>
                            </a-col>
                        </a-row>
                    </div>
                </a-col>
                <a-col :span="2" class="d-flex justify-content-center">
                    <a-divider type="vertical" style="height: 100%"></a-divider>
                </a-col>
                <a-col :span="10">
                    <a-row class="profile_body_title">
                        <a-col :span="24">
                            <span>Đổi mật khẩu</span>
                        </a-col>
                    </a-row>
                    <div class="profile_body_container">
                        <a-form
                            layout="vertical"
                            ref="changePasswordForm"
                            :model="changePasswordFormState"
                            :rules="changePasswordRules">
                            <a-row>
                                <a-col :span="24">
                                    <a-form-item label="Mật khẩu hiện tại" name="currentPassword">
                                        <a-input-password
                                            v-model:value="
                                                changePasswordFormState.currentPassword
                                            "></a-input-password>
                                    </a-form-item>
                                </a-col>
                            </a-row>
                            <a-row>
                                <a-col :span="24">
                                    <a-form-item label="Mật khẩu mới" name="newPassword">
                                        <a-input-password
                                            v-model:value="
                                                changePasswordFormState.newPassword
                                            "></a-input-password>
                                    </a-form-item>
                                </a-col>
                            </a-row>
                            <a-form-item class="d-flex justify-content-end">
                                <a-button
                                    :disabled="
                                        !changePasswordFormState.currentPassword &&
                                        !changePasswordFormState.newPassword
                                    "
                                    type="primary"
                                    @click="onFinishChangePassword()">
                                    Thay đổi
                                </a-button>
                            </a-form-item>
                        </a-form>
                    </div>
                </a-col>
            </a-row>
            <a-divider class="m-0"></a-divider>
            <a-row class="profile_footer">
                <a-col :span="24">
                    <span
                        >Tổng đài hỗ trợ và chăm sóc khách hàng:
                        <span class="text-primary"> 0865493798 </span>
                    </span>
                </a-col>
            </a-row>
        </div>
    </div>
</template>

<style scoped>
    .profile_body {
        padding: 10px;
        display: flex;
        justify-content: space-between;
    }
    .profile_body_title span {
        font-weight: 500;
        font-size: 16px;
    }
    .profile_body_item div:nth-child(2) {
        display: flex;
        justify-content: end;
    }

    .profile_body_container {
        padding: 5px;
    }

    .profile_body_item {
        margin-top: 20px;
    }
    .profile_body_item span {
        font-size: 17px;
    }
    .profile_body_item div:nth-child(2) {
        font-weight: 500;
    }
</style>
