import mongoose from 'mongoose'
import bluebird from 'bluebird'
import { MONGODB_DATABASE_NAME } from '../Common/config.js'

mongoose.Promise = bluebird
mongoose.set('debug', IS_DEVELOPMENT)
mongoose
    .connect(
        `mongodb://127.0.0.1/${MONGODB_DATABASE_NAME}?replicaSet=rs0`,
        {
            'bufferCommands': true,
            'autoIndex': true,
            // 'useCreateIndex': true,
            // 'useFindAndModify': false,
            // 'poolSize': 5,
            'serverSelectionTimeoutMS': 5000,
            'heartbeatFrequencyMS': 45000,
            // 'reconnectTries': 30,
            // 'reconnectInterval': 1000,
            'connectTimeoutMS': 30000,
            'socketTimeoutMS': 30000,
            // 'family': 0,
            'autoCreate': false
        }
    )
    .catch(error => console.error('MongoDB connection is errored', error))

mongoose.connection.on('connected', () => console.log('MongoDB connected'));
mongoose.connection.on('open', () => console.log('MongoDB open'));
mongoose.connection.on('disconnected', () => console.log('MongoDB disconnected'));
mongoose.connection.on('reconnected', () => console.log('MongoDB reconnected'));
mongoose.connection.on('disconnecting', () => console.log('MongoDB disconnecting'));
mongoose.connection.on('close', () => console.log('MongoDB close'));

export default mongoose