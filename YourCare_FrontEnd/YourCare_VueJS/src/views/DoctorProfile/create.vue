<script setup>
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
        yearExperience: "",
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
        specialtyIDs: [
            {
                required: "true",
                type: "array",
                message: "Please choose at least one specialty.",
            },
        ],
        applicationImage: [
            {
                required: true,
                message: "Please upload at least one image."
            }
        ]
    };

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
        specialties.value = result.data;
    };

    onMounted(async () => {
        userData.id = route.params.id;
        if (userData.id) {
            await getUserData();
        }
        await getSpecialties();
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
                formData.append("userImage", formState.applicationUserImage[0].originFileObj);
                formData.append("doctorTitle", formState.doctorTitle);
                formData.append("doctorDescription", formState.doctorDescription);
                formData.append("yearExperience", formState.yearExperience);

                formState.specialtyIDs.forEach((id, index) => {
                    console.log(id);
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
                        v-model:fileList="formState.applicationUserImage"
                        action=""
                        list-type="picture-card"
                        :max-count="1">
                        <div>
                            <PlusOutlined />
                            <div style="margin-top: 8px">Upload</div>
                        </div>
                    </a-upload>
                </a-form-item>
                <a-form-item class="col-md-7" label="Specialties">
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
            <a-form-item label="Year experience" name="yearExperience">
                <a-input-number
                    style="width: 100%"
                    v-model:value="formState.yearExperience"></a-input-number>
            </a-form-item>
            <a-form-item label="Doctor description" name="doctorDescription">
                <a-textarea v-model:value="formState.doctorDescription"></a-textarea>
            </a-form-item>
            <a-form-item class="text-center">
                <a-button @click="onFinish" style="width: 100%" type="primary">Create</a-button>
            </a-form-item>
        </a-form>
        <!-- <form
            @submit.prevent="onFinish"
            class="col-md-6"
            method="post"
            enctype="multipart/form-data">
            <div class="doctor-information-container">
                <h4 class="text-center" style="color: #22c55e">Doctor information</h4>
                <div class="mb-3 d-flex">
                    <img
                        id="previewImage"
                        :src="previewUrl"
                        alt="Avatar Preview"
                        style="padding: 2px; width: 200px; height: 200px; border: 1px solid #ddd" />
                    <div class="p-2 d-flex flex-column">
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >AvatarFile<span class="text-danger">*</span></label
                            >
                            <input
                                @change="handleFileChange"
                                id="fileInput"
                                class="form-control"
                                type="file"
                                required />
                        </div>
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >DoctorTitle<span class="text-danger">*</span></label
                            >
                            <input
                                v-model="formState.doctorTitle"
                                class="form-control"
                                type="text"
                                required />
                        </div>
                    </div>
                </div>

                <div class="mb-3 form-items d-flex">
                    <div>
                        <span class="form-label"
                            >Specialties<span class="text-danger">*</span></span
                        >
                        <div class="dropdown">
                            <button
                                class="btn btn-outline-success dropdown-toggle"
                                type="button"
                                data-bs-toggle="dropdown"
                                aria-expanded="false">
                                Select Specialties
                            </button>
                            <ul @click.stop class="p-3 dropdown-menu drop-down-spe">
                                <li v-for="spe in specialties" :key="spe.specialtyID">
                                    <div class="form-check">
                                        <input
                                            class="form-check-input"
                                            type="checkbox"
                                            name="selectedSpecializations"
                                            :value="spe"
                                            :id="'spe_' + spe.specialtyID"
                                            v-model="chosenSpecialties" />
                                        <label
                                            @click.stop
                                            class="form-check-label"
                                            :for="'spe_' + spe.specialtyID">
                                            {{ spe.title }}
                                        </label>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                    <div class="doctor-specialties-container">
                        <label
                            class="specialization-item-capsule"
                            v-for="item in chosenSpecialties"
                            >{{ item.title }}</label
                        >
                    </div>
                </div>

                <div class="mb-3 form-items">
                    <label class="form-label">Experience<span class="text-danger">*</span></label>
                    <input
                        v-model="formState.yearExperience"
                        class="form-control"
                        type="number"
                        min="1"
                        required />
                </div>

                <div class="mb-3 form-items">
                    <label class="form-label">DoctorDescription</label>
                    <textarea
                        v-model="formState.doctorDescription"
                        style="min-height: 100px; max-height: 300px"
                        class="form-control"
                        required></textarea>
                </div>
                <div class="d-flex justify-content-center">
                    <button class="btn btn-success w-100" type="submit">Create</button>
                </div>
            </div>
        </form> -->
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
