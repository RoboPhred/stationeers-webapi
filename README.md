# WebAPI for Stationeers

Provies a RESTful API for reading and writing data to a live Stationeers game.

# Installation

Requires [BepInEx 5.0.1](https://github.com/BepInEx/BepInEx/releases) or later.

1. Install BepInEx in the Stationeers steam folder.
2. Launch the game, reach the main menu, then quit back out.
3. In the steam folder, there should now be a folder BepInEx/Plugins
4. Create a folder `stationeers-webapi` in the BepInEx/Plugins folder.
5. Extract the release zip file to this folder.

# Compatibility

This mod has been tested against Stationeers 0.2.2821.13996 and its Dedicated Server.

# Usage

Make HTTP requests to your server's port.
Expect responses to be `application/json`

# Configuration

Create a file called `config.json` inside the `stationeers-webapi` folder under `BepInEx/Plugins`.
This file should be a json object with the following properties.

- `enabled` (bool): Specify whether the mod should be enabled or disabled. Set to `true` to enable.
- `port` (number): The port to use. Defaults to your server's game port.
- `protocol` (`http` or `https`, optional): Sets the protocol to use for the api.
  - Note: https prevents malacious networks from stealing login credentials, and is required for the `steam` authentication mode.
- `passwordAuthentication`: Settings for unsecure plaintext password authentication.
  - `enabled`: Set to true to enable this authentication mechanism.
  - `password`: The password to require.
- `steamAuthentication`: Settings for authenticating with steam user accounts.
  - `enabled`: Set to true to enable this authentication mechanism.
  - `allowedSteamIds`: An array of steam id strings to allow.  If not specified, any steam id can log in.
- `jwtSecret` (string, optional): The encryption key to encode the user's login token. If unset, a random key will be used.
  - Note: Set this if you want to remember logins across server restarts.

# Security

There are two factors to the security of any RCON mod: Connection and authentication

## Connection security

The security of the connection between you and the server determines if any outside party can intercept your connection to steal passwords and login tokens.
The connection mode is determined by the `protocol` configuration parameter.

Connection security comes in two forms:

## http

HTTP mode is insecure, as malacious networks can intercept packets and find passwords and login tokens.
Despite this, the origional built-in rcon server also runs in http mode, and has the same security problems. Running in http mode will not make you any less
secure than the game already is, save for extending the attack surface to the additional commands enabled by this api.

HTTP mode cannot be used with steam authentication, as steam sensibly refuses to authenticate unless the connection uses https.

## https

HTTPS adds encryption to the http connection, preventing malacious networks from stealing passwords and login tokens.

HTTPS mode is a work in progress, and currently cannot be used.

## Authentication

Authentication controls how the mod determines who is and is not allowed to execute commands on the server.  Multiple authentication mechanisms can be
enabled at the same time and used concurrently.

### Anonymous Authentication

If neither password nor steam authentication is enabled, the server will allow anonymous users access without any authentication.
This is not a discrete mode, but instead only applies when no other mode is available.

### Password Authentication

Password authentication accepts a plaintext (unsecured) password.  At the moment, there is only support for one password, and it will grant access to all api endpoints.
To enable password authentication, set the configuration option`passwordAuthentication.enabled` to `true`, and set your password in `passwordAuthentication.password`.

To login with a password, make a request to `/api/login/password?password=foobar`, where `foobar` is the configured password.

### Steam

Steam authentication involves using the Steam OpenID servers to require the user to log in with their steam account. The mod never receives their steam password, only their username, id, and a session key. This is used to verify with steam that the user did indeed log in on behalf of the mod. This can be used to allow steam id logins without risking the user's credentials being leaked, and provides the best security for the api.

Steam authentication is still a work in progress. A functional prototype is available, but due to a requirement imposed by steam, it requires the use of https connection security. More details about implementing steam authentication will be provided once https is fully implemented.

## Login Tokens

For all authentication methods that require a login, the user must log in using the appropriate endpoint. On a successful login, the mod will return a login token under the `authorization` property.
This token must be sent with all requests in one of two ways:

- In an `Authorization` cookie.
- In an `Authorization` header, in the form `Bearer <token>`.

Failure to send this token, or sending an invalid token, will result in the command being rejected.

These tokens uniquely identify your user to the system, and connect your login with your future requests. They are valid for 1 hour, after which you must log in again.
These tokens are encrypted. By default, the encryption key is randomly generated each time the server starts up. You may choose to use a persistent key by specifying the `jwtSecret` configuration property. However, if a third party obtains this key, they will be able to generate forged login requests and bypass security. It is recommended that this parameter remains empty so a random encrpytion key will be used every server restart.

For additional security, tokens are bound to the ip address of the client when the token was issued.  If the client's ip changes, they will need to login again.  This is to provide a small amount of protection against the case where a token is intercepted by a malacious third party, as can happen when using the http protocol.

## Supported Requests

### GET /api/status

Gets the general server status.

### GET /api/settings

Gets game and server settings.

### POST /api/settings

#### Body

- `name` (string, optional): Sets the server name.
- `password` (string, optional): Sets the server password
- `maxPlayers` (int, optional): Sets the max players allowed on the server.
- `startingCondition` (string, optional): Sets the starting condition for new players.
- `respawnCondition` (string, optional): Sets the respawn condition for new players.

### GET /api/settings/starting-conditions

Gets an array of valid starting conditions on this server.

### GET /api/settings/respawn-conditions

Gets an array of valid respawn conditions on this server.

### GET /api/players

Gets a list of all players on the server

### POST /api/players/:steamId/kick

Kick a player from the server.
steamId must be a steamId of a player on the server.

#### Body

- `reason` (string, optional): The reason message for the kick. Can be empty.

### POST /api/players/:steamId/ban

Bans a player from the server.
steamId should be a valid steamId, but the player does not have to be on the server. If they are on the server, they will be kicked.

#### Body

- `hours` (int): The number of hours to ban the player for.
- `reason` (string, optional): The reason message for the ban. Can be empty.

### GET /api/bans

Gets an array of all bans.

### GET /api/bans/:steamId

Gets a ban by steam id.

### DELETE /api/bans/:steamId

Removes a ban by steam id.

### GET /api/chat

Gets an array of chat messages currently known to the server.

### POST /api/chat

Sends a chat message to the server

#### Body

- `message` (string): The message to send to the server.

### GET /api/things

Gets an array of all things.

### GET /api/things/:thingId

Gets a thing by its id.

### POST /api/things/:thingId

Change properties of a thing.

#### Body

- `customName` (string, optional): Change the labeler-given name of a device.
- `accessState` (int, optional): Set the bitmask of allowed access card colors.

### GET /api/devices

Gets an array of all device things.

### GET /api/devices/:deviceId

Gets a device by reference id.

### POST /api/devices/:deviceId

Change properties of a device.

#### Body

- `customName` (string, optional): Change the labeler-given name of a device.
- `accessState` (int, optional): Set the bitmask of allowed access card colors.

### GET /api/devices/:deviceId/logic

Gets all readable logic values for a device.

### GET /api/devices/:deviceId/logic/:logicType

Gets a readable logic value by logic type.

### POST /api/devices/:deviceId/logic/:logicType

Writes a writable logic value by logic type.

#### Body

- `value` (number): The value to write to the logic type.

### GET /api/items

Gets an array of all item things.

### GET /api/atmospheres

Gets an array of all atmosphere cells.

### GET /api/pipe-networks

Gets an array of all pipe networks.

### GET /api/saves

Gets the time the game last saved this session.

### POST /api/saves

Save the game

### GET /api/station-contacts

Gets a list of station contacts, including satalites and traders.

### GET /api/station-contacts/:contactId

Gets a station contact by contact id.

### POST /api/station-contacts/:contactId

Change properties of a station contact.

### DELETE /api/station-contacts/:contactId

Delete a station contact by contact id.

### GET /api/stationpedia/ic/instructions

Gets a dictionary of IC10 instructions.

### GET /api/stationpedia/ic/enums

Gets a dictionary of all possible enums that IC10 instructions can reference.

### GET /api/stationpedia/logic/slottypes

Gets a dictionary of possible logic slot types.

### GET /api/stationpedia/logic/types

Gets a dictionary of possible logic types.

### GET /api/stationpedia/things

Gets a dictionary of all possible items, structures and things with properties.

#### Body

- `fileName` (string): The name of the file to save.

# Compiling from source

This mod can be compiled using the .net SDK tools. Visual studio will do nicely, but any text editor will do as long as the .net sdk is installed.

## Dependencies

This mod makes use of both nuget dependencies, and takes dependencies on game files. For copyright reasons, the game files must be taken from your own game and cannot be redistributed.

### Local dependencies

This library uses nuget to install most local dependencies. You can get these dependencies by running `dotnet restore` in the source directory.

As this mod references many game dlls, you need to give it copies of these dlls for it to compile properly. The project is set up to expect these in the `/externals` folder

### Game dependencies

Copy the following files from `Stationeers/rocketstation_Data/Managed` to a folder called `/externals` in the root of the source directory:

- `Assembly-CSharp.dll`
- `Assembly-CSharp-firstpass.dll`
- `UnityEngine.dll`
- `UnityEngine.CoreModule.dll`
- `UnityEngine.UI.dll`
- `com.unity.multiplayer-hlapi.Runtime.dll`

Copy `BepInEx.dll` from your BepInEx install to the `/externals` folder.

## Compiling

Assuming you have installed the .net sdk properly, the project can be built with `dotnet build` from the command line.

# TODO

- More endpoints
  - Endpoints for derived from Things
    - DynamicThing ( => Item)
    - Structure ( => Device)
  - Endpoint for transmitters (special class of ILogicable seperate from devices)
  - Endpoint for fabricators / manufacturing
    - Get recipes
    - post recipe to facbicator
- Test chat endpoint on dedicated servers
- Strip colors from chat name. Probably still include them in message.
