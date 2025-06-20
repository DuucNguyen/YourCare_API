<script setup>
    import { reactive, ref, onMounted } from "vue";
    import ApiUser from "@/api/ApiUser";
    import { useAuthStore } from "@/stores/auth-store";

    const authStore = useAuthStore();
    const user = ref({});

    onMounted(async () => {
        var result = await authStore.getUserInfo();
        user.value = result;
    });

    const logOut = () => {
        authStore.logOut();
    };
</script>

<template>
    <header>
        <nav
            class="navbar navbar-expand-sm navbar-toggleable-sm navbar-light bg-white border-bottom box-shadow">
            <div class="d-flex pe-5 ps-5">
                <div class="logo-container">
                    <RouterLink class="logo navbar-brand" :to="{ name: 'home' }"
                        >YourCare</RouterLink
                    >
                </div>
                <button
                    class="navbar-toggler"
                    type="button"
                    data-bs-toggle="collapse"
                    data-bs-target=".navbar-collapse"
                    aria-controls="navbarSupportedContent"
                    aria-expanded="false"
                    aria-label="Toggle navigation">
                    <span class="navbar-toggler-icon"></span>
                </button>
                <div class="navbar-collapse collapse d-sm-inline-flex justify-content-between">
                    <ul class="navbar-nav flex-grow-1">
                        <li class="nav-item">
                            <RouterLink class="nav-link text-dark" :to="{ name: 'home' }"
                                >Trang chủ</RouterLink
                            >
                        </li>
                        <li class="nav-item">
                            <a class="nav-link text-dark" href="/About">Giới thiệu</a>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="nav-user-container" v-if="user">
                <a-dropdown>
                    <div class="nav-user">
                        <i class="bx bx-user-circle fs-4"></i>
                        <span>
                            {{ user.fullName }}
                            <i class="text-secondary bx bxs-down-arrow" style="font-size: 10px"></i>
                        </span>
                    </div>
                    <template #overlay>
                        <a-menu>
                            <a-menu-item
                                v-if="user.roleName[0] == 'Admin'"
                                key="0"
                                class="profile-menu-item">
                                <RouterLink :to="{ name: 'Admin_Dashboards' }">
                                    Dashboards
                                </RouterLink>
                            </a-menu-item>
                            <a-menu-item
                                v-else-if="user.roleName[0] == 'Doctor'"
                                :key="user.roleName[0] == 'Admin' ? 1 : 0"
                                key="0"
                                class="profile-menu-item">
                                <RouterLink :to="{ name: 'Doctor_Dashboard_View' }">
                                    Dashboards
                                </RouterLink>
                            </a-menu-item>
                            <a-menu-item
                                :key="
                                    user.roleName[0] == 'Admin' || user.roleName[0] == 'Doctor'
                                        ? 1
                                        : 0
                                "
                                class="profile-menu-item">
                                <RouterLink :to="{ name: 'User_Appointment_View' }">
                                    Lịch sử đặt khám
                                </RouterLink>
                            </a-menu-item>
                            <a-menu-item key="1" class="profile-menu-item">
                                <RouterLink :to="{ name: 'User_PatientProfile_View' }"
                                    >Hồ sơ</RouterLink
                                >
                            </a-menu-item>
                            <a-menu-item class="profile-menu-item">
                                <RouterLink :to="{ name: 'User_Profile_View' }">
                                    Tài khoản</RouterLink
                                >
                            </a-menu-item>
                            <a-menu-divider />
                            <a-menu-item key="3" class="profile-menu-item" @click="logOut"
                                >Đăng xuất</a-menu-item
                            >
                        </a-menu>
                    </template>
                </a-dropdown>
            </div>
            <div v-else>
                <RouterLink class="login-button" :to="{ name: 'login' }">Đăng nhập</RouterLink>
            </div>
        </nav>
    </header>
</template>
<style>
    html {
        font-size: 14px;
    }

    @media (min-width: 768px) {
        html {
            font-size: 16px;
        }
    }

    html {
        position: relative;
        min-height: 100%;
    }

    body {
        margin-bottom: 60px;
        overflow-x: hidden;
    }
    .login-button {
        padding: 8px 12px;
        margin-right: 20px;
        border: 1px solid #1975dc;
        color: #1975dc;
        text-decoration: none;
        border-radius: 5px;
        font-weight: 500;
    }
    .login-button:hover {
        background: #1975dc;
        color: #fff;
    }

    .navbar {
        width: 100%;
        display: flex;
        justify-content: space-between;
    }
    .nav-user-container {
        margin-right: 20px;
    }
    .nav-user {
        padding: 3px;
        display: flex;
        align-items: center;
        cursor: pointer;
    }
    .nav-user:hover {
        color: #0d61c1;
        background-color: #f7f7f7;
    }
    .nav-user span {
        font-size: 15px;
        font-weight: 500;
    }
    .logo-container {
        background: linear-gradient(to right, #03fc5e, #3903fc);
        -webkit-background-clip: text;
        -webkit-text-fill-color: transparent;
        background-clip: text;
        color: transparent;
    }

    .logo {
        font-size: 30px;
        font-weight: 700;
    }

    .button-authen {
        width: 100%;
        padding: 5px 20px;
        border-radius: 5px;
        background: linear-gradient(to right, #03fc5e, #3903fc);
        color: white;
        text-decoration: none;
        border: none;
    }

    .button-authen:hover {
        background: linear-gradient(to left, #03fc5e, #3903fc);
        color: white;
    }

    .profile-menu-item a {
        text-decoration: none;
    }
    .profile-menu-item:hover {
        background: #f1f5f9 !important;
        color: #1975dc !important;
    }
</style>
