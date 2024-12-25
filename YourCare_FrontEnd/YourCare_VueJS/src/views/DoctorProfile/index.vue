<script setup>
    import ApiDoctorProfile from "@/api/ApiDoctorProfile";
</script>
<template>
    <h1>Admin_DoctorProfile_View</h1>
    <button @click="ApiDoctorProfile.GetALl()">Get all doctor</button>

</template>
