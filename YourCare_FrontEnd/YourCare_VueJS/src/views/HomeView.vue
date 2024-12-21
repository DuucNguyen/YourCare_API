<script setup>
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
</script>

<template>
        <h1>HomeView</h1>
        <button @click="ApiDoctorProfile.GetALl()">Get all doctor</button>
</template>

