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
                        <RouterLink :to="{ name: 'login' }">Login</RouterLink>
                        <RouterLink class="navigation-chosen" :to="{ name: 'register' }"
                            >Register</RouterLink
                        >
                    </div>
                    <hr />
                    <section>
                        <form
                            @submit.prevent="onFinish"
                            id="account"
                            method="post"
                            class="form-login">
                            <div class="form-group mt-3">
                                <label for="email">Email</label>
                                <input
                                    v-model="formState.email"
                                    id="email"
                                    class="form-control"
                                    placeholder="Email" />
                            </div>
                            <div class="form-group mt-5">
                                <Button title="Send email" :isDisabled="isDisabled" />
                            </div>
                            <div class="mt-3 mb-3">
                                <Message :context="message" :isError="isSucceeded" />
                            </div>
                            <div class="form-group mt-5 d-flex justify-content-between text-center">
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

<script setup>
    import TokenService from "@/api/TokenService";
    import { computed, reactive } from "vue";
    import { useAuthStore } from "@/stores/auth-store";

    // //
    import Button from "@/components/Button.vue";
    import Message from "@/components/Message.vue";

    const authStore = useAuthStore();

    const message = computed(() => authStore.message);
    const isSucceeded = computed(() => authStore.isSucceeded);

    const formState = reactive({
        email: "",
    });

    const isDisabled = computed(() => {
        return !formState.email;
    });

    const onFinish = () => {
        return authStore.sendEmailRegister(formState.email).catch((error) => {
            console.log("ERROR: SendMail ==> " + error);
        });
    };
</script>
<style>
    .img_container {
        position: relative;
        width: 450px;
        height: 450px;
        object-fit: contain;
        margin: 0 auto;
        border-radius: 50%;
        --radius: 250px;
    }
</style>
