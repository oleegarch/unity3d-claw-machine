const ecosystem = require('./ecosystem.config.cjs');

ecosystem.apps.forEach(app => {
	if(app.name === 'grabby') {
		app.instances = 4;
	}
	app.watch = false;
})

module.exports = ecosystem;