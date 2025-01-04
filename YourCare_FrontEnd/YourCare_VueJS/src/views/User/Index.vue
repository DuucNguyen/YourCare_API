<script setup>
    import ApiUser from "@/api/ApiUser";
    import { reactive, ref, onMounted, onUpdated } from "vue";

    import { RouterLink, useRoute, useRouter } from "vue-router";

    const route = useRoute();
    const router = useRouter();

    const pageParams = reactive({
        pageNumber: route.query.pageNumber || 1,
        pageSize: route.query.pageSize || 10,
        searchValue: route.query.searchValue || "",
        totalRecords: 0,
        statusFilter: false,
    });
    const data = ref([]);

    const getData = async () => {
        try {
            var result = await ApiUser.GetAllByLimit(pageParams);

            if (result.data.isSucceeded) {
                data.value = result.data.data;
                pageParams.pageNumber = result.data.pageNumber;
                pageParams.pageSize = result.data.pageSize;
                pageParams.totalRecords = result.data.totalRecords;

                if (pageParams.statusFilter) {
                    //check changes in pageParams
                    if (
                        result.data.totalPages < pageParams.pageNumber &&
                        pageParams.totalRecords > 0
                    ) {
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
            pageParams.pageNumber = route.query.pageNumber || 1;
            pageParams.pageSize = route.query.pageSize || 10;
            pageParams.searchValue = route.query.searchValue || "";
            pageParams.statusFilter = true;
            getData();
        }
    });
</script>

<template>
    <h1>User</h1>
    <div>
        <a-pagination
            @change="onChange"
            v-model="pageParams.pageNumber"
            :total="pageParams.totalRecords"
            :show-total="(total, range) => `${range[0]}-${range[1]} of ${total} items`"
            show-size-changer
            show-quick-jumper
            class="m-3 text-end"></a-pagination>

        <table class="table table-responsive table-bordered">
            <thead>
                <tr>
                    <th>Image</th>
                    <th>FullName</th>
                    <th>Email</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="user in data">
                    <td>
                        <div style="width: 100px; height: 100px">
                            <img
                                style="width: 100%; height: 100%; object-fit: contain"
                                :src="user.imageString"
                                alt="avatar" />
                        </div>
                    </td>
                    <td>{{ user.fullName }}</td>
                    <td>{{ user.email }}</td>
                    <td>
                        <RouterLink
                            class="m-1 btn btn-primary"
                            :to="{
                                name: 'Admin_DoctorProfile_Create',
                                params: { id: user.id },
                            }"
                            >Create Doctor Profile</RouterLink
                        >
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</template>
