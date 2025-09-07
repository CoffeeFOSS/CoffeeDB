## Planning

View [Planned Features](https://github.com/orgs/CoffeeFOSS/projects/1/views/2)

## Front End (Angular)

[Install Node.js](https://nodejs.org/en)
In the `/Frontend` directory

```
npm install -g @angular/cli
npm install # initial install only
ng serve --watch
```

## Back End (.NET)

[Install .NET 9](https://dotnet.microsoft.com/en-us/download)
In the `/Backend` directory,

```
dotnet restore # initial install only
dotnet watch # or dotnet run
```

Most changes to backend does not refresh via `dotnet watch`, so you'll need to restart the backend often. Might as well use `dotnet run` to save yourself from disappointment and confusion.

## Environment

For now, non-secret information will go inside `appsettings.Development.json`. Eventually we'll use appsettings.json for deployment, but no secret information should go in there.

## Commits

Please structure your commits like `[feat] some feature`, it doesnt matter what goes in `[]` as long as it makes sense.

## Dev QoL

If you are using VSCode, install the workspace extensions in `/.vscode/extensions`.

Postman collection files will be updated in `/misc/postman`, later on we'll add proper API documentation.

The current system design file `/misc/system_design/CoffeeDB.excalidraw` can be viewed on [Excalidraw](https://excalidraw.com/)
