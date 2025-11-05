export class APIError extends Error {
    constructor(code, message) {
        super(message);
        this.name = 'APIError';
        this.code = code;
    }
}

export default function(error, req, res, next) {
    let code = 500;
    let message = 'unknown_error';

    if(typeof error === 'string') {

        message = error;

        switch(error) {
            case 'google_play_games_auth_error_by_code': {
                code = 400;
                break;
            }
        }
    }

    if(error instanceof APIError) {
        code = error.code ?? 500;
        message = error.message;
    }
    
    console.error(`В API запросе ${req.path} отсылаю ошибку с кодом ${code} и сообщением ${message}`, error);

    res.status(code).json({ message });
}