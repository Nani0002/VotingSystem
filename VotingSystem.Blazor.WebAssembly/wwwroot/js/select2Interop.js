window.select2Interop = {
    init: function (id, initialValues, dotNetRef) {
        const el = document.getElementById(id);
        if (!el) return;

        $(el).select2({
            tags: true,
            tokenSeparators: [','],
            data: initialValues.map(val => ({ id: val, text: val })),
            theme: 'bootstrap-5'
        });

        $(el).val(initialValues).trigger('change');

        $(el).on('change', function () {
            const values = $(el).val();
            dotNetRef.invokeMethodAsync('OnSelect2Changed', values);
        });
    }
};