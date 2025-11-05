import UserSchema from './UserSchema.js'
import lodash from 'lodash'
import pick from 'pick-deep'
const { clone, merge } = lodash

UserSchema.methods.localSet = function(field, value) {
    this.__localUpdate = this.__localUpdate ?? {};
    this.__localUpdate.$set = this.__localUpdate.$set ?? {};
    if(typeof field === 'string') {
        this.__localUpdate.$set[field] = value;
        this.$set(field, value);
    }
    else if(typeof field === 'object' && field !== null) {
        for(const key in field) {
            this.localSet(key);
        }
    }
}
UserSchema.methods.localUnset = function(field) {
    this.__localUpdate = this.__localUpdate ?? {};
    this.__localUpdate.$unset = this.__localUpdate.$unset ?? {};
    if(typeof field === 'string') {
        this.clearField(field, '$unset');
        this.__localUpdate.$unset[field] = '';
        this.$set(field, undefined);
    }
    else if(typeof field === 'object' && field !== null) {
        for(const key in field) {
            this.localUnset(key);
        }
    }
}
UserSchema.methods.localInc = function(field, value) {
    this.__localUpdate = this.__localUpdate ?? {};
    this.__localUpdate.$inc = this.__localUpdate.$inc ?? {};
    if(typeof field === 'string') {
        this.clearField(field, '$inc');
        this.__localUpdate.$inc[field] = this.__localUpdate.$inc[field] ?? 0;
        this.__localUpdate.$inc[field] += value;
        this.localModify(field, (obj, lastField) => {
            if(obj instanceof Map) {
                obj.set(lastField, obj.get(lastField) ?? 0);
                obj.set(lastField, obj.get(lastField) + value);
            }
            else {
                obj[lastField] = obj[lastField] ?? 0;
                obj[lastField] += value;
            }
        });
    }
    else if(typeof field === 'object' && field !== null) {
        for(const key in field) {
            this.localInc(key, field[key]);
        }
    }
}
UserSchema.methods.localPush = function(field, value, unique = true, additionalOptions = {}) {
    const operator = unique === true ? '$addToSet' : '$push';
    this.__localUpdate = this.__localUpdate ?? {};
    this.__localUpdate[operator] = this.__localUpdate[operator] ?? {};
    if(typeof field === 'string') {
        this.clearField(field, operator);
        const arrayValue = Array.isArray(value) ? value : [value];
        this.__localUpdate[operator][field] = this.__localUpdate[operator][field] ?? { $each: [], ...additionalOptions };
        this.__localUpdate[operator][field].$each.push(...arrayValue);
        this.localModify(field, (obj, lastField) => {
            obj[lastField] = obj[lastField] ?? [];
            obj[lastField].push(...arrayValue);
        });
    }
    else if(typeof field === 'object' && field !== null) {
        for(const key in field) {
            this.localPush(key, field[key], unique, limit);
        }
    }
}
UserSchema.methods.localPull = function(field, value) {
    this.__localUpdate = this.__localUpdate ?? {};
    this.__localUpdate.$pull = this.__localUpdate.$pull ?? {};
    if(typeof field === 'string') {
        this.clearField(field, '$pull');
        this.__localUpdate.$pull[field] = value;
        this.localModify(field, (obj, lastField) => {
            obj[lastField] = obj[lastField] ?? [];
            obj[lastField] = obj[lastField].filter(val => {
                if(
                    typeof value === 'object' &&
                    value !== null &&
                    value._id != null
                ) {
                    return !val._id.equals(value._id);
                }
                return value !== val;
            });
        });
    }
    else if(typeof field === 'object' && field !== null) {
        for(const key in field) {
            this.localPull(key, field[key]);
        }
    }
}

// метод который очищает апдейты другого типа
// чтобы избежать ошибки
// Updating the path 'path' would create a conflict at 'path'
UserSchema.methods.clearField = function(field, broadcast) {
    for(const key of Object.keys(this.__localUpdate).filter(key => key !== broadcast)) {
        if(this.__localUpdate[key][field] != null) {
            delete this.__localUpdate[key][field];
        }
    }
}

// сохраняет всё что было изменено с помощью кэша в __localUpdate
// если передать сокет, после обновления он отправит все обновлённые данные клиенту
UserSchema.methods.localSave = async function() {
    const Model = this.model('Users');

    // строим переменные 
    // updated пойдёт юзеру
    // update пойдёт в базу данных
    let updated = {};
    const update = clone(this.__localUpdate ?? {});
    const fieldsToUpdateLocal = [];
    for(const key in update) {
        for(const keyIn in update[key]) {
            fieldsToUpdateLocal.push(keyIn);
        }
    }

    this.__localUpdate = {};

    if(Object.keys(update).length > 0) {
        const doc = await Model.findByIdAndUpdate(
            this._id,
            update,
            {
                returnDocument: 'after',
                lean: true,
                projection: fieldsToUpdateLocal
            }
        );
        // документ может отсутствовать (например, мы его удалили с базы данных)
        // но чтобы updated был не пустым, запишу данные с текущего документа (else)
        if(doc != null) {
            updated = pick(doc, fieldsToUpdateLocal);
            merge(this, updated);
        }
        else {
            updated = pick(this, fieldsToUpdateLocal);
        }
    }

    return updated;
}

// метод, который берёт предпоследний объект (preLastObject) ключа (key)
// и запускает аргумент (modify) с аргументами этого объекта и последнего ключа (preLastObject, key[key.length - 1])
// для того, чтобы модифицировать предпоследний объект самостоятельно (например, с помощью push)
UserSchema.methods.localModify = function(key, modify) {
    const allKeys = key.split('.');
    const keys = allKeys.slice(0, -1);
    let preLastObject = this;
    for(const keyIn of keys) {
        preLastObject = preLastObject[keyIn];
    }
    modify(preLastObject, allKeys[allKeys.length - 1]);
}