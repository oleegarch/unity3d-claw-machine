import pick from 'pick-deep'
import UserSchema from './UserSchema.js'

export const mainPublicFields = [
    '_id',
    'authorization.platform',
    'authorization.id',
    'firstName',
    'lastName',
    'coins',
    'score',
    'loadsCount'
];
export const visiblePublicFields = [
    ...mainPublicFields,
    'gender',
    'lang',
    'timezone'
];
export const allPublicFields = [
    ...visiblePublicFields,
    'settings',
    'training',
    'game',
    'loadsCollection',
    'registeredAt',
    'updatedAt'
];

export const publicFields = { mainPublicFields, visiblePublicFields, allPublicFields };

UserSchema.statics.getPublicFields = function(type = 'mainPublicFields') {
    return publicFields[type];
}
UserSchema.methods.toPublicFields = function(type = 'mainPublicFields') {
    const fields = publicFields[type];
    return pick(this, fields);
}