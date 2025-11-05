export const TEST_TRIES_FOR_NEW_USERS = 20;
export const LIVE_TRIES_FOR_NEW_USERS = 0;
export const TEST_GAME_INTERVAL_GENERATING_NEW_LOADS = 3600 * 24 * 1000;
export const LIVE_GAME_INTERVAL_GENERATING_NEW_LOADS = 3600 * 24 * 1000;
export const FREE_TEST_TRIES_FOR_INTERVAL = 5;
export const FREE_TEST_TRIES_INTERVAL = 3600 * 24 * 1000;

export function generateGameSid() {
    return Math.floor(Math.random() * 1000000);
}