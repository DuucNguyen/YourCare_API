<script setup>
    import { onMounted, computed, reactive, ref } from "vue";
    import { useRoute } from "vue-router";
    import { useAuthStore } from "@/stores/auth-store";
    import { useRouter } from "vue-router";
    //
    import Message from "@/components/Message.vue";
    import Button from "@/components/Button.vue";

    var route = useRoute();
    var router = useRouter();

    const userId = route.params.userId;
    const code = route.params.code;

    const authStore = useAuthStore();

    onMounted(async () => {
        await authStore.confirmEmail(userId, code);
    });

    const confirmMessage = computed(() => authStore.message);
    const confirmIsSucceeded = computed(() => authStore.isSucceeded);

    const formState = reactive({
        password: "",
        confirmPassword: "",
    });

    const createMessage = ref("");
    const createIsSucceeded = ref(false);

    const onFinish = async () => {
        const rs = await authStore.createPassword(userId, formState.password).catch((error) => {
            console.log("Create password ERROR: " + error);
        });

        createMessage.value = rs.data.message;
        createIsSucceeded.value = rs.data.isSucceeded;

        if (createIsSucceeded) {
            await new Promise((resolve) => setTimeout(resolve, 3000));
            router.push({ name: "create-profile", params: { userId } });
        }
    };
</script>
<template>
    <div class="container">
        <div class="mt-3 mb-3">
            <Message :context="confirmMessage" :isError="confirmIsSucceeded" />
        </div>
        <div v-if="confirmIsSucceeded">
            <div class="w-100">
                <ul>
                    <li>
                        <div>1</div>
                        <div>Confirm email</div>
                    </li>
                    <li>
                        <div>2</div>
                        <div>Create password</div>
                    </li>
                    <li>
                        <div>3</div>
                        <div>Create profile</div>
                    </li>
                </ul>
            </div>
            <div class="w-100 d-flex justify-content-center">
                <form @submit.prevent="onFinish" class="w-50 form-container">
                    <div class="w-100 mb-5 text-center">
                        <h3>Create password</h3>
                    </div>

                    <Message :context="createMessage" :isError="createIsSucceeded" />
                    <!-- //display error only -->

                    <div class="form-group m-3 d-flex align-items-center">
                        <label for="password">Password:</label>
                        <input
                            type="password"
                            v-model="formState.password"
                            id="password"
                            class="form-control"
                            placeholder="Password" />
                    </div>
                    <div class="form-group m-3 d-flex align-items-center">
                        <label class="" for="password">Confirm Password:</label>
                        <input
                            type="password"
                            v-model="formState.confirmPassword"
                            id="confirmPassword"
                            class="form-control"
                            placeholder="Password" />
                    </div>
                    <Button class="mt-5" title="Create" />
                </form>
            </div>
        </div>
    </div>
</template>
<style scoped>
    ul {
        display: flex;
        justify-content: space-evenly;
    }
    li {
        display: flex;
        list-style: none;
        align-items: center;
        font-weight: 500;
        color: #22c55e;
        padding: 5px 10px;
        border-radius: 3px;
    }
    li:nth-child(2) {
        background: #1975dc;
        color: #fff;
    }
    li div:first-child {
        background-color: #22c55e;
        padding: 4px 10px;
        margin: 0 10px 0 0;
        border-radius: 3px;
        color: #fff;
        font-weight: 500;
    }

    .form-container {
        margin-top: 50px;
        padding: 10px;
        border: 1px solid #1975dc;
        border-radius: 5px;
    }
    .form-group label {
        width: 180px;
        font-weight: 500;
    }
</style>
