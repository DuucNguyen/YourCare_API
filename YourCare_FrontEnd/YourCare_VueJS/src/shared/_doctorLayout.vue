<script setup>
    import { RouterView } from "vue-router";
    import Header from "./Header.vue";
    import DoctorSideBar from "./DoctorSideBar.vue";
</script>

<template>
    <Header />
    <div class="doctor_layout_container">
        <RouterView />
    </div>
</template>
