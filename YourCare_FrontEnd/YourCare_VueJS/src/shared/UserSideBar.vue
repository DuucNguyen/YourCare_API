<script setup>
    const { page } = defineProps(["page"]);

    import { useAuthStore } from "@/stores/auth-store";
    const authStore = useAuthStore();

    const logOut = () => {
        authStore.logOut();
    };
</script>

<template>
    <div class="sidebar">
        <div class="d-flex flex-column">
            <RouterLink
                :class="'sidebar_link ' + (page == 'history' ? 'sidebar_li_chosen' : '')"
                :to="{ name: 'User_Appointment_View' }"
                >Lịch sử đặt khám</RouterLink
            >
            <RouterLink
                :class="'sidebar_link ' + (page == 'patientProfile' ? 'sidebar_li_chosen' : '')"
                :to="{ name: 'User_PatientProfile_View' }"
                >Hồ sơ</RouterLink
            >
            <RouterLink
                :class="'sidebar_link ' + (page == 'profile' ? 'sidebar_li_chosen' : '')"
                :to="{ name: 'User_Profile_View' }"
                >Tài khoản</RouterLink
            >
            <div class="sidebar_link" @click="logOut" >Đăng xuất</div>
        </div>
    </div>
</template>
<style scoped>
    .sidebar {
        margin-top: 10px;
        position: sticky;
        top: 50px;
        min-width: 300px;
        background: #fff;
        padding: 20px 0px;
        border-radius: 8px;
        box-shadow: 1px 1px 20px #ddd;
        height: fit-content;
    }

    .sidebar_link {
        text-decoration: none;
        padding: 15px 0px;
        padding-left: 20px;
        border-left: 4px solid transparent;
        cursor: pointer;
        color: #000;
        font-size: 16px;
    }

    .sidebar_link:hover {
        background-color: #f9fafb;
        /* border-left: 4px solid #1975dc; */
        color: #1975dc;
    }

    .sidebar_li_chosen {
        background-color: #f9fafb;
        border-left: 4px solid #1975dc !important;
        color: #1975dc;
    }
</style>
