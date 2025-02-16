<script setup>
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiSpecialty from "@/api/ApiSpecialty";
    import { reactive, ref, onMounted } from "vue";
    import { RouterLink, useRoute, useRouter } from "vue-router";

    const route = useRoute();
    const router = useRouter();

    const pageParams = reactive({
        pageNumber: route.params.pageNumber || 1,
        pageSize: route.params.pageSize || 10,
        searchValue: route.params.searchValue || "",
        specialtyID: route.query.specialtyID || "", //get from query initially
        totalPages: 0,
        totalRecords: 0,
        statusFilter: false,
    });

    const doctorData = ref([]);
    const speData = ref([]);

    const getData = async () => {
        try {
            var speResult = await ApiSpecialty.GetAllSpeID();
            speData.value = speResult.data.data;

            var doctorResult = await ApiDoctorProfile.GetAllByLimit(pageParams);
            doctorData.value = doctorResult.data.data;
            pageParams.pageNumber = doctorResult.data.pageNumber;
            pageParams.pageSize = doctorResult.data.pageSize;
            pageParams.totalPages = doctorResult.data.totalPages;
            pageParams.totalRecords = doctorResult.data.totalRecords;
            if (pageParams.statusFilter) {
                if (
                    pageParams.pageNumber > doctorResult.data.totalPages ||
                    doctorResult.data.totalRecords > 0
                ) {
                    pageParams.pageNumber = 1;
                    router.push({
                        name: "Public_DanhSachBacSi",
                        query: {
                            pageNumber: 1,
                            pageSize: pageParams.pageSize,
                            searchValue: pageParams.searchValue,
                            specialtyID: pageParams.specialtyID,
                        },
                    });
                } else {
                    router.push({
                        //push to current url
                        name: "Public_DanhSachBacSi",
                        query: {
                            pageNumber: pageParams.pageNumber,
                            pageSize: pageParams.pageSize,
                            searchValue: pageParams.searchValue,
                            specialtyID: pageParams.specialtyID,
                        },
                    });
                }
                pageParams.statusFilter = !pageParams.statusFilter;
            }
        } catch (error) {
            console.log(error);
        }
    };

    const onChange = (page, pageSize) => {
        pageParams.pageNumber = page;
        pageParams.pageSize = pageSize;

        pageParams.statusFilter = true;
        getData();
    };

    const onUpdated = () => {
        if (Object.keys(route.query).length === 0) {
            pageParams.pageNumber = route.params.pageNumber || 1;
            pageParams.pageSize = route.params.pageSize || 10;
            pageParams.searchValue = route.params.searchValue || "";
            pageParams.specialtyID = route.params.specialtyID || "";

            pageParams.statusFilter = true;
            getData();
        }
    };

    onMounted(() => {
        getData();
    });
</script>
<template>
    <div class="search-bar-container">
        <div class="search-bar">
            <form method="GET">
                <input
                    @input="getData()"
                    type="text"
                    name="txtSearch"
                    placeholder="Tìm theo Bác sĩ ..."
                    v-model="pageParams.searchValue" />
                <button type="submit">
                    <i class="bx bx-search"></i>
                </button>
            </form>
        </div>
    </div>
    <div class="search-result-container row">
        <div class="p-0 col-md-3">
            <label class="form-label fw-bold">Chuyên khoa</label>
            <div class="mt-4 form-group specialization-container">
                <form method="get">
                    <div class="form-check mb-2">
                        <input
                            @click="getData()"
                            v-model="pageParams.specialtyID"
                            checked=""
                            type="radio"
                            class="form-check-input radio-lg"
                            name="specialization"
                            id="specialization-all"
                            value="all" />
                        <label class="form-check-label" for="specialization-all"> Tất cả </label>
                    </div>
                    <div v-for="spe in speData" class="form-check mb-2">
                        <input
                            @click="getData()"
                            type="radio"
                            class="form-check-input radio-lg"
                            name="specialization"
                            :id="spe.specialtyID"
                            :value="spe.specialtyID"
                            v-model="pageParams.specialtyID" />
                        <label class="form-check-label" :for="spe.specialtyID">
                            {{ spe.title }}
                        </label>
                    </div>
                </form>
            </div>
        </div>
        <div class="ms-3 p-0 col-md-8 border-start">
            <div class="p-3">
                <p class="m-0 text-secondary">
                    Tìm thấy
                    <span class="text-decoration-underline text-dark fw-bold">
                        {{ pageParams.totalRecords }}
                    </span>
                    kết quả.
                </p>
            </div>
            <div v-if="doctorData.length > 0">
                <div class="">
                    <div
                        v-for="doc in doctorData"
                        class="doctor-search-item p-4 border-top d-flex justify-content-between">
                        <div style="cursor: pointer" class="d-flex">
                            <div
                                style="width: 120px; height: 120px"
                                class="me-3 doctor-img rounded-circle">
                                <img :src="doc.imageString" alt="doctorImg" />
                            </div>
                            <div class="me-3 doctor-search-info">
                                <h5 class="mb-2">
                                    <span>{{ doc.doctorTitle }}</span> -
                                    <span> {{ doc.fullName }}</span>
                                </h5>
                                <div class="mb-1 d-flex flex-wrap">
                                    <label
                                        v-for="spe in doc.specialties"
                                        class="specialization-item-capsule"
                                        >{{ spe.title }}</label
                                    >
                                </div>
                                <p class="m-0">{{ doc.address }}</p>
                            </div>
                        </div>
                        <div class="d-flex align-items-center">
                            <RouterLink
                                class="button-hover"
                                :to="{ name: 'User_Doctor_Detail', params: { doctorID: doc.doctorProfileID } }">
                                Đặt khám <i class="bx bx-right-arrow-alt"></i>
                            </RouterLink>
                        </div>
                    </div>
                </div>

                <a-pagination
                    @change="onChange"
                    v-model="pageParams.pageNumber"
                    :total="pageParams.totalRecords"
                    :pageSize="pageParams.pageSize"
                    :show-total="(total, range) => `${range[0]}-${range[1]} of ${total} items`"
                    show-quick-jumper
                    show-page-changer
                    class="crud-layout-pagination"></a-pagination>
            </div>
            <div v-else class="m-3 text-secondary">
                <h2>
                    <i class="bx bx-info-circle"></i> Không có kết quả tìm kiếm với từ khoá vừa sử
                    dụng.
                </h2>
                <p>Bạn có thể kiểm tra và thử lại với một số gợi ý sau:</p>
                <ul>
                    <li>Kiểm tra xem từ đó có đúng chính tả không.</li>
                    <li>
                        Hãy thử giảm số lượng từ trong cụm từ tìm kiếm của bạn hoặc thử tìm kiếm lại
                        bằng cụm từ tổng quát hơn.
                    </li>
                    <li>
                        Nếu cụm từ tìm kiếm của bạn có nhiều hơn một từ, hãy kiểm tra khoảng cách.
                    </li>
                    <li>Thay đổi tuỳ chọn tìm kiếm hoặc cụm từ khác phổ biến hơn.</li>
                </ul>
            </div>
        </div>
    </div>
</template>
<style>
    /**
    Searchbar
    **/

    .search-bar-container {
        position: absolute;
        left: 0;
        top: 60px;
        width: 100vw;
        padding: 20px;
        background: #1975dc;
        display: flex;
        align-items: center;
    }

    .search-bar {
        width: 50%;
        margin: auto;
    }

    .search-bar form {
        display: flex;
        background: #fff;
        align-items: center;
        justify-content: space-between;
        border-radius: 50px;
    }

    .search-bar input {
        width: 100%;
        padding: 10px 20px;
        border: none;
        border-radius: 50px;
    }

    .search-bar input:focus {
        outline: none;
    }

    .search-bar button {
        padding: 5px 10px;
        display: flex;
        align-items: center;
        background: none;
        border: none;
        font-size: 30px;
        color: #1975dc;
        cursor: pointer;
        text-align: center;
    }

    .search-result-container {
        margin-top: 100px;
    }

    /**
    spe
    **/

    .specialization-container {
        padding-left: 10px;
        height: 400px;
        overflow-y: auto;
    }

    /* WebKit browsers (Chrome, Safari, Edge) */
    .specialization-container::-webkit-scrollbar {
        width: 10px;
    }

    .specialization-container::-webkit-scrollbar-track {
        background: #f1f1f1; /* Background of the track */
        border-radius: 10px;
    }

    .specialization-container::-webkit-scrollbar-thumb {
        background: linear-gradient(to bottom, #03fc5e, #3903fc); /* Color of the scroll thumb */
        border-radius: 10px; /* Rounded corners on the thumb */
        border: 1px solid #f1f1f1; /* Add a small border around the thumb */
    }

    /**
    Doctor
    **/

    .doctor-search-item {
        border-radius: 15px;
        box-shadow: 1px 1px 10px #ddd;
    }

    .doctor-search-item i {
        font-size: 16px;
    }

    .doctor-search-item h6 {
        font-size: 20px;
        color: #1975dc;
    }

    .doctor-search-info p {
        font-size: 13px;
        font-weight: 500;
    }

    .doctor-search-info:hover h5 {
        text-decoration: underline;
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
</style>
