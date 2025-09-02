## Front End (Angular)

In the `/Frontend` directory

```
npm install # initial install only
ng serve --watch
```

## Back End (.NET)

In the `/Backend` directory,

```
dotnet restore # initial install only
dotnet watch # or dotnet run
```

Most changes to backend does not refresh via `dotnet watch`, so you'll need to restart the backend often. Might as well use `dotnet run` to save yourself from disappointment and confusion.

## Commits

Please structure your commits like `[feat] some feature`, it doesnt matter what goes in `[]` as long as it makes sense.

## Dev QoL

If you are using VSCode, install the workspace extensions in `/.vscode/extensions`.

Postman collection files will be updated in `/misc/postman`, later on we'll add proper API documentation.

The current system design file `/misc/system_design/CoffeeDB.excalidraw` can be viewed on [Excalidraw](https://excalidraw.com/)
