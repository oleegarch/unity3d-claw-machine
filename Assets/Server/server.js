import './global.js'
import httpServer from './httpServer.js'
import mongoose from './Database/mongoose.js'
import { removeCachedUsers } from './Database/Users/cachedUsers.js'
import { unityServer } from './API/usersGame.js'

// когда случается коннект к базе данных (единожды)
// слушаем порт и отправляем pm2 готовность
mongoose.connection.once('connected', () => {
    console.info('MongoDB подключение успешно')
    httpServer.listen(PORT, HOST, () => {
        console.info(`Сервер запущен на http://${HOST}:${PORT}`)
        // отсылаем pm2 готовность
        if(process.send) {
            process.send('ready')
        }
    })
})

// обработчик отвечающий за перезагрузку сервера
// все кэшированные данные должны быть сохранены, после чего следует перезагрузка
global.serverRestartingStarted = false
global.restartServer = async function() {
	if(global.serverRestartingStarted) {
		return
	}

	console.info('Запущен рестарт сервера')
	global.serverRestartingStarted = true

	// убиваем процесс юнити
	await unityServer.kill()

	// очищаем кешированных пользователей
	await removeCachedUsers()

	// вырубаем аппу
	await new Promise(resolve => httpServer.close(resolve))
	console.info('Сервер закрыт...')
	
	// закрываем базу данных
	await mongoose.connection.close()

	console.info('Закрыли базу данных, выходим...')			
	process.exit(0)
}