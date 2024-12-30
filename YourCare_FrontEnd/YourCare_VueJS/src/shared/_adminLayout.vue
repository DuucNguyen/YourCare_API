<script setup>
    import Header from "./Header.vue";
</script>
<template>
    <Header />
    <div class="container">
        <main role="main" class="pb-3">
            <h1>_AdminLayout</h1>
            <RouterView />
        </main>
    </div>
</template>
