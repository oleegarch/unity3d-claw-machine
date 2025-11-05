const dateFormat = 'DD, HH:mm';

const envVars = (port, isMaster = false) => ({
	instance_var: 'INSTANCE_ID',
	env_development: {
		"IS_MASTER": isMaster,
		"PORT": port,
		"NODE_ENV": "development",
	},
	env_production : {
		"IS_MASTER": isMaster,
		"PORT": port,
		"NODE_ENV": "production"
	},
});

const controlVars = {
	min_uptime: '1s',
	listen_timeout: 3000,
	kill_timeout: 20000,
	shutdown_with_message: true,
	wait_ready: true,
	max_restarts: 3,
	restart_delay: 1000,
	autorestart: true,
};

module.exports = {
	apps : [
		{
			// main
			name: 'grabby',
			script: './server.js',
			args: '',
			instances: 2,
			exec_mode: 'cluster',
			watch: true,
			ignore_watch: ['node_modules', 'controlState.json'],
			max_memory_restart: '1000M',
			increment_var: 'PORT',
			source_map_support: false,
			...envVars(4000),

			// logs
			log_date_format: dateFormat,
			error_file: '/home/oleg/.pm2/logs/grabby-errors.log',
			out_file: '/home/oleg/.pm2/logs/grabby-outs.log',
			combine_logs: true,

			// control flow
			...controlVars,
			/*cron_restart: '1 0 * * *',*/
			vizion: true,
			/*post_update: ['npm update'],*/
			force: false
		},
		{
			// main
			name: 'grabbyMaster',
			script: './Master/server.js',
			watch: true,
			ignore_watch: ['node_modules', 'controlState.json'],
			max_memory_restart: '500M',
			...envVars(null, true),

			// logs
			log_date_format: dateFormat,
			error_file: '/home/oleg/.pm2/logs/grabbyMaster-errors.log',
			out_file: '/home/oleg/.pm2/logs/grabbyMaster-outs.log',

			// control flow
			...controlVars
		}
	]
};