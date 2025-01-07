<script setup></script>
<template>
    <h1>HomePage</h1>
    <RouterLink :to="{ name: 'Admin_Specialty_View' }">Specialty</RouterLink>
    <br />
    <RouterLink :to="{ name: 'Admin_User_View' }">User</RouterLink>
    <br />
    <RouterLink :to="{ name: 'Admin_DoctorProfile_View' }">DoctorProfile</RouterLink>
</template>

