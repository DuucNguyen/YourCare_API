<script setup>
    import ApiUser from "@/api/ApiUser";
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiSpecialty from "@/api/ApiSpecialty";

    import { reactive, ref, onMounted } from "vue";
    import { useRoute, useRouter } from "vue-router";

    //
    import { createVNode } from "vue";
    import { Modal } from "ant-design-vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { notification } from "ant-design-vue";

    const route = useRoute();
    const router = useRouter();

    const formState = reactive({
        doctorProfileID: "",
        applicationUserID: "",
        applicationUserImage: null,
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

    const specialties = ref([]);

    const getDoctorProfileData = async () => {
        var result = await ApiDoctorProfile.GetByID(formState.doctorProfileID);
        if (result.data.isSucceeded) {
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
    });

    /**
     * IMAGE
     * **/
    const previewUrl = ref(null); // Reactive reference to hold the preview URL

    const handleFileChange = (event) => {
        const file = event.target.files[0]; // Get the selected file

        if (file) {
            formState.applicationUserImage = file; //for sending request with file.
            const reader = new FileReader(); // Create a FileReader object
            reader.onload = (e) => {
                previewUrl.value = e.target.result; // Update the reactive `previewUrl`
            };
            reader.readAsDataURL(file); // Read the file as a Data URL
        }
    };

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
                var formData = new FormData();
                formData.append("userID", userData.id);
                formData.append("userImage", formState.applicationUserImage);
                formData.append("doctorTitle", formState.doctorTitle);
                formData.append("doctorDescription", formState.doctorDescription);
                formData.append("yearExperience", formState.yearExperience);

                formState.specialties.forEach((item, index) => {
                    formData.append(`specialtyIDs[${index}]`, item.specialtyID);
                });

                var result = await ApiDoctorProfile.Update(formData);

                var type = result.data.isSucceeded ? "success" : "error";
                var context = result.data.message;

                showNotification(type, "Create status", context);
            },
            onCancel() {
                console.log("Cancel update");
            },
        });
    };

    const showModalSpeError = () => {
        Modal.error({
            title: "ERROR",
            content:
                "You haven't choose any specialty for this doctor. Choose at least one specialty before continue.",
        });
    };

    const showNotification = (type, message, context) => {
        notification[type]({
            message: message,
            description: context,
        });
    };

    const onFinish = async () => {
        if (formState.specialties.length <= 0) {
            showModalSpeError();
            return;
        }
        showUpdateConfirm();
    };

    const isChecked = (spe) => {
        return formState.specialties.some((x) => x.specialtyID === spe.specialtyID);
    };

    const toogleSpecialty = (spe) => {
        console.log(spe.title);
        var index = formState.specialties.findIndex((x) => x.specialtyID === spe.specialtyID);

        if (index === -1) {
            formState.specialties.push(spe);
        } else {
            formState.specialties.splice(index, 1);
        }
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

        <form
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
                                            :id="'spe_' + spe.specialtyID"
                                            :checked="isChecked(spe)"
                                            @change="toogleSpecialty(spe)" />
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
                            v-for="item in formState.specialties"
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
                    <button class="btn btn-success w-100" type="submit">Update</button>
                </div>
            </div>
        </form>
    </div>
</template>
<style>
    .basic-information-container {
        padding: 20px;
        border-radius: 8px;
        border: 2px solid #1975dc;
    }
    .doctor-information-container {
        padding: 20px;
        border-radius: 8px;
        border: 2px solid #22c55e;
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
