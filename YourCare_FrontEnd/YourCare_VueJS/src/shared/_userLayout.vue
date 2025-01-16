<script setup lang="ts">
    import { RouterView } from "vue-router";
    import Header from "./Header.vue";
</script>

<template>
    <Header/>
    <div class="container">
        <RouterView />
    </div>
</template>
