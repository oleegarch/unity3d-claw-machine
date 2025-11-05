import { API_ROUTE } from './Common/config.js'
import express from 'express'
import bodyParser from 'body-parser'
import cors from 'cors'

import onError from './API/error.js'
import usersRouter from './API/users.js'
import usersGameRouter from './API/usersGame.js'

const app = express();
app.disable('x-powered-by');
app.set('trust proxy', 1);
app.use(bodyParser.urlencoded({ limit: '5mb', extended: false }));
app.use(bodyParser.json({ limit: '5mb' }));
app.use(cors());

app.use(API_ROUTE + '/users', usersRouter);
app.use(API_ROUTE + '/users/game', usersGameRouter);
app.use(onError);

export default app;