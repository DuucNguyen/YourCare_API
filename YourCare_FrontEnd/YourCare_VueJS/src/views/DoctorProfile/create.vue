<script setup>
    import ApiUser from "@/api/ApiUser";
    import ApiSpecialty from "@/api/ApiSpecialty";

    import { reactive, ref, onMounted } from "vue";
    import { useRoute, useRouter } from "vue-router";

    const route = useRoute();
    const router = useRouter();

    const formState = reactive({
        applicationUserID: "",
        applicationUserImage: null,
        doctorTitle: "",
        doctorDescription: "",
        yearExperience: "",
        specialtyIDs: [],
    });

    const specialties = ref([]);

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
            userData.fullName = result.data.fullName;
            userData.email = result.data.email;
            userData.dob = result.data.dob;
            userData.address = result.data.address;
            userData.phoneNumber = result.data.phoneNumber;
            userData.gender = result.data.gender;
            userData.imageString = result.data.imageString;
        }
    };

    const getSpecialties = async () => {
        specialties.value = await ApiSpecialty.GetAllSpeID();
    };

    onMounted(() => {
        var id = route.params.id;
        if (id) {
            getUserData(id);
        }
        getSpecialties();
    });
</script>

<template>
    <div class="d-flex justify-content-between">
        <div class="col-md-5 basic-information-container">
            <h4 class="text-center" style="color: #1975DC;">Basic information</h4>
            <div class="mb-3 form-items">
                <label class="form-label">Full Name<span class="text-danger">*</span> </label>
                <input class="form-control" type="text" />
            </div>
            <div class="mb-3 form-items">
                <label class="form-label">Email<span class="text-danger">*</span></label>
                <input class="form-control" type="text" />
            </div>
            <div class="mb-3 form-items">
                <label class="form-label">Address<span class="text-danger">*</span></label>
                <input class="form-control" type="text" />
            </div>
            <div class="mb-3 form-items">
                <label class="form-label">PhoneNumber<span class="text-danger">*</span></label>
                <input class="form-control" type="text" />
            </div>
            <div class="mb-3 form-items d-flex">
                <label class="form-label me-5">Gender<span class="text-danger">*</span></label>
                <div>
                    <div class="d-flex">
                        <input id="male" class="form-check" type="radio" value="true" />
                        <label class="ms-1" for="male">Male</label>
                    </div>
                    <div class="d-flex">
                        <input id="female" class="form-check" type="radio" value="false" />
                        <label class="ms-1" for="female">Female</label>
                    </div>
                </div>
            </div>
            <div class="mb-3 form-items">
                <label class="form-label">Date of birth<span class="text-danger">*</span></label>
                <input class="form-control" type="date" />
            </div>
        </div>

        <form method="post" enctype="multipart/form-data">
            <div class="doctor-information-container">
                <h4 class="text-center" style="color: #22C55E;">Doctor information</h4>
                <div class="mb-3 d-flex">
                    <img
                        id="previewImage"
                        src="#"
                        alt="Avatar Preview"
                        style="padding: 2px; width: 200px; height: 200px; border: 1px solid #ddd" />
                    <div class="p-2 d-flex flex-column">
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >AvatarFile<span class="text-danger">*</span></label
                            >
                            <input id="fileInput" class="form-control" type="file" />
                        </div>
                        <div class="mb-3 form-items">
                            <label class="form-label"
                                >DoctorTitle<span class="text-danger">*</span></label
                            >
                            <input class="form-control" type="text" />
                        </div>
                    </div>
                </div>

                <div class="mb-3 form-items">
                    <label class="form-label"
                        >Specializations<span class="text-danger">*</span></label
                    >
                    <div class="dropdown">
                        <button
                            class="btn btn-outline-success dropdown-toggle"
                            type="button"
                            data-bs-toggle="dropdown"
                            aria-expanded="false">
                            Select Specializations
                        </button>
                        <ul style="height: 200px; overflow-y: auto" class="p-3 dropdown-menu">
                            <!-- @foreach (Specialization spe in ViewBag.specializations) {
                        <li>
                            <div class="form-check">
                                <input
                                    class="form-check-input"
                                    type="checkbox"
                                    name="selectedSpecializations"
                                    value="@spe.Id"
                                    id="spe_@spe.Id" />
                                <label class="form-check-label" for="spe_@spe.Id">
                                    @spe.Name
                                </label>
                            </div>
                        </li>
                        } -->
                        </ul>
                    </div>
                </div>

                <div class="mb-3 form-items">
                    <label class="form-label">Experience<span class="text-danger">*</span></label>
                    <input class="form-control" type="number" />
                </div>

                <div class="mb-3 form-items">
                    <label class="form-label">DoctorDescription</label>
                    <textarea class="form-control"></textarea>
                </div>
                <div class="d-flex justify-content-center">
                    <button class="btn btn-success w-100" type="submit">Create</button>
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
</style>
