<script setup></script>

<template>
    <div style="background: #fff" class="container">
        <h1>Dashboard</h1>
        <RouterLink :to="{ name: 'Admin_Specialty_View' }">Specialty</RouterLink>
        <br />
        <RouterLink :to="{ name: 'Admin_User_View' }">User</RouterLink>
        <br />
        <RouterLink :to="{ name: 'Admin_DoctorProfile_View' }">DoctorProfile</RouterLink>
    </div>
</template>
