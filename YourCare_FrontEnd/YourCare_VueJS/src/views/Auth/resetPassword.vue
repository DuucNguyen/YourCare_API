<script setup>
    import { MailOutlined, SolutionOutlined, LoadingOutlined } from "@ant-design/icons-vue";
    import { DescriptionsItem, message } from "ant-design-vue";
    import { Callbacks, valHooks } from "jquery";
    import { ref, onMounted, h, reactive } from "vue";
    import { useRoute } from "vue-router";
    import { useAuthStore } from "@/stores/auth-store";

    const route = useRoute();
    const authStore = useAuthStore();

    const items = [
        {
            title: "Gửi email",
        },
        {
            title: "Xác nhận",
        },
        {
            title: "Tạo mật khẩu mới",
        },
    ];

    const formRef = ref();
    const formState = reactive({
        password: "",
        newPassword: "",
        code: route.params.code ?? "",
        userId: route.params.userId ?? "",
    });
    const rules = ref({
        password: [
            {
                required: true,
                message: "Vui lòng nhập mật khẩu mới",
                trigger: "change",
            },
            {
                validator: (rule, value, callback) => {
                    if (!/[A-Z]/.test(value)) {
                        callback(new Error("Mật khẩu phải chứa ít nhất một chữ cái in hoa."));
                    } else if (!/[!@#$%^&*(),.?":{}|<>]/.test(value)) {
                        callback(new Error("Mật khẩu phải chứa ít nhất một kí tự đặc biệt."));
                    } else {
                        callback();
                    }
                },
                trigger: "change",
            },
        ],
        newPassword: [
            {
                required: true,
                message: "Vui lòng xác nhận mật khẩu mới",
                trigger: "change",
            },
            {
                validator: (rule, value, callback) => {
                    if (value !== formState.password) {
                        callback(new Error("Mật khẩu mới không khớp."));
                    } else {
                        callback();
                    }
                },
                trigger: "change",
            },
        ],
    });

    onMounted(() => {});

    const onFinish = async () => {
        if (!formState.userId) {
            message.error("Lỗi. vui lòng gửi lại yêu cầu đổi mật khẩu.");
            return;
        }
        if (!formState.code) {
            message.error("Lỗi. vui lòng gửi lại yêu cầu đổi mật khẩu.");
            return;
        }

        var resetPasswordFormState = {
            userId: formState.userId,
            code: formState.code,
            password: formState.password,
        };
        var result = await authStore.resetPassword(resetPasswordFormState);
        if (result.data.isSucceeded) {
            message.success(result.data.message);
        } else {
            message.error(result.data.message);
        }
    };
</script>

<template>
    <div class="container mt-5">
        <div class="step-container">
            <a-steps :current="2" :items="items"></a-steps>
        </div>
        <a-row class="form-container">
            <a-row class="w-100">
                <a-col :span="24" class="form-title">
                    <span>Tạo mật khẩu mới</span>
                </a-col>
            </a-row>
            <a-divider class="m-2"></a-divider>
            <a-col :span="24">
                <a-form
                    :label-col="{ span: 8 }"
                    :wrapper-col="{ span: 16 }"
                    ref="formRef"
                    v-model:model="formState"
                    :rules="rules">
                    <a-form-item name="password" label="Mật khẩu">
                        <a-input v-model:value="formState.password"></a-input>
                    </a-form-item>
                    <a-form-item name="newPassword" label="Xác nhận mật khẩu">
                        <a-input v-model:value="formState.newPassword"></a-input>
                    </a-form-item>
                    <a-form-item class="w-100">
                        <a-row class="w-100">
                            <a-col :span="24" class="d-flex justify-content-end">
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
                                    @confirm="onFinish">
                                    <template #title>
                                        <span class="fs-6 text-primary"
                                            >Xác nhận đặt lại mật khẩu</span
                                        >
                                        <br />
                                        <span class="text-danger"
                                            >Vui lòng kiểm tra lại thông tin phía trên.</span
                                        >
                                    </template>
                                    <a-button
                                        type="primary"
                                        style="height: 35px; font-weight: 500"
                                        class="fs-6">
                                        Tạo mật khẩu mới
                                    </a-button>
                                </a-popconfirm>
                            </a-col>
                        </a-row>
                    </a-form-item>
                </a-form>
            </a-col>
        </a-row>
    </div>
</template>

<style scoped>
    :deep(.ant-steps-item-icon) {
        display: flex !important;
        align-items: center !important;
        justify-content: center !important;
    }

    .step-container {
        width: 80vw;
        margin: auto;
    }

    .form-container {
        width: 40vw;
        margin: auto;
        margin-top: 50px;
        padding: 20px;
        box-shadow: 1px 1px 20px #ddd;
        border-radius: 10px;
    }

    .form-title {
        width: 100%;
        font-size: 24px;
        font-weight: 500;
        display: flex;
        justify-content: center;
        align-items: center;
    }
</style>
