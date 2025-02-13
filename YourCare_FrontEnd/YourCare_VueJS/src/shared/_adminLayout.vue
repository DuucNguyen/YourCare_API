<script setup>
    import { ref, reactive, onMounted, onBeforeMount } from "vue";
    import ApiUser from "@/api/ApiUser";
    import { useAuthStore } from "@/stores/auth-store";

    const authStore = useAuthStore();
    const user = ref({});

    onBeforeMount(async () => {
        user.value = await authStore.getUserInfo();
        console.log(user);
    });
</script>
<template>
    <div class="admin_header border-bottom">
        <div class="admin_header_title">
            <RouterLink :to="{ name: 'Admin_Dashboards' }">Dashboads</RouterLink>
        </div>
    </div>
    <div class="sidebar container">
        <div class="sidebar_header">
            <div class="sidebar_header_img">
                <img :src="user.image" />
            </div>
            <div class="sidebar_header_user_info">
                <span>{{ user.fullName }}</span>
                <div>
                    <span v-for="role in user.roleName">
                        {{ role }}
                    </span>
                </div>
            </div>
        </div>
        <div class="sidebar_body">
            <div class="sidebar_body_item_container">
                <ul>
                    <li>
                        <RouterLink
                            class="sidebar_body_item"
                            :to="{ name: 'Admin_DoctorProfile_View' }">
                            <i class="bx bxs-user-rectangle fs-2"></i>
                            <div class="sidebar_body_item_title">
                                <span>DoctorProfile</span>
                                <i class="bx bx-link-external"></i>
                            </div>
                        </RouterLink>
                    </li>
                    <li>
                        <RouterLink
                            class="sidebar_body_item"
                            :to="{ name: 'Admin_Specialty_View' }">
                            <i class="bx bxs-category fs-2"></i>
                            <div class="sidebar_body_item_title">
                                <span>Specialty</span>
                                <i class="bx bx-link-external"></i>
                            </div>
                        </RouterLink>
                    </li>
                    <li>
                        <RouterLink class="sidebar_body_item" :to="{ name: 'Admin_User_View' }">
                            <i class="bx bxs-user-circle fs-2"></i>
                            <div class="sidebar_body_item_title">
                                <span>User</span>
                                <i class="bx bx-link-external"></i>
                            </div>
                        </RouterLink>
                    </li>
                    <li>
                        <RouterLink class="sidebar_body_item" :to="{ name: 'Admin_TimeSlot_View' }">
                            <i class='bx bxs-time-five fs-2' ></i>
                            <div class="sidebar_body_item_title">
                                <span>TimeSlot</span>
                                <i class="bx bx-link-external"></i>
                            </div>
                        </RouterLink>
                    </li>
                </ul>
            </div>
        </div>
        <div class="sidebar_footer"></div>
    </div>
    <div class="body_container">
        <main role="main" class="pb-3">
            <RouterView />
        </main>
    </div>
</template>
<style>
    body {
        background: #ececec;
    }

    .body_container {
        margin-top: 80px;
        width: 92%;
        position: absolute;
        right: 0;
        margin-right: 30px;
    }
    .admin_header {
        padding: 5px 20px;
        background-color: #fff;
        position: fixed;
        width: 100%;
        height: 60px;
        z-index: 99;
        display: flex;
        align-items: center;
    }
    .admin_header_title {
        background: linear-gradient(to right, #3903fc, #03fc5e);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
        color: transparent;
    }

    .admin_header_title a {
        font-size: 30px;
        font-weight: 700;
        text-decoration: none;
    }

    .sidebar_header {
        padding: 10px 0;
        display: flex;
        align-items: center;
        border-bottom: 1px solid #fff;
    }
    .sidebar_header_img {
        width: 50px;
        height: 50px;
        margin-right: 10px;
        border-radius: 50%;
        overflow: hidden;
        flex-shrink: 0;
    }

    .sidebar_header_img img {
        width: 100%;
        object-fit: cover;
    }

    .sidebar_header_user_info {
        color: #fff;
        visibility: hidden;
        opacity: 0;
        transition: all 0.3s ease;
    }

    .sidebar_header_user_info div:nth-child(2) {
        color: #999;
        font-size: 14px;
    }

    .sidebar {
        margin-top: 60px;
        position: fixed;
        height: 100vh;
        width: 60px;
        background: #fff;
        padding: 0 5px;
        z-index: 99;
        transition: width 0.3s ease, background-color 0.1s ease;
    }

    .sidebar:hover {
        width: 300px;
        background: #101a33;
    }

    .sidebar_body_item_container ul {
        list-style: none;
        padding: 0 5px;
    }

    .sidebar_body_item {
        margin: 5px 0;
        padding: 3px;
        display: flex;
        align-items: center;
        text-decoration: none;
        font-size: 18px;
        font-weight: 300;
        color: #101a33;
        border-bottom: 2px solid transparent;
    }

    .sidebar_body_item i {
        margin-right: 10px;
        transition: color 0.2s ease;
    }

    .sidebar_body_item_title {
        width: 100%;
        display: flex;
        justify-content: space-between;
        align-items: center;
        color: #fff;
        opacity: 0;
        visibility: hidden;
        transition: visibility 0.3s ease, opacity 0.3s ease;
    }

    .sidebar_body_item_title i {
        visibility: hidden;
        opacity: 0;
    }

    .sidebar:hover .sidebar_header_user_info {
        visibility: visible;
        opacity: 1;
    }

    .sidebar:hover .sidebar_body_item_title {
        visibility: visible;
        opacity: 1;
    }
    .sidebar:hover .sidebar_body_item i {
        color: #fff;
    }

    .sidebar_body_item:hover {
        border-bottom: 2px solid #fff;
    }

    .sidebar_body_item:hover .sidebar_body_item_title i {
        visibility: visible;
        opacity: 1;
    }
</style>
