<template>
    <div class="main-context">
        <div class="context">
            <div class="context-text">
                <h1>
                    <a href="/">YourCare</a>
                </h1>
                <h3>
                    Chúng tôi xây dựng nền tảng y tế thông minh với sứ mệnh rút ngắn khoảng cách
                    giữa Bệnh nhân, Bác sĩ và các Cơ sở y tế
                </h3>
            </div>

            <div class="context-product">
                <section id="product">
                    <div class="product-container">
                        <div class="product">
                            <img src="./assets//img/svg/dartboard.svg" />
                            <h2>Tìm chính xác</h2>
                            <p>
                                Thông tin về phòng khám và bác sĩ luôn được cập nhật chính xác bởi
                                đội ngũ chuyên nghiệp của YourCare. Bạn hoàn toàn có thể yên tâm tự
                                nghiên cứu, đánh giá và đưa ra sự lựa chọn bác sĩ phù hợp với tình
                                hình bệnh án của mình mà không cần phải mất nhiều thời gian.
                            </p>
                        </div>
                        <div class="product">
                            <img src="./assets//img/svg/medical-appointment.svg" />
                            <h2>Đặt lịch khám dễ dàng</h2>
                            <p>
                                Chỉ với vài thao tác đơn giản, bạn có thể đặt lịch khám Online một
                                cách nhanh chóng mà không cần phải gọi điện hoặc lo lắng rằng không
                                có thông tin liên lạc của Bác sĩ.
                            </p>
                        </div>
                        <div class="product">
                            <img src="./assets//img/svg/star.svg" />
                            <h2>Tái khám thuận tiện</h2>
                            <p>
                                YourCare sẽ giúp hỗ trợ nhắc nhở khi đến thời gian tái khám. Bạn
                                hoàn toàn có thể yên tâm tin tưởng chọn lựa YourCare trở thành cầu
                                nối để việc khám bệnh trở nên thuận tiện và hiệu quả hơn bao giờ
                                hết.
                            </p>
                        </div>
                        <div class="product">
                            <img src="./assets//img/svg/news.svg" />
                            <h2>Thông tin y tế chính thống</h2>
                            <p>
                                Trang thông tin sức khỏe chính thống và hữu ích hàng đầu Việt Nam.
                                Toàn bộ nội dung được biên soạn bởi đội ngũ Bác sĩ, Dược sĩ nhằm mục
                                đích cung cấp kiến thức y khoa chính thống cho cộng đồng.
                            </p>
                        </div>
                    </div>
                </section>
            </div>
        </div>
    </div>

    <div class="area">
        <ul class="circles">
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
            <li></li>
        </ul>
    </div>
</template>
<style scoped>
    @import url("https://fonts.googleapis.com/css?family=Exo:400,700");

    * {
        margin: 0px;
        padding: 0px;
    }

    body {
        font-family: "Exo", sans-serif;
    }

    .main-context {
        width: 100%;
        position: absolute;
        top: 10vh;
        z-index: 1;
    }

    .context {
        margin: auto;
        display: flex;
        flex-direction: column;
        justify-content: center;
        align-items: center;
        padding: 10px;
        width: 90%;
        border-radius: 5px;
    }

    .context-text h1 {
        text-align: center;
        font-size: 50px;
        font-weight: 600;
        margin-bottom: 30px;
        color: white;
    }

    .context-text a {
        text-decoration: none;
    }

    .context-product {
        border-radius: 5px;
    }

    .product-container {
        display: flex;
        flex-wrap: wrap;
        justify-content: space-around;
    }

    .product {
        width: 40%;
        background: #fff;
        padding: 30px;
        margin: 20px;
        border-radius: 5px !important;
        transition: width 0.2s, font-size 0.2s;
    }

    .product img {
        width: 40px;
        height: 40px;
        margin-bottom: 10px;
    }

    .product:hover {
        width: 45%;
        font-size: 18px;
    }

    .area {
        background: linear-gradient(to bottom, #03fc5e, #3903fc);
        background: -webkit-linear-gradient(to left, #8f94fb, #4e54c8);
        width: 100%;
        height: 100vh;
    }

    .circles {
        position: absolute;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        overflow: hidden;
    }

    .circles li {
        position: absolute;
        display: block;
        list-style: none;
        width: 20px;
        height: 20px;
        background: rgba(255, 255, 255, 0.2);
        animation: animate 25s linear infinite;
        bottom: -150px;
    }

    .circles li:nth-child(1) {
        left: 25%;
        width: 80px;
        height: 80px;
        animation-delay: 0s;
    }

    .circles li:nth-child(2) {
        left: 10%;
        width: 20px;
        height: 20px;
        animation-delay: 2s;
        animation-duration: 12s;
    }

    .circles li:nth-child(3) {
        left: 70%;
        width: 20px;
        height: 20px;
        animation-delay: 4s;
    }

    .circles li:nth-child(4) {
        left: 40%;
        width: 60px;
        height: 60px;
        animation-delay: 0s;
        animation-duration: 18s;
    }

    .circles li:nth-child(5) {
        left: 65%;
        width: 20px;
        height: 20px;
        animation-delay: 0s;
    }

    .circles li:nth-child(6) {
        left: 75%;
        width: 110px;
        height: 110px;
        animation-delay: 3s;
    }

    .circles li:nth-child(7) {
        left: 35%;
        width: 150px;
        height: 150px;
        animation-delay: 7s;
    }

    .circles li:nth-child(8) {
        left: 50%;
        width: 25px;
        height: 25px;
        animation-delay: 15s;
        animation-duration: 45s;
    }

    .circles li:nth-child(9) {
        left: 20%;
        width: 15px;
        height: 15px;
        animation-delay: 2s;
        animation-duration: 35s;
    }

    .circles li:nth-child(10) {
        left: 85%;
        width: 150px;
        height: 150px;
        animation-delay: 0s;
        animation-duration: 11s;
    }

    .circles li:nth-child(11) {
        left: 55%;
        width: 35px;
        height: 45px;
        animation-delay: 13s;
        animation-duration: 35s;
    }

    .circles li:nth-child(12) {
        left: 30%;
        width: 25px;
        height: 25px;
        animation-delay: 5s;
        animation-duration: 15s;
    }

    .circles li:nth-child(13) {
        left: 75%;
        width: 50px;
        height: 50px;
        animation-delay: 0s;
        animation-duration: 21s;
    }

    @keyframes animate {
        0% {
            transform: translateY(0) rotate(0deg);
            opacity: 1;
            border-radius: 0;
        }

        100% {
            transform: translateY(-1000px) rotate(720deg);
            opacity: 0;
            border-radius: 50%;
        }
    }
</style>
