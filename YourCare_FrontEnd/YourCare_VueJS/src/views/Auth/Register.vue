<template>
    <div class="auth-container">
        <div class="container">
            <div class="row d-flex justify-content-between">
                <div class="col-md-6">
                    <h1>YourCare</h1>
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
                            <!-- <div class="form-group mt-3">
                                <label for="password">Password</label>
                                <input
                                    type="password"
                                    v-model="formState.password"
                                    id="password"
                                    class="form-control"
                                    placeholder="Password" />
                            </div>
                            <div class="form-group mt-3">
                                <label for="confirmPassword">Confirm Password</label>
                                <input
                                    type="password"
                                    v-model="formState.confirmPassword"
                                    id="confirmPassword"
                                    class="form-control"
                                    placeholder="Password" />
                            </div>
                             -->

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

<style scoped>
    .auth-container {
        overflow: hidden;
        width: 100vw;
        height: 100vh;
        background: #1a76e3;
    }
    .auth-form {
        margin: 200px 0 0 0;
        padding: 30px;
        border-radius: 5px;
        background-color: #fff;
    }

    .nav-container {
        display: flex;
        justify-content: space-around;
    }

    .nav-container a {
        text-decoration: none;
        font-size: 20px;
        font-weight: 500;
        color: black;
    }

    .nav-container a:hover {
        color: #3903fc;
    }

    .navigation-chosen {
        border-bottom: 3px solid #03fc5e !important;
        color: #3903fc !important;
    }
</style>
