export const IS_DEVELOPMENT = process.env.NODE_ENV !== 'production'
export const IS_PRODUCTION = !IS_DEVELOPMENT
export const IS_NODE = typeof process !== 'undefined' && process.versions != null && process.versions.node != null
export const MONGODB_DATABASE_NAME = IS_PRODUCTION ? 'grabby' : 'grabbytest'

export const PORT = Number(process.env.PORT)

export const SCHEME = 'https://'
export const DOMAIN = IS_PRODUCTION ? 'oleegarch.com' : 'jumpshoot.oleegarch.com'
export const APP_ROUTE = '/grabby'

export const API_ROUTE = APP_ROUTE + '/api'
export const API_SERVER_GAME_ROUTE = APP_ROUTE + '/api/game'

export const BASE_URL = SCHEME + DOMAIN
export const API_URL = SCHEME + DOMAIN + API_ROUTE
export const APP_URL = SCHEME + DOMAIN + APP_ROUTE
export const SERVER_GAME_URL = SCHEME + DOMAIN + API_SERVER_GAME_ROUTE

export const CLEAR_CACHED_USERS_INTERVAL = 60000 * 5;