import crypto from 'crypto'
import mongoose from '../mongoose.js'
import { getLoadsCollectionSchema, loadNames } from '../../Common/loads.js'
import {
    FREE_TEST_TRIES_INTERVAL,
    LIVE_GAME_INTERVAL_GENERATING_NEW_LOADS,
    LIVE_TRIES_FOR_NEW_USERS,
    TEST_GAME_INTERVAL_GENERATING_NEW_LOADS,
    TEST_TRIES_FOR_NEW_USERS,
    generateGameSid
} from '../../Common/game.js'

const { Schema } = mongoose;

const SceneSchema = new Schema({
    'loads': [
        {
            'name': { type: String, enum: loadNames, required: true },
            'index': { type: Number, required: true },
            'position': {
                'x': { type: Number, required: true },
                'y': { type: Number, required: true },
                'z': { type: Number, required: true }
            },
            'rotation': {
                'x': { type: Number, required: true },
                'y': { type: Number, required: true },
                'z': { type: Number, required: true }
            },
            'scale': {
                'x': { type: Number, required: true },
                'y': { type: Number, required: true },
                'z': { type: Number, required: true }
            }
        }
    ]
});

export const options = {
	discriminatorKey: 'authPlatform',
	timestamps: {
		'createdAt': 'registeredAt',
		'updatedAt': 'updatedAt'
	}
};
export const schema = {
    'displayName': { type: String, required: true },
    'gender': { type: String, enum: ['male', 'female'], default: 'male' },
    'locale': { type: String, default: 'ru' },
    'timezone': { type: Number, default: 3 },
    'authPlatform': { type: String, enum: ['GooglePlayGames', 'AppStore', 'VKontakte'], required: true },
    'authId': { type: String, index: true, required: true },
    'settings': {
        'sounds': { type: Boolean, default: true },
        'music': { type: Boolean, default: true },
        'vibration': { type: Boolean, default: true },
        'hiddenInLeaders': { type: Boolean, default: false }
    },
    'training': {
        'mainScene': { type: Boolean, default: false },
        'gameScene': { type: Boolean, default: false }
    },
    'coins': { type: Number, default: 0 },
    'score': { type: Number, default: 0 },
    'loadsCount': { type: Number, default: 0 },
    'loadsCollection': getLoadsCollectionSchema({ type: Number, default: 0 }),
    'game': {
        'testTries': { type: Number, default: TEST_TRIES_FOR_NEW_USERS },
        'liveTries': { type: Number, default: LIVE_TRIES_FOR_NEW_USERS },
        'nextFreeTestTriesAt': { type: Date, default: () => Date.now() + FREE_TEST_TRIES_INTERVAL },
        'state': {
            'type': { type: String, enum: ['test', 'live'] },
            'started': { type: Boolean, default: false }
        },
        'test': {
            'seed': { type: Number, default: generateGameSid },
            'won': { type: Number, default: 0 },
            'wonLoads': getLoadsCollectionSchema({ type: Number }),
            'lose': { type: Number, default: 0 },
            'nextGenerateAt': { type: Date, default: () => Date.now() + TEST_GAME_INTERVAL_GENERATING_NEW_LOADS },
            'scene': SceneSchema
        },
        'live': {
            'seed': { type: Number, default: generateGameSid },
            'won': { type: Number, default: 0 },
            'wonLoads': getLoadsCollectionSchema({ type: Number }),
            'lose': { type: Number, default: 0 },
            'nextGenerateAt': { type: Date, default: () => Date.now() + LIVE_GAME_INTERVAL_GENERATING_NEW_LOADS },
            'scene': SceneSchema
        }
    },
    'lastRequest': { type: Date, default: Date.now },
	'accessToken': { type: String, unique: true, default: () => crypto.randomBytes(32).toString('hex') },
};

const UserSchema = new Schema(schema, options);

export default UserSchema;