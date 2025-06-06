﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using mxor.shared;
using mxor.world.Structures;

namespace mxor
{
    public partial class WorldThreads
    {
        public void ViewVisibleThread()
        {
            Output.WriteLine("[WORLD SERVER]View Visible Thread started");
            while (true)
            {
                ArrayList deadPlayers = new ArrayList();

                // Clean 
                lock (WorldServer.Clients)
                {
                    foreach (string clientKey in WorldServer.Clients.Keys)
                    {
                        // Collect dead players to arraylist
                        WorldClient thisclient = WorldServer.Clients[clientKey] as WorldClient;

                        if (thisclient.Alive == false)
                        {
                            // Add dead Player to the List - we need later to clear them
                            deadPlayers.Add(clientKey);
                        }
                    }

                    CleanDeadPlayers(deadPlayers);
                }

                CheckPlayerViews();
                CheckPlayerMobViews();
                CheckForStaticSubways();
                CheckForStaticObjectsViewsInRange();
                Thread.Sleep(500);
            }
        }

        private static void CheckForServerEntites()
        {
            // This can later replace ALL Methods 
            lock (WorldServer.gameServerEntities)
            {
                foreach (var serverEntity in WorldServer.gameServerEntities)
                {
                    lock (WorldServer.Clients)
                    {
                        foreach (var clientKey in WorldServer.Clients.Keys)
                        {
                            WorldClient thisclient = WorldServer.Clients[clientKey] as WorldClient;
                            if (thisclient != null)
                            {
                                // ToDo: Server Entity doesnt match a real rule currently so we need a class or something
                                //ClientView clientEntityView = thisclient.viewMan.getViewForEntityAndGo(serverEntity, NumericalUtils.ByteArrayToUint16(thismob.getGoId(), 1));
                            }
                        }
                    }
                }
            }
        }

        public static void CheckForStaticSubways()
        {
            int subwayCount = WorldServer.subways.Count;
            for (int i = 0; i < subwayCount; i++)
            {
                Subway thisSubway = (Subway) WorldServer.subways[i];

                lock (WorldServer.Clients)
                {
                    foreach (string clientKey in WorldServer.Clients.Keys)
                    {
                        // Loop through all clients
                        WorldClient thisclient = WorldServer.Clients[clientKey] as WorldClient;

                        if (thisclient.Alive)
                        {
                            // Check if

                            if (thisclient.playerData.getOnWorld() == true &&
                                thisclient.playerData.waitForRPCShutDown == false)
                            {
                                double playerX = 0;
                                double playerY = 0;
                                double playerZ = 0;
                                NumericalUtils.LtVector3dToDoubles(thisclient.playerInstance.Position.getValue(),
                                    ref playerX, ref playerY, ref playerZ);
                                Maths mathUtils = new Maths();
                                bool objectInCircle = mathUtils.IsInCircle((float) playerX, (float) playerZ,
                                    (float) thisSubway.worldObject.pos_x, (float) thisSubway.worldObject.pos_z, 5000);

                                // EntityHackString
                                String entityHackString =
                                    "" + thisSubway.worldObject.metrId + "" + thisSubway.worldObject.mxoStaticId;
                                UInt64 entityStaticId = UInt64.Parse(entityHackString);

                                ClientView view = Store.currentClient.viewMan.GetViewForEntityAndGo(entityStaticId,
                                    NumericalUtils.ByteArrayToUint16(thisSubway.worldObject.type, 1));
                                
                                if (!view.viewCreated &&
                                    thisSubway.worldObject.metrId == thisclient.playerData.getDistrictId() &&
                                    thisclient.playerData.getOnWorld() &&
                                    objectInCircle)
                                {
                                    ServerPackets pak = new ServerPackets();
                                    pak.SendSpawnGameObject(thisclient, thisSubway.gameObjectData, entityStaticId);
                                    view.spawnId = thisclient.playerData.spawnViewUpdateCounter;
                                    view.viewCreated = true;
                                }


                                // Delete SubwayView 
                                if (view.viewCreated && !objectInCircle &&
                                    thisSubway.worldObject.metrId == thisclient.playerData.getDistrictId())
                                {
                                    ServerPackets packets = new ServerPackets();
                                    packets.sendDeleteViewPacket(thisclient, view.ViewID);
                                    thisclient.viewMan.removeViewByViewId(view.ViewID);
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CheckForStaticObjectsViewsInRange()
        {
            Maths mathUtils = new Maths();
            // Used to spawn static ObjectViews in Range and handle them (Signpost, Collectors)
            lock (WorldServer.Clients)
            {
                foreach (KeyValuePair<string, WorldClient> client in WorldServer.Clients)
                {
                    WorldClient thisclient = client.Value;
                    // Loop through all clients
                    if (thisclient.Alive)
                    {
                        // Check if
                        double playerX = 0;
                        double playerY = 0;
                        double playerZ = 0;
                        NumericalUtils.LtVector3dToDoubles(thisclient.playerInstance.Position.getValue(), ref playerX,
                            ref playerY, ref playerZ);
                        
                        
                        if (thisclient.playerData.getOnWorld() &&
                            thisclient.playerData.waitForRPCShutDown == false)
                        {
                            // Get Objects in Range
                            var objects = from staticWorldObject in DataLoader.getInstance().WorldObjectsDB
                                where mathUtils.IsInCircle((float) playerX, (float) playerZ,
                                    (float) staticWorldObject.pos_x, (float) staticWorldObject.pos_z, 5000)
                                && staticWorldObject.metrId == thisclient.playerData.getDistrictId()
                                select staticWorldObject;

                            foreach (StaticWorldObject staticWorldObject in objects)
                            {
                                UInt16 typeId = NumericalUtils.ByteArrayToUint16(staticWorldObject.type, 1);
                                // WE get all staticObjects in range but we dont just want them all
                                switch (typeId)
                                {
                                    case 8400:
                                        var signPosts = from signPost in DataLoader.getInstance().Signposts
                                            where signPost.mxoStaticId == staticWorldObject.staticId
                                            select signPost;
                                        if (signPosts.Count() == 1)
                                        {
                                            String entityHackString =
                                                "" + staticWorldObject.metrId + "" + staticWorldObject.mxoStaticId;
                                            UInt64 entityStaticId = UInt64.Parse(entityHackString);

                                            ClientView view = Store.currentClient.viewMan.GetViewForEntityAndGo(entityStaticId,
                                                NumericalUtils.ByteArrayToUint16(staticWorldObject.type, 1));
                                
                                            if (!view.viewCreated && thisclient.playerData.getOnWorld())
                                            {

                                                // ToDo: Refaktor ? 
                                                /*
                                                ServerPackets pak = new ServerPackets();
                                                pak.SendSpawnStaticObject(thisclient, thisSubway.gameObjectData, entityStaticId);
                                                view.spawnId = thisclient.playerData.spawnViewUpdateCounter;
                                                view.viewCreated = true;
                                                */
                                            }

                                        }
                                        break;
                                }
                            }
                            
                        }
                    }
                }
            }
        }

        private static void CheckPlayerMobViews()
        {
            // Spawn/Update for mobs
            int npcCount = WorldServer.mobs.Count;
            for (int i = 0; i < npcCount; i++)
            {
                Mob thismob = (Mob) WorldServer.mobs[i];

                lock (WorldServer.Clients)
                {
                    foreach (string clientKey in WorldServer.Clients.Keys)
                    {
                        // Loop through all clients
                        WorldClient thisclient = WorldServer.Clients[clientKey] as WorldClient;

                        if (thisclient.Alive == true)
                        {
                            // Check if

                            if (thisclient.playerData.getOnWorld() == true &&
                                thisclient.playerData.waitForRPCShutDown == false)
                            {
                                Maths math = new Maths();
                                double playerX = 0;
                                double playerY = 0;
                                double playerZ = 0;
                                NumericalUtils.LtVector3dToDoubles(thisclient.playerInstance.Position.getValue(),
                                    ref playerX, ref playerY, ref playerZ);
                                Maths mathUtils = new Maths();
                                bool mobIsInCircle = mathUtils.IsInCircle((float) playerX, (float) playerZ,
                                    (float) thismob.getXPos(), (float) thismob.getZPos(), 5000);

                                // Spawn Mob if its in Visibility Range
                                ClientView mobView = thisclient.viewMan.GetViewForEntityAndGo(thismob.getEntityId(),
                                    NumericalUtils.ByteArrayToUint16(thismob.getGoId(), 1));
                                if (mobView.viewCreated == false &&
                                    thismob.getDistrict() == thisclient.playerData.getDistrictId() &&
                                    thisclient.playerData.getOnWorld() && mobIsInCircle)
                                {
#if DEBUG
                                    ServerPackets pak = new ServerPackets();
                                    pak.sendSystemChatMessage(thisclient,
                                        "Mob with Name " + thismob.getName() + " with new View ID " + mobView.ViewID +
                                        " spawned", "BROADCAST");
#endif

                                    ServerPackets mobPak = new ServerPackets();
                                    mobPak.SpawnMobView(thisclient, thismob, mobView);
                                    mobView.spawnId = thisclient.playerData.spawnViewUpdateCounter;
                                    mobView.viewCreated = true;
                                    thismob.isUpdateable = true;
                                    thismob.DoMobUpdate(thismob);
                                }

                                // Delete Mob's View from Client if we are outside
                                if (mobView.viewCreated == true && !mobIsInCircle &&
                                    thismob.getDistrict() == thisclient.playerData.getDistrictId())
                                {
                                    // ToDo: delete mob
                                    ServerPackets packets = new ServerPackets();
                                    packets.sendDeleteViewPacket(thisclient, mobView.ViewID);
#if DEBUG
                                    packets.sendSystemChatMessage(thisclient,
                                        "MobView (" + thismob.getName() + " LVL: " + thismob.getLevel() +
                                        " ) with View ID " + mobView.ViewID + " is out of range and is deleted!",
                                        "MODAL");
#endif
                                    thisclient.viewMan.removeViewByViewId(mobView.ViewID);
                                    thismob.isUpdateable = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void CheckPlayerViews()
        {
            // Check for Player Views
            lock (WorldServer.Clients)
            {
                foreach (string clientKey in WorldServer.Clients.Keys)
                {
                    // get Current client
                    WorldClient currentClient = WorldServer.Clients[clientKey] as WorldClient;
                    // Loop all Clients and check if we need to create a view for it
                    foreach (string clientOtherKey in WorldServer.Clients.Keys)
                    {
                        WorldClient otherClient = WorldServer.Clients[clientOtherKey] as WorldClient;
                        if (otherClient != currentClient)
                        {
                            ClientView clientView = currentClient.viewMan.GetViewForEntityAndGo(
                                otherClient.playerData.getEntityId(),
                                NumericalUtils.ByteArrayToUint16(otherClient.playerInstance.GetGoid(), 1));

                            // Create
                            Maths math = new Maths();
                            double currentPlayerX = 0;
                            double currentPlayerY = 0;
                            double currentPlayerZ = 0;

                            double otherPlayerX = 0;
                            double otherPlayerY = 0;
                            double otherPlayerZ = 0;

                            NumericalUtils.LtVector3dToDoubles(currentClient.playerInstance.Position.getValue(),
                                ref currentPlayerX, ref currentPlayerY, ref currentPlayerZ);
                            NumericalUtils.LtVector3dToDoubles(currentClient.playerInstance.Position.getValue(),
                                ref otherPlayerX, ref otherPlayerY, ref otherPlayerZ);
                            Maths mathUtils = new Maths();
                            bool playerIsInCircle = mathUtils.IsInCircle((float) currentPlayerX, (float) currentPlayerZ,
                                (float) otherPlayerX, (float) otherPlayerZ, 5000);
                            if (clientView.viewCreated == false &&
                                currentClient.playerData.getDistrictId() == otherClient.playerData.getDistrictId() &&
                                otherClient.playerData.getOnWorld() && currentClient.playerData.getOnWorld() &&
                                playerIsInCircle)
                            {
                                // Spawn player
                                ServerPackets pak = new ServerPackets();
                                pak.sendSystemChatMessage(currentClient,
                                    "Player " + StringUtils.charBytesToString_NZ(otherClient.playerInstance
                                        .CharacterName.getValue()) + " with new View ID " +
                                    clientView.ViewID + " jacked in", "BROADCAST");
                                pak.SendPlayerSpawn(currentClient, otherClient, clientView.ViewID);
                                clientView.spawnId = currentClient.playerData.spawnViewUpdateCounter;
                                clientView.viewCreated = true;
                            }


                            if (clientView.viewCreated && !playerIsInCircle)
                            {
                                ServerPackets packets = new ServerPackets();
                                packets.sendSystemChatMessage(currentClient,
                                    "Player " + StringUtils.charBytesToString_NZ(otherClient.playerInstance
                                        .CharacterName.getValue()) + " with View ID " + clientView.ViewID +
                                    " jacked out!", "MODAL");
                                packets.sendDeleteViewPacket(currentClient, clientView.ViewID);
                                currentClient.viewMan.removeViewByViewId(clientView.ViewID);
                            }
                        }
                    }
                }
            }
        }

        private static void CleanDeadPlayers(ArrayList deadPlayers)
        {
            foreach (string key in deadPlayers)
            {
                WorldClient deadClient = WorldServer.Clients[key] as WorldClient;
                foreach (string client in WorldServer.Clients.Keys)
                {
                    WorldClient otherclient = WorldServer.Clients[client] as WorldClient;
                    ClientView view = otherclient.viewMan.GetViewForEntityAndGo(deadClient.playerData.getEntityId(),
                        NumericalUtils.ByteArrayToUint16(deadClient.playerInstance.GetGoid(), 1));

                    Store.dbManager.WorldDbHandler.SetOnlineStatus(otherclient.playerData.getCharID(), 0);
                    ServerPackets pak = new ServerPackets();
                    pak.sendDeleteViewPacket(otherclient, view.ViewID);
                    Store.margin.RemoveClientsByCharId(otherclient.playerData.getCharID());
                }

                string handle = StringUtils.charBytesToString_NZ(deadClient.playerInstance.CharacterName.getValue());
                new BuddylistHandler().ProcessAnnounceFriendsOffline(deadClient.playerData.getCharID(), handle);
                // Views are now deleted to other players
                // ToDo: Cleanup Missions (kill all running missions the player have)
                // ToDo: Cleanup Teams (if your mission team has more than one player, you need to announce an update for the mission team to your mates)
                // ToDo: Announce friendlists from other users that you are going offline (just collect all players whohave this client in list and send the packet)
                // ToDo: Finally save the current character Data to the Database^^
                Output.WriteLine("Removed inactive Client with Key " + key);
                lock (WorldServer.Clients)
                {
                    WorldServer.Clients.Remove(key);
                }
            }
        }
    }
}