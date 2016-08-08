$(function () {
    $.FroalaEditor.DefineIcon('insertbar', { NAME: 'bar-chart' });
    $.FroalaEditor.RegisterCommand('insertbar', {
        title: 'Bar Chart',
        focus: true,
        undo: true,
        refreshAfterCallback: true,
        callback: function () {
            this.html.insert('<table style="width: 100%;" class="highchart" data-graph-container-before="1" data-graph-type="column">' +
                '<caption>Table example</caption>' +
                '<thead><tr><th>Values</th><th>Counts' +
                '</th></tr></thead><tbody><tr><td style="width: 50%;">Val1</td><td style="width: 50%;">1</td></tr>' +
                '<tr><td style="width: 50%;">Val2<br></td><td style="width: 50%;">2</td></tr><tr><td style="width: 50%;">Val3' +
                '<br></td><td style="width: 50%;">3</td></tr><tr><td style="width: 50%;">Val4<br></td><td style="width: 50%;">4</td>' +
                '</tr><tr><td style="width: 50%;">Val5<br></td><td style="width: 50%;">5</td></tr></tbody></table>');
        }
    });
    $.FroalaEditor.DefineIcon('insertpie', { NAME: 'pie-chart' });
    $.FroalaEditor.RegisterCommand('insertpie', {
        title: 'Pie Chart',
        focus: true,
        undo: true,
        refreshAfterCallback: true,
        callback: function () {
            this.html.insert('<table style="width: 100%;" class="highchart" data-graph-container-before="1" data-graph-type="pie">' +
                '<caption>Table example</caption>' +
                '<thead><tr><th>Values</th><th>Counts' +
                '</th></tr></thead><tbody><tr><td style="width: 50%;">Val1</td><td style="width: 50%;">1</td></tr>' +
                '<tr><td style="width: 50%;">Val2<br></td><td style="width: 50%;">2</td></tr><tr><td style="width: 50%;">Val3' +
                '<br></td><td style="width: 50%;">3</td></tr><tr><td style="width: 50%;">Val4<br></td><td style="width: 50%;">4</td>' +
                '</tr><tr><td style="width: 50%;">Val5<br></td><td style="width: 50%;">5</td></tr></tbody></table>');
        }
    });
    $.FroalaEditor.DefineIcon('insertlin', { NAME: 'line-chart' });
    $.FroalaEditor.RegisterCommand('insertlin', {
        title: 'Line Chart',
        focus: true,
        undo: true,
        refreshAfterCallback: true,
        callback: function () {
            this.html.insert('<table style="width: 100%;" class="highchart" data-graph-container-before="1" data-graph-type="line">' +
                '<caption>Table example</caption>' +
                '<thead><tr><th>Values</th><th>Counts' +
                '</th></tr></thead><tbody><tr><td style="width: 50%;">Val1</td><td style="width: 50%;">1</td></tr>' +
                '<tr><td style="width: 50%;">Val2<br></td><td style="width: 50%;">2</td></tr><tr><td style="width: 50%;">Val3' +
                '<br></td><td style="width: 50%;">3</td></tr><tr><td style="width: 50%;">Val4<br></td><td style="width: 50%;">4</td>' +
                '</tr><tr><td style="width: 50%;">Val5<br></td><td style="width: 50%;">5</td></tr></tbody></table>');
        }
    });
    $('#title').froalaEditor({
        toolbarInline: true,
        dragInline: false,
        toolbarButtons: ['bold', 'italic', 'underline', 'strikeThrough', 'fontFamily', 'fontSize', 'color', 'align', '|', 'undo', 'redo'],
        toolbarButtonsSM: ['bold', 'italic', 'underline', 'strikeThrough', 'fontFamily', 'fontSize', '-', 'color', 'align', '|', 'undo', 'redo'],
        toolbarButtonsMD: ['bold', 'italic', 'underline', 'strikeThrough', 'fontFamily', 'fontSize', 'color', 'align', '|', 'undo', 'redo'],
        toolbarButtonsXS: ['bold', 'italic', 'underline', 'strikeThrough', 'fontFamily', '-', 'fontSize', 'color', 'align', '|', 'undo', 'redo'],
        theme: "@ViewBag.Style",
        heightMin: 100,
        charCounterMax: 50,
        toolbarVisibleWithoutSelection: true
    });
    $('#edit1').froalaEditor({
        toolbarInline: true,
        dragInline: false,
        toolbarButtons: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'fontFamily', 'fontSize', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '|', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsSM: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', '-', 'fontFamily', 'fontSize', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', '-', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', 'insertLink', 'insertImage', '-', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '-', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsMD: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'fontFamily', 'fontSize', '-', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '-', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsXS: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', '-', 'subscript', 'superscript', 'fontFamily', 'fontSize', 'color', '-', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', '-', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', '-', 'quote', 'insertHR', '|', 'undo', 'redo', '|', 'clearFormatting', '-', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        pluginsEnabled: null,
        theme: "@ViewBag.Style",
        heightMin: 200,
        toolbarVisibleWithoutSelection: true
    });
    $('#edit2').froalaEditor({
        toolbarInline: true,
        dragInline: false,
        toolbarButtons: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'fontFamily', 'fontSize', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '|', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsSM: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', '-', 'fontFamily', 'fontSize', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', '-', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', 'insertLink', 'insertImage', '-', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '-', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsMD: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'fontFamily', 'fontSize', '-', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '-', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsXS: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', '-', 'subscript', 'superscript', 'fontFamily', 'fontSize', 'color', '-', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', '-', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', '-', 'quote', 'insertHR', '|', 'undo', 'redo', '|', 'clearFormatting', '-', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        pluginsEnabled: null,
        theme: "@ViewBag.Style",
        heightMin: 200,
        toolbarVisibleWithoutSelection: true
    });
    $('#edit3').froalaEditor({
        toolbarInline: true,
        dragInline: false,
        toolbarButtons: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'fontFamily', 'fontSize', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '|', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsSM: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', '-', 'fontFamily', 'fontSize', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', '-', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', 'insertLink', 'insertImage', '-', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '-', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsMD: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', 'subscript', 'superscript', 'fontFamily', 'fontSize', '-', 'color', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', 'quote', 'insertHR', '|', 'undo', 'redo', '-', 'clearFormatting', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        toolbarButtonsXS: ['fullscreen', 'bold', 'italic', 'underline', 'strikeThrough', '-', 'subscript', 'superscript', 'fontFamily', 'fontSize', 'color', '-', 'emoticons', 'inlineStyle', 'paragraphStyle', 'paragraphFormat', 'align', '-', 'formatOL', 'formatUL', 'outdent', 'indent', '-', 'insertLink', 'insertImage', 'insertVideo', 'insertFile', 'insertTable', '-', 'quote', 'insertHR', '|', 'undo', 'redo', '|', 'clearFormatting', '-', 'selectAll', 'html', 'insertlin', 'insertpie', 'insertbar'],
        pluginsEnabled: null,
        theme: "@ViewBag.Style",
        heightMin: 200,
        toolbarVisibleWithoutSelection: true
    });
});