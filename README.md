## Planning

View [Planned Features](https://github.com/users/robchendev/projects/1/views/2)

## Front End (Angular)

[Install Node.js](https://nodejs.org/en)
In the `/Frontend` directory

Initial install

```
npm install -g @angular/cli
npm install
```

Run development build

```
ng serve
```

### Testing (Jest)

If you're on Mac, and youre running into issues running `npm run test:watch`, you can fix it by installing watchman and tweaking file descriptor sizes:

```bash
brew install watchman
sudo launchctl limit maxfiles 16384 16384 && ulimit -n 16384
```

## Database (PostgreSQL)

Install PostgreSQL:

- [MacOS](https://postgresapp.com/)
- [Windows/Linux](https://www.postgresql.org/download/)

The backend expects to connect using the following credentials, also defined in `appsettings.Development.json`:

- Database: coffeedb
- Username: postgres
- Password: 1234

## Back End (.NET)

[Install .NET 9](https://dotnet.microsoft.com/en-us/download)
In the `/Backend` directory,

Initial install

```
dotnet restore
```

Run development build

```
dotnet watch
```

Most changes to backend does not refresh via `dotnet watch`, so you'll need to restart the backend often. Might as well use `dotnet run` to save yourself from disappointment and confusion.

## Environment

For now, non-secret information will go inside `appsettings.Development.json`. Eventually we'll use appsettings.json for deployment, but no secret information should go in there.

## Commits

Please structure your commits like `[feat] some feature`, it doesnt matter what goes in `[]` as long as it makes sense.

## Dev QoL

If you are using VSCode, install the workspace extensions in `/.vscode/extensions`.

View postman collection and current system design files at
https://drive.google.com/drive/folders/1DwAAinVfh1acoeLnDJgPP51gNhoecVis?usp=sharing

## MacOS Quirks

If you are on MacOS, disable AirPlay Receiver, otherwise you will randomly get 403 Forbidden errors on every request. It was a massive pain to figure out what was wrong. https://stackoverflow.com/a/70562478
