<script setup>
    // import { ref, reactive, onMounted, onBeforeMount } from "vue";
    // import ApiUser from "@/api/ApiUser";
    // import { useAuthStore } from "@/stores/auth-store";
    import { RouterView } from "vue-router";
    import Header from "./Header.vue";

    // const authStore = useAuthStore();
    // const user = ref({});

    // onBeforeMount(async () => {
    //     user.value = await authStore.getUserInfo();
    //     console.log(user);
    // });
</script>
<template>
    <!-- <div class="admin_body">
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
                            <RouterLink class="sidebar_body_item" :to="{ name: 'home' }">
                                <i class="bx bxs-home-heart"></i>
                                <div class="sidebar_body_item_title">
                                    <span>Home</span>
                                    <i class="bx bx-link-external"></i>
                                </div>
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink
                                class="sidebar_body_item"
                                :to="{ name: 'Admin_DoctorProfile_View' }">
                                <i class="bx bxs-user-rectangle"></i>
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
                                <i class="bx bxs-category"></i>
                                <div class="sidebar_body_item_title">
                                    <span>Specialty</span>
                                    <i class="bx bx-link-external"></i>
                                </div>
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink class="sidebar_body_item" :to="{ name: 'Admin_User_View' }">
                                <i class="bx bxs-user-circle"></i>
                                <div class="sidebar_body_item_title">
                                    <span>User</span>
                                    <i class="bx bx-link-external"></i>
                                </div>
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink
                                class="sidebar_body_item"
                                :to="{ name: 'Admin_TimeSlot_View' }">
                                <i class="bx bxs-time-five"></i>
                                <div class="sidebar_body_item_title">
                                    <span>TimeSlot</span>
                                    <i class="bx bx-link-external"></i>
                                </div>
                            </RouterLink>
                        </li>
                        <li>
                            <RouterLink class="sidebar_body_item" :to="{ name: 'Admin_Role_View' }">
                                <i class='bx bx-sitemap'></i>
                                <div class="sidebar_body_item_title">
                                    <span>Role</span>
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
    </div> -->

    <Header />
    <div class="doctor_layout_container">
        <RouterView />
    </div>
</template>
