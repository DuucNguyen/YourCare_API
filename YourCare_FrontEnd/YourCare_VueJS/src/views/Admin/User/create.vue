<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";

    import ApiUser from "@/api/ApiUser";
    import { ref, reactive } from "vue";

    //
    import { PlusOutlined } from "@ant-design/icons-vue";
    //
    import { createVNode } from "vue";
    import { Modal } from "ant-design-vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { notification } from "ant-design-vue";

    const formState = reactive({
        fullName: "",
        email: "",
        phoneNumber: "",
        dob: "",
        gender: true,
        address: "",
        image: [], //ant require an array of file
        password: "",
    });

    const formRef = ref(); //serve as form object for binding below

    const labelCol = {
        //serve as label controls
        span: 6, //represent ant-col
    };

    const wrapperCol = {
        //same as label but this for input controls
        span: 10,
    };

    const rules = {
        fullName: [
            //name same as attribute in input tag that have to validate
            {
                required: true,
                message: "Please input user full name", //error message
                trigger: "change", //event to trigger this validation // default = change
            },
        ],
        email: [
            {
                required: true,
                message: "Please input user email",
                trigger: "change",
            },
            {
                type: "email",
                message: "Please input valid email format (@gmail.com/@...com).",
                trigger: "change",
            },
        ],
        password: [
            {
                required: true,
                message: "Please input default password",
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
        dob: [
            {
                required: true,
                message: "Please input user date of birth.",
                trigger: "blur",
            },
        ],
        image: [
            {
                required: true,
                type: "array",
                message: "Please upload at least one image.",
            },
        ],
        gender: [
            {
                required: true,
                message: "Please choose at least one gender.",
            },
        ],
    };

    const onFinish = () => {
        formRef.value
            .validate() //check if form is validate
            .then(() => {
                showUpdateConfirm();
            })
            .catch((error) => {
                console.log("CREATE FROM ERROR: ", error);
            });
    };

    const resetForm = () => {
        formRef.value.resetFields(); // remove all valud in form
    };

    const showUpdateConfirm = () => {
        Modal.confirm({
            title: "Are you sure create this user?",
            icon: createVNode(ExclamationCircleOutlined),
            content: "Are you sure to perform this action.",
            okText: "Yes",
            cancelText: "No",
            async onOk() {
                var formData = new FormData();
                formData.append("fullName", formState.fullName);
                formData.append("email", formState.email);
                formData.append("phoneNumber", formState.phoneNumber);
                formData.append("dob", formState.dob);
                formData.append("gender", formState.gender);
                formData.append("address", formState.address);
                formData.append("password", formState.password);
                formData.append("image", formState.image[0].originFileObj); //get raw file (binary) to send as IFormFile

                var result = await ApiUser.Create(formData);
                if (result.data.isSucceeded) {
                    console.log(result.data.message);
                }
                var type = result.data.isSucceeded ? "success" : "error";
                var description = result.data.message;

                showNotification(type, "Create user", description);
            },
            onCancel() {
                console.log("Cancel create");
            },
        });
    };

    const showNotification = (type, message, description) => {
        notification[type]({
            message: message,
            description: description,
        });
    };
</script>

<template>
    <div class="admin_dashboard">
        <AdminSideBar active-item="user" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_body">
                <div class="crud-layout-header">
                    <h2 class="crud-layout-header-title">Create User</h2>
                    <RouterLink class="crud-layout-header-button" :to="{ name: 'Admin_User_View' }"
                        >Back to Lists</RouterLink
                    >
                </div>
                <div class="crud-layout-form">
                    <a-form
                        ref="formRef"
                        :model="formState"
                        :rules="rules"
                        :label-col="labelCol"
                        :wrapper-col="wrapperCol">
                        <a-form-item class="mb-5" label="Image" name="image">
                            <a-upload
                                v-model:fileList="formState.image"
                                action="/upload.do"
                                list-type="picture-card"
                                :maxCount="1">
                                <div>
                                    <PlusOutlined />
                                    <div style="margin-top: 8px">Upload</div>
                                </div>
                            </a-upload>
                        </a-form-item>

                        <a-form-item label="FullName" name="fullName">
                            <a-input v-model:value="formState.fullName"></a-input>
                        </a-form-item>
                        <a-form-item label="Email" name="email">
                            <a-input v-model:value="formState.email"></a-input>
                        </a-form-item>
                        <a-form-item label="Phone" name="phoneNumber">
                            <a-input v-model:value="formState.phoneNumber"></a-input>
                        </a-form-item>
                        <a-form-item label="Date of birth" name="dob">
                            <a-date-picker
                                v-model:value="formState.dob"
                                placeholder="Pick a date"
                                style="width: 100%"></a-date-picker>
                        </a-form-item>
                        <a-form-item label="Gender" name="gender">
                            <a-radio-group v-model:value="formState.gender">
                                <a-radio value="true">Male</a-radio>
                                <a-radio value="false">Female</a-radio>
                            </a-radio-group>
                        </a-form-item>
                        <a-form-item label="Address" name="address">
                            <a-input v-model:value="formState.address"></a-input>
                        </a-form-item>
                        <a-form-item label="Password" name="password">
                            <a-input v-model:value="formState.password"></a-input>
                        </a-form-item>
                        <a-form-item :wrapper-col="{ span: 24, offset: 10 }">
                            <!-- default ant : 24 colums; offset mean add columns (margin) in the left item -->
                            <a-button type="primary" @click="onFinish">Create</a-button>
                            <a-button style="margin-left: 10px" @click="resetForm">Reset</a-button>
                        </a-form-item>
                    </a-form>
                </div>
            </div>
        </div>
    </div>
</template>
<style>
    .admin_dashboard {
        display: flex;
    }
    .admin_dashboard_body {
        display: flex;
        flex-direction: column;
        padding-left: 30px;
    }
    .admin_dashboard_section {
        flex-grow: 1;
    }
</style>
