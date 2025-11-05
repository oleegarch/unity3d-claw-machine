import lodash from 'lodash'
import express from 'express'
import validate from './validate.js'
import { getLocalUser } from './users.js'
import { APIError } from './error.js'
import SpawnUnityServer from '../Utils/spawnUnityServer.js'
import antiMultiRequest from './antiMultiRequest.js'

const { isEqual } = lodash;
const router = express.Router();

export const unityServer = new SpawnUnityServer('/home/oleg/Grabby/GameServer/grabby.x86_64', global.PORT + 100);

router.post(
    '/reload',
    validate({
        'accessToken': { type: String, required: true },
        'gameType': { type: String, enum: ['test', 'live'], required: true }
    }),
    getLocalUser,
    antiMultiRequest(function(req, res, next) {
        const user = req.user;
        const gameType = req.body.gameType;

        const renewed = user.renewGame(gameType);

        if(renewed) {
            return res.json(user.game.toObject());
        }

        const taken = user.takeOneTry(gameType);

        if(taken) {
            user.setNewGame(gameType);
            res.json(user.game.toObject());
        }
        else {
            next(new APIError(402, 'tries_not_enough'));
        }
    })
);

router.post(
    '/started',
    validate({
        'accessToken': { type: String, required: true },
        'gameType': { type: String, enum: ['test', 'live'], required: true },
        'gameScene': { type: String, required: true }
    }),
    getLocalUser,
    antiMultiRequest(function(req, res, next) {
        const user = req.user;
        const gameType = req.body.gameType;
        let gameScene = req.body.gameScene;

        try {
            gameScene = JSON.parse(gameScene);
        } catch(e) {
            console.error('Не удалось распарсить json объект сцены игры', e);
            return next(new APIError(400, 'invalid_gameScene'));
        }

        const taken = user.takeOneTry(gameType);

        if(!taken) {
            return next(new APIError(402, 'tries_not_enough'));
        }

        user.localSet('game.state.type', gameType);
        user.localSet('game.state.started', true);
        user.localSet(`game.${gameType}.scene`, gameScene);

        const field = gameType === 'test' ? 'testTries' : 'liveTries';
        res.json({ 'leftTries': user.game[field] });
    })
);

router.post(
    '/ended',
    validate({
        'accessToken': { type: String, required: true },
        'gameType': { type: String, enum: ['test', 'live'], required: true },
        'controlState': { type: String, required: true }
    }),
    getLocalUser,
    antiMultiRequest(async function(req, res, next) {
        const user = req.user;
        const gameType = req.body.gameType;
        let controlState = req.body.controlState;

        if(user.game.state.started !== true) {
            return next(new APIError(400, 'game_not_started'));
        }
        if(gameType !== user.game.state.type) {
            return next(new APIError(400, 'invalid_gameType'));
        }

        try {
            controlState = JSON.parse(controlState);
            controlState.startScene = user.game[gameType].scene;
        } catch(e) {
            console.error('Не удалось распарсить json объект состояния игры', e);
            return next(new APIError(400, 'invalid_controlState'));
        }

        const wonLoads = controlState.wonLoads;
        let error, simulatedWonLoads, endScene, isWon = false;

        try {
            const response = await unityServer.sendHttpRequest('/getWonLoads', controlState);
            error = response.error;
            simulatedWonLoads = response.simulatedWonLoads;
            endScene = response.endScene;
            isWon = simulatedWonLoads.length > 0;
        } catch(e) {
            console.error('Ошибка при отправке запроса к юнити для симуляции', e);
            return next(new APIError(500, 'server_error'));
        }

        console.log('Ответ ёпта ', error, simulatedWonLoads, endScene);

        if(error) {
            console.error('getWonLoads error', error);
            return next(new APIError(400, 'invalid_controlState'));
        }
        
        if(!isEqual(simulatedWonLoads, wonLoads)) {
            console.error('getWonLoads not equals', simulatedWonLoads, wonLoads);
            return next(new APIError(400, 'wonLoads_not_equals'));
        }

        user.localUnset('game.state.type');
        user.localSet('game.state.started', false);
        user.localSet(`game.${gameType}.scene`, endScene);
        if(isWon) {
            user.localInc(`game.${gameType}.won`, simulatedWonLoads.length);
        }
        else {
            user.localInc(`game.${gameType}.lose`, 1);
        }
        for(const wonLoadName of simulatedWonLoads) {
            user.localInc(`game.${gameType}.wonLoads.${wonLoadName}`, 1);
            user.localInc(`loadsCollection.${wonLoadName}`, 1);
        }

        let newScore = 0, newCoins = 0;
        if(gameType === 'live') {
            if(isWon) {
                user.localInc('loadsCount', simulatedWonLoads.length);
                newScore = simulatedWonLoads.length * 1000;
                newCoins = simulatedWonLoads.length * 10;
            }
            else {
                newScore = 100;
                newCoins = 1;
            }
        }
        else {
            if(isWon) {
                newScore = simulatedWonLoads.length * 100;
                newCoins = simulatedWonLoads.length;
            }
            else {
                newScore = 10;
            }
        }
        user.localInc('score', newScore);
        user.localInc('coins', newCoins);

        const userGame = user.game.toObject();
        res.json({ simulatedWonLoads, endScene, newScore, newCoins, userGame });
    })
);

export default router;