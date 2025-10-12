import {
	ClassicEditor,
	Alignment,
	Autoformat,
	Bold,
	Code,
	Italic,
	Strikethrough,
	Subscript,
	Superscript,
	Underline,
	BlockQuote,
	CloudServices,
	CodeBlock,
	Essentials,
	FindAndReplace,
	Font,
	Heading,
	Highlight,
	HorizontalLine,
	GeneralHtmlSupport,
	AutoImage,
	Image,
	ImageCaption,
	ImageInsert,
	ImageResize,
	ImageStyle,
	ImageToolbar,
	ImageUpload,
	Base64UploadAdapter,
	PictureEditing,
	Indent,
	IndentBlock,
	TextPartLanguage,
	AutoLink,
	Link,
	LinkImage,
	List,
	ListProperties,
	TodoList,
	MediaEmbed,
	Mention,
	PageBreak,
	Paragraph,
	PasteFromOffice,
	RemoveFormat,
	SpecialCharacters,
	SpecialCharactersEssentials,
	Style,
	Table,
	TableCaption,
	TableCellProperties,
	TableColumnResize,
	TableProperties,
	TableToolbar,
	TextTransformation,
	WordCount,
	CKFinder,
	CKFinderUploadAdapter
} from 'ckeditor5';
import ResizableHeight from 'ResizableHeight';
import InsertInternalImage from 'InsertInternalImage';

let ckeditor_config = {
	plugins: [
		Alignment,
		Autoformat,
		AutoImage,
		AutoLink,
		BlockQuote,
		Bold,
		CloudServices,
		Code,
		CodeBlock,
		Essentials,
		FindAndReplace,
		Font,
		GeneralHtmlSupport,
		Heading,
		Highlight,
		HorizontalLine,
		Image,
		ImageCaption,
		ImageInsert,
		ImageResize,
		ImageStyle,
		ImageToolbar,
		ImageUpload,
		Base64UploadAdapter,
		Indent,
		IndentBlock,
		Italic,
		Link,
		LinkImage,
		List,
		ListProperties,
		MediaEmbed,
		Mention,
		PageBreak,
		Paragraph,
		PasteFromOffice,
		PictureEditing,
		RemoveFormat,
		SpecialCharacters,
		SpecialCharactersEssentials,
		Strikethrough,
		Style,
		Subscript,
		Superscript,
		Table,
		TableCaption,
		TableCellProperties,
		TableColumnResize,
		TableProperties,
		TableToolbar,
		TextPartLanguage,
		TextTransformation,
		TodoList,
		Underline,
		WordCount,
		CKFinder,
		CKFinderUploadAdapter,
		ResizableHeight,
		InsertInternalImage
	],
	toolbar: {
		shouldNotGroupWhenFull: true,
		items: [
			// --- Document-wide tools ----------------------------------------------------------------------
			'undo',
			'redo',
			'|',
			'findAndReplace',
			'selectAll',
			'|',

			// --- "Insertables" ----------------------------------------------------------------------------

			'link',
			//'insertInternalImage',
			'insertImage',
			//'ckfinder',
			'uploadImage',
			'insertTable',
			'blockQuote',
			'mediaEmbed',
			'codeBlock',
			'pageBreak',
			'horizontalLine',
			'specialCharacters',
			'-',

			// --- Block-level formatting -------------------------------------------------------------------
			'heading',
			'style',
			'|',

			// --- Basic styles, font and inline formatting -------------------------------------------------------
			'bold',
			'italic',
			'underline',
			'strikethrough',
			{
				label: 'Basic styles',
				icon: 'text',
				items: [
					'fontSize',
					'fontFamily',
					'fontColor',
					'fontBackgroundColor',
					'highlight',
					'superscript',
					'subscript',
					'code',
					'|',
					'textPartLanguage',
					'|',
				],
			},
			'removeFormat',
			'|',

			// --- Text alignment ---------------------------------------------------------------------------
			'alignment',
			'|',

			// --- Lists and indentation --------------------------------------------------------------------
			'bulletedList',
			'numberedList',
			'todoList',
			'|',
			'outdent',
			'indent',
		],
	},
	fontFamily: {
		options: [
			'default',
			'Arial, Helvetica, sans-serif',
			'Courier New, Courier, monospace',
			'Georgia, serif',
			'Lucida Sans Unicode, Lucida Grande, sans-serif',
			'Tahoma, Geneva, sans-serif',
			'Times New Roman, Times, serif',
			'Trebuchet MS, Helvetica, sans-serif',
			'Verdana, Geneva, sans-serif'
		],
		supportAllValues: true,
	},
	fontSize: {
		options: [
			'10px',
			'default',
			'14px',
			'16px',
			'18px',
			'20px',
			'24px'
		],
		supportAllValues: true,
	},
	heading: {
		options: [
			{ model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
			{
				model: 'heading1',
				view: 'h1',
				title: 'Heading 1',
				class: 'ck-heading_heading1',
			},
			{
				model: 'heading2',
				view: 'h2',
				title: 'Heading 2',
				class: 'ck-heading_heading2',
			},
			{
				model: 'heading3',
				view: 'h3',
				title: 'Heading 3',
				class: 'ck-heading_heading3',
			},
			{
				model: 'heading4',
				view: 'h4',
				title: 'Heading 4',
				class: 'ck-heading_heading4',
			},
			{
				model: 'heading5',
				view: 'h5',
				title: 'Heading 5',
				class: 'ck-heading_heading5',
			},
			{
				model: 'heading6',
				view: 'h6',
				title: 'Heading 6',
				class: 'ck-heading_heading6',
			},
		],
	},
	htmlSupport: {
		allow: [
			// Enables all HTML features.
			{
				name: /.*/,
				attributes: true,
				classes: true,
				styles: true,
			},
		],
		disallow: [
			{
				attributes: [
					{ key: /^on(.*)/i, value: true },
					{
						key: /.*/,
						value: /(\b)(on\S+)(\s*)=|javascript:|(<\s*)(\/*)script/i,
					},
					{ key: /.*/, value: /data:(?!image\/(png|jpeg|gif|webp))/i },
				],
			},
			{ name: 'script' },
		],
	},
	image: {
		resizeOptions: [
			{
				name: 'resizeImage:original',
				label: 'Default image width',
				value: null,
			},
			{
				name: 'resizeImage:custom',
				label: 'Custom',
				value: 'custom'
			},
			{
				name: 'resizeImage:50',
				label: '50% page width',
				value: '50',
			},
			{
				name: 'resizeImage:75',
				label: '75% page width',
				value: '75',
			},
		],
		toolbar: [
			'imageTextAlternative',
			'toggleImageCaption',
			'|',
			'imageStyle:inline',
			'imageStyle:wrapText',
			'imageStyle:breakText',
			'|',
			'resizeImage',
		],
		insert: {
			integrations: ['url'],
		},
	},
	list: {
		properties: {
			styles: true,
			startIndex: true,
			reversed: true,
		},
	},
	link: {
		decorators: {
			toggleDownloadable: {
				mode: 'manual',
				label: 'Downloadable',
				attributes: {
					download: 'file',
				},
			},
		},
		addTargetToExternalLinks: true,
		defaultProtocol: 'https://',
	},
	placeholder: 'Type or paste your content here!',
	style: {
		definitions: [
			{
				name: 'Title',
				element: 'h1',
				classes: ['document-title'],
			},
			{
				name: 'Subtitle',
				element: 'h2',
				classes: ['document-subtitle'],
			},
			{
				name: 'Callout',
				element: 'p',
				classes: ['callout'],
			},
			{
				name: 'Side quote',
				element: 'blockquote',
				classes: ['side-quote'],
			},
			{
				name: 'Needs clarification',
				element: 'span',
				classes: ['needs-clarification'],
			},
			{
				name: 'Wide spacing',
				element: 'span',
				classes: ['wide-spacing'],
			},
			{
				name: 'Small caps',
				element: 'span',
				classes: ['small-caps'],
			},
			{
				name: 'Code (dark)',
				element: 'pre',
				classes: ['stylish-code', 'stylish-code-dark'],
			},
			{
				name: 'Code (bright)',
				element: 'pre',
				classes: ['stylish-code', 'stylish-code-bright'],
			},
		],
	},
	table: {
		contentToolbar: [
			'tableColumn',
			'tableRow',
			'mergeTableCells',
			'tableProperties',
			'tableCellProperties',
			'toggleTableCaption',
		],
	},
	menuBar: {
		isVisible: true
	},
	ckfinder: {
		uploadUrl: ckfinder_upload_url + "&token=" + new UcHelpers().GetAccessToken(),
	}
}
let ckeditor_config_basic = {
	plugins: [
		Alignment,
		Autoformat,
		AutoImage,
		AutoLink,
		BlockQuote,
		Bold,
		CloudServices,
		Code,
		CodeBlock,
		Essentials,
		FindAndReplace,
		Font,
		GeneralHtmlSupport,
		Heading,
		Highlight,
		HorizontalLine,
		Image,
		ImageCaption,
		ImageInsert,
		ImageResize,
		ImageStyle,
		ImageToolbar,
		ImageUpload,
		Base64UploadAdapter,
		Indent,
		IndentBlock,
		Italic,
		Link,
		LinkImage,
		List,
		ListProperties,
		MediaEmbed,
		Mention,
		PageBreak,
		Paragraph,
		PasteFromOffice,
		PictureEditing,
		RemoveFormat,
		SpecialCharacters,
		SpecialCharactersEssentials,
		Strikethrough,
		Style,
		Subscript,
		Superscript,
		Table,
		TableCaption,
		TableCellProperties,
		TableColumnResize,
		TableProperties,
		TableToolbar,
		TextPartLanguage,
		TextTransformation,
		TodoList,
		Underline,
		WordCount,
		CKFinder,
		CKFinderUploadAdapter,
		ResizableHeight,
		InsertInternalImage
	],
	toolbar: {
		shouldNotGroupWhenFull: true,
		items: [
			// --- Block-level formatting -------------------------------------------------------------------
			'heading',
			'style',
			'|',

			// --- Basic styles, font and inline formatting -------------------------------------------------------
			'bold',
			'italic',
			'underline',
			'strikethrough',
			{
				label: 'Basic styles',
				icon: 'text',
				items: [
					'fontSize',
					'fontFamily',
					'fontColor',
					'fontBackgroundColor',
					'highlight',
					'superscript',
					'subscript',
					'code',
					'|',
					'textPartLanguage',
					'|',
				],
			},
			'removeFormat',
			'|',

			// --- Text alignment ---------------------------------------------------------------------------
			'alignment',
			'|',

			// --- Lists and indentation --------------------------------------------------------------------
			'bulletedList',
			'numberedList',
			'todoList',
			'|',
			'outdent',
			'indent',
		],
	},
	fontFamily: {
		options: [
			'default',
			'Arial, Helvetica, sans-serif',
			'Courier New, Courier, monospace',
			'Georgia, serif',
			'Lucida Sans Unicode, Lucida Grande, sans-serif',
			'Tahoma, Geneva, sans-serif',
			'Times New Roman, Times, serif',
			'Trebuchet MS, Helvetica, sans-serif',
			'Verdana, Geneva, sans-serif'
		],
		supportAllValues: true,
	},
	fontSize: {
		options: [
			'10px',
			'default',
			'14px',
			'16px',
			'18px',
			'20px',
			'24px'
		],
		supportAllValues: true,
	},
	heading: {
		options: [
			{ model: 'paragraph', title: 'Paragraph', class: 'ck-heading_paragraph' },
			{
				model: 'heading1',
				view: 'h1',
				title: 'Heading 1',
				class: 'ck-heading_heading1',
			},
			{
				model: 'heading2',
				view: 'h2',
				title: 'Heading 2',
				class: 'ck-heading_heading2',
			},
			{
				model: 'heading3',
				view: 'h3',
				title: 'Heading 3',
				class: 'ck-heading_heading3',
			},
			{
				model: 'heading4',
				view: 'h4',
				title: 'Heading 4',
				class: 'ck-heading_heading4',
			},
			{
				model: 'heading5',
				view: 'h5',
				title: 'Heading 5',
				class: 'ck-heading_heading5',
			},
			{
				model: 'heading6',
				view: 'h6',
				title: 'Heading 6',
				class: 'ck-heading_heading6',
			},
		],
	},
	htmlSupport: {
		allow: [
			// Enables all HTML features.
			{
				name: /.*/,
				attributes: true,
				classes: true,
				styles: true,
			},
		],
		disallow: [
			{
				attributes: [
					{ key: /^on(.*)/i, value: true },
					{
						key: /.*/,
						value: /(\b)(on\S+)(\s*)=|javascript:|(<\s*)(\/*)script/i,
					},
					{ key: /.*/, value: /data:(?!image\/(png|jpeg|gif|webp))/i },
				],
			},
			{ name: 'script' },
		],
	},
	image: {
		resizeOptions: [
			{
				name: 'resizeImage:original',
				label: 'Default image width',
				value: null,
			},
			{
				name: 'resizeImage:custom',
				label: 'Custom',
				value: 'custom'
			},
			{
				name: 'resizeImage:50',
				label: '50% page width',
				value: '50',
			},
			{
				name: 'resizeImage:75',
				label: '75% page width',
				value: '75',
			},
		],
		toolbar: [
			'imageTextAlternative',
			'toggleImageCaption',
			'|',
			'imageStyle:inline',
			'imageStyle:wrapText',
			'imageStyle:breakText',
			'|',
			'resizeImage',
		],
		insert: {
			integrations: ['url'],
		},
	},
	list: {
		properties: {
			styles: true,
			startIndex: true,
			reversed: true,
		},
	},
	link: {
		decorators: {
			toggleDownloadable: {
				mode: 'manual',
				label: 'Downloadable',
				attributes: {
					download: 'file',
				},
			},
		},
		addTargetToExternalLinks: true,
		defaultProtocol: 'https://',
	},
	placeholder: 'Type or paste your content here!',
	style: {
		definitions: [
			{
				name: 'Title',
				element: 'h1',
				classes: ['document-title'],
			},
			{
				name: 'Subtitle',
				element: 'h2',
				classes: ['document-subtitle'],
			},
			{
				name: 'Callout',
				element: 'p',
				classes: ['callout'],
			},
			{
				name: 'Side quote',
				element: 'blockquote',
				classes: ['side-quote'],
			},
			{
				name: 'Needs clarification',
				element: 'span',
				classes: ['needs-clarification'],
			},
			{
				name: 'Wide spacing',
				element: 'span',
				classes: ['wide-spacing'],
			},
			{
				name: 'Small caps',
				element: 'span',
				classes: ['small-caps'],
			},
			{
				name: 'Code (dark)',
				element: 'pre',
				classes: ['stylish-code', 'stylish-code-dark'],
			},
			{
				name: 'Code (bright)',
				element: 'pre',
				classes: ['stylish-code', 'stylish-code-bright'],
			},
		],
	},
	table: {
		contentToolbar: [
			'tableColumn',
			'tableRow',
			'mergeTableCells',
			'tableProperties',
			'tableCellProperties',
			'toggleTableCaption',
		],
	},
	menuBar: {
		isVisible: true
	},
	ckfinder: {
		uploadUrl: ckfinder_upload_url + "&token=" + new UcHelpers().GetAccessToken(),
	}
}
async function initEditor(selector, height) {
	let e = await ClassicEditor.create(document.querySelector(selector), ckeditor_config);
	document.querySelector(selector.replace("#", "#wrp_") + " .ck-editor__main").style.height = height;
	return e;
}
async function initEditorBasic(selector, height) {
	let e = await ClassicEditor.create(document.querySelector(selector), ckeditor_config_basic);
	document.querySelector(selector.replace("#", "#wrp_") + " .ck-editor__main").style.height = height;
	return e;
}
window.initEditor = initEditor;
window.initEditorBasic = initEditorBasic;