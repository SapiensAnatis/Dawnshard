# PhotonStateManager

This is a very basic ASP.NET Core microservice that acts as a stateful storage of Photon rooms for the purposes of matchmaking and display in the main API server.

It operates by receiving webhook events from the Photon server, which trigger rooms to be created, updated, and deleted in the backing Redis storage. It further exposes a REST API to retrieve this data from the main API server.

## API

PhotonStateManager exposes a very simple REST API that is divided into two halves: 'private' endpoints designed for consumption by the Photon server, and 'public' endpoints that are designed to be consumed by the main Dragalia Lost API server. The private endpoints are under the /event/ route group, and the public endpoints are under the /get/ route group.

The private endpoints are secured by bearer token authentication set by an environment variable `PHOTON_TOKEN`.

### /get/ endpoints

- `/get/gamelist`: Returns a list of currently open games that are available to join for public matchmaking.
- `/get/byid/{roomId}`: Searches for a room by its numeric passcode. This can include private rooms as well. If found, returns 200 OK, otherwise 404 Not Found.
- `/get/ishost/{viewerId}`: Returns a scalar boolean indicating whether a user with the provided player ID is a host in any room.
- `/get/byviewerid/{viewerId}`: Returns the room that a player is in, or 404 if they could not be found in a room.
