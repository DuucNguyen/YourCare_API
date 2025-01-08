<script setup>
    import Header from "@/shared/Header.vue";
</script>
<template>
    <Header />

    <div class="container">
        <h1>HomePage</h1>
        <RouterLink :to="{ name: 'Admin_Specialty_View' }">Specialty</RouterLink>
        <br />
        <RouterLink :to="{ name: 'Admin_User_View' }">User</RouterLink>
        <br />
        <RouterLink :to="{ name: 'Admin_DoctorProfile_View' }">DoctorProfile</RouterLink>
    </div>
</template>

