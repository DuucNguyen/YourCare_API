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
        specialtyIDs: [],
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

    const getUserDataByDoctorID = async () => {
        var result = await ApiUser.GetByDoctorID(formState.doctorProfileID);
        if (result.data.isSucceeded) {
            userData.id = result.data.data.id;
            userData.fullName = result.data.data.fullName;
            userData.email = result.data.data.email;
            userData.dob = result.data.data.dob;
            userData.address = result.data.data.address;
            userData.phoneNumber = result.data.data.phoneNumber;
            userData.gender = result.data.data.gender;
            userData.imageString = result.data.data.imageString;
        }
    };

    const getDoctorProfileData = async () => {
        var result = await ApiDoctorProfile.GetByUserID(formState.doctorProfileID);
        if(result.data.isSucceeded){
            //here
        }
    };

    const getSpecialties = async () => {
        var result = await ApiSpecialty.GetAllSpeID();
        specialties.value = result.data;
    };

    onMounted(async () => {
        formState.doctorProfileID = route.params.id;
        if (formState.doctorProfileID) {
            getUserDataByDoctorID();
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
            title: "Are you sure create this specialty?",
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

                chosenSpecialties.value.forEach((item, index) => {
                    formData.append(`specialtyIDs[${index}]`, item.specialtyID);
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
        if (chosenSpecialties.value.length <= 0) {
            showModalSpeError();
            return;
        }
        showUpdateConfirm();
    };
</script>
<template></template>
