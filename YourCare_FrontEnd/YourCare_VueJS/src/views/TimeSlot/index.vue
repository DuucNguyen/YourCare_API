<script setup>
    import { ref, reactive, onMounted } from 'vue';
    import ApiTimeSlot from '@/api/ApiTimeSlot';

    const timeSlots = ref([]);
    const formState_create = reactive({
        id: null,
        startTime: null,
        endTime: null,
    });

    onMounted(()=>{
        timeSlots.value = ApiTimeSlot.GetAll();
        console.log(timeSlots.value.length);
    });


</script>
<template>



</template>