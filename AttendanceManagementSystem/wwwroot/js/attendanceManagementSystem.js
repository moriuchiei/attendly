$(function () {

    const $workStatus = $('#WorkStatus');
    const $submitButton = $('.submit-btn');
    // 共通------------------------------------------------------------------------
    // ローディング処理
    $('.submitBtn').on('click', function () {
        // ダブルサブミット抑止処理
        if ($(this).data('clicked')) {
            return false;
        }
        $(this).data('clicked', true);

        // ローディング表示
        $('#loadingOverlay').css('display', 'flex');
    });

    // 日次一覧画面---------------------------------------------------------------
    // 管理者専用、承認ボタン押下時
    $(".ApprovalBtn").on("click", function () {
        // 承認対象日付を取得して送信
        $("#input-display-date").val($(this).data("date"));
        $("#approve-form").trigger("submit");
    });

    // 日次登録画面---------------------------------------------------------------
    $workStatus.on('change', function () {
        var selectedValue = $(this).val();
        var startTime = $('input[name="StartTime"]');
        var endTime = $('input[name="EndTime"]');

        if (selectedValue === "1") {
            // 出勤区分が出勤の場合
            startTime.prop('disabled', false).val("09:00").removeClass('input-disabled-bg');
            endTime.prop('disabled', false).val("18:00").removeClass('input-disabled-bg');
        } else if (selectedValue === "2" || selectedValue === "3") {
            // 出勤区分が有給休暇・欠勤の場合
            startTime.prop('disabled', true).val("00:00").addClass('input-disabled-bg');
            endTime.prop('disabled', true).val("00:00").addClass('input-disabled-bg');
        } else {
            // 出勤区分が未選択の場合
            startTime.prop('disabled', true).val("00:00").addClass('input-disabled-bg');
            endTime.prop('disabled', true).val("00:00").addClass('input-disabled-bg');
        }

        // 勤務区分未選択の場合は登録ボタン非活性化
        if ($workStatus.val() === "" || $workStatus.val() === "0") {
            $submitButton.prop('disabled', true);
            $submitButton.addClass('disabled-btn');
        } else {
            $submitButton.prop('disabled', false);
            $submitButton.removeClass('disabled-btn');
        }
    });

    // 初期状態でも反映
    if ($('#WorkStatus').val() != "1") {
        $('#WorkStatus').trigger('change');
    }

});