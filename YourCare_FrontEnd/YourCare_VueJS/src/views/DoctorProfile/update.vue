<script setup>
    import ApiUser from "@/api/ApiUser";
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiSpecialty from "@/api/ApiSpecialty";

    import { reactive, ref, onMounted, watch } from "vue";
    import { useRoute, useRouter } from "vue-router";

    //
    import { createVNode } from "vue";
    import { Modal } from "ant-design-vue";
    import { PlusOutlined } from "@ant-design/icons-vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { notification } from "ant-design-vue";

    const route = useRoute();
    const router = useRouter();

    const formRef = ref();
    const labelCol = {
        span: 24,
    };
    const wrapperCol = {
        span: 24,
    };

    const formState = reactive({
        doctorProfileID: "",
        applicationUserID: "",
        applicationUserImage: [],
        doctorTitle: "",
        doctorDescription: "",
        yearExperience: "",
        specialties: [],
    });

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
        yearExperience: [
            {
                required: "true",
                message: "Please input year experience.",
            },
            {
                type: "number",
                min: 1,
                max: 50,
                message: "number must be in range 1-50.",
            },
        ],
        specialties: [
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

    const specialties = ref([]);

    const getDoctorProfileData = async () => {
        var result = await ApiDoctorProfile.GetByID(formState.doctorProfileID);
        if (result.data.isSucceeded) {
            formState.doctorProfileID = result.data.data.doctorProfileID;
            formState.applicationUserID = result.data.data.applicationUserID;
            formState.doctorTitle = result.data.data.doctorTitle;
            formState.doctorDescription = result.data.data.doctorDescription;
            formState.yearExperience = result.data.data.yearExperience;
            formState.specialties = result.data.data.specialties;

            userData.id = result.data.data.userID;
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
        specialties.value = result.data;
    };

    onMounted(async () => {
        formState.doctorProfileID = route.params.id;
        if (formState.doctorProfileID) {
            await getDoctorProfileData();
        }
        await getSpecialties();

        //convert to valid format for binding a-select :option
        formState.specialties = formState.specialties.map((spe) => ({
            label: spe.title,
            value: spe.specialtyID,
        }));

        //for the first time - load from db
        //convert to valid format for binding
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
            title: "Are you sure update this doctor profile?",
            icon: createVNode(ExclamationCircleOutlined),
            content: "Are you sure to perform this action.",
            okText: "Yes",
            cancelText: "No",
            async onOk() {
                try {
                    var formData = new FormData();
                    formData.append("userID", userData.id);
                    formData.append(
                        "userImage",
                        base64ToFile(
                            formState.applicationUserImage[0].thumbUrl,
                            "applicationUserImage",
                        ),
                    );
                    formData.append("doctorProfileID", formState.doctorProfileID);
                    formData.append("doctorTitle", formState.doctorTitle);
                    formData.append("doctorDescription", formState.doctorDescription);
                    formData.append("yearExperience", formState.yearExperience);

                    formState.specialties.forEach((item, index) => {
                        console.log(item);
                        formData.append(`specialtyIDs[${index}]`, item);
                    });

                    var result = await ApiDoctorProfile.Update(formData);

                    var type = result.data.isSucceeded ? "success" : "error";
                    var context = result.data.message;

                    showNotification(type, "Create status", context);
                } catch (error) {
                    console.log(error);
                }
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

    const onFinish = async () => {
        formRef.value
            .validate()
            .then(() => {
                showUpdateConfirm();
            })
            .catch((error) => {
                console.log("error:" + error);
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
    <div class="d-flex justify-content-between">
        <div class="col-md-5 basic-information-container">
            <h4 class="text-center" style="color: #1975dc">Basic information</h4>
            <div class="mb-3 form-items">
                <label class="form-label">Full Name<span class="text-danger">*</span> </label>
                <input readonly v-model="userData.fullName" class="form-control" type="text" />
            </div>
            <div class="mb-3 form-items">
                <label class="form-label">Email<span class="text-danger">*</span></label>
                <input readonly v-model="userData.email" class="form-control" type="text" />
            </div>
            <div class="mb-3 form-items">
                <label class="form-label">Address<span class="text-danger">*</span></label>
                <input readonly v-model="userData.address" class="form-control" type="text" />
            </div>
            <div class="mb-3 form-items">
                <label class="form-label">PhoneNumber<span class="text-danger">*</span></label>
                <input readonly v-model="userData.phoneNumber" class="form-control" type="text" />
            </div>
            <div class="mb-3 form-items d-flex">
                <label class="form-label me-5">Gender<span class="text-danger">*</span></label>
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
                <label class="form-label">Date of birth<span class="text-danger">*</span></label>
                <input readonly v-model="userData.dob" class="form-control" type="date" />
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
                        v-model:file-list="formState.applicationUserImage"
                        action="https://www.mocky.io/v2/5cc8019d300000980a055e76"
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
                <a-form-item class="col-md-7" label="Specialties">
                    <a-select
                        mode="multiple"
                        v-model:value="formState.specialties"
                        :options="
                            specialties.map((spe) => ({ label: spe.title, value: spe.specialtyID }))
                        "
                        placeholder="Select specialties">
                    </a-select>
                </a-form-item>
            </div>
            <a-form-item label="Doctor title" name="doctorTitle">
                <a-input v-model:value="formState.doctorTitle"></a-input>
            </a-form-item>
            <a-form-item label="Year experience" name="yearExperience">
                <a-input-number
                    style="width: 100%"
                    v-model:value="formState.yearExperience"></a-input-number>
            </a-form-item>
            <a-form-item label="Doctor description" name="doctorDescription">
                <a-textarea v-model:value="formState.doctorDescription"></a-textarea>
            </a-form-item>
            <a-form-item class="text-center">
                <a-button @click="onFinish" style="width: 100%" type="primary">Update</a-button>
            </a-form-item>
        </a-form>
    </div>
</template>
<style>
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
