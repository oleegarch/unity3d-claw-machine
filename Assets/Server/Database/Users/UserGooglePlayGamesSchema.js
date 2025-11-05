import axios from 'axios'
import mongoose from 'mongoose'
import { google } from 'googleapis'
import { options } from './UserSchema.js'
import auth from '../../auth.js'

const Schema = mongoose.Schema;

// google play games schema
export const schema = {};
export const UserGooglePlayGamesSchema = new Schema(schema, options);


UserGooglePlayGamesSchema.statics.getPlayerInfoByAuthorization = async function(authorization) {
    const oauth2Client = new google.auth.OAuth2(
        auth.google.clientId,
        auth.google.clientSecret,
        auth.google.redirectURI
    );
    try {
        const { tokens } = await oauth2Client.getToken(authorization.payload);
        oauth2Client.setCredentials(tokens);

        const { data } = await axios.get(`https://www.googleapis.com/games/v1/players/me?access_token=${tokens.access_token}`);
        console.log(`Авторизовал пользователя через Google Play Games с идентификатором ${data.playerId} и именем ${data.displayName}`);
        return data;
    } catch(e) {
        console.error('Ошибка в авторизации пользователя в Google Play Games по его server auth code', e);
        throw 'google_play_games_auth_error_by_code';
    }
}

UserGooglePlayGamesSchema.methods.fillDataByAuthorization = async function(authorization) {
    const googlePlayer = authorization.payload;
    this.localSet('displayName', googlePlayer.displayName);
}

export default UserGooglePlayGamesSchema;