<template>
    <div class="row d-flex justify-content-between">
        <div class="col-md-4">
            <div class="nav-container">
                <RouterLink class="navigation-chosen" :to="{ name: 'login' }">Login</RouterLink>
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
                            v-model="formState.password"
                            id="password"
                            class="form-control"
                            placeholder="Password" />
                        <!-- <span asp-validation-for="Input.Password" class="text-danger"></span> -->
                    </div>
                    <div>{{ formState.username }} - {{ formState.password }}</div>
                    <div class="form-group mt-5">
                        <button :disabled="isDisabled" type="submit" class="button-authen">
                            Log in
                        </button>
                    </div>
                    <div class="form-group mt-3 d-flex justify-content-between">
                        <p>
                            <RouterLink :to="{ name: 'forgot-password' }"
                                >Forgot password ?</RouterLink
                            >
                        </p>
                    </div>
                </form>
            </section>
        </div>
        <div class="col-md-6">
            <div class="img-container"></div>
        </div>
    </div>
</template>

<script setup>
    import TokenService from "@/api/TokenService";
    import { computed, reactive } from "vue";
    import { useAuthStore } from "@/stores/auth-store";

    const authStore = useAuthStore();
    const formState = reactive({
        username: "",
        password: "",
    });

    const isDisabled = computed(() => {
        return !(formState.username && formState.password);
    });

    const onFinish = () => {
        return authStore.login(formState.username,formState.password).catch((error) => {
            console.log("ERROR: LOGIN==>" + error);
            if(error.response.status == 200){
                console.log("Login successfully !");
            }
            if (error.response.status === 401 || error.response.status === 500) {
                console.log("Username or password is incorrect.");
            } else {
                console.log("ERROR: " + error.response);
            }
        });
    };
</script>

<style scoped>
    .button {
        width: 100%;
    }
    /*
.img-container {
    width: 100%;
    height: 100%;

}

img {
    width: 100%;
    height: auto;
    border-radius: 8px;
}*/

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
