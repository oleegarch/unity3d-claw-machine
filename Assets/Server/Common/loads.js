const loads = [
    {
        name: 'octopus'
    },
    {
        name: 'shark'
    },
    {
        name: 'teddy'
    },
    {
        name: 'turtle'
    }
];
export const loadNames = loads.map(load => load.name);

export function getLoadsCollectionSchema(value) {
    const schema = {};
    loads.forEach(load => schema[load.name] = value);
    return schema;
}

export default loads;