sofia.config = {

	// Change this to clear all user settings
	settingsPrefix: '20231101',

	// Enables the use of online sources (Google Maps, FCBH, Jesus Film, etc.)
	enableOnlineSources: false,

	// first load
	windows: [ // lpnrmw
		{type: 'bible', data: {textid: 'nr', fragmentid: 'JN3_16'}},
		{type: 'bible', data: {textid: 'cei1974', fragmentid: 'JN3_16'}},
		{type: 'commentary', data: {textid: 'commabbrmh', fragmentid: 'JN3_16'}}
	],

	// URL to content
	// (1) Leave blank to use local content folder.
	// (2) Enter URL (http://www.biblesite.com/) for CORS enabled sites
	// (3) Enter URL (http://www.biblesite.com/) and update
	baseContentUrl: '',

	// (1) Leave blank for local files or for CORS enabled CDN
	// (2) Enter path of script that will convert all files to JSONP (e.g., api.php)
	baseContentApiPath: '',

	// if your are creating a CDN for your content (see above) and want to send a key
	baseContentApiKey: '',

	// file name of texts lists
	textsIndexPath: 'texts.json',

	// URL to about page
	aboutPagePath: 'about.html',

	// (1) Leave blank for JSON search
	// (2) Enter path of script that will return JSON data
	serverSearchPath: '',

	// texts shown before the "MORE" button ("eng-NASB1995", "eng-kjv", "eng_net")
	topTexts: [],

	// new window
	newBibleWindowVersion: 'nr', // lpnrmw

	// new bible verse
	newWindowFragmentid: 'JN3_16',

	// new commentary window
//	newCommentaryWindowTextId: 'comm_eng_wesley',
	newCommentaryWindowTextId: 'commabbrmh', // lpnrmw

	// language for top
//	pinnedLanguage: 'English',
	pinnedLanguage: 'Italian', // lpnrmw

	// language(s) for top
	pinnedLanguages: ['Italian', 'English', 'Greek', 'Hebrew'], // lpnrmw

	// Override the browser and user's choice for UI language
	defaultLanguage: '',

	// URL to custom CSS
	customCssUrl: '',

	// Faith Comes by Hearing
	fcbhKey: '',

	// any texts you want to ignore from FCBH
	fcbhTextExclusions: [''],

	// true: live parse all versions
	// false: loads texts_fcbh.json
	fcbhLoadVersions: false,

	// jesus film media
	jfmKey: ''
};

sofia.customConfigs = {
	"dbs": {
		customCssUrl: 'dbs.css'
	}
};
