import { APIError } from './error.js'
import Schema from 'validate'

export default function validate(schema) {
    const check = new Schema(schema);

    return async (req, res, next) => {
        const errors = check.validate(req.body);

        if(errors.length > 0) {
            console.error('Юзер не прошёл валидацию ', errors.map(error => error.message), req.body);
            return next(new APIError(400, 'validation_failed'));
        }

        next();
    };
}