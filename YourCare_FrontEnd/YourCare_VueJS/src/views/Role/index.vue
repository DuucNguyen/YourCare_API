<script setup>
    import ApiRole from "@/api/ApiRole";
    import { ref, onMounted } from "vue";

    var roles = ref([]);

    onMounted(async () => {
        var result = await ApiRole.GetAll();
        if (result.data.isSucceeded) {
            roles.value = result.data.data;
        }
    });
</script>
<template>
    <div class="main">
        <div class="crud-layout-header">
            <h2 class="crud-layout-header-title">Manage Role</h2>
            <RouterLink class="crud-layout-header-button" :to="{ name: 'Admin_Role_Create' }">
                Create
            </RouterLink>
        </div>
        <div class="main_body">
            <table class="table table-responsive table-hover table-bordered">
                <thead>
                    <tr>
                        <th>Name</th>
                        <th>Status</th>
                        <th>Number of user</th>
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="role in roles">
                        <td>
                            <a-tag :color="role.roleName === 'Admin' ? '#cd201f' : '#3b5999'">
                                {{ role.roleName }}
                            </a-tag>
                        </td>
                        <td>
                            <a-tag :color="role.isActive ? '#87d068' : '#f50'">
                                {{ role.isActive ? "Đang hoạt động" : "Dừng" }}
                            </a-tag>
                        </td>
                        <td>
                            <a-tag :color="role.userCount > 0 ? '#87d068' : '#cd201f'">
                                {{ role.userCount }}
                            </a-tag>
                        </td>
                        <td>
                            <div v-if="role.roleName !== 'Admin'" class="d-flex">
                                <RouterLink
                                    class="user_btn"
                                    :to="{
                                        name: 'Admin_Role_Update',
                                        params: { id: role.roleId },
                                    }">
                                    <i style="color: royalblue" class="bx bxs-edit"></i>
                                    Update
                                </RouterLink>
                                <div class="user_btn">
                                    <i style="color: brown" class="bx bxs-trash"></i>
                                    Deactivate
                                </div>
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
</template>
<style scoped>
    .main {
        box-shadow: 1px 1px 20px #ddd;
    }
    .main_body {
        padding: 10px;
    }
</style>
