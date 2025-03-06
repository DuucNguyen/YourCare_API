<script setup>
    import ApiUser from "@/api/ApiUser";
    import ApiRole from "@/api/ApiRole";
    import { reactive, ref, onMounted, onUpdated } from "vue";
    import { RouterLink, useRoute, useRouter } from "vue-router";
    import { message } from "ant-design-vue";

    //
    const route = useRoute();
    const router = useRouter();

    const roles = ref([]);
    const showModalChangeRole = ref(false);
    const selectedUser = ref({});

    const formChangeRole = ref();
    const formStateChangeRole = reactive({
        roleId: "",
    });

    const rulesChangeRole = {
        roleId: [
            {
                required: "true",
                message: "Chọn 1 trước khi tiếp tục",
            },
        ],
    };

    const pageParams = reactive({
        pageNumber: route.params.pageNumber || 1,
        pageSize: route.params.pageSize || 10,
        searchValue: route.params.searchValue || "",
        totalRecords: 0,
        statusFilter: false,
    });

    const data = ref([]);

    const InitRole = async () => {
        var result = await ApiRole.GetAll();
        if (result.data.isSucceeded) {
            roles.value = result.data.data;
        }
    };

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

    const openModalChangeRole = (user) => {
        selectedUser.value = user;
        showModalChangeRole.value = true;
    };

    const onFinishChangeRole = () => {
        formChangeRole.value
            .validate()
            .then(async () => {
                var formData = new FormData();
                formData.append("UserId", selectedUser.value.id);
                formData.append("RoleId", formStateChangeRole.roleId);

                var result = await ApiRole.ChangeUserRole(formData);

                if (result.data.isSucceeded) {
                    message.success(result.data.message);
                    showModalChangeRole.value = false;

                    await getData();
                } else {
                    message.error(result.data.message);
                    showModalChangeRole.value = false;
                }
            })
            .catch((error) => {
                console.log(error);
            });
    };

    onMounted(async () => {
        await getData();
        await InitRole();
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
        <div>
            <div v-for="user in data" class="user_item">
                <div class="col-md-2 user_item_image">
                    <img :src="user.imageString" />
                </div>
                <div class="col-md-10 user_item_info">
                    <a-row>
                        <a-col :span="4">Họ và tên</a-col>
                        <a-col :span="12">
                            <span>{{ user.fullName }}</span>
                        </a-col>
                    </a-row>
                    <a-divider class="m-1"></a-divider>
                    <div class="mt-3">
                        <a-row>
                            <a-col :span="4">Email</a-col>
                            <a-col :span="12">
                                <span>{{ user.email }}</span>
                            </a-col>
                        </a-row>
                        <a-row>
                            <a-col :span="4">SĐT</a-col>
                            <a-col :span="12">
                                <span>{{ user.phoneNumber }}</span>
                            </a-col>
                        </a-row>
                        <a-divider class="m-1"></a-divider>
                        <a-row class="user_btn_container">
                            <a-col :span="4">
                                <a-tooltip placement="top">
                                    <template #title>
                                        <span>Change role</span>
                                    </template>
                                    <a class="user_btn" @click="openModalChangeRole(user)">
                                        <i style="color: coral" class="bx bx-git-compare"></i>Change
                                        role
                                    </a>
                                </a-tooltip>
                            </a-col>
                            <a-col :span="4"
                                ><a-tooltip placement="top">
                                    <template #title>
                                        <span>Details</span>
                                    </template>
                                    <RouterLink
                                        class="user_btn"
                                        :to="{
                                            name: 'Admin_User_Detail',
                                            params: { id: user.id },
                                        }">
                                        <i style="color: blueviolet" class="bx bxs-user-detail"></i>
                                        Detail
                                    </RouterLink>
                                </a-tooltip>
                            </a-col>
                            <a-col :span="4">
                                <a-tooltip placement="top">
                                    <template #title>
                                        <span>Create doctor profile</span>
                                    </template>
                                    <RouterLink
                                        class="user_btn"
                                        :to="{
                                            name: 'Admin_DoctorProfile_Create',
                                            params: { id: user.id },
                                        }"
                                        ><i style="color: forestgreen" class="bx bxs-user-badge"></i
                                        >Create doctor</RouterLink
                                    >
                                </a-tooltip>
                            </a-col>
                            <a-col :span="4"
                                ><a-tooltip placement="top">
                                    <template #title>
                                        <span>Update</span>
                                    </template>
                                    <RouterLink
                                        class="user_btn"
                                        :to="{
                                            name: 'Admin_User_Update',
                                            params: { id: user.id },
                                        }"
                                        ><i style="color: royalblue" class="bx bxs-edit"></i
                                        >Update</RouterLink
                                    >
                                </a-tooltip>
                            </a-col>
                            <a-col :span="4">
                                <a-tooltip placement="top">
                                    <template #title>
                                        <span>Deactivate</span>
                                    </template>
                                    <RouterLink
                                        class="user_btn"
                                        :to="{
                                            name: 'Admin_User_Update',
                                            params: { id: user.id },
                                        }">
                                        <i style="color: brown" class="bx bxs-trash"></i>
                                        Update</RouterLink
                                    >
                                </a-tooltip>
                            </a-col>
                        </a-row>
                    </div>
                </div>
            </div>
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
    </div>

    <a-modal
        width="750px"
        title="Thay đổi vai trò trong hệ thống"
        v-model:open="showModalChangeRole"
        centered>
        <a-row>
            <a-col :span="24">
                <a-alert
                    message="Thay đổi này có tác động đến quyền truy cập của người dùng này trong hệ thống."
                    banner />
            </a-col>
        </a-row>

        <a-row>
            <a-col :span="12">
                <a-divider orientation="left" orientation-margin="0px">Hiện tại</a-divider>
                <div class="mt-3 d-flex">
                    <div style="width: 80px; height: 80px" class="user_item_image">
                        <img :src="selectedUser.imageString" />
                    </div>
                    <div class="mt-1 ms-3 user_item_info">
                        <a-row>
                            <a-col :span="24">
                                <span class="fs-6">{{ selectedUser.fullName }}</span>
                            </a-col>
                            <a-tag
                                :color="selectedUser.roleName === 'Admin' ? '#cd201f' : '#3b5999'">
                                {{
                                    selectedUser.roleName ? selectedUser.roleName : "chưa cấu hình"
                                }}
                            </a-tag>
                        </a-row>
                    </div>
                </div>
            </a-col>
            <a-col :span="11">
                <a-divider orientation="left" orientation-margin="0px">Thay đổi</a-divider>
                <div class="mt-3 d-flex">
                    <div style="width: 80px; height: 80px" class="user_item_image">
                        <img :src="selectedUser.imageString" />
                    </div>
                    <div class="mt-1 ms-3 user_item_info">
                        <a-row>
                            <a-col :span="24">
                                <span class="fs-6">{{ selectedUser.fullName }}</span>
                            </a-col>
                        </a-row>
                        <a-row style="min-width: 150px">
                            <a-col :span="24">
                                <a-form
                                    ref="formChangeRole"
                                    :model="formStateChangeRole"
                                    :rules="rulesChangeRole">
                                    <a-form-item name="roleId">
                                        <a-select
                                            v-model:value="formStateChangeRole.roleId"
                                            placeholder="Chọn vai trò">
                                            <a-select-option
                                                v-for="role in roles"
                                                :value="role.roleId">
                                                {{ role.roleName }}
                                            </a-select-option>
                                        </a-select>
                                    </a-form-item>
                                </a-form>
                            </a-col>
                        </a-row>
                    </div>
                </div>
            </a-col>
        </a-row>
        <template #footer>
            <a-button key="Hủy" @click="showModalChangeRole = false">Hủy</a-button>
            <a-button key="Xác nhận" type="primary" @click="onFinishChangeRole">
                Xác nhận
            </a-button>
        </template>
    </a-modal>
</template>

<style scoped>
    .user_item {
        border: 1px solid #ddd;
        border-radius: 5px;
        margin: 10px 0px;
        padding: 10px;
        display: flex;
        align-items: center;
        justify-content: space-between;
    }

    .user_item_image {
        width: 120px;
        height: 120px;
        border-radius: 50%;
        border: 5px solid #1975dc;
        overflow: hidden;
    }
    .user_item_image img {
        width: 100%;
        height: 100%;
        object-fit: contain;
    }

    .user_item_info span {
        font-weight: 500;
        font-size: 16px;
    }
    .user_btn_container {
        display: flex;
        align-items: end;
        justify-content: end;
        width: 100%;
        cursor: pointer;
    }
</style>
