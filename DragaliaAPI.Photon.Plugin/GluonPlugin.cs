using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Hive.Plugin;

namespace DragaliaAPI.Photon.Plugin
{
    public class GluonPlugin : PluginBase
    {
        private IPluginLogger logger;

        public override string Name => nameof(GluonPlugin);

        public override bool SetupInstance(
            IPluginHost host,
            Dictionary<string, string> config,
            out string errorMsg
        )
        {
            this.logger = host.CreateLogger(this.Name);
            return base.SetupInstance(host, config, out errorMsg);
        }

        public override void OnCreateGame(ICreateGameCallInfo info)
        {
            this.logger.InfoFormat(
                "---------------------------------- OnCreateGame {0} by user {1}",
                info.Request.GameId,
                info.UserId
            );
            this.logger.InfoFormat("------------------------------------ actor properties start");
            LogHashtable(info.Request.ActorProperties);
            this.logger.InfoFormat("------------------------------------ actor properties end");
            this.logger.InfoFormat("------------------------------------ game properties start");
            LogHashtable(info.Request.GameProperties);
            this.logger.InfoFormat("------------------------------------ game properties end");

            this.logger.InfoFormat("------------------------------------ unk properties start");
            LogHashtable(info.CreateOptions["CustomProperties"] as IDictionary);
            this.logger.InfoFormat("------------------------------------ unk properties end");

            info.Request.ActorProperties.Add("ViewerId", info.Request.ActorProperties["PlayerId"]);
            info.Request.GameProperties.Add("RoomId", 4201337);
            info.Continue();
        }

        private void LogHashtable(IDictionary table)
        {
            foreach (object key in table.Keys)
            {
                if (table[key] is byte[] roomData)
                {
                    this.logger.InfoFormat(
                        "---------------------------------- yoyoyo room data just dropped {0}",
                        Convert.ToBase64String(roomData)
                    );
                }
                else if (table[key] is string[] unk250)
                {
                    this.logger.InfoFormat(
                        "---------------------------------- yoyoyo game data just dropped {0}",
                        string.Join(",", unk250)
                    );
                }
                else
                {
                    this.logger.InfoFormat(
                        "---------------------------------- Name: {0}, value: {1}",
                        key,
                        table[key]
                    );
                }
            }
        }
    }
}
