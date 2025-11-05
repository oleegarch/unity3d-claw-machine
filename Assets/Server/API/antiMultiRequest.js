import { APIError } from './error.js'
const requests = {};

export default function antiMultiRequest(handler) {
	return async (req, res, next) => {
		if(requests[req.user._id.toString()] === true) {
			return next(new APIError(429, 'too_much_requests'));
		}

		requests[req.user._id.toString()] = true;

		await handler(req, res, next);

		delete requests[req.user._id.toString()];
	}
};