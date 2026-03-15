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
		const buildPath = vscode.Uri.joinPath(this._extensionUri, 'media', 'unity-build');
		const loaderUri = webview.asWebviewUri(vscode.Uri.joinPath(buildPath, 'Build.loader.js'));
		const frameworkUri = webview.asWebviewUri(vscode.Uri.joinPath(buildPath, 'Build.framework.js'));
		const dataUri = webview.asWebviewUri(vscode.Uri.joinPath(buildPath, 'Build.data'));
		const wasmUri = webview.asWebviewUri(vscode.Uri.joinPath(buildPath, 'Build.wasm'));

		return `<!DOCTYPE html>
            <html lang="en">
			<head>
				<meta charset="UTF-8">
				<!--
					Use a content security policy to only allow loading styles from our extension directory,
					and only allow scripts that have a specific nonce.
					(See the 'webview-sample' extension sample for img-src content security policy examples)
				-->
				<meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src ${webview.cspSource}; script-src ${webview.cspSource} 'nonce-${nonce}' 'unsafe-eval'; connect-src ${webview.cspSource};">
				<meta name="viewport" content="width=device-width, initial-scale=1.0">
				<title>VS Buddy</title>
				<style>
                    html, body {
						padding: 0;
						margin: 0;
						width: 100%;
						height: 100%;
						overflow: hidden;
						display: flex;
					}
					#unity-container {
						width: 100% !important;
						height: 100% !important;
						position: absolute;
					}
					#unity-canvas {
						width: 100% !important;
						height: 100% !important;
						display: block;
					}
                </style>
			</head>
            <body>
                <div id="unity-container" class="unity-desktop">
                    <canvas id="unity-canvas" tabindex="-1"></canvas>
                </div>
                <script nonce="${nonce}" src="${loaderUri}"></script>
                <script nonce="${nonce}">
                    const vscode = acquireVsCodeApi();
                    createUnityInstance(document.querySelector("#unity-canvas"), {
                        dataUrl: "${dataUri}",
                        frameworkUrl: "${frameworkUri}",
                        codeUrl: "${wasmUri}",
                        streamingAssetsUrl: "StreamingAssets",
                        companyName: "DefaultCompany",
                        productName: "VS Buddy",
                        productVersion: "0.1",
						devicePixelRatio: 1,
                    });
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
