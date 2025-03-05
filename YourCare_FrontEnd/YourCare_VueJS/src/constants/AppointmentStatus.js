class AppointmentStatus {
    static PROCESSING = "Đang xử lí";
    static WAITING = "Đang đợi";
    static COMPLETED = "Đã khám xong";
    static CANCELLED = "Đã hủy";

    static values() {
        return Object.freeze([this.PROCESSING, this.WAITING, this.COMPLETED, this.CANCELLED]);
    }
}

Object.freeze(AppointmentStatus);

export default AppointmentStatus;

