<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";

    import ApiUser from "@/api/ApiUser";
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiSpecialty from "@/api/ApiSpecialty";

    import { reactive, ref, onMounted } from "vue";
    import { useRoute, useRouter } from "vue-router";

    //
    import { PlusOutlined } from "@ant-design/icons-vue";
    import { createVNode } from "vue";
    import { message, Modal } from "ant-design-vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { notification } from "ant-design-vue";

    const route = useRoute();
    const router = useRouter();

    const formState = reactive({
        applicationUserID: "",
        applicationUserImage: [],
        doctorTitle: "",
        doctorDescription: "",
        startCareerYear: "",
        specialtyIDs: [],
    });

    const rules = {
        applicationImage: [
            {
                required: "true",
                message: "Please upload at least one image.",
                type: "array",
            },
        ],
        doctorTitle: [
            {
                required: "true",
                message: "Please input doctor title.",
            },
        ],
        doctorDescription: [
            {
                required: "true",
                message: "Please input doctor description.",
            },
        ],
        startCareerYear: [
            {
                required: "true",
                message: "Please input year experience.",
            },
            {
                type: "number",
                message: "Input must be a year",
            },
            {
                pattern: generateYearRegex(),
                message: "Input must be from 1980 - now",
            },
        ],
        specialtyIDs: [
            {
                required: "true",
                type: "array",
                message: "Please choose at least one specialty.",
            },
        ],
        applicationUserImage: [
            {
                required: true,
                message: "Please upload at least one image.",
            },
        ],
    };

    function generateYearRegex() {
        //get current year regex
        const currentYear = new Date().getFullYear();
        return new RegExp(
            `^(198[0-9]|199[0-9]|20[0-${Math.floor(currentYear / 100) % 10}][0-${
                currentYear % 10
            }])$`,
        );
    }

    const specialties = ref([]);
    const formRef = ref();
    const labelCol = {
        span: 24,
    };
    const wrapperCol = {
        span: 24,
    };

    const userData = reactive({
        id: "",
        fullName: "",
        email: "",
        dob: "",
        address: "",
        phoneNumber: "",
        gender: true,
        imageString: "",
    });

    const getUserData = async () => {
        var result = await ApiUser.GetByID(userData.id);
        if (result.data.isSucceeded) {
            userData.fullName = result.data.data.fullName;
            userData.email = result.data.data.email;
            userData.dob = result.data.data.dob;
            userData.address = result.data.data.address;
            userData.phoneNumber = result.data.data.phoneNumber;
            userData.gender = result.data.data.gender;
            userData.imageString = result.data.data.imageString;
        }
    };

    const getSpecialties = async () => {
        var result = await ApiSpecialty.GetAllSpeID();
        specialties.value = result.data.data;
    };

    onMounted(async () => {
        userData.id = route.params.id;
        if (userData.id) {
            await getUserData();
        }
        await getSpecialties();

        formState.applicationUserImage.push({
            uid: "-1",
            name: "applicationUserImage",
            status: "done",
            thumbUrl: userData.imageString, //set url as thumb associate with file obj.thumbUrl
        });
    });

    /**
     * Form
     * **/

    const showUpdateConfirm = () => {
        Modal.confirm({
            title: "Are you sure create this specialty?",
            icon: createVNode(ExclamationCircleOutlined),
            content: "Are you sure to perform this action.",
            okText: "Yes",
            cancelText: "No",
            async onOk() {
                var formData = new FormData();
                formData.append("userID", userData.id);

                // formData.append("userImage", formState.applicationUserImage[0].originFileObj);
                formData.append(
                    "userImage",
                    base64ToFile(
                        formState.applicationUserImage[0].thumbUrl,
                        "applicationUserImage",
                    ),
                );
                formData.append("doctorTitle", formState.doctorTitle);
                formData.append("doctorDescription", formState.doctorDescription);
                formData.append("startCareerYear", formState.startCareerYear);

                formState.specialtyIDs.forEach((id, index) => {
                    formData.append(`specialtyIDs[${index}]`, id);
                });

                var result = await ApiDoctorProfile.Create(formData);
                

                var type = result.data.isSucceeded ? "success" : "error";
                var context = result.data.message;

                showNotification(type, "Create status", context);
            },
            onCancel() {
                console.log("Cancel update");
            },
        });
    };

    const showNotification = (type, message, context) => {
        notification[type]({
            message: message,
            description: context,
        });
    };

    const onFinish = () => {
        formRef.value
            .validate()
            .then(() => {
                showUpdateConfirm();
            })
            .catch((error) => {
                console.log("error: " + error);
            });
    };

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
</script>

<template>
    <div class="admin_dashboard">
        <AdminSideBar active-item="appointment" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_body">
                <div class="d-flex justify-content-between">
                    <div class="col-md-5 basic-information-container">
                        <h4 class="text-center" style="color: #1975dc">Basic information</h4>
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >Full Name<span class="text-danger">*</span>
                            </label>
                            <input
                                readonly
                                v-model="userData.fullName"
                                class="form-control"
                                type="text" />
                        </div>
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >Email<span class="text-danger">*</span></label
                            >
                            <input
                                readonly
                                v-model="userData.email"
                                class="form-control"
                                type="text" />
                        </div>
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >Address<span class="text-danger">*</span></label
                            >
                            <input
                                readonly
                                v-model="userData.address"
                                class="form-control"
                                type="text" />
                        </div>
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >PhoneNumber<span class="text-danger">*</span></label
                            >
                            <input
                                readonly
                                v-model="userData.phoneNumber"
                                class="form-control"
                                type="text" />
                        </div>
                        <div class="mb-3 form-items d-flex">
                            <label class="form-label me-5"
                                >Gender<span class="text-danger">*</span></label
                            >
                            <div>
                                <div v-if="userData.gender === true" class="d-flex">
                                    <input
                                        readonly
                                        v-model="userData.gender"
                                        id="male"
                                        class="form-check"
                                        type="radio"
                                        value="true" />
                                    <label class="ms-1" for="male">Male</label>
                                </div>
                                <div v-else class="d-flex">
                                    <input
                                        readonly
                                        v-model="userData.gender"
                                        id="female"
                                        class="form-check"
                                        type="radio"
                                        value="false" />
                                    <label class="ms-1" for="female">Female</label>
                                </div>
                            </div>
                        </div>
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >Date of birth<span class="text-danger">*</span></label
                            >
                            <input
                                readonly
                                v-model="userData.dob"
                                class="form-control"
                                type="date" />
                        </div>
                    </div>
                    <a-form
                        class="col-md-6 doctor-information-container"
                        ref="formRef"
                        :model="formState"
                        :rules="rules"
                        :label-col="labelCol"
                        :wrapper-col="wrapperCol">
                        <h4 class="text-center" style="color: #22c55e">Doctor information</h4>
                        <div class="d-flex justify-content-between">
                            <a-form-item class="col-md-5" label="Image" name="applicationUserImage">
                                <a-upload
                                    v-model:fileList="formState.applicationUserImage"
                                    action=""
                                    list-type="picture-card"
                                    :max-count="1"
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
                            <a-form-item class="col-md-7" label="Specialties" name="specialtyIDs">
                                <a-select mode="multiple" v-model:value="formState.specialtyIDs">
                                    <a-select-option
                                        v-for="spe in specialties"
                                        :key="spe.specialtyID"
                                        :value="spe.specialtyID"
                                        >{{ spe.title }}</a-select-option
                                    >
                                </a-select>
                            </a-form-item>
                        </div>
                        <a-form-item label="Doctor title" name="doctorTitle">
                            <a-input v-model:value="formState.doctorTitle"></a-input>
                        </a-form-item>
                        <a-form-item label="Start Career Year" name="startCareerYear">
                            <a-input-number
                                style="width: 100%"
                                v-model:value="formState.startCareerYear"></a-input-number>
                        </a-form-item>
                        <a-form-item label="Doctor description" name="doctorDescription">
                            <a-textarea v-model:value="formState.doctorDescription"></a-textarea>
                        </a-form-item>
                        <a-form-item class="text-center">
                            <a-button @click="onFinish" style="width: 100%" type="primary"
                                >Create</a-button
                            >
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

    .basic-information-container {
        padding: 20px;
        border-radius: 8px;
        border: 2px solid #1975dc;
        background: #fff;
    }
    .doctor-information-container {
        padding: 20px;
        border-radius: 8px;
        border: 2px solid #22c55e;
        background: #fff;
    }

    .doctor-specialties-container {
        padding: 10px;
    }

    .drop-down-spe {
        width: 300px;
        height: 200px;
        overflow-y: auto;
    }
    .drop-down-spe label {
        width: 100%;
    }
    .drop-down-spe li {
        font-weight: 500;
        padding: 0px 10px;
    }
    .drop-down-spe li:hover {
        background: #22c55e;
        color: #fff;
        border-radius: 3px;
    }
</style>
