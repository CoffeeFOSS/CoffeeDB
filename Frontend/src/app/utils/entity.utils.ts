export function getEntityBaseUrlFromEntityName(entityName: string) {
  switch (entityName) {
    case 'Roaster':
      return 'roasters';
    default:
      console.error('Entity type not handled in user contributions!');
      return 'notfound';
  }
}

export function getEntityBaseUrlFromEntityPath(entityPath: string) {
  switch (entityPath) {
    case 'roasters':
      return 'roasters';
    default:
      console.error('Entity type not handled in user contributions!');
      return 'notfound';
  }
}
