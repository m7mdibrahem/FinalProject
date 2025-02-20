$(document).ready(function () {
    $(document).on("click", ".a_categ", function (e) {
        var categId = $(this).data("id");
        $.ajax({
            url: "/Customer/Home/CategoryCity",
            type: "GET",
            data: { id: categId },
            success: function (response) {
                $(".trip_content").html(response);
                document.getElementById("target").scrollIntoView({ behavior: 'smooth' });
            },
            error: function () {
                alert("error city");
            }
        });
    });
});
