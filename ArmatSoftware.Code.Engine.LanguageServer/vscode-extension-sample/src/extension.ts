import * as path from 'path';
import { workspace, ExtensionContext } from 'vscode';
import {
    LanguageClient,
    LanguageClientOptions,
    ServerOptions,
    TransportKind
} from 'vscode-languageclient/node';

let client: LanguageClient;

export function activate(context: ExtensionContext) {
    // Get the language server executable path from configuration
    const config = workspace.getConfiguration('codeEngineLanguageServer');
    const serverPath = config.get<string>('serverPath') || 
        path.join(context.extensionPath, 'server', 'ArmatSoftware.Code.Engine.LanguageServer.exe');

    // Server options for the language server
    const serverOptions: ServerOptions = {
        run: { 
            command: serverPath, 
            transport: TransportKind.stdio 
        },
        debug: { 
            command: serverPath, 
            transport: TransportKind.stdio 
        }
    };

    // Client options
    const clientOptions: LanguageClientOptions = {
        // Register the server for Code Engine files
        documentSelector: [
            { scheme: 'file', language: 'codeengine' },
            { scheme: 'file', language: 'csharp', pattern: '**/*.ce' }
        ],
        synchronize: {
            // Notify the server about file changes to Code Engine files
            fileEvents: workspace.createFileSystemWatcher('**/*.ce')
        }
    };

    // Create the language client
    client = new LanguageClient(
        'codeEngineLanguageServer',
        'Code Engine Language Server',
        serverOptions,
        clientOptions
    );

    // Start the client (and server)
    client.start();
}

export function deactivate(): Thenable<void> | undefined {
    if (!client) {
        return undefined;
    }
    return client.stop();
}