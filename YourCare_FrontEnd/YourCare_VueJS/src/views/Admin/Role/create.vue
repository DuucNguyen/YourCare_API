<script setup>
    import AdminSideBar from "@/shared/AdminSideBar.vue";

    import ApiRole from "@/api/ApiRole";
    import { ref, reactive, onMounted, watch } from "vue";

    import { message, notification } from "ant-design-vue";

    const formRef = ref();
    const formState = reactive({
        roleName: "",
        roleClaims: [],
    });

    const rules = {
        roleName: [
            {
                required: true,
                message: "Vui lòng điền tên vai trò",
                trigger: "change",
            },
        ],
        roleClaims: [
            {
                type: "array",
                required: true,
                message: "Chọn ít nhất một quyền.",
            },
        ],
    };

    const checkBox_state = reactive({
        indeterminate: false,
        isCheckedAll: false,
        checkList_DoctorProfile: [],
        checkList_PatientProfile: [],
        checkList_User: [],
        checkList_Role: [],
        checkList_Specialty: [],
        checkList_TimeSlot: [],
        checkList_Timetable: [],
        checkList_All: [],
    });

    const data = [
        {
            name: "DoctorProfile",
            actions: ["Create", "View", "Update", "Delete"],
        },
        {
            name: "PatientProfile",
            actions: ["Create", "View", "Update", "Delete"],
        },
        {
            name: "User",
            actions: ["Create", "View", "Update", "Delete"],
        },
        {
            name: "Role",
            actions: ["Create", "View", "Update", "Delete"],
        },
        {
            name: "Specialty",
            actions: ["Create", "View", "Update", "Delete"],
        },
        {
            name: "TimeSlot",
            actions: ["Create", "View", "Update", "Delete"],
        },
        {
            name: "Timetable",
            actions: ["Create", "View", "Update", "Delete"],
        },
    ];

    const onCheckAll = (e) => {
        Object.assign(checkBox_state, {
            checkList_All: e.target.checked ? data : [],
            checkList_DoctorProfile: e.target.checked
                ? data.find((x) => x.name === "DoctorProfile").actions
                : [],
            checkList_PatientProfile: e.target.checked
                ? data.find((x) => x.name === "PatientProfile").actions
                : [],
            checkList_User: e.target.checked ? data.find((x) => x.name === "User").actions : [],
            checkList_Role: e.target.checked ? data.find((x) => x.name === "Role").actions : [],
            checkList_Specialty: e.target.checked
                ? data.find((x) => x.name === "Specialty").actions
                : [],
            checkList_TimeSlot: e.target.checked
                ? data.find((x) => x.name === "TimeSlot").actions
                : [],
            checkList_Timetable: e.target.checked
                ? data.find((x) => x.name === "Timetable").actions
                : [],
        });
    };

    watch(
        () => [
            ...checkBox_state.checkList_DoctorProfile,
            ...checkBox_state.checkList_PatientProfile,
            ...checkBox_state.checkList_Role,
            ...checkBox_state.checkList_User,
            ...checkBox_state.checkList_Specialty,
            ...checkBox_state.checkList_TimeSlot,
            ...checkBox_state.checkList_Timetable,
        ],
        (chosenClaims) => {
            var total_actions = data.reduce((sum, x) => sum + x.actions.length, 0);
            var count_actions = chosenClaims.length;

            checkBox_state.isCheckedAll = count_actions === total_actions;
            checkBox_state.indeterminate = count_actions && count_actions < total_actions;

            formState.roleClaims = data.flatMap((item) =>
                checkBox_state[`checkList_${item.name}`].map(
                    (action) => `${formState.roleName}_${item.name}_${action}`,
                ),
            );
        },
    );

    const claimBinding = (name) => `checkList_${name}`;

    const onFinish = () => {
        formRef.value
            .validate()
            .then(async () => {
                var formData = new FormData();

                formData.append("RoleName", formState.roleName);

                formState.roleClaims.forEach((claim, index) => {
                    formData.append(`RoleClaims[${index}]`, claim);
                });

                var result = await ApiRole.CreateListRoleClaim(formData);
                if (result.data.isSucceeded) {
                    message.success(result.data.message);
                } else {
                    message.error(result.data.message);
                }
            })
            .catch((error) => {
                console.log(error);
            });
    };
</script>

<template>
    <div class="admin_dashboard">
        <AdminSideBar style="position: sticky; top: 68px" active-item="role" />
        <div class="admin_dashboard_section">
            <div class="admin_dashboard_body">
                <div class="main">
                    <div class="crud-layout-header">
                        <h2 class="crud-layout-header-title">Create Role</h2>
                        <RouterLink
                            class="crud-layout-header-button"
                            :to="{ name: 'Admin_Role_View' }">
                            Index
                        </RouterLink>
                    </div>
                    <div class="main_body">
                        <a-form layout="vertical" ref="formRef" :model="formState" :rules="rules">
                            <a-form-item name="roleName" label="Tên vai trò mới">
                                <a-input
                                    v-model:value="formState.roleName"
                                    placeholder="Tên vai trò"></a-input>
                            </a-form-item>
                            <a-checkbox
                                class="checkbox_all"
                                v-model:checked="checkBox_state.isCheckedAll"
                                :indeterminate="checkBox_state.indeterminate"
                                @change="onCheckAll"
                                >Check all</a-checkbox
                            >
                            <a-form-item name="roleClaims" label="Quyền truy cập">
                                <div class="claim_container">
                                    <div class="claim_item" v-for="claim in data">
                                        <div class="claim_header">
                                            <span>{{ claim.name }}</span>
                                        </div>
                                        <div class="claim_body">
                                            <a-checkbox-group
                                                class="claim_group"
                                                v-model:value="
                                                    checkBox_state[claimBinding(claim.name)]
                                                ">
                                                <a-checkbox
                                                    v-for="action in claim.actions"
                                                    :value="action">
                                                    {{ action }}
                                                </a-checkbox>
                                            </a-checkbox-group>
                                        </div>
                                    </div>
                                </div>
                            </a-form-item>
                            <a-form-item class="w-100 d-flex justify-content-end">
                                <a-popconfirm
                                    placement="bottom"
                                    ok-text="Xác nhận"
                                    :okButtonProps="{
                                        type: 'primary',
                                        style: {
                                            height: '30px',
                                            width: '150px',
                                        },
                                    }"
                                    :cancelButtonProps="{
                                        style: {
                                            height: '30px',
                                            width: '80px',
                                        },
                                    }"
                                    cancel-text="Hủy"
                                    @confirm="onFinish">
                                    <template #title>
                                        <span class="fs-6 text-primary">Xác nhận tạo mới</span>
                                        <br />
                                        <span class="text-danger"
                                            >Vui lòng kiểm tra lại thông tin phía trên.</span
                                        >
                                    </template>
                                    <a-button
                                        type="primary"
                                        style="height: 40px; font-weight: 500"
                                        class="w-100 fs-6">
                                        Tạo mới
                                    </a-button>
                                </a-popconfirm>
                            </a-form-item>
                        </a-form>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<style scoped>
    .admin_dashboard {
        display: flex;
    }
    .admin_dashboard_body {
        display: flex;
        flex-direction: column;
        padding-left: 30px;
    }
    .main {
        margin: auto;
        width: 60%;
        background-color: #fff;
        box-shadow: 0px 10px 20px #ddd;
    }
    .main_body {
        padding: 20px;
    }

    .claim_item {
        border: 1px solid #1975dc;
        border-radius: 5px;
        padding: 20px 10px;
        margin: 30px 0px;
        position: relative;
        width: fit-content;
    }
    .claim_header {
        text-align: center;
        width: fit-content;
        padding: 0px 15px;
        position: absolute;
        top: -17px;
        background: #1975dc;
        color: #fff;
        border-radius: 50px;
        font-size: 18px;
        font-weight: 500;
    }
    .claim_body {
        display: flex;
    }
    .claim_container {
        display: flex;
        justify-content: space-between;
        flex-wrap: wrap;
    }
    .checkbox_all {
        border: 1px solid #1975dc;
        color: #1975dc;
        padding: 2px 15px;
        border-radius: 50px;
        font-size: 18px;
        font-weight: 500;
    }
</style>
