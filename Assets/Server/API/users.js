import express from 'express'
import validate from './validate.js';
import Users, { UsersGooglePlayGames } from '../Database/Users/Users.js'
import { APIError } from './error.js'
import { schema as userSchema } from '../Database/Users/UserSchema.js'

const router = express.Router();

export async function getLocalUser(req, res, next) {
    const accessToken = req.body.accessToken;
    const authId = req.query.authId;

    try {
        const user = await Users.findByAccessToken(accessToken);
        if(user == null) throw new APIError(401, 'invalid_access_token');
        if(authId !== user.authId) throw new APIError(401, 'invalid_query_authId');
        user.localSet('lastRequest', new Date());
        req.user = user;
        console.log(`Поступил новый запрос ${req.path} от пользователя ${user.displayName} с идентификатором ${user.authId}`);
        next();
    } catch(e) {
        next(e);
    }
}

router.post(
    '/getAccessToken',
    validate({
        'platform': {
            type: String,
            enum: userSchema.authPlatform.enum,
            required: true
        },
        'payload': {
            type: String,
            required: true
        }
    }),
    async (req, res, next) => {
        const authorization = req.body;
        const authId = req.query.authId;

        try {
            switch(authorization.platform) {
                case 'GooglePlayGames': {
                    const googlePLayer = await UsersGooglePlayGames.getPlayerInfoByAuthorization(authorization);
                    authorization.id = googlePLayer.playerId;
                    authorization.payload = googlePLayer;
                    if(authId !== authorization.id) throw new APIError(401, 'invalid_query_authId');
                    const user = await Users.findOrCreate(authorization);
                    return res.send(user.accessToken);
                }
                default: {
                    throw new APIError(400, 'unknown_platform');
                }
            }
        } catch(e) {
            next(e);
        }
    }
);

router.post(
    '/getLocalUser',
    validate({
        'accessToken': { type: String, required: true }
    }),
    getLocalUser,
    function(req, res) {
        const user = req.user;
        return res.json(user.toPublicFields('allPublicFields'));
    }
)

router.post(
    '/saveTraining',
    validate({
        'accessToken': { type: String, required: true },
        'trainingType': { type: String, enum: Object.keys(userSchema.training), required: true }
    }),
    getLocalUser,
    function(req, res, next) {
        const user = req.user;
        const trainingType = req.body.trainingType;

        user.localSet(`training.${trainingType}`, true);

        return res.json(user.training.toObject());
    }
)

router.post(
    '/changeSettings',
    function(req, res, next) {
        try {
            req.body.settings = JSON.parse(req.body.settings);
            next();
        } catch(e) {
            return next(new APIError(400, 'json_parse_error'));
        }
    },
    validate({
        'accessToken': { type: String, required: true },
        'settings': {
            'sounds': { type: Boolean },
            'music': { type: Boolean },
            'vibration': { type: Boolean },
            'hiddenInLeaders': { type: Boolean }
        }
    }),
    getLocalUser,
    function(req, res, next) {
        const user = req.user;
        const settings = req.body.settings;

        for(const key in settings) {
            if(
                user.settings[key] != null &&
                user.settings[key] !== settings[key]
            ) {
                user.localSet(`settings.${key}`, settings[key]);
            }
        }

        return res.json(user.settings.toObject());
    }
)

export default router;