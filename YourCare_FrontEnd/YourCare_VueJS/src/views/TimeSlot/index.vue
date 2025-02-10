<script setup>
    import { ref, reactive, onMounted } from "vue";
    import ApiTimeSlot from "@/api/ApiTimeSlot";
    import dayjs from "dayjs";

    //
    import {
        PlusOutlined,
        MinusCircleOutlined,
        ExclamationCircleOutlined,
    } from "@ant-design/icons-vue";
    //
    import { createVNode } from "vue";
    import { Modal } from "ant-design-vue";
    import { notification } from "ant-design-vue";
    import { resolveTypeElements } from "vue/compiler-sfc";

    const timeSlots = ref([]);

    const formState = reactive({
        timeSlots: [], //return value of time-range-picker are 2
    });

    onMounted(async () => {
        var result = await ApiTimeSlot.GetAll();
        timeSlots.value = result.data.data;
        timeSlots.value.forEach((item) => {
            formState.timeSlots.push({
                timeRange: [dayjs(item.startTime, "HH:mm:ss"), dayjs(item.endTime, "HH:mm:ss")],
            });
        });
    });

    const formRef = ref();

    const formItemLayout = {
        labelCol: {
            xs: {
                span: 10,
            },
            sm: {
                span: 8,
            },
        },
        wrapperCol: {
            xs: {
                span: 10,
            },
            sm: {
                span: 16,
            },
        },
    };
    const formItemLayoutWithOutLabel = {
        wrapperCol: {
            xs: {
                span: 24,
                offset: 0,
            },
            sm: {
                span: 24,
                offset: 8,
            },
        },
    };

    const validateTime = (value) => {
        //value is timeRange[]

        if (!value || value.length !== 2) {
            return Promise.reject("Start time and End time are required.");
        }
        if (dayjs(value[1]).isBefore(dayjs(value[0]))) {
            //no need all hail ant-design
            return Promise.reject("Endtime must be after StartTime");
        }
        return Promise.resolve();
    };

    const addTimeSlot = () => {
        formState.timeSlots.push({ timeRange: [] });
    };

    const removeTimeSlot = (timeSlot) => {
        var index = formState.timeSlots.indexOf(timeSlot);
        if (index !== -1) {
            formState.timeSlots.splice(index, 1);
        }
    };

    const onFinish = () => {
        formRef.value
            .validate()
            .then(() => {
                showConfirmation();
            })
            .catch((error) => {
                console.log("Error: " + error);
            });
    };

    const showConfirmation = () => {
        Modal.confirm({
            title: "Save changes ?",
            icon: createVNode(ExclamationCircleOutlined),
            content: "Are you sure to perform this action ?",
            okText: "Yes",
            cancelText: "Cancel",
            onOk: async () => {
                var formData = new FormData();
                formState.timeSlots.forEach((timeSlot, index) => {
                    // Convert timestamp to "HH:mm:ss" format using Day.js
                    const formatTime = (timestamp) => dayjs(timestamp).format("HH:mm:ss");

                    formData.append(
                        `timeSlots[${index}].startTime`,
                        formatTime(timeSlot.timeRange[0]),
                    );
                    formData.append(
                        `timeSlots[${index}].endTime`,
                        formatTime(timeSlot.timeRange[1]),
                    );
                });

                var result = await ApiTimeSlot.CreateRange(formData);

                var type = result.data.isSucceeded ? "success" : "error";
                var description = result.data.message;

                showNotification(type, "Save changes status", description);
            },
            onCancel: () => {},
        });
    };

    const showNotification = (type, message, description) => {
        notification[type]({
            message: message,
            description: description,
        });
    };
</script>
<template>
    <div class="main">
        <div class="crud-layout-header">
            <h2 class="crud-layout-header-title">Manage TimeSlot</h2>
        </div>
        <div class="main_body">
            <a-form ref="formRef" :model="formState">
                <a-form-item
                    v-bind="formItemLayout"
                    class="dynamic_item"
                    v-for="(timeSlot, index) in formState.timeSlots"
                    :key="index"
                    :label="'Time Slot - ' + (index + 1)"
                    :name="['timeSlots', index, 'timeRange']"
                    :rules="[
                        {
                            required: true,
                            message: 'Please input value',
                            trigger: 'change',
                        },
                        {
                            validator: (rule, value) => validateTime(value),
                            trigger: 'change',
                        },
                    ]">
                    <a-time-range-picker
                        v-model:value="timeSlot.timeRange"
                        size="large"
                        format="h:mm a"
                        :minute-step="15" />

                    <MinusCircleOutlined
                        class="dynamic-delete-button"
                        @click="removeTimeSlot(timeSlot)"
                        v-if="formState.timeSlots.length > 1" />
                </a-form-item>

                <a-form-item v-bind="formItemLayoutWithOutLabel">
                    <a-button
                        style="width: 50%"
                        @click="addTimeSlot()"
                        class="d-flex align-items-center justify-content-center"
                        type="dashed">
                        <PlusOutlined />Add slot
                    </a-button>
                </a-form-item>
                <a-form-item class="d-flex justify-content-end">
                    <a-button @click="onFinish()" type="primary">Save</a-button>
                </a-form-item>
                <a-alert
                    message="Click Save button to save any changes."
                    type="warning"
                    show-icon />
            </a-form>
        </div>
    </div>
</template>
<style scoped>
    .main {
        margin: auto;
        width: 50%;
        background-color: #fff;
        box-shadow: 0px 10px 20px #ddd;
    }
    .main_body {
        padding: 20px;
    }
    .dynamic_item {
        display: flex;
        align-items: center;
        justify-content: center;
    }
    .dynamic-delete-button {
        cursor: pointer;
        font-size: 24px;
        color: #999;
        cursor: pointer;
        position: absolute;
        top: 10px;
        right: -30px;
    }
    .dynamic-delete-button:hover {
        color: red;
    }
</style>
