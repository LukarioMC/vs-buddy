import * as vscode from 'vscode';

// This method is called when your extension is activated
// Your extension is activated the very first time the command is executed
export function activate(context: vscode.ExtensionContext) {
	const provider = new GameViewProvider(context.extensionUri);

	context.subscriptions.push(
		vscode.window.registerWebviewViewProvider(GameViewProvider.viewType, provider)
	);
}

class GameViewProvider implements vscode.WebviewViewProvider {
	public static readonly viewType = 'vs-buddy.buddyView';
	private _view?: vscode.WebviewView;

	constructor(private readonly _extensionUri: vscode.Uri) {
		this._extensionUri = _extensionUri;
	}

	public resolveWebviewView(
		webviewView: vscode.WebviewView,
		_context: vscode.WebviewViewResolveContext,
		_token: vscode.CancellationToken,
	) {
		this._view = webviewView;
		webviewView.webview.options = {
			enableScripts: true,
			localResourceRoots: [this._extensionUri]
		};
		webviewView.webview.html = this._getHtmlForWebview(webviewView.webview);
	}

	public getWebview(): vscode.Webview {
		if (this._view === undefined) {
			throw new Error('Panel is not active');
		} else {
			return this._view.webview;
		}
	}

	private _getHtmlForWebview(webview: vscode.Webview) {
		const nonce = getNonce();
		// This is where you will eventually link your Unity index.html
		return `<!DOCTYPE html>
            <html lang="en">
			<head>
				<meta charset="UTF-8">
				<!--
					Use a content security policy to only allow loading styles from our extension directory,
					and only allow scripts that have a specific nonce.
					(See the 'webview-sample' extension sample for img-src content security policy examples)
				-->
				<meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src ${webview.cspSource}; script-src 'nonce-${nonce}';">
				<meta name="viewport" content="width=device-width, initial-scale=1.0">
				<title>VS Buddy</title>
			</head>
            <body>
                <h1>Game Loading...</h1>
                <script nonce="${nonce}">
                    const vscode = acquireVsCodeApi();
                    // Unity loader logic goes here
                </script>
            </body>
            </html>`;
	}
}

function getNonce() {
	let text = '';
	const possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
	for (let i = 0; i < 32; i++) {
		text += possible.charAt(Math.floor(Math.random() * possible.length));
	}
	return text;
}

