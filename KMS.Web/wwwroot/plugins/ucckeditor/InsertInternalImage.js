import Plugin from 'Ckeditor5CorePlugin';

import { ButtonView, icons } from 'ckeditor5';


export default class InsertInternalImage extends Plugin {
    init() {
        const editor = this.editor;
		editor.ui.componentFactory.add('insertInternalImage', locale => {
			// The button will be an instance of ButtonView.
			const button = new ButtonView(locale);

			button.set({
				icon: icons.image,
				tooltip: true,
				class: "insertInternalImage",
				label: "Insert Internal Image"
			});

			button.on('execute', () => {
				editor.model.change(writer => {
					const viewFragment = editor.data.processor.toView(`<img src="https://www.google.com/images/branding/googlelogo/1x/googlelogo_color_272x92dp.png" alt="Italian Trulli">`);
					const modelFragment = editor.data.toModel(viewFragment);
					editor.model.insertContent(modelFragment, editor.model.document.selection);
				});
			});

            return button;
        });
    }
}
