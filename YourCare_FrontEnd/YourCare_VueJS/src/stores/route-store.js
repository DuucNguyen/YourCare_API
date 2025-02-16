import { defineStore } from "pinia";
export const useRouteStore = defineStore({
    id: "route",
    state: () => ({
        data: null,
    }),
    actions: {
        setData(data) {
            this.data = data;
        },
    },
    persist: true  // 🔥 Enable persistence 
});
