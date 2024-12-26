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
                            <!-- <div asp-validation-summary="ModelOnly" class="text-danger"></div> -->
                            <div class="form-group mt-3">
                                <label for="email">Email/Username</label>
                                <input
                                    v-model="formState.username"
                                    id="email"
                                    class="form-control"
                                    placeholder="Email/Username" />
                            </div>
                            <div class="form-group mt-3">
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
                            <div class="form-group mt-5">
                                <Button title="Login" :isDisabled="isDisabled" />
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

    const authStore = useAuthStore();
    const formState = reactive({
        username: "",
        password: "",
        confirmPassword: "",
    });

    const isDisabled = computed(() => {
        return !(formState.username && formState.password && formState.confirmPassword);
    });

    const onFinish = () => {
        return authStore
            .register(formState.username, formState.password, formState.confirmPassword)
            .catch((error) => {
                console.log("ERROR: REGISTER ==> " + error);
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
