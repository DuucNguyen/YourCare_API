<script setup>
    import { reactive, ref, onMounted } from "vue";
    import ApiUser from "@/api/ApiUser";

    const user = ref({});

    onMounted(async () => {
        var result = await ApiUser.GetUser();
        user.value = result.data;
    });
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
                <div class="nav-user">
                    <i class="bx bx-user-circle fs-4"></i>
                    <span>
                        {{ user.fullName }}
                    </span>
                </div>
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
</style>
