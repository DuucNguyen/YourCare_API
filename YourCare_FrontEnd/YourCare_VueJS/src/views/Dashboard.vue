<template>
    <div class="dashboards_container">
        <div class="dashboards_title">
            <h3>Dashboard</h3>
        </div>
        <div class="dashboards_body">
            <div class="dashboards_body_left col-md-7">
                <div class="dashboards_body_item_container">
                    <RouterLink
                        class="dashboards_body_item"
                        :to="{ name: 'Admin_DoctorProfile_View' }"
                        ><i class="bx bxs-user-rectangle fs-1"></i> DoctorProfile</RouterLink
                    >

                    <RouterLink class="dashboards_body_item" :to="{ name: 'Admin_Specialty_View' }">
                        <i class="bx bxs-category fs-1"></i> Specialty</RouterLink
                    >
                    <RouterLink class="dashboards_body_item" :to="{ name: 'Admin_User_View' }"
                        ><i class="bx bxs-user-circle fs-1"></i> User</RouterLink
                    >
                </div>
            </div>
            <div class="dashboards_body_right col-md-5"></div>
        </div>
    </div>
</template>
<style scoped>
    .dashboards_container {
        background: #fff;
        border-radius: 8px;
        overflow: hidden;
    }
    .dashboards_title {
        background: #0d61c1;
        color: #fff;
        padding: 10px;
    }
    .dashboards_title h3 {
        margin: 0;
    }
    .dashboards_body {
        padding: 10px;
    }

    .dashboards_body_item_container {
        display: flex;
        justify-content: space-around;
    }
    .dashboards_body_item {
        display: flex;
        align-items: center;
        justify-content: center;
        flex-direction: column;
        width: calc(100% / 4);
        height: 150px;
        border: 5px solid #0d61c1;
        border-radius: 5px;
        font-size: 20px;
        font-weight: 500;
        color: #0d61c1;
        cursor: pointer;
        text-decoration: none;
        color: #0d61c1;
    }
    .dashboards_body_item:hover {
        color: #fff;
        background-color: #0d61c1;
    }
</style>
