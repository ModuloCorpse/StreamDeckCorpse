﻿using CorpseLib.DataNotation;
using CorpseLib.Json;
using CorpseLib.Network;
using CorpseLib.Web;
using CorpseLib.Web.Http;

namespace StreamDeckCorpse
{
    public class StreamDeckProtocol(string uuid, string registerEvent, DataObject info) : WebSocketProtocol
    {
        public static StreamDeckProtocol? NewConnection(string[] args)
        {
            int port = -1;
            string pluginUUID = string.Empty;
            string registerEvent = string.Empty;
            string info = string.Empty;
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-port":
                    {
                        port = int.Parse(args[++i]);
                        break;
                    }
                    case "-pluginUUID":
                    {
                        pluginUUID = args[++i];
                        break;
                    }
                    case "-registerEvent":
                    {
                        registerEvent = args[++i];
                        break;
                    }
                    case "-info":
                    {
                        info = args[++i];
                        break;
                    }
                }
            }
            if (port == -1 ||
                string.IsNullOrEmpty(pluginUUID) ||
                string.IsNullOrEmpty(registerEvent) ||
                string.IsNullOrEmpty(info))
                return null;
            return NewConnection(port, pluginUUID, registerEvent, JsonParser.Parse(info));
        }

        public static StreamDeckProtocol? NewConnection(int port, string pluginUUID, string registerEvent, DataObject info)
        {
            StreamDeckProtocol protocol = new(pluginUUID, registerEvent, info);
            TCPAsyncClient twitchIRCClient = new(protocol, URI.Build("ws").Host("localhost").Port(port).Build());
            twitchIRCClient.Start();
            return protocol;
        }


        private readonly DataObject m_Info = info;
        private readonly string m_UUID = uuid;
        private readonly string m_RegisterEvent = registerEvent;

        protected override void OnWSOpen(Response message)
        {
            DataObject obj = new()
            {
                { "event", m_RegisterEvent },
                { "uuid", m_UUID }
            };
            Send(JsonParser.NetStr(obj));
        }

        protected override void OnWSMessage(string message)
        {
            DataObject receivedEvent = JsonParser.Parse(message);
            if (receivedEvent.TryGet("action", out string? actionID) &&
                receivedEvent.TryGet("event", out string? @event) &&
                receivedEvent.TryGet("context", out string? context) &&
                receivedEvent.TryGet("device", out string? device) &&
                receivedEvent.TryGet("payload", out DataObject? payload) &&
                payload!.TryGet("settings", out DataObject? settings) &&
                payload.TryGet("coordinates", out DataObject? coordinates) &&
                coordinates!.TryGet("column", out int? column) &&
                coordinates.TryGet("row", out int? row) &&
                payload.TryGet("isInMultiAction", out bool? isInMultiAction))
            {

            }
        }
    }
}
