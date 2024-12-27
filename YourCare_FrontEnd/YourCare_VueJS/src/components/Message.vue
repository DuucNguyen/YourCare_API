<script setup>
    import { computed, defineProps } from "vue";

    const props = defineProps(["context", "isError"]);
    const classObject = computed(() => ({
        "alert-danger": !props.isError,
    }));



</script>

<template>
    <div v-if="context" class="alert alert-success alert-dismissible fade show" :class="classObject" role="alert">
        <strong>{{ context }}</strong>
        <!-- <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button> -->
    </div>
</template>