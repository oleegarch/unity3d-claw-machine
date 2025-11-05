import os from 'os'
import { IS_DEVELOPMENT, IS_PRODUCTION, PORT } from './Common/config.js'

// глобальные переменные
global.IS_DEVELOPMENT = IS_DEVELOPMENT
global.IS_PRODUCTION = IS_PRODUCTION
global.PORT = PORT
global.HOST = 'localhost'
global.REMOTE_ADDRESS = os.hostname()

// запускаем рестарт сервера
// если ловим сообщение о шатдауне
function restart() {
	global.restartServer();
}
process.once('SIGINT', restart)
process.once('SIGTERM', restart)
process.on('message', function(message) {
	if(message === 'shutdown') {
		restart();
	}
})

// все необработанные ошибки
// идут сюда
process.on('unhandledRejection', (reason, promise) => {
  console.log('Необработанная ошибка:', promise, 'причина:', reason);
})