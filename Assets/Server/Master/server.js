import '../global.js'
import mongoose from './Database/mongoose.js'

// когда случается коннект к базе данных (единожды)
// отправляем pm2 готовность
mongoose.connection.once('connected', () => {
    console.log('MongoDB подключение успешно')
    if(process.send) {
        process.send('ready')
    }
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

	// закрываем базу данных
	mongoose.connection.close(() => {
		console.info('Закрыли базу данных, выходим...')			
		process.exit(0)
	})
}