import { CLEAR_CACHED_USERS_INTERVAL } from '../../Common/config.js'

export const cachedUsers = new Map();

export function getCachedUser(id) {
    return cachedUsers.get(id);
}
export function getCachedUserByAccessToken(accessToken) {
    for(const user of cachedUsers.values()) {
        if(user.accessToken === accessToken) {
            return user;
        }
    }
    return null;
}
export function setCachedUser(id, user) {
    cachedUsers.set(id, user);
}
export function clearCachedUsers() {
    const time = Date.now();
    const idsToClear = [];
    cachedUsers.forEach((user, id) => {
        if(time - user.lastRequest.getTime() > CLEAR_CACHED_USERS_INTERVAL) {
            idsToClear.push(id);
            user.localSave();
            console.log(`Удалил с кеша пользователя ${user.displayName} с идентификатором ${user.authId}`);
        }
    })
    idsToClear.forEach(id => cachedUsers.delete(id));
}
export async function removeCachedUsers() {
    try {
        await Promise.all(Array.from(cachedUsers.values()).map(cachedUser => cachedUser.localSave()));
    } catch(e) {
        console.error('Ошибка при удалении кешированных пользователей, а конкретнее при сохранении', e);
    }
    cachedUsers.clear();
}

setInterval(clearCachedUsers, CLEAR_CACHED_USERS_INTERVAL);