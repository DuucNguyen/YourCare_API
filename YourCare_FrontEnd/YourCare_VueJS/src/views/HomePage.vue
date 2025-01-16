<script setup>
    import Header from "@/shared/Header.vue";
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiSpecialty from "@/api/ApiSpecialty";
    import { reactive, ref, onMounted } from "vue";

    const doctorData = ref([]);
    const speData = ref([]);

    const getData = async () => {
        try {
            var docResult = await ApiDoctorProfile.GetAllByLimit({
                pageNumber: 1,
                pageSize: 10,
            });
            doctorData.value = docResult.data.data;
            var speResult = await ApiSpecialty.GetAllSpeID();
            speData.value = speResult.data.data;
        } catch (error) {
            console.log("get data error: " + error);
        }
    };

    onMounted(async () => {
        await getData();
    });
</script>
<template>
    <Header />
    <div class="container">
        <div class="text-center">
            <h1 class="display-6 fw-bold">Đặt lịch khám trực tuyến</h1>
            <p>Tìm Bác sĩ chính xác - Đặt lịch khám dễ dàng</p>
        </div>

        <div class="mb-5 row doctor-section">
            <div class="mb-3 d-flex justify-content-between">
                <div>
                    <h2 class="mb-0 fs-4 fw-bold">Đặt khám bác sĩ</h2>
                    <p class="mb-0">Phiếu khám kèm số thứ tự và thời gian của bạn được xác nhận.</p>
                </div>
                <div class="d-flex align-items-center">
                    <RouterLink class="button-hover" :to="{ name: 'Public_DanhSachBacSi' }">
                        Xem thêm <i class="bx bx-right-arrow-alt"></i>
                    </RouterLink>
                </div>
            </div>
            <div class="doctor-items">
                <div v-for="doc in doctorData" class="doctor-item">
                    <div class="doctor-item-info">
                        <div class="doctor-img">
                            <img :src="doc.imageString" />
                        </div>
                        <a class="doctor-title">
                            <span>{{ doc.doctorTitle }}</span>
                            <span>{{ doc.fullName }}</span>
                        </a>
                        <div class="doctor-spes">
                            <label
                                v-for="spe in doc.specialties"
                                class="specialization-item-capsule"
                                >{{ spe.title }}</label
                            >
                            <br />
                        </div>
                        <p class="mb-0">{{ doc.address }}</p>
                    </div>
                    <a class="appointment-link"
                        >Đặt lịch khám <i class="fs-4 bx bx-chevron-right"></i
                    ></a>
                </div>
            </div>
        </div>
        <div class="row specialization-section">
            <div class="mb-3">
                <h2 class="mb-0 fs-4 fw-bold">Đặt lịch theo Chuyên khoa</h2>
                <p class="mb-0">Danh sách bác sĩ theo chuyên khoa</p>
            </div>
            <div class="specialization-items">
                <RouterLink
                    v-for="spe in speData"
                    class="text-decoration-none"
                    :to="{
                        name: 'Public_DanhSachBacSi',
                        query: { specialtyID: spe.specialtyID },
                    }">
                    <div class="specialization-item">
                        <div class="specialization-img">
                            <img :src="spe.imageString" alt="spe" />
                        </div>
                        <p>{{ spe.title }}</p>
                    </div>
                </RouterLink>
            </div>
        </div>
    </div>
</template>
<style>
    /* Custom CSS to make the radio buttons larger */
    .radio-lg {
        width: 1rem;
        height: 1rem;
    }

    /* Optional: Add more spacing between radio button and label */
    .form-check-label {
        margin-left: 10px;
    }

    .doctor-items {
        padding: 20px 0;
        display: flex;
        overflow-x: scroll;
    }

    /* WebKit browsers (Chrome, Safari, Edge) */
    .doctor-items::-webkit-scrollbar {
        height: 11px;
        min-width: 25px;
    }

    .doctor-items::-webkit-scrollbar-track {
        background: #f1f1f1; /* Background of the track */
        border-radius: 10px;
    }

    .doctor-items::-webkit-scrollbar-thumb {
        background: linear-gradient(to left, #03fc5e, #3903fc); /* Color of the scroll thumb */
        border-radius: 10px; /* Rounded corners on the thumb */
        border: 2px solid #f1f1f1; /* Add a small border around the thumb */
    }

    .doctor-item {
        margin: 0px 10px;
        min-width: 280px;
        display: flex;
        flex-direction: column;
        border: 1px solid #ddd;
        border-radius: 10px;
        align-items: center;
        cursor: pointer;
    }

    .doctor-item:hover {
        box-shadow: 0.1px 0.1px 5px 0.1px #ddd;
    }

    .doctor-item:hover a {
        color: #197bdd;
        text-decoration: underline;
    }

    .doctor-item:hover i {
        transform: translateX(5px);
    }

    .doctor-item-info {
        padding: 10px;
        display: flex;
        height: 250px;
        flex-direction: column;
        align-items: center;
        text-align: center;
    }

    .doctor-title {
        display: flex;
        flex-direction: column;
        text-decoration: none;
        color: black;
        font-size: 18px;
        font-weight: 500;
        cursor: pointer;
    }

    .doctor-title:hover {
        color: #197bdd;
        text-decoration: underline;
    }

    .doctor-spes {
        align-items: center;
    }

    .doctor-img {
        width: 96px;
        height: 96px;
        border-radius: 50px;
        overflow: hidden;
    }

    .doctor-img img {
        max-width: 100%;
        object-fit: cover;
    }

    .appointment-link {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 8px;
        width: 100%;
        border-top: 1px solid #ddd;
        text-decoration: none;
        color: black;
        font-weight: 500;
        font-size: 14px;
        cursor: pointer;
        text-decoration: none !important;
    }

    .appointment-link:hover {
        color: #197bdd;
    }

    .appointment-link i {
        transition: transform 0.1s ease-in-out;
        font-size: 18px;
    }

    .specialization-items {
        display: flex;
        flex-wrap: wrap;
        flex-grow: 6;
    }

    .specialization-item {
        margin-bottom: 10px;
        margin-right: 10px;
        padding: 15px;
        display: flex;
        width: calc(100vw / 7.5);
        flex-direction: column;
        align-items: center;
        border-radius: 12px;
        cursor: pointer;
    }

    .specialization-item:hover {
        box-shadow: 1px 1px 40px #ddd;
    }

    .specialization-item p {
        margin: 0;
        font-size: 14px;
        font-weight: 500;
        text-align: center;
        color: #000;
    }

    .specialization-img {
        margin-bottom: 10px;
        width: 64px;
        height: 64px;
    }

    .specialization-img img {
        width: 100%;
    }
</style>

