﻿export function loadTinyMCE(textareaSelector) {
    if (window.tinyMCE !== undefined) {
        window.tinyMCE.init({
            selector: textareaSelector,
            themes: 'silver',
            skin: 'tinymce-5',
            // height: ,
            relative_urls: false, // avoid image upload fuck up
            browser_spellcheck: true,
            branding: false,
            block_formats: 'Paragraph=p; Header 2=h2; Header 3=h3; Header 4=h4; Preformatted=pre',
            fontsize_formats: '8pt 10pt 12pt 14pt 18pt 24pt 36pt',
            plugins: 'advlist autolink autosave link image lists charmap preview anchor pagebreak searchreplace wordcount visualblocks visualchars code fullscreen insertdatetime media nonbreaking save table directionality template codesample emoticons',
            toolbar: 'formatselect | fontsizeselect | bold italic underline strikethrough | forecolor backcolor | removeformat | emoticons link hr image table codesample media | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | code | fullscreen',
            save_onsavecallback: function () {
                $('#btn-save').trigger('click');
            },
            paste_data_images: true,
            images_upload_url: '/image',
            images_upload_credentials: true,
            extended_valid_elements: 'img[class|src|border=0|alt|title|hspace|vspace|width|height|align|onmouseover|onmouseout|name|loading=lazy]',
            body_class: 'post-content',
            content_css: '/css/dist/tinymce-bs-bundle.min.css',
            codesample_languages: [
                { text: 'Bash', value: 'bash' },
                { text: 'C#', value: 'csharp' },
                { text: 'C', value: 'c' },
                { text: 'C++', value: 'cpp' },
                { text: 'CSS', value: 'css' },
                { text: 'Dart', value: 'dart' },
                { text: 'Dockerfile', value: 'dockerfile' },
                { text: 'F#', value: 'fsharp' },
                { text: 'Go', value: 'go' },
                { text: 'HTML/XML', value: 'markup' },
                { text: 'JavaScript', value: 'javascript' },
                { text: 'Json', value: 'json' },
                { text: 'Less', value: 'less' },
                { text: 'Lua', value: 'lua' },
                { text: 'Markdown', value: 'markdown' },
                { text: 'PowerShell', value: 'powershell' },
                { text: 'Plain Text', value: 'plaintext' },
                { text: 'Python', value: 'python' },
                { text: 'PHP', value: 'php' },
                { text: 'Ruby', value: 'ruby' },
                { text: 'Rust', value: 'rust' },
                { text: 'SCSS', value: 'scss' },
                { text: 'SQL', value: 'sql' },
                { text: 'Swift', value: 'swift' },
                { text: 'TypeScript', value: 'typescript' },
                { text: 'Visual Basic', value: 'vb' },
                { text: 'YAML', value: 'yaml' }
            ],
            setup: function (editor) {
                editor.on('NodeChange', function (e) {
                    if (e.element.tagName === 'IMG') {
                        e.element.setAttribute('loading', 'lazy');
                    }
                });
            }
        });
    }
}

export function loadMdEditor(textareaSelector) {
    if (window.SimpleMDE) {
        simplemde = new SimpleMDE({
            element: $(textareaSelector)[0],
            spellChecker: false,
            status: false
        });

        inlineAttachment.editors.codemirror4.attach(simplemde.codemirror, {
            uploadUrl: '/image',
            urlText: '![file]({filename})',
            onFileUploadResponse: function (xhr) {
                var result = JSON.parse(xhr.responseText),
                    filename = result[this.settings.jsonFieldName];

                if (result && filename) {
                    var newValue;
                    if (typeof this.settings.urlText === 'function') {
                        newValue = this.settings.urlText.call(this, filename, result);
                    } else {
                        newValue = this.settings.urlText.replace(this.filenameTag, filename);
                    }
                    var text = this.editor.getValue().replace(this.lastValue, newValue);
                    this.editor.setValue(text);
                    this.settings.onFileUploaded.call(this, filename);
                }
                return false;
            }
        });
    }
}