<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";

    import ApiSpecialty from "@/api/ApiSpecialty";
    import { onMounted, onUpdated, reactive, ref, computed } from "vue";
    import { useRouter, useRoute } from "vue-router";

    //
    import { createVNode } from "vue";
    import { Modal } from "ant-design-vue";
    import { ExclamationCircleOutlined } from "@ant-design/icons-vue";
    import { notification } from "ant-design-vue";

    const route = useRoute();
    const router = useRouter();

    const data = ref([]);

    const pageParams = reactive({
        pageNumber: route.query.pageNumber || 1,
        pageSize: route.query.pageSize || 10,
        searchValue: route.query.searchValue || "",
        totalRecords: 0,
        statusFilter: false,
    });

    const getData = async () => {
        try {
            var result = await ApiSpecialty.GetAllByLimit(pageParams);
            if (result.data.isSucceeded) {
                data.value = result.data.data;
                pageParams.totalRecords = result.data.totalRecords;
                pageParams.pageNumber = result.data.pageNumber;
                pageParams.pageSize = result.data.pageSize;

                if (pageParams.statusFilter) {
                    if (
                        result.data.totalPages < pageParams.pageNumber &&
                        pageParams.totalRecords > 0
                    ) {
                        router.push({
                            name: "Admin_Specialty_View",
                            query: {
                                pageNumber: 1,
                                pageSize: pageParams.pageSize,
                                searchValue: pageParams.searchValue,
                            },
                        });
                    } else {
                        router.push({
                            name: "Admin_Specialty_View",
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
            console.log("GetAllByLimit: " + error);
        }
    };

    const onChange = (page, pageSize) => {
        pageParams.pageNumber = page;
        pageParams.pageSize = pageSize;

        pageParams.statusFilter = true;
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

    /**
     * Delete
     * **/
    const showDeleteConfirm = (specialtyID) => {
        Modal.confirm({
            title: "Are you sure delete this specialty?",
            icon: createVNode(ExclamationCircleOutlined),
            content: "This action can not be undone.",
            okText: "Yes",
            okType: "danger",
            cancelText: "No",
            async onOk() {
                var result = await ApiSpecialty.Delete(specialtyID);
                var type = result.data.isSucceeded ? "success" : "danger";
                var context = result.data.message;

                showNotification(type, "Delete status", context);
            },
            onCancel() {
                console.log("Cancel deletion");
            },
        });
    };

    const showNotification = (type, message, context) => {
        notification[type]({
            message: message,
            description: context,
        });
    };
</script>

<template>
    <div class="admin_dashboard">
        <AdminSideBar style="position: sticky; top: 68px" active-item="specialty" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_body">
                <div class="crud-layout-header">
                    <h2 class="crud-layout-header-title">Manage Specialty</h2>
                    <RouterLink
                        class="crud-layout-header-button"
                        :to="{ name: 'Admin_Specialty_Create' }"
                        >Create <i class="ms-1 bx bx-plus-circle"></i
                    ></RouterLink>
                </div>
                <div class="crud-layout-table">
                    <table class="table table-responsive table-bordered table-hover">
                        <thead>
                            <tr>
                                <th>Image</th>
                                <th>Title</th>
                                <th>Actions</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr v-for="item in data">
                                <td>
                                    <div class="img-container">
                                        <img :src="item.imageString" alt="image" />
                                    </div>
                                </td>
                                <td>{{ item.title }}</td>
                                <td>
                                    <a-tooltip placement="top">
                                        <template #title>
                                            <span>Update</span>
                                        </template>
                                        <RouterLink
                                            class="fs-3 text-primary"
                                            :to="{
                                                name: 'Admin_Specialty_Update',
                                                params: { id: item.specialtyID },
                                            }"
                                            ><i class="bx bxs-edit"></i
                                        ></RouterLink>
                                    </a-tooltip>
                                    <a-tooltip placement="top">
                                        <template #title>
                                            <span>Delete</span>
                                        </template>
                                        <a
                                            class="fs-3 text-danger"
                                            @click="showDeleteConfirm(item.specialtyID)">
                                            <i class="bx bxs-trash"></i>
                                        </a>
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
<style>
    .admin_dashboard {
        display: flex;
    }
    .admin_dashboard_body {
        display: flex;
        flex-direction: column;
        padding-left: 30px;
    }

    .img-container {
        width: 50px;
        height: 50px;
        padding: 5px;
        border: 1px solid #ddd;
        border-radius: 3px;
    }
    td img {
        width: 100%;
    }

    td {
        font-weight: 500;
    }
</style>
