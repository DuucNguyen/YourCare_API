<script setup>
    import { MailOutlined, SolutionOutlined, LoadingOutlined } from "@ant-design/icons-vue";
    import { ref, onMounted, h, reactive } from "vue";
    import { useAuthStore } from "@/stores/auth-store";
    import { message } from "ant-design-vue";
    import { error } from "jquery";

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
        email: "",
    });
    const rules = ref({
        email: [
            {
                required: true,
                message: "Vui lòng nhập email.",
                trigger: "change",
            },
        ],
    });

    const onFinish = async () => {
        var result = await authStore.sendEmailForgotPassword(formState.email);

        if (result.data.isSucceeded) {
            message.success(result.data.message);
        } else {
            message.error(result.data.message);
        }
    };
</script>

<template>
    <div class="body-container">
        <div class="step-container">
            <a-steps :current="0" :items="items"></a-steps>
        </div>
        <a-row class="form-container">
            <a-row class="w-100">
                <a-col :span="24" class="form-title">
                    <span>Nhập email</span>
                </a-col>
            </a-row>
            <a-divider class="m-2"></a-divider>
            <a-col :span="24">
                <a-form
                    :wrapper-col="{ span: 24 }"
                    ref="formRef"
                    v-model:model="formState"
                    :rules="rules">
                    <a-form-item name="email">
                        <a-input v-model:value="formState.email"></a-input>
                    </a-form-item>
                    <a-row>
                        <a-col :span="24" class="w-100 d-flex justify-content-end">
                            <a-form-item>
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
                                            >Xác nhận đặt lại mật khẩu
                                        </span>
                                        <br />
                                        <span class="text-danger">
                                            Vui lòng kiểm tra lại thông tin phía trên.
                                        </span>
                                    </template>
                                    <a-button
                                        type="primary"
                                        style="height: 40px; font-weight: 500"
                                        class="w-100 fs-6">
                                        Gửi xác nhận
                                    </a-button>
                                </a-popconfirm>
                            </a-form-item>
                        </a-col>
                    </a-row>
                </a-form>
            </a-col>
        </a-row>
    </div>
</template>

<style scoped>
    .body-container {
        height: 100vh;
        overflow-y: hidden;
        padding-top: 50px;
    }

    :deep(.ant-steps-item-icon) {
        /* display: flex !important;
        align-items: center !important;
        justify-content: center !important; */
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
