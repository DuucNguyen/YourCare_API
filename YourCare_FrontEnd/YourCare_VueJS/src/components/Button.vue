<script setup>
    import { computed, defineProps, reactive } from "vue";

    const props = defineProps(["title", "isDisabled"]);
    const classObject = computed(() => ({
        disabled: props.isDisabled,
    }));
</script>

<template>
    <button class="button" :class="classObject" :disabled="props.isDisabled">
        {{ title }}
    </button>
</template>
<style scoped>
    .button {
        width: 100%;
        background-color: #1a76e3;
        color: #fff;
        font-size: 18px;
        font-weight: 500;
        border: none;
        border-radius: 5px;
        padding: 10px 0;
    }

    .disabled {
        background-color: #ddd;
        cursor: not-allowed;
    }
</style>
