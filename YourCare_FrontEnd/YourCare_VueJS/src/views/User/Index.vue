<script setup>
    import ApiUser from "@/api/ApiUser";
    import { reactive, ref, onMounted, onUpdated } from "vue";
    import { RouterLink, useRoute, useRouter } from "vue-router";

    const route = useRoute();
    const router = useRouter();

    const pageParams = reactive({
        pageNumber: route.params.pageNumber || 1,
        pageSize: route.params.pageSize || 10,
        searchValue: route.params.searchValue || "",
        totalRecords: 0,
        statusFilter: false,
    });

    const data = ref([]);

    const getData = async () => {
        try {
            console.log(pageParams.pageNumber);

            var result = await ApiUser.GetAllByLimit(pageParams);

            if (result.data.isSucceeded) {
                data.value = result.data.data;
                pageParams.pageNumber = result.data.pageNumber;
                pageParams.pageSize = result.data.pageSize;
                pageParams.totalRecords = result.data.totalRecords;

                if (pageParams.statusFilter) {
                    //check changes in pageParams
                    if (
                        pageParams.pageNumber > result.data.totalPages &&
                        pageParams.totalRecords > 0
                    ) {
                        pageParams.pageNumber = 1;
                        router.push({
                            name: "Admin_User_View",
                            query: {
                                pageNumber: 1,
                                pageSize: pageParams.pageSize,
                                searchValue: pageParams.searchValue,
                            },
                        });
                    } else {
                        router.push({
                            name: "Admin_User_View",
                            query: {
                                pageNumber: pageParams.pageNumber,
                                pageSize: pageParams.pageSize,
                                searchValue: pageParams.searchValue,
                            },
                        });
                    }

                    pageParams.statusFilter = !pageParams.statusFilter;
                }
            }
        } catch (error) {
            console.log("GetAllUserByLimit: " + error);
        }
    };

    const onChange = (page, pageSize) => {
        //pagination call this
        pageParams.pageNumber = page;
        pageParams.pageSize = pageSize;
        pageParams.statusFilter = true; //mark as changed
        getData();
    };

    onMounted(() => {
        getData();
    });

    onUpdated(() => {
        if (Object.keys(route.query).length === 0) {
            pageParams.pageNumber = route.params.pageNumber || 1;
            pageParams.pageSize = route.params.pageSize || 10;
            pageParams.searchValue = route.params.searchValue || "";
            pageParams.statusFilter = true;
            getData();
        }
    });
</script>

<template>
    <div class="crud-layout-header">
        <h2 class="crud-layout-header-title">Manage User</h2>
        <RouterLink class="crud-layout-header-button" :to="{ name: 'Admin_User_Create' }"
            >Create</RouterLink
        >
    </div>
    <div class="crud-layout-table">
        <table class="table table-responsive table-bordered">
            <thead>
                <tr>
                    <th>Image</th>
                    <th>Information</th>
                    <th>Appointments</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="user in data">
                    <td>
                        <div style="width: 90px; height: 120px">
                            <img
                                style="width: 100%; height: 100%; object-fit: cover"
                                :src="user.imageString"
                                alt="avatar" />
                        </div>
                    </td>
                    <td>
                        <div class="d-flex flex-column information-container">
                            <div><span>FullName:</span>{{ user.fullName }}</div>
                            <div><span>Email:</span>{{ user.email }}</div>
                        </div>
                    </td>
                    <td></td>
                    <td>
                        <a-tooltip placement="top">
                            <template #title>
                                <span>Details</span>
                            </template>
                            <RouterLink
                                class="fs-3 text-primary"
                                :to="{
                                    name: 'Admin_User_Detail',
                                    params: { id: user.id },
                                }"
                                ><i class="bx bxs-user-detail"></i
                            ></RouterLink>
                        </a-tooltip>
                        <a-tooltip placement="top">
                            <template #title>
                                <span>Create doctor profile</span>
                            </template>
                            <RouterLink
                                class="fs-3 text-success"
                                :to="{
                                    name: 'Admin_DoctorProfile_Create',
                                    params: { id: user.id },
                                }"
                                ><i class="bx bxs-user-badge"></i
                            ></RouterLink>
                        </a-tooltip>
                        <a-tooltip placement="top">
                            <template #title>
                                <span>Update</span>
                            </template>
                            <RouterLink
                                class="fs-3 text-success"
                                :to="{
                                    name: 'Admin_User_Update',
                                    params: { id: user.id },
                                }"
                                ><i class="bx bxs-edit"></i
                            ></RouterLink>
                        </a-tooltip>
                        <a-tooltip placement="top">
                            <template #title>
                                <span>Deactivate</span>
                            </template>
                            <RouterLink
                                class="fs-3 text-danger"
                                :to="{
                                    name: 'Admin_User_Update',
                                    params: { id: user.id },
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
</template>
