<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";
</script>
<template>
    <div class="admin_dashboard">
        <AdminSideBar active-item="appointment" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_title">
                <h3>Appointment</h3>
            </div>
            <div class="admin_dashboard_body"></div>
        </div>
    </div>
</template>
