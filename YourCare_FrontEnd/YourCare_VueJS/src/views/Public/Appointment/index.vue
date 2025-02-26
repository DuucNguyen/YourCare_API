<script setup>
    import ApiAppointment from "@/api/ApiAppointment";
    import { useAuthStore } from "@/stores/auth-store";
    import { ref, reactive, onMounted } from "vue";

    import dayjs from "dayjs";
    //
    import UserSideBar from "@/shared/UserSideBar.vue";

    const authStore = useAuthStore();

    const appointments = ref([]);
    const chosenAppointment = ref({});

    const InitAppointments = async () => {
        var user = await authStore.getUserInfo();
        var result = await ApiAppointment.GetAllByUserID(user.id);
        if (result.data.isSucceeded) {
            appointments.value = result.data.data;
        }
    };

    const searchAppointment = () => {
        //do it here
    };

    onMounted(async () => {
        try {
            await InitAppointments();
            console.log(appointments.value.length);
        } catch (error) {
            console.log(error);
        }
    });
</script>

<template>
    <div class="d-flex">
        <UserSideBar page="history" />
        <div class="profile_container">
            <a-row class="profile_head">
                <a-col :span="24">
                    <span class="profile_head_title"> Lịch khám </span>
                </a-col>
            </a-row>
            <a-divider class="m-0"></a-divider>
            <a-row class="profile_body">
                <a-col :span="8" class="appointment_container">
                    <div class="appointment_search">
                        <a-input
                            @input="searchAppointment"
                            style="height: 40px"
                            placeholder="Mã phiếu khám, tên bệnh nhân,...">
                        </a-input>
                    </div>
                    <div>
                        <template v-for="item in appointments">
                            <div class="appointment_item">
                                <a-row>
                                    <a-col :span="24">
                                        <span style="font-weight: 500; font-size: 17px">{{
                                            item.doctorName
                                        }}</span>
                                    </a-col>
                                </a-row>
                                <a-row>
                                    <a-col :span="20">
                                        <span> {{ item.timeTableStartTime }} - </span>
                                        <span style="color: #1975dc; font-weight: 500">
                                            {{ dayjs(item.timetableDate).format("DD-MM-YYYY") }}
                                        </span>
                                        <br />
                                        <span>
                                            {{ item.patientProfileName }}
                                        </span>
                                        <br />
                                        <span class="text-secondary" style="font-size: 14px">
                                            {{ item.status }}
                                        </span>
                                    </a-col>
                                    <a-col :span="4" class="text-center">
                                        <span style="font-size: 12px">STT</span>
                                        <br />
                                        <span
                                            style="
                                                font-weight: 500;
                                                font-size: 20px;
                                                color: #22c55e;
                                            "
                                            >{{ item.timeTableOrder }}</span
                                        >
                                    </a-col>
                                </a-row>
                            </div>
                        </template>
                    </div>
                </a-col>
                <a-col :span="16"></a-col>
            </a-row>
            <a-divider class="m-0"></a-divider>
            <a-row class="profile_footer">
                <a-col :span="24">
                    <span
                        >Tổng đài hỗ trợ và chăm sóc khách hàng:
                        <span class="text-primary">0865493798</span></span
                    >
                </a-col>
            </a-row>
        </div>
    </div>
</template>
<style scoped>
    .appointment_container {
        margin-left: 10px;
        padding: 5px;
        height: 500px;
        max-height: 500px;
        overflow-y: scroll;
        background: #f3f4f6;
    }

    .appointment_item {
        background-color: #fff;
        padding: 5px 8px;
        margin: 5px;
        border: 1px solid #ddd;
        border-radius: 5px;
        cursor: pointer;
    }
    .appointment_item:hover {
        border: 1px solid #1975dc;
        background: #f9fafb;
    }

    .appointment_search {
        position: sticky;
        top: 0;
        z-index: 1;
    }
</style>
