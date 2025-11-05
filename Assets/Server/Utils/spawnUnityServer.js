import axios from 'axios'
import kill from 'tree-kill'
import { spawn } from 'child_process'
import { SERVER_GAME_URL } from '../Common/config.js'

export default class SpawnUnityServer {
    constructor(serverLocation, serverPort) {
        this.child = spawn('xvfb-run', [
            `--auto-servernum`,
            `--server-args='-screen 0 640x480x24:32'`,
            serverLocation,
            'SERVER_PORT', serverPort,
            `-batchmode`,
            `-nographics`
        ]);
        this.child.stdout.on('data', data => {
            console.log(`stdout: ${data}`);
        })
        this.child.stderr.on('data', data => {
            console.error(`stderr: ${data}`);
        })
        this.child.on('close', code => {
            console.log(`child process exited with code ${code}`);
        })
        this.serverPort = serverPort;
    }
    async sendHttpRequest(path, data = {}) {
        const response = await axios.post(SERVER_GAME_URL + path, data);
        return response.data;
    }
    async kill() {
        await new Promise((resolve, reject) =>
            kill(this.child.pid, err =>
                err ? reject(err) : resolve()
            )
        );
    }
}