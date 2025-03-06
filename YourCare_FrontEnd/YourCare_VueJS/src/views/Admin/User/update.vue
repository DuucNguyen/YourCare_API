<script setup>
    import ApiUser from "@/api/ApiUser";
    import { reactive, ref, onMounted } from "vue";

    import { useRoute } from "vue-router";
    const route = useRoute();

    import dayjs from "dayjs";
    const dateFormat = "YYYY/MM/DD";

    //
    import { PlusOutlined, ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { notification } from "ant-design-vue";
    import { Modal } from "ant-design-vue";
    import { createVNode } from "vue";

    const formRef = ref();
    const labelCol = {
        span: 4,
    };

    const wrapperCol = {
        span: 24,
    };

    const formState = reactive({
        userID: route.params.id || "",
        fullName: "",
        email: "",
        phoneNumber: "",
        address: "",
        gender: true,
        dob: "",
        image: [],
    });

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
        dob: [
            {
                required: true,
                message: "Please input user date of birth.",
                trigger: "change",
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

    const getData = async () => {
        var result = await ApiUser.GetByID(formState.userID);
        if (result.data.isSucceeded) {
            formState.fullName = result.data.data.fullName;
            formState.email = result.data.data.email;
            formState.phoneNumber = result.data.data.phoneNumber;
            formState.address = result.data.data.address;
            formState.gender = result.data.data.gender;
            formState.dob = dayjs(result.data.data.dob.split("T")[0], dateFormat);

            formState.image.push({
                id: "-1",
                name: "applicationUserImage",
                status: "done",
                thumbUrl: result.data.data.imageString,
            });
        }
    };

    onMounted(async () => {
        await getData();
    });

    const onFinish = () => {
        formRef.value
            .validate()
            .then(() => {
                showUpdateConfirmation();
            })
            .catch((error) => {
                console.log(error);
            });
    };

    const resetForm = () => {
        formRef.value.resetFields();
    };

    const showUpdateConfirmation = () => {
        Modal.confirm({
            title: "Update confirmation",
            icon: createVNode(ExclamationCircleOutlined),
            content: "Are you sure to update this user information ?",
            okText: "Yes",
            cancelText: "No",
            async onOk() {
                const formData = new FormData();

                formData.append(
                    "image",
                    base64ToFile(formState.image[0].thumbUrl, "applicationImage"),
                );
                formData.append("id", formState.userID);
                formData.append("fullName", formState.fullName);
                formData.append("email", formState.email);
                formData.append("phoneNumber", formState.phoneNumber);
                formData.append("address", formState.address);
                formData.append("dob", dayjs(formState.dob).format(dateFormat));
                formData.append("gender", formState.gender);

                const result = await ApiUser.Update(formData);
                const type = result.data.isSucceeded ? "success" : "error";
                const description = result.data.message;

                showNotification(type, "Update status", description);
            },
            onCancel() {
                console.log("Cancel update");
            },
        });
    };

    const showNotification = (type, title, description) => {
        notification[type]({
            message: title,
            description: description,
        });
    };

    /**
     * Image
     * **/
    //validate file size
    const validateFile = (file) => {
        const maxSize = 2 * 1024 * 1024; //2mb
        if (file.size > maxSize) {
            this.$message.error("File size muse be less than 2mb.");
            return false;
        }
        return true;
    };

    //convert base64 to File
    function base64ToFile(base64String, fileName = "file.png") {
        // Extract the base64 data
        const byteString = atob(base64String.split(",")[1]); // Decode base64 string
        const mimeType = base64String.match(/data:(.*?);base64/)[1]; // Extract MIME type

        // Convert binary string to array of bytes
        const arrayBuffer = new ArrayBuffer(byteString.length);
        const uint8Array = new Uint8Array(arrayBuffer);

        for (let i = 0; i < byteString.length; i++) {
            uint8Array[i] = byteString.charCodeAt(i);
        }

        // Create a Blob and return a File object
        return new File([uint8Array], fileName, { type: mimeType });
    }

    /**
     * Image
     * **/
    const previewVisible = ref(false); //toogle modal
    const previewImage = ref(""); //base64 - img src
    const previewTitle = ref(""); //modal title

    //convert file to base 64 to preview
    function getBase64(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => resolve(reader.result);
            reader.onerror = (error) => reject(error);
        });
    }
    //modal preview
    const handlePreview = async (file) => {
        if (!file.thumbUrl && !file.preview) {
            file.preview = await getBase64(file.originFileObj);
        }
        previewImage.value = file.thumbUrl || file.preview;
        previewVisible.value = true;
        previewTitle.value =
            file.name || file.thumbUrl.substring(file.thumbUrl.lastIndexOf("/") + 1);
    };

    //modal close
    const handleCancel = () => {
        previewVisible.value = false;
        previewTitle.value = "";
    };
</script>

<template>
    <div class="crud-layout-header">
        <h2 class="crud-layout-header-title">Update User</h2>
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
                    :maxCount="1"
                    :beforeUpload="validateFile"
                    @preview="handlePreview">
                    <div>
                        <PlusOutlined />
                        <div style="margin-top: 8px">Upload</div>
                    </div>
                </a-upload>
                <a-modal
                    :open="previewVisible"
                    :title="previewTitle"
                    :footer="null"
                    @cancel="handleCancel">
                    <img alt="example" style="width: 100%" :src="previewImage" />
                </a-modal>
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
                    <a-radio :value="true">Male</a-radio>
                    <a-radio :value="false">Female</a-radio>
                </a-radio-group>
            </a-form-item>
            <a-form-item label="Address" name="address">
                <a-input v-model:value="formState.address"></a-input>
            </a-form-item>
            <a-form-item :wrapper-col="{ span: 24, offset: 10 }">
                <!-- default ant : 24 colums; offset means add columns (margin) in the left item -->
                <a-button type="primary" @click="onFinish">Update</a-button>
                <a-button style="margin-left: 10px" @click="resetForm">Reset</a-button>
            </a-form-item>
        </a-form>
    </div>
</template>
