<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";

    import ApiSpecialty from "@/api/ApiSpecialty";
    import { reactive, ref, onMounted, onUpdated } from "vue";

    import { useRoute, useRouter } from "vue-router";
    //
    import { createVNode } from "vue";
    import { Modal } from "ant-design-vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { notification } from "ant-design-vue";

    const router = useRouter();
    const route = useRoute();

    const formState = reactive({
        specialtyID: "",
        title: "",
        image: null,
        imageString: "",
    });

    const loadData = async () => {
        const id = route.params.id;
        var result = await ApiSpecialty.GetByID(id);

        if (result.data.isSucceeded) {
            formState.specialtyID = result.data.data.specialtyID;
            formState.title = result.data.data.title;
            formState.imageString = result.data.data.imageString;

            previewUrl.value = formState.imageString;
        }
    };

    /**
     * Load Image
     * **/
    const previewUrl = ref(null); // Reactive reference to hold the preview URL

    const handleFileChange = (event) => {
        const file = event.target.files[0]; // Get the selected file

        if (file) {
            formState.image = file; //for sending request with file.
            const reader = new FileReader(); // Create a FileReader object
            reader.onload = (e) => {
                previewUrl.value = e.target.result; // Update the reactive `previewUrl`
            };
            reader.readAsDataURL(file); // Read the file as a Data URL
        }
    };

    const showUpdateConfirm = (specialtyID) => {
        Modal.confirm({
            title: "Are you sure delete this specialty?",
            icon: createVNode(ExclamationCircleOutlined),
            content: "Are you sure to perform this action.",
            okText: "Yes",
            okType: "danger",
            cancelText: "No",
            async onOk() {
                var formData = new FormData();
                formData.append("specialtyID", formState.specialtyID);
                formData.append("title", formState.title);
                formData.append("image", formState.image);

                var result = await ApiSpecialty.Update(formData);
                var type = result.data.isSucceeded ? "success" : "error";
                var context = result.data.message;

                loadData();
                showNotification(type, "Update status", context);
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

    onMounted(() => {
        loadData();
    });
</script>
<template>
    <div class="admin_dashboard">
        <AdminSideBar style="position: sticky; top: 68px" active-item="specialty" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_body">
                <div class="crud-layout-header">
                    <h2 class="crud-layout-header-title">Create Specialty</h2>
                    <RouterLink
                        class="crud-layout-header-button"
                        :to="{ name: 'Admin_Specialty_View' }"
                        >Index</RouterLink
                    >
                </div>
                <div class="row d-flex justify-content-center">
                    <div class="col-md-6 form-create">
                        <form
                            @submit.prevent="showUpdateConfirm(formState.specialtyID)"
                            enctype="multipart/form-data">
                            <div class="mb-3 form-group">
                                <label for="title" class="control-label"
                                    >Title<span class="text-danger">*</span></label
                                >
                                <input
                                    v-model="formState.title"
                                    id="title"
                                    class="form-control"
                                    placeholder="title for specialty"
                                    required />
                            </div>

                            <div class="mb-3 form-group d-flex justify-content-between">
                                <div>
                                    <label for="fileInput" class="control-label"
                                        >Image<span class="text-danger">*</span></label
                                    >
                                    <input
                                        id="fileInput"
                                        type="file"
                                        class="form-control"
                                        @change="handleFileChange"
                                        required />
                                </div>
                                <div class="img-container">
                                    <img id="previewImage" style="width: 100%" :src="previewUrl" />
                                </div>
                            </div>
                            <div class="mb-3 form-group d-flex justify-content-end">
                                <input type="submit" value="Update" class="btn btn-primary" />
                            </div>
                        </form>
                    </div>
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
    .form-create {
        padding: 20px 30px;
        border: 3px solid #1975dc;
        border-top: none;
        border-bottom-right-radius: 5px;
        border-bottom-left-radius: 5px;
        font-weight: 500;
        background: #fff;
    }
    .img-container {
        width: 200px;
        height: 200px;
        border: 1px solid #ddd;
        padding: 3px;
        border-radius: 5px;
        background: #fff;
    }
</style>
