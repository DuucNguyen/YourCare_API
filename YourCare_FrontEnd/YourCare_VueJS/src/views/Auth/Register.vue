<script setup></script>
<template>
    <div class="row d-flex justify-content-between">
        <div class="col-md-4">
            <div class="nav-container">
                <RouterLink :to="{ name: 'login' }">Login</RouterLink>
                <RouterLink class="navigation-chosen" :to="{ name: 'register' }">Register</RouterLink>
            </div>
            <hr />
        </div>
    </div>
</template>
<style scoped>
    .button {
        width: 100%;
    }
    /*
.img-container {
    width: 100%;
    height: 100%;

}

img {
    width: 100%;
    height: auto;
    border-radius: 8px;
}*/

    .nav-container {
        display: flex;
        justify-content: space-around;
    }

    .nav-container a {
        text-decoration: none;
        font-size: 20px;
        font-weight: 500;
        color: black;
    }

    .nav-container a:hover {
        color: #3903fc;
    }

    .navigation-chosen {
        border-bottom: 3px solid #03fc5e !important;
        color: #3903fc !important;
    }
</style>