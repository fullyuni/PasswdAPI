# PasswdAPI
This is Yunus Tezcan's submission for a code challenge from Brain Corp.

A minimal HTTP service that exposes the user and group information on a UNIX-like system that is usually locked away in the UNIX /etc/passwd and /etc/groups files.

This project is a .NET Core app written in C#.

## Setup
Because this is a .NET Core application, you will need .NET Core installed on your environment. Please use the following link and instructions on the page to install the latest version of .NET Core: https://dotnet.microsoft.com/download

Before launching the application, please check that the file appsettings.json (included in the project) is pointing to the desired file paths. The default file paths are set to:
```JSON
  "AppConfiguration": {
    "passwdFilePath": "/etc/passwd",
    "groupFilePath": "/etc/group"
  }
```
To launch the api console, navigate to the project folder in terminal (cd .../PasswdAPI/) and run the command:
```bash
dotnet run
```
Note: Since the data set was realtively minimal, I chose to forgo the traditional DB model and use a collection of in-memory tables that are built on startup and updated using the FileSystemWatcher. I figured this would reduce the amount of setup work needed for the end-user (the tester).

## Usage
Once the app is launched you can access the API from your browser using: https://localhost:5001/api

The initial landing page has all the possibl API commands outlined. Simply append the following to the localhost path given above:
```HTML
/users
```
Return a list of all users on the system, as defined in the /etc/passwd file.
```HTML
/users/query[?name=<nq>][&uid=<uq>][&gid=<gq>][&comment=<cq>][&home=<hq>][&shell=<sq>]
```
Return a list of users matching all of the specified query fields.

Note: While I did not find any naturally occuring examples, users apparently can have multiple gids. My code assumes this case and can handle multiple gids in the format: 1,-2,3,(etc.)
```HTML
/users/<uid>
```
Return a single user with <uid>.
```HTML
/users/<uid>/groups
```
Return all the groups for a given user.

Note: The test was unclear of the relationship between users and groups (gids -> gid vs name -> members) so I output the results of both possible options.
```HTML
/groups
```
Return a list of all groups on the system, a defined by /etc/group.
```HTML
/groups/query[?name=<nq>][&gid=<gq>][&member=<mq1>[&member=<mq2>][&...]
```
Return a list of groups matching all of the specified query fields.
```HTML
/groups/<gid>
```
Return a single group with <gid>.
```HTML
/changes
```
Returns all changes that have occured to files while application is running.
