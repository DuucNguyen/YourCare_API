<script setup>
    import TokenService from "@/api/TokenService";
    import { computed, onMounted, reactive, ref } from "vue";
    import { useAuthStore } from "@/stores/auth-store";
    import { useRouter } from "vue-router";

    // //
    import Button from "@/components/Button.vue";
    import Message from "@/components/Message.vue";

    const authStore = useAuthStore();

    const externalLogins = ref([]);

    // Directly bind to store properties
    const message = computed(() => authStore.message);
    const isSucceeded = computed(() => authStore.isSucceeded);

    const formState = reactive({
        username: "",
        password: "",
    });

    const buttonTitle = ref("Login");
    const isDisabled = computed(() => {
        return !(formState.username && formState.password);
    });

    const InitExternalLogin = async () => {
        const result = await authStore.getExternalLogin();
        externalLogins.value = result.data;
    };

    onMounted(async () => {
        await InitExternalLogin();
        console.log(externalLogins.value);
    });

    const onFinish = async () => {
        buttonTitle.value = "Waiting...";
        isDisabled.value = true;

        await authStore
            .login(formState.username, formState.password)
            .catch((error) => {
                console.log("ERROR: LOGIN ==> " + error);
                if (error.response.status == 200) {
                    console.log("Login successfully !");
                }
                if (error.response.status === 401 || error.response.status === 500) {
                    console.log("Username or password is incorrect.");
                } else {
                    console.log("ERROR: " + error.response);
                }
            })
            .finally(() => {
                buttonTitle.value = "Login";
                isDisabled.value = false;
            });
    };

    const externalLogin = async () => {
        await authStore.externalLogin();
    };
</script>

<template>
    <div class="auth-container">
        <div class="container">
            <div class="row d-flex justify-content-between login_form">
                <div class="col-md-6 img_container position-relative">
                    <img
                        src="/src/assets/19e7d647915ec2db2cc54178b44bad4d.png"
                        alt="img"
                        class="img-fluid p-2" />

                    <!-- Circular text placement -->
                    <div class="text-circle text1">Nhanh chóng</div>
                    <div class="text-circle text2">Dễ dàng</div>
                    <div class="text-circle text3">Tiện lợi</div>
                    <div class="text-circle text4">Chuyên nghiệp</div>
                    <div class="text-circle text5">Uy tín</div>
                </div>
                <div class="col-md-6 auth-form">
                    <div class="nav-container">
                        <RouterLink class="navigation-chosen" :to="{ name: 'login' }"
                            >Login</RouterLink
                        >
                        <RouterLink :to="{ name: 'register' }">Register</RouterLink>
                    </div>
                    <hr />
                    <section>
                        <form
                            @submit.prevent="onFinish"
                            id="account"
                            method="post"
                            class="form-login">
                            <!-- <div asp-validation-summary="ModelOnly" class="text-danger"></div> -->
                            <div class="form-group mt-3">
                                <label for="email">Email/Username</label>
                                <input
                                    v-model="formState.username"
                                    id="email"
                                    class="form-control"
                                    placeholder="Email/Username" />
                                <!-- <span asp-validation-for="Input.Email" class="text-danger"></span> -->
                            </div>
                            <div class="form-group mt-3">
                                <label for="password">Password</label>
                                <input
                                    type="password"
                                    v-model="formState.password"
                                    id="password"
                                    class="form-control"
                                    placeholder="Password" />
                                <!-- <span asp-validation-for="Input.Password" class="text-danger"></span> -->
                            </div>
                            <div class="form-group mt-5">
                                <Button :title="buttonTitle" :isDisabled="isDisabled" />
                            </div>
                            <div class="mt-3 mb-3">
                                <Message :context="message" :isError="isSucceeded" />
                            </div>

                            <!-- <div v-if="externalLogins" class="external_login_container">
                                <div
                                    v-for="item in externalLogins"
                                    class="external_login_item"
                                    @click="externalLogin(item.name)">
                                    <i class="bx bxl-google" v-if="item.name === 'Google'"></i>
                                    <i
                                        class="bx bxl-facebook-circle"
                                        v-if="item.name === 'Facebook'"></i>
                                    <span> Đăng nhập với {{ item.displayName }}</span>
                                </div>
                            </div> -->

                            <div class="form-group mt-3 d-flex justify-content-between text-center">
                                <RouterLink class="w-100" :to="{ name: 'forgot-password' }"
                                    >Forgot password ?</RouterLink
                                >
                            </div>
                        </form>
                    </section>
                </div>
            </div>
        </div>
    </div>
</template>

<style scoped>
    .img_container {
        position: relative;
        width: 450px;
        height: 450px;
        object-fit: contain;
        margin: 0 auto;
        border-radius: 50%;
        --radius: 250px;
    }
    .external_login_container {
        width: 100%;
    }

    .external_login_item {
        display: flex;
        align-items: center;
        justify-content: center;
        border: 1px solid #ea4335;
        padding: 8px 0px;
        border-radius: 5px;
        color: #ea4335;
        cursor: pointer;
    }
    .external_login_item:hover {
        color: #fff;
        background: #ea4335;
    }
    .external_login_item i {
        font-size: 25px;
        margin-right: 10px;
    }
</style>
