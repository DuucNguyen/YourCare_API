import { compile } from "vue";

const userRoutes = [
    {
        path: "/user",
        component: () => import("@/shared/_userLayout.vue"),
        children: [
            {
                path: "/doctors/:specicaltyID?",
                name: "User_Doctor_View",
                meta: {title: "DoctorList"},
                component: () => import("@/views/Public/Doctor/index.vue"),
            },
            {
                path: "/doctors/:doctorID",
                name: "User_Doctor_Detail",
                meta: {title: "DoctorDetail"},
                component: () => import("@/views/Public/Doctor/detail.vue"),
            },
            {
                path: "/make-appointment",
                name: "User_Doctor_MakeAppointment",
                meta: {title: "MakeAppointment"},
                component: () => import("@/views/Public/Doctor/makeAppointment.vue"),
            },
            {
                path: "/appointments/history",
                name: "User_Appointment_View",
                meta: {title: "History"},
                component: () => import ("@/views/Public/Appointment/index.vue")
            },
            {
                path: "/patient-profile",
                name: "User_PatientProfile_View",
                meta: {title: "PatientProfile"},
                component: () => import ("@/views/Public/PatientProfile/index.vue")
            },
            {
                path: "/my-profile",
                name: "User_Profile_View",
                meta: {title: "MyProfile"},
                component: () => import ("@/views/Public/Profile/profile.vue")
            }
        ],
    },
];

export default userRoutes;
