<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";

    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
    import ApiSpecialty from "@/api/ApiSpecialty";
    import ApiUser from "@/api/ApiUser";
    import { reactive, ref, onMounted, onUpdated } from "vue";
    import { useRoute, useRouter } from "vue-router";

    const route = useRoute();
    const router = useRouter();

    const data = ref([]);

    const pageParams = reactive({
        pageNumber: route.params.pageNumber || 1,
        pageSize: route.params.pageSize || 10,
        searchValue: route.params.searchValue || "",
        specialtyID: route.params.specialtyID || "",
        totalRecords: 0,
        statusFilter: false, //serve as a flag to check if pageParams is in url
    });

    const getData = async () => {
        try {
            var result = await ApiDoctorProfile.GetAllByLimit(pageParams);
            if (result.data.isSucceeded) {
                data.value = result.data.data;
                pageParams.pageNumber = result.data.pageNumber;
                pageParams.pageSize = result.data.pageSize;
                pageParams.totalRecords = result.data.totalRecords;

                if (pageParams.statusFilter) {
                    //check if filter is active
                    if (
                        pageParams.pageNumber > result.data.totalPages &&
                        pageParams.totalRecords > 0
                    ) {
                        pageParams.pageNumber = 1;

                        router.push({
                            name: "Admin_DoctorProfile_View",
                            query: {
                                pageNumber: 1,
                                pageSize: pageParams.pageSize,
                                searchValue: pageParams.searchValue,
                                specialtyID: pageParams.specialtyID,
                            },
                        });
                    } else {
                        router.push({
                            name: "Admin_DoctorProfile_View",
                            query: {
                                pageNumber: pageParams.pageNumber,
                                pageSize: pageParams.pageSize,
                                searchValue: pageParams.searchValue,
                                specialtyID: pageParams.specialtyID,
                            },
                        });
                    }
                    pageParams.statusFilter = !pageParams.statusFilter; //toggle filter status
                }
            }
        } catch (error) {
            console.log("ERROR: GETALLBYLIMIT doctorProfile." + error);
        }
    };

    onMounted(() => {
        getData();
    });

    onUpdated(() => {
        if (Object.keys(route.query).length === 0) {
            pageParams.pageNumber = route.params.pageNumber || 1;
            pageParams.pageSize = route.params.pageSize || 10;
            pageParams.searchValue = route.params.searchValue || "";
            pageParams.specialtyID = route.params.specialtyID || "";
            pageParams.statusFilter = true;

            getData();
        }
    });

    const onChange = (page, pageSize) => {
        pageParams.pageNumber = page;
        pageParams.pageSize = pageSize;
        pageParams.statusFilter = true;
        getData();
    };
</script>

<template>
    <div class="admin_dashboard">
        <AdminSideBar style="position: sticky; top: 65px" active-item="doctor" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_body">
                <div class="crud-layout-header">
                    <h2 class="crud-layout-header-title">Manage DoctorProfile</h2>
                </div>
                <a-pagination
                    @change="onChange"
                    v-model:current="pageParams.pageNumber"
                    :total="pageParams.totalRecords"
                    :pageSize="pageParams.pageSize"
                    :show-total="(total, range) => `${range[0]}-${range[1]} of ${total} items`"
                    show-size-changer
                    show-quick-jumper
                    class="crud-layout-pagination"></a-pagination>
                <div class="crud-layout-table">
                    <table class="table table-responsive table-bordered">
                        <thead>
                            <tr>
                                <th>Image</th>
                                <th>Information</th>
                                <th>Specialties</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="item in data">
                                <td>
                                    <div
                                        style="width: 90px; height: 120px; object-fit: fill"
                                        class="list-image-container">
                                        <img :src="item.imageString" alt="avatar" />
                                    </div>
                                </td>

                                <td>
                                    <div class="d-flex flex-column information-container">
                                        <div><span>FullName:</span>{{ item.fullName }}</div>
                                        <div><span>Email:</span>{{ item.email }}</div>
                                        <div><span>Phone:</span>{{ item.phoneNumber }}</div>
                                        <div><span>Address:</span>{{ item.address }}</div>
                                    </div>
                                </td>
                                <td>
                                    <label
                                        class="specialization-item-capsule"
                                        v-for="spe in item.specialties">
                                        {{ spe.title }}
                                    </label>
                                </td>
                                <td class="text-end">
                                    <a-tooltip placement="top">
                                        <template #title>
                                            <span>Timetable</span>
                                        </template>
                                        <RouterLink
                                            class="fs-3 text-primary"
                                            :to="{
                                                name: 'Admin_DoctorProfile_Timetable',
                                                params: { id: item.doctorProfileID },
                                            }"
                                            ><i class="bx bx-calendar"></i
                                        ></RouterLink>
                                    </a-tooltip>
                                    <a-tooltip placement="top">
                                        <template #title>
                                            <span>Update</span>
                                        </template>
                                        <RouterLink
                                            class="fs-3 text-success"
                                            :to="{
                                                name: 'Admin_DoctorProfile_Update',
                                                params: { id: item.doctorProfileID },
                                            }"
                                            ><i class="bx bxs-edit"></i
                                        ></RouterLink>
                                    </a-tooltip>
                                    <a-tooltip placement="top">
                                        <template #title>
                                            <span>Delete</span>
                                        </template>
                                        <RouterLink
                                            class="fs-3 text-danger"
                                            :to="{
                                                name: 'Admin_DoctorProfile_Update',
                                                params: { id: item.doctorProfileID },
                                            }"
                                            ><i class="bx bxs-trash"></i
                                        ></RouterLink>
                                    </a-tooltip>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                    <a-pagination
                        @change="onChange"
                        v-model:current="pageParams.pageNumber"
                        :total="pageParams.totalRecords"
                        :pageSize="pageParams.pageSize"
                        :show-total="(total, range) => `${range[0]}-${range[1]} of ${total} items`"
                        show-size-changer
                        show-quick-jumper
                        class="crud-layout-pagination"></a-pagination>
                </div>
            </div>
        </div>
    </div>
</template>
<style scoped>
    .information-container span {
        display: inline-block;
        width: 100px;
        font-weight: 500;
    }
    .admin_dashboard_body {
        display: flex;
        flex-direction: column;
        padding-left: 30px;
    }

    .admin_dashboard {
        display: flex;
    }
</style>
