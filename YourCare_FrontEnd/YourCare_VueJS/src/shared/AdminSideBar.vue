<script setup>
    import { ref, onBeforeMount } from "vue";
    import { useAuthStore } from "@/stores/auth-store";

    const { activeItem } = defineProps(["activeItem"]);

    const isShrinkView = ref(true);

    const toggleSidebar = () => {
        isShrinkView.value = !isShrinkView.value;
    };

    const authStore = useAuthStore();
    const user = ref({});

    onBeforeMount(async () => {
        user.value = await authStore.getUserInfo();
    });
</script>
<template>
    <div :class="['sidebar-container', { shrink: isShrinkView }]">
        <button
            class="sidebar-viewButton"
            type="button"
            :aria-label="isShrinkView ? 'Expand Sidebar' : 'Shrink Sidebar'"
            @click="toggleSidebar">
            <i class="bx bx-left-arrow-alt"></i>
        </button>
        <div class="sidebar-wrapper">
            <div class="sidebar-profileSection">
                <img :src="user.image" />
                <div>
                    <span>{{ user.fullName }}</span> <br />
                    <a-tag color="#cd201f">
                        {{ user.roleName }}
                    </a-tag>
                </div>
            </div>
            <ul class="sidebar-list">
                <li :class="['sidebar-listItem', { active: activeItem === 'dashboard' }]">
                    <RouterLink :to="{ name: 'Admin_Dashboards' }">
                        <i class="bx bxs-dashboard"></i>
                        <span class="sidebar-listItemText">Thống kê</span>
                    </RouterLink>
                </li>
                <li :class="['sidebar-listItem', { active: activeItem === 'appointment' }]">
                    <RouterLink :to="{ name: 'Admin_Appointments' }"
                        ><i class="bx bxs-calendar"></i>
                        <span class="sidebar-listItemText">Lịch khám hôm nay</span>
                    </RouterLink>
                </li>
                <li :class="['sidebar-listItem', { active: activeItem === 'user' }]">
                    <RouterLink :to="{ name: 'Admin_User_View' }">
                        <i class='bx bxs-user-circle' ></i>
                        <span class="sidebar-listItemText">User</span>
                    </RouterLink>
                </li>
                <li :class="['sidebar-listItem', { active: activeItem === 'doctor' }]">
                    <RouterLink :to="{ name: 'Admin_DoctorProfile_View' }">
                        <i class='bx bxs-user-badge'></i>
                        <span class="sidebar-listItemText">Doctor</span>
                    </RouterLink>
                </li>
                <li :class="['sidebar-listItem', { active: activeItem === 'specialty' }]">
                    <RouterLink :to="{ name: 'Admin_Specialty_View' }">
                        <i class='bx bxs-category'></i>
                        <span class="sidebar-listItemText">Specialty</span>
                    </RouterLink>
                </li>
                <li :class="['sidebar-listItem', { active: activeItem === 'role' }]">
                    <RouterLink :to="{ name: 'Admin_Role_View' }">
                        <i class='bx bx-git-pull-request'></i>
                        <span class="sidebar-listItemText">Role</span>
                    </RouterLink>
                </li>
                <!-- <li :class="['sidebar-listItem', { active: activeItem === 'timeSlot' }]">
                    <RouterLink :to="{ name: 'Admin_TimeSlot_View' }">
                        <i class='bx bxs-time' ></i>
                        <span class="sidebar-listItemText">TimeSlot</span>
                    </RouterLink>
                </li> -->
            </ul>
        </div>
    </div>
</template>
<style scoped>
    * {
        box-sizing: border-box;
    }

    .sidebar-container {
        background-color: #fff;
        border-radius: 10px;
        padding: 16px;
        transition: width 0.3s ease-in-out;
        width: 240px;
        height: calc(100vh - 80px);
        position: relative;
    }

    .sidebar-container.shrink {
        width: 95px;
    }

    .sidebar-wrapper {
        display: flex;
        flex-direction: column;
        height: 100%;
        position: relative;
    }

    .sidebar-viewButton {
        position: absolute;
        width: 32px;
        height: 32px;
        border-radius: 50%;
        border: none;
        right: -20px;
        top: 16px;
        background-color: #1975dc;
        color: #fff;
        cursor: pointer;
        display: flex;
        align-items: center;
        justify-content: center;
        box-shadow: 1px 1px 10px #ddd;
        z-index: 1;
    }

    .sidebar-container.shrink .sidebar-viewButton {
        transform: rotate(180deg);
    }

    .sidebar-list {
        list-style: none;
        padding: 0;
        margin: 12px 0 0;
    }

    .sidebar-listItem {
        display: flex;
        align-items: center;
        border-radius: 4px;
    }

    .sidebar-listItem:hover {
        background: #f1f5f9;
    }

    .sidebar-listItem.active {
        background-color: #1975dc;
        color: #fff !important;
    }

    .sidebar-listItem a {
        width: 100%;
        padding: 20px;
        display: flex;
        align-items: center;
        cursor: pointer;
        font-weight: 500;
        text-decoration: none;
        color: #000;
    }
    .sidebar-listItem.active a {
        color: #fff;
    }

    .sidebar-listItem i {
        font-size: 24px;
        margin-right: 8px;
        width: 20px;
        text-align: center;
    }

    .shrink .sidebar-listItem i {
        margin-right: 0;
    }

    .sidebar-listItemText {
        white-space: nowrap;
    }

    .sidebar-listItemText {
        opacity: 1;
        transition: all 0.3s ease;
    }
    .shrink .sidebar-listItemText {
        opacity: 0;
    }
    .sidebar-profileSection {
        height: 80px;
        display: flex;
        align-items: center;
    }

    .sidebar-profileSection img {
        width: 60px;
        height: 60px;
        min-width: 60px;
        min-height: 60px;
        border-radius: 50%;
        margin-right: 15px;
        border: 1px solid #1975dc;
        padding: 2px;
    }

    .sidebar-profileSection span {
        visibility: visible;
        opacity: 1;
        transition: all 0.2s ease-in-out;
        font-weight: 500;
    }

    .shrink .sidebar-profileSection span {
        visibility: hidden;
        opacity: 0;
    }
</style>
