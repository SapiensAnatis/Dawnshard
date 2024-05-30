# PhotonPlugin

The Photon plugin is a DLL that integrates into Photon Server product to provide the custom logic required for the game's co-op mode to function.

To install this plugin, you must first acquire a license for Photon Server. This requires joining the Photon industries or gaming circle for $125/mo, and then purchasing a Photon Server license for $95/mo.

## How to build the plugin

The plugin can be compiled from the source code provided from each release, but you must first add a reference to
PhotonHivePlugin.dll, which can be downloaded as part of
the [Plugins SDK](https://www.photonengine.com/sdks#server-sdkserverserverplugin) from Photon's website. Builds of the
plugin are not provided with the releases to avoid redistributing this proprietary component.

Ensure the plugin is compiled in Release mode before deploying to production.

## How to set up a Photon server

### Part 1: Configuring Photon Server

1. Follow
   the [instructions from the Photon documentation](https://doc.photonengine.com/server/current/getting-started/photon-server-in-5min#ip_address_config)
   to setting up the server. Pay particular attention to step 7 if you intend to expose the server on a public or local
   IP address.
2. Place the plugin binary files in `Plugins/GluonPlugin/<VERSION>/bin`.
3. Install the plugin, by editing `LoadBalancing/GameServer/bin/plugin.config`. Below is a sample configuration:

   ```xml

   <root>
       <PluginSettings Enabled="True">
           <Plugins>
               <Plugin
                       Name="GluonPlugin"
                       Version="v1"
                       AssemblyName="DragaliaAPI.Photon.Plugin.dll"
                       Type="DragaliaAPI.Photon.Plugin.GluonPluginFactory"
                       ApiServerUrl="https://dawnshard.co.uk/"
                       StateManagerUrl="https://photonstatemanager.dawnshard.co.uk/"
                       DungeonRecordMultiEndpoint="dungeon_record/record_multi"
                       TimeAttackEndpoint="dungeon_record/record_time_attack"
                       ReplayTimeoutSeconds="30"
                       BearerToken="primary-token"
                       RandomMatchingStartDelayMs="30000"
                       EnableSecondaryServer="true"
                       SecondaryViewerIdCriterion="1000000000"
                       SecondaryApiServerUrl="https://orchis.cherrymint.live/2.19.0_20220719103923/"
                       SecondaryStateManagerUrl="https://photonstatemanager.cherrymint.live/"
                       SecondaryBearerToken="secondary-token"
                       EnableDiscordIntegration="true"
                       DiscordUrl="https://discordbot.cherrymint.live/bot/"
                       DiscordBearerToken="discord-token"/>
           </Plugins>
       </PluginSettings>
   </root>
   ```

   The config values have the following meanings:

   | Key                        | Explanation                                                                                                                                                                                                                                    |
   | -------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
   | Name                       | Do not change. Name of the plugin as sent to the Dragalia client.                                                                                                                                                                              |
   | Version                    | Plugin version -- the example above will look in Plugins/GluonPlugin/v1/bin for binary files. Can be hot reloaded.                                                                                                                             |
   | AssemblyName               | Do not change. Plugin assembly name.                                                                                                                                                                                                           |
   | Type                       | Do not change. Plugin factory type name.                                                                                                                                                                                                       |
   | ApiServerUrl               | URL of the main API server. Used to make requests for party data.                                                                                                                                                                              |
   | StateManagerUrl            | URL of hosted room state manager (e.g. DragaliaAPI.Photon.StateManager)                                                                                                                                                                        |
   | DungeonRecordMultiEndpoint | Endpoint to call on behalf of players when clearing a quest                                                                                                                                                                                    |
   | TimeAttackEndpoint         | Endpoint to call when registering a Time Attack clear.                                                                                                                                                                                         |
   | ReplayTimeoutSeconds       | The number of seconds players have to confirm whether they want to play again after a clear                                                                                                                                                    |
   | BearerToken                | Token to embed in Authorization header when making requests to the state manager                                                                                                                                                               |
   | RandomMatchingStartDelayMs | The duration in milliseconds to wait for a random matching room (e.g. invasion events) to start when more than one player is present.                                                                                                          |
   | EnableSecondaryServer      | Whether or not to enable secondary server connections. This allows another API server to host rooms on the same Photon server. Each server must have a distinct viewer ID range, as this is how the plugin determines which server belongs to. |
   | SecondaryViewerIdCriterion | The threshold value at which the viewer ID of a joining player means they are joining from the secondary server. All players with viewer IDs less than this value will be considered as being from the primary server.                         |
   | SecondaryApiServerUrl      | Used instead of ApiServerUrl when a room is in secondary server mode.                                                                                                                                                                          |
   | StateManagerUrl            | Used instead of StateManagerUrl when a room is in secondary server mode.                                                                                                                                                                       |
   | SecondaryBearerToken       | Used instead of BearerToken when a room is in secondary server mode.                                                                                                                                                                           |
   | EnableDiscordIntegration   | Whether to enable requests to a Discord bot for posting active rooms. N.B. due to a bug, setting this to false does not disable Discord integration.                                                                                           |
   | DiscordUrl                 | The URL of a server to send Discord bot requests to.                                                                                                                                                                                           |
   | DiscordBearerToken         | The bearer token to use in the Authorization header when making Discord bot requests.                                                                                                                                                          |

4. Update LoadBalancing/GameServer/bin/GameServer.xml.config to increase the maximum property size:

```xml

<Limits>
    <Inbound>
        <EventCache>
            <EventsCount>100000</EventsCount>
            <SlicesCount>1000</SlicesCount>
            <ActorEventsCount>100000</ActorEventsCount>
        </EventCache>

        <Properties>
            <MaxPropertiesSizePerGame>510000</MaxPropertiesSizePerGame>
            <MaxPropertiesSizePerRequest>9000000</MaxPropertiesSizePerRequest>
        </Properties>
        <Operations>
        </Operations>
    </Inbound>
</Limits>
```

5. Optionally, update `bin_Win64/PhotonServer.config` and decrease the `MinimumTimeout` and `MaximumTimeout` values.
   These will control the amount of time before a player who stops sending packets is kicked from the room. The defaults
   cause players to leave after between 10 and 30 seconds of inactivity. For Dragalia, it can be beneficial to decrease
   this so that 1) AI units take over faster in quests and 2) if a player leaves while the room is loading, other
   players are not locked in the loading screen waiting for them for too long.

6. If deploying in production, follow best practices such as:

   - disable debug logging
   - remove the console log appender
   - change the AuthToken from the default on all 3 servers
   - ensure the server-to-server port (4520) is not publicly accessible

### Part 2: Configuring the state manager

The state manager application should be comparitively easy to deploy. It is a Docker container that expects to be
deployed alongside a `redis/redis-stack` image. See the `docker-compose.yml` file in the repository for reference. It
does not need to be on the same server as Photon.

It expects an environment variable, `PHOTON_TOKEN`, to match that which is configured in `plugin.config` above, so as to
authenticate requests from the Photon server. A sample Docker compose file could look like:

```yaml
version: "3.4"

services:
  photonstatemanager:
    hostname: photonstatemanager
    image: sapiensanatis/dragalia-api-statemanager:latest
    ports:
      - "3000:80"
    environment:
      PHOTON_TOKEN: yourtoken

  redis:
    hostname: redis
    image: redis/redis-stack-server
```

In `appsettings.json` configure the following values:

- `$.SeqOptions`: set up your Seq logging config, or leave it disabled if you don't want to log to Seq.
- `$.ConnectionStrings.Redis`: set up your Redis Stack connection string. The default will suffice if using
  docker-compose with the default networking.
- `$.RedisOptions.KeyExpiryTimeMins`: this can be changed to control the time after which rooms naturally expire in
  Redis.

### Part 3: Configuring the main API server

In the main API `appsettings.json`, configure the following values in `$.PhotonOptions`:

- `ServerUrl`: the Photon server URL. Must end with :5055 due to Dragalia using a legacy client.
- `StateManagerUrl` the Photon state manager URL.

Configure the following environment variables:

- `PHOTON_TOKEN`: the same photon token as the plugin and state manager use. Used to authenticate requests from Photon.

#### Other servers

If you are not using DragaliaAPI / Dawnshard as your main API server, you will need to implement the following
endpoints:

- `/heroparam/batch`: Get party information based on the list of viewer IDs and party numbers in the request body.
  See [HeroParamService.cs](https://github.com/SapiensAnatis/Dawnshard/blob/develop/DragaliaAPI/Services/Photon/HeroParamService.cs).
- `/matching/get_room_list`: Get a list of all open rooms. Typically can call `/get/gamelist` on the state manager.
  - Additional work must be done by the API server to populate fields such as host lead character.
    See [MatchingService.cs](https://github.com/SapiensAnatis/Dawnshard/blob/develop/DragaliaAPI/Services/Photon/MatchingService.cs).
- `/matching/get_room_list_by_quest_id`: Get a list of open rooms for a quest. Can call `/get/gamelist?questId=XXXXX` on
  the state manager.
- `/matching/get_room_name`: Get a private room based on the passcode. Can call `/get/byid/{id}` on the state manager.

Additionally:

- You must set `is_host` correctly in `/dungeon_start/start_multi`, `/dungeon/fail`, and `/dungeon_record/record_multi`.
  The state manager provides an endpoint: `GET /get/ishost/<viewerid>` returning a response body of `true` or `false` (
  JSON scalar) to help with this.
- `/dungeon_record/record_multi` must be able to accept requests from the Photon server. The Photon server will provide
  an `Authorization Bearer <PHOTON_TOKEN>` header, as well as `Auth-ViewerId` for authentication.
