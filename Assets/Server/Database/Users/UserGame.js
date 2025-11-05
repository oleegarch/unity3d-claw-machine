import UserSchema from './UserSchema.js'
import {
    FREE_TEST_TRIES_FOR_INTERVAL,
    FREE_TEST_TRIES_INTERVAL,
    LIVE_GAME_INTERVAL_GENERATING_NEW_LOADS,
    TEST_GAME_INTERVAL_GENERATING_NEW_LOADS,
    generateGameSid
} from '../../Common/game.js'

UserSchema.methods.renewTestTries = function() {
    if(new Date(this.game.nextFreeTestTriesAt).getTime() <= Date.now()) {
        this.localSet('game.nextFreeTestTriesAt', new Date(Date.now() + FREE_TEST_TRIES_INTERVAL));
        this.localInc('game.testTries', FREE_TEST_TRIES_FOR_INTERVAL);
        return true;
    }
    return false;
}

UserSchema.methods.renewGame = function(gameType = 'test') {
    const game = this.game[gameType];

    if(new Date(game.nextGenerateAt).getTime() <= Date.now()) {
        this.setNewGame(gameType);
        return true;
    }

    return false;
}
UserSchema.methods.setNewGame = function(gameType = 'test') {
    const intervalGenerationNewLoads = gameType === 'test' ? TEST_GAME_INTERVAL_GENERATING_NEW_LOADS : LIVE_GAME_INTERVAL_GENERATING_NEW_LOADS;
    this.localSet(`game.${gameType}.seed`, generateGameSid());
    this.localSet(`game.${gameType}.won`, 0);
    this.localSet(`game.${gameType}.wonLoads`, {});
    this.localSet(`game.${gameType}.lose`, 0);
    this.localSet(`game.${gameType}.nextGenerateAt`, new Date(Date.now() + intervalGenerationNewLoads));
    this.localSet(`game.${gameType}.scene`, { 'loads': [] });
}
UserSchema.methods.takeOneTry = function(gameType = 'test', count = 1) {
    const field = gameType === 'test' ? 'testTries' : 'liveTries';
    if(this.game[field] >= count) {
        this.localInc(`game.${field}`, -count);
        return true;
    }
    return false;
}