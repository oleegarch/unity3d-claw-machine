import mongoose from 'mongoose'
import UserSchema from './UserSchema.js'
import UserGooglePlayGamesSchema from './UserGooglePlayGamesSchema.js'
import './UserDataManipulator.js'
import './UserPublicFields.js'
import './UserFindOrCreate.js'
import './UserGame.js'

// model
export const Users = mongoose.model('Users', UserSchema)
export const UsersGooglePlayGames = Users.discriminator('UsersGooglePlayGames', UserGooglePlayGamesSchema, 'GooglePlayGames')

export default Users