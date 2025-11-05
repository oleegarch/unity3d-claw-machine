import { createServer } from 'http'
import app from './app.js'

const httpServer = createServer(app)

export default httpServer