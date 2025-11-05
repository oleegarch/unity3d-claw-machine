import UserSchema from './UserSchema.js'
import { getCachedUser, getCachedUserByAccessToken, setCachedUser } from './cachedUsers.js'

UserSchema.statics.findByAuthorizationId = function(id) {
    return this.findOne({ 'authId': id });
}
UserSchema.statics.findOrCreate = async function(authorization) {
    let cachedUser = getCachedUser(authorization.id);

    // для начала проверим есть ли пользователь в кеше
    if(cachedUser != null) {
        console.log(`Возвращаю кешированного пользователя ${cachedUser.displayName} с идентификатором ${cachedUser.authId}`);
        return cachedUser;
    }

    // если в кеше нету
    // ищем в базе данных
    let user = await this.findByAuthorizationId(authorization.id);

    // если в базе данных нету
    // значит регистрируем пользователя
    if(user == null) {
        user = new this({ 'authPlatform': authorization.platform, 'authId': authorization.id });
        await user.fillDataByAuthorization(authorization);
        await user.save();
        await user.localSave();
    }

    // за время выполнения асинхронных операций
    // пользователь мог сдублировать запрос и авторизоваться
    // поэтому снова проверим наличие его в кеше
    cachedUser = getCachedUser(authorization.id);
    if(cachedUser != null) {
        return cachedUser;
    }

    // если в кеше его нет
    // значит устанавливаем его в кеш
    setCachedUser(authorization.id, user);
    console.log(`Установил в кеш пользователя ${user.displayName} с идентификатором ${authorization.id}`);

    return user;
}
UserSchema.statics.findByAccessToken = async function(accessToken) {
    const cachedUser = getCachedUserByAccessToken(accessToken);

    if(cachedUser != null) {
        console.log(`Возвращаю кешированного пользователя ${cachedUser.displayName} с идентификатором ${cachedUser.authId}`);
        return cachedUser;
    }

    const user = await this.findOne({ accessToken });

    if(user != null) {
        setCachedUser(user.authId, user);
    }

    return user;
}