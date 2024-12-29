<script setup>
    import { reactive, ref } from "vue";
    import { useAuthStore } from "@/stores/auth-store";
    import { useRoute, useRouter } from "vue-router";

    //
    import Button from "@/components/Button.vue";
    import Message from "@/components/Message.vue";

    const route = useRoute();
    const router = useRouter();
    const authStore = useAuthStore();

    const userId = route.params.userId;

    const formState = reactive({
        FullName: "",
        Gender: true,
        Dob: "",
        PhoneNumber: "",
    });

    const createMessage = ref("");
    const createIsSucceeded = ref(false);

    const onFinish = async () => {
        const rs = await authStore.createProfile(userId, formState).catch((error) => {
            console.log("Create profile ERROR: " + error);
            createMessage.value = "Create profile failed. Invalid fields";
            createIsSucceeded.value = false;
        });

        createMessage.value = rs.data.message;
        createIsSucceeded.value = rs.data.isSucceeded;

        if (createIsSucceeded) {
            await new Promise((resolve) => setTimeout(resolve, 3000));
            router.push({ name: "home" });
        }
    };
</script>
<template>
    <div class="container">
        <div class="mt-3 mb-3">
            <Message context="Create password successfully." :isError="true" />
        </div>
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
                    <h2>Create Profile</h2>
                </div>

                <Message :context="createMessage" :isError="createIsSucceeded" />

                <div class="form-group m-3 d-flex align-items-center">
                    <label for="fullName">Full Name<span class="text-danger">*</span></label>
                    <input
                        v-model="formState.FullName"
                        id="fullName"
                        class="form-control"
                        placeholder="FullName" />
                </div>
                <div class="form-group m-3 d-flex align-items-center">
                    <label for="dob">Date of birth<span class="text-danger">*</span></label>
                    <input type="date" v-model="formState.Dob" id="dob" class="form-control" />
                </div>
                <div class="form-group m-3 d-flex align-items-center">
                    <label for="phoneNumber">PhoneNumber<span class="text-danger">*</span></label>
                    <input
                        v-model="formState.PhoneNumber"
                        id="phoneNumber"
                        class="form-control"
                        placeholder="Phone number" />
                </div>
                <div class="m-3 d-flex form-group">
                    <label>Gender<span class="text-danger">*</span></label>
                    <div class="form-check">
                        <input
                            v-model="formState.Gender"
                            class="form-check-input"
                            type="radio"
                            name="gender"
                            id="male"
                            checked />
                        <label class="form-check-label" for="male"> Male </label>
                    </div>
                    <div class="form-check">
                        <input
                            v-model="formState.Gender"
                            class="form-check-input"
                            type="radio"
                            name="gender"
                            id="female" />
                        <label class="form-check-label" for="female"> Female </label>
                    </div>
                </div>
                <Button class="mt-5" title="Create" />
            </form>
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
    li:nth-child(3) {
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
