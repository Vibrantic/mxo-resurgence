﻿using System;
using System.Collections;
using System.Data;

using mxor.databases.interfaces;
using mxor.shared;
using System.Collections.Generic;
using mxor.world.Structures;
using MySql.Data.MySqlClient;

namespace mxor.databases
{

    public class MyWorldDbAccess : IWorldDBHandler
    {

        private IDbConnection conn;
        private IDbCommand queryExecuter;
        private IDataReader dr;

        public MyWorldDbAccess()
        {
            var config = Store.config;
            /* Params: Host, port, database, user, password */
            conn = new MySqlConnection("Server=" + config.DBParams.Host + ";" + "Database=" + config.DBParams.DatabaseName + ";" + "User ID=" + config.DBParams.Username + ";" + "Password=" + config.DBParams.Password + ";" + "Pooling=false;");
        }

        public void OpenConnection()
        {
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        public void CloseConnection()
        {
            if (conn.State != ConnectionState.Closed)
            {
                conn.Close();
            }
        }

        public void ExecuteNonResultQuery(string query)
        {
            OpenConnection();
            string updateQuery = query;
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = updateQuery;
            queryExecuter.ExecuteNonQuery();
            CloseConnection();
        }


        public uint GetUserIdForCharId(byte[] charIdHex)
        {
            OpenConnection();
            uint charId = NumericalUtils.ByteArrayToUint32(charIdHex, 1);
            Output.OptWriteLine("[WORLD] Checking from DB:" + charId);
            string sqlQuery = "Select userid from characters where charid ='" + charId + "'";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            uint userId = new uint();

            while (dr.Read())
            {
                userId = (uint)dr.GetDecimal(0);
            }

            dr.Close();
            CloseConnection();
            return userId;

        }

        public void AddHandleToFriendList(string handleToAdd, uint charId)
        {
            uint friendId = GetCharIdByHandle(handleToAdd);
            ExecuteNonResultQuery("INSERT INTO buddylist SET charId='" + charId +
                                  "', friendId='" + friendId + "', is_ignored=0 ");

        }

        public void RemoveHandleFromFriendList(string handleToRemove, uint charId)
        {
            uint friendId = GetCharIdByHandle(handleToRemove);
            ExecuteNonResultQuery(
                "DELETE FROM buddylist WHERE charId='" + charId + "' AND friendId='" + friendId + "' ");

        }

        public ArrayList FetchPlayersWhoAddedMeToBuddylist(uint charId)
        {
            OpenConnection();
            ArrayList friends = new ArrayList();
            string query = "SELECT C.handle, C.charId, C.is_online, B.friendId FROM buddylist B LEFT JOIN characters C ON B.charId=C.charId WHERE B.friendId = '" + charId + "' ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                Hashtable data = new Hashtable
                {
                    { "handle", dr.GetString(0) },
                    { "online", dr.GetInt16(1) }
                };
                friends.Add(data);
            }

            dr.Close();
            CloseConnection();
            return friends;
        }

        public ArrayList FetchFriendList(uint charId)
        {
            OpenConnection();
            ArrayList friends = new ArrayList();
            string query = "SELECT C.handle, C.is_online, B.friendId FROM buddylist B LEFT JOIN characters C ON B.friendId=C.charId WHERE B.charId = '" + charId + "' ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                Hashtable data = new Hashtable
                {
                    { "handle", dr.GetString(0) },
                    { "online", dr.GetInt16(1) }
                };
                friends.Add(data);
            }

            dr.Close();
            CloseConnection();
            return friends;

        }


        public Faction FetchFaction(uint factionId)
        {
            Faction factionData = new Faction();
            OpenConnection();
            string query = "SELECT f.id, f.name, f.master_player_handle, f.money FROM factions f LEFT JOIN characters c ON f.id=c.factionId WHERE f.id= '" + factionId + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                factionData.factionId = (uint)dr.GetInt32(0);
                factionData.name = dr.GetString(1);
                factionData.masterPlayerHandle = dr.GetString(2);
                factionData.money = (uint)dr.GetInt32(3);
            }

            dr.Close();
            CloseConnection();

            factionData.crews = GetCrewsForFaction(factionData.factionId);
            return factionData;
        }

        public bool IsFactionnameAvailable(string factionname)
        {
            bool isFactionNameAvailable = true;
            OpenConnection();
            string query = "SELECT id FROM factions WHERE name = '" + factionname.Trim() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                // if we have just one entry - we have found a faction
                isFactionNameAvailable = false;
            }
            CloseConnection();
            return isFactionNameAvailable;
        }

        public bool IsHandleCaptainOfACrew(string handle)
        {
            bool isCaptainOfCrew = false;
            OpenConnection();
            string query = "SELECT id FROM crews WHERE master_player_handle = '" + handle.Trim() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                // if we have just one entry - we have found a faction
                isCaptainOfCrew = true;
            }
            CloseConnection();
            return isCaptainOfCrew;
        }

        public uint CreateFaction(string factionname, Crew masterCrew, Crew secondCrew)
        {
            ExecuteNonResultQuery("INSERT INTO factions SET name='" + factionname.Trim() + "', master_player_handle='" + masterCrew.characterMasterName + "', money=0, created_at=NOW() ");

            OpenConnection();
            string query = "SELECT id FROM factions WHERE name='" + factionname.Trim() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();

            uint factionId = 0;
            while (dr.Read())
            {
                factionId = (uint)dr.GetInt32(0);
            }
            CloseConnection();

            if (factionId > 0)
            {
                ExecuteNonResultQuery("UPDATE crews SET faction_id= '" + factionId +
                                      "' WHERE master_player_handle = '" + masterCrew.characterMasterName +
                                      "' LIMIT 1");
                ExecuteNonResultQuery("UPDATE crews SET faction_id= '" + factionId +
                                      "' WHERE master_player_handle = '" + secondCrew.characterMasterName +
                                      "' LIMIT 1");

                OpenConnection();
                string crewIdsQuery = "SELECT id FROM crews WHERE faction_id='" + factionId + "' ";
                queryExecuter = conn.CreateCommand();
                queryExecuter.CommandText = crewIdsQuery;
                dr = queryExecuter.ExecuteReader();


                List<uint> crewIds = new List<uint>();
                while (dr.Read())
                {
                    crewIds.Add((uint)dr.GetInt32(0));

                }
                CloseConnection();

                foreach (uint crewId in crewIds)
                {
                    ExecuteNonResultQuery("UPDATE characters SET factionId = '" + factionId + "' WHERE crewId = '" + crewId + "' ");
                }
            }

            return factionId;
        }

        public void IncreaseCrewMoney(uint crewId, uint amount)
        {
            ExecuteNonResultQuery("UPDATE crews SET money = money + '" + amount + "' WHERE id = '" + crewId.ToString() + "' LIMIT 1");

        }

        public void DecreaseCrewMoney(uint crewId, uint amount)
        {
            ExecuteNonResultQuery("UPDATE crews SET money = money - '" + amount + "' WHERE id = '" + crewId.ToString() +
                                  "' LIMIT 1");

        }

        public void IncreaseFactionMoney(uint crewId, uint amount)
        {
            ExecuteNonResultQuery("UPDATE factions SET money = money + '" + amount + "' WHERE id = '" +
                                  crewId.ToString() + "' LIMIT 1");
        }

        public void DecreaseFactionMoney(uint crewId, uint amount)
        {
            ExecuteNonResultQuery("UPDATE factions SET money = money - '" + amount + "' WHERE id = '" +
                                  crewId.ToString() + "' LIMIT 1");
        }

        public Crew GetCrewData(uint crewId)
        {
            Crew theCrew = new Crew();
            OpenConnection();
            string query = "SELECT id, crew_name, master_player_handle, money, org, faction_id, faction_rank FROM crews WHERE id='" +
                           crewId + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                theCrew.crewId = (uint)dr.GetInt32(0);
                theCrew.crewName = dr.GetString(1);
                theCrew.characterMasterName = dr.GetString(2);
                theCrew.money = (uint)dr.GetInt32(3);
                theCrew.org = (ushort)dr.GetInt16(4);
                theCrew.factionId = (uint)dr.GetInt32(5);
                theCrew.factionRank = (ushort)dr.GetInt32(6);
            }

            dr.Close();
            CloseConnection();
            return theCrew;
        }

        public List<Crew> GetCrewsForFaction(uint factionID)
        {
            List<Crew> tmpCrews = new List<Crew>();

            OpenConnection();
            string query =
                "SELECT cr.id, cr.crew_name, cr.master_player_handle, cr.faction_id, cr.faction_rank, c.is_online FROM crews cr LEFT JOIN characters c ON cr.master_player_handle=c.handle WHERE cr.faction_id = '" +
                factionID + "' ";

            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();
            while (dr.Read())
            {
                Crew theCrew = new Crew
                {
                    crewId = (uint)dr.GetInt32(0),
                    crewName = dr.GetString(1),
                    characterMasterName = dr.GetString(2),
                    factionId = (uint)dr.GetInt32(3),
                    factionRank = (ushort)dr.GetInt16(4),
                    masterIsOnline = (ushort)dr.GetInt16(5)
                };
                tmpCrews.Add(theCrew);
            }
            dr.Close();
            CloseConnection();

            List<Crew> crews = new List<Crew>();
            foreach (Crew theCrew in tmpCrews)
            {
                theCrew.masterPlayerCharId = GetCharIdByHandle(theCrew.characterMasterName);
                crews.Add(theCrew);
            }

            return crews;
        }

        public List<CrewMember> GetCrewMembersForCrewId(uint crewId)
        {
            List<CrewMember> crewMembers = new List<CrewMember>();
            OpenConnection();
            string query =
                "SELECT cm.char_id, cm.is_captain, cm.is_first_mate, c.handle, c.is_online FROM crew_members cm LEFT JOIN characters c ON cm.char_id=c.charId WHERE cm.crew_id = '" + crewId + "' ";

            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = query;
            dr = queryExecuter.ExecuteReader();
            while (dr.Read())
            {
                uint charId = (uint)dr.GetInt32(0);
                bool isCaptain = dr.GetBoolean(1);
                bool isFirstMate = dr.GetBoolean(2);
                string handle = dr.GetString(3);
                ushort isOnline = (ushort)dr.GetInt16(4);

                CrewMember member = new CrewMember
                {
                    charId = charId,
                    handle = handle,
                    isCaptain = isCaptain,
                    isFirstMate = isFirstMate,
                    isOnline = isOnline
                };
                crewMembers.Add(member);
            }
            CloseConnection();

            return crewMembers;
        }

        public void AddMemberToCrew(uint charId, uint crewId, uint factionId, ushort isCaptain, ushort isFirstMate)
        {
            ExecuteNonResultQuery("INSERT INTO crew_members SET char_id='" + charId + "', crew_id='" + crewId + "', is_first_mate=" + isFirstMate + ", is_captain=" + isCaptain + ", created_at=NOW() ");
            ExecuteNonResultQuery("UPDATE characters SET crewId='" + crewId + "', factionId='" + factionId + "' WHERE charId='" + charId + "' ");
        }

        public void RemoveMemberFromCrew(uint charId, uint crewId)
        {
            ExecuteNonResultQuery("DELETE FROM crew_members WHERE crew_id = '" + crewId + "' AND char_id ='" + charId + "' LIMIT 1");
            ExecuteNonResultQuery("UPDATE characters SET crewId = 0, factionId = 0 WHERE charId='" + charId + "' ");
        }

        public void AddCrewToFaction(uint factionId, uint crewId)
        {
            ExecuteNonResultQuery("UPDATE crews SET faction_id = '" + factionId + "' WHERE id='" + crewId + "' ");
        }

        public void RemoveCrewFromFaction(uint factionId, uint crewId)
        {
            ExecuteNonResultQuery("UPDATE crews SET faction_id = '0' WHERE id='" + crewId + "' AND faction_id = '" + factionId + "' ");
        }


        public string GetPathForDistrictKey(string key)
        {
            OpenConnection();
            string sqlQuery = "select path from districts where districts.key = '" + key + "'";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            string path = "";

            while (dr.Read())
            {
                path = dr.GetString(0);
            }

            dr.Close();
            CloseConnection();
            return path;

        }


        public void UupdateSourceHlForObjectTracking(ushort sourceDistrict, ushort sourceHl, uint lastObjectId)
        {
            OpenConnection();
            string sqlQuery = "SELECT id,HardlineId, DistrictId, objectId FROM data_hardlines WHERE DistrictId = '" + sourceDistrict.ToString() + "' AND HardlineId = '" + sourceHl.ToString() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            ushort theId = 0;
            ushort hardlineId = 0;
            ushort districtId = 0;
            uint objectId = 0;

            while (dr.Read())
            {
                theId = (ushort)dr.GetInt16(0);
                hardlineId = (ushort)dr.GetInt16(1);
                districtId = (ushort)dr.GetInt16(2);
                objectId = (uint)dr.GetInt32(3);


            }

            dr.Close();
            CloseConnection();

            if (objectId == 0 || objectId != lastObjectId)
            {
                ExecuteNonResultQuery("UPDATE data_hardlines SET objectId = '" + lastObjectId.ToString() +
                                      "' WHERE id = '" + theId.ToString() + "' LIMIT 1");
#if DEBUG
                Output.WriteLine("[WORLD DB] UPDATE Hardline " + hardlineId.ToString() + " in District " + districtId.ToString() + " with Object ID : " + lastObjectId.ToString());
#endif
            }


        }

        public void UpdateLocationByHL(ushort district, ushort hardline)
        {
            OpenConnection();
            string sqlQuery = "SELECT DH.X,DH.Y,DH.Z,DH.ROT,DIS.key,DH.DistrictId FROM data_hardlines AS DH, districts as DIS WHERE DH.DistrictId = '" + district.ToString() + "' AND DH.HardLineId = '" + hardline.ToString() + "' AND DH.DistrictId=DIS.id ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                double x = (double)dr.GetFloat(0);
                double y = (double)dr.GetFloat(1);
                double z = (double)dr.GetFloat(2);
                string disKey = dr.GetString(4);
                Store.currentClient.playerData.setDistrict(disKey);
                Store.currentClient.playerData.setDistrictId((uint)dr.GetInt16(5));
                Store.currentClient.playerInstance.Position.setValue(NumericalUtils.doublesToLtVector3d(x, y, z));
                //Store.currentClient.playerInstance.YawInterval.setValue((byte)dr.GetDecimal(3));
            }
            dr.Close();
            CloseConnection();
            SavePlayer(Store.currentClient);
        }

        public void SetBackground(string backgroundText)
        {
            uint charID = Store.currentClient.playerData.getCharID();

            ExecuteNonResultQuery("UPDATE characters SET background = '" + backgroundText + "' WHERE charId = '" +
                                  charID + "' LIMIT 1");

        }

        public void SetPlayerValues()
        {
            OpenConnection();
            uint charID = Store.currentClient.playerData.getCharID();
            string sqlQuery = "Select handle,x,y,z,rotation,healthC,healthM,innerStrC,innerStrM,level,profession,alignment,pvpflag,firstName,lastName,exp,cash,district,districtId,factionId,crewId, repMero, repMachine, repNiobe, repEPN, repCYPH, repGM, repZion from characters where charId='" + charID + "'";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {

                Store.currentClient.playerInstance.CharacterID.setValue(charID);
                Store.currentClient.playerInstance.CharacterName.setValue(dr.GetString(0));

                double x = dr.GetDouble(1);
                double y = dr.GetDouble(2);
                double z = dr.GetDouble(3);

                Store.currentClient.playerInstance.Position.setValue(NumericalUtils.doublesToLtVector3d(x, y, z));
                Store.currentClient.playerInstance.YawInterval.setValue((byte)dr.GetDecimal(4));
                Store.currentClient.playerInstance.Health.setValue((ushort)dr.GetDecimal(5));
                Store.currentClient.playerInstance.MaxHealth.setValue((ushort)dr.GetDecimal(6));

                Store.currentClient.playerInstance.InnerStrengthAvailable.setValue((ushort)dr.GetDecimal(7));
                Store.currentClient.playerInstance.InnerStrengthMax.setValue((ushort)dr.GetDecimal(8));

                Store.currentClient.playerInstance.Level.setValue((byte)dr.GetDecimal(9));

                Store.currentClient.playerInstance.TitleAbility.setValue((uint)dr.GetDecimal(10));
                Store.currentClient.playerInstance.OrganizationID.setValue((byte)dr.GetDecimal(11));

                //data.setPlayerValue("pvpFlag",(int)dr.GetDecimal(12));
                Store.currentClient.playerInstance.RealFirstName.setValue(dr.GetString(13));
                Store.currentClient.playerInstance.RealLastName.setValue(dr.GetString(14));

                Store.currentClient.playerData.setExperience((long)dr.GetDecimal(15));
                Store.currentClient.playerData.setInfo((long)dr.GetDecimal(16));
                Store.currentClient.playerData.setDistrict(dr.GetString(17));
                Store.currentClient.playerData.setDistrictId((uint)dr.GetInt16(18));



                uint factionId = (uint)dr.GetInt16(19);
                uint crewId = (uint)dr.GetInt16(20);

                Store.currentClient.playerInstance.FactionID.enable();
                Store.currentClient.playerInstance.FactionID.setValue(factionId);

                Store.currentClient.playerInstance.CrewID.enable();
                Store.currentClient.playerInstance.CrewID.setValue(crewId);

                Store.currentClient.playerInstance.ReputationMerovingian.setValue((ushort)dr.GetDecimal(21));
                Store.currentClient.playerInstance.ReputationMachines.setValue((ushort)dr.GetDecimal(22));
                Store.currentClient.playerInstance.ReputationNiobe.setValue((ushort)dr.GetDecimal(23));
                Store.currentClient.playerInstance.ReputationPluribusNeo.setValue((ushort)dr.GetDecimal(24));
                Store.currentClient.playerInstance.ReputationCypherites.setValue((ushort)dr.GetDecimal(25));
                Store.currentClient.playerInstance.ReputationGMOrganization.setValue((ushort)dr.GetDecimal(26));
                Store.currentClient.playerInstance.ReputationZionMilitary.setValue((ushort)dr.GetDecimal(27));


            }

            dr.Close();
            CloseConnection();

        }

        public uint GetCharIdByHandle(string handle)
        {
            OpenConnection();
            string sqlQuery = "SELECT charId FROM characters WHERE handle = '" + handle.Trim() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;

            dr = queryExecuter.ExecuteReader();

            uint charId = 0;
            while (dr.Read())
            {
                charId = (uint)dr.GetInt32(0);
            }

            dr.Close();
            CloseConnection();

            return charId;
        }

        public Hashtable GetCharInfo(uint charId)
        {
            OpenConnection();
            string sqlQuery = "SELECT firstName,lastName,background, district, repMero, repMachine, repNiobe, repEPN, repCYPH, repGM, repZion, exp, cash FROM characters WHERE charId = '" + charId.ToString() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;

            dr = queryExecuter.ExecuteReader();

            Hashtable data = new Hashtable();
            while (dr.Read())
            {
                data.Add("firstname", dr.GetString(0));
                data.Add("lastname", dr.GetString(1));
                data.Add("background", dr.GetString(2));
                data.Add("district", dr.GetString(3));
                data.Add("repMero", dr.GetInt16(4));
                data.Add("repMachine", dr.GetInt16(5));
                data.Add("repNiobe", dr.GetInt16(6));
                data.Add("repEPN", dr.GetInt16(7));
                data.Add("repCYPH", dr.GetInt16(8));
                data.Add("repGM", dr.GetInt16(9));
                data.Add("repZion", dr.GetInt16(10));
                data.Add("exp", dr.GetInt32(11));
                data.Add("cash", dr.GetInt32(12));
            }
            dr.Close();
            CloseConnection();

            return data;
        }

        public void SetReputation(uint charId, string reputationColumn, int value)
        {
            OpenConnection();
            string sqlQuery = "UPDATE characters SET " + reputationColumn + "=" + value + " WHERE charId= " + charId +
                              " LIMIT 1";
            ExecuteNonResultQuery(sqlQuery);

        }

        public void SetOrgId(uint charId, int orgId)
        {
            OpenConnection();
            string sqlQuery = "UPDATE characters SET alignment = " + orgId + " WHERE charId= " + charId + " LIMIT 1";
            ExecuteNonResultQuery(sqlQuery);
        }

        public Hashtable GetCharInfoByHandle(string handle)
        {
            OpenConnection();
            handle = handle.Substring(0, handle.Length - 1);
            string sqlQuery = "SELECT c.charId, c.firstName, c.lastName, c.background, c.alignment, c.conquest_points, f.name as faction_name, fc.crew_name as crew_name FROM characters c LEFT JOIN crews fc ON c.crewId=fc.id LEFT JOIN factions f ON c.factionId=f.id WHERE c.handle = '" + handle.Trim() + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;

            dr = queryExecuter.ExecuteReader();

            Hashtable data = new Hashtable();

            while (dr.Read())
            {
                data.Add("charId", (uint)dr.GetInt32(0));
                data.Add("firstname", dr.GetString(1));
                data.Add("lastname", dr.GetString(2));
                data.Add("background", dr.GetString(3));
                data.Add("alignment", (ushort)dr.GetInt16(4));
                data.Add("conquest_points", (uint)dr.GetInt32(5));
                if (!dr.IsDBNull(6))
                {
                    data.Add("faction_name", dr.GetString(6));
                }

                if (!dr.IsDBNull(7))
                {
                    data.Add("crew_name", dr.GetString(7));
                }

                data.Add("handle", handle);
            }

            dr.Close();
            CloseConnection();

            return data;
        }

        public void SetRsiValues()
        {
            OpenConnection();
            int charID = (int)Store.currentClient.playerData.getCharID();
            string sqlQuery = "Select sex,body,hat,face,shirt,coat,pants,shoes,gloves,glasses,hair,facialdetail,shirtcolor,pantscolor,coatcolor,shoecolor,glassescolor,haircolor,skintone,tattoo,facialdetailcolor,leggins from rsivalues where charId='" + charID + "'";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();
            int[] rsiValues = new int[22];

            while (dr.Read())
            {
                //we will read just one row
                for (int i = 0; i < rsiValues.Length; i++)
                {
                    int temp = (int)dr.GetDecimal(i); //int to int
                    rsiValues[i] = temp;
                }
            }

            Store.currentClient.playerData.setRsiValues(rsiValues);
            dr.Close();
            CloseConnection();
        }

        public void SetOnlineStatus(uint charId, ushort isOnline)
        {
            ExecuteNonResultQuery("UPDATE characters SET is_online = '" + isOnline + "' WHERE charid= '" + charId +
                                  "' LIMIT 1");

        }

        public void ResetOnlineStatus()
        {
            ExecuteNonResultQuery("UPDATE characters SET is_online = '0' ");
        }


        public void UpdateRsiPartValue(string part, uint value, uint charId)
        {
            ExecuteNonResultQuery("UPDATE rsivalues SET " + part + "=" + value + " WHERE charid= '" + charId + "' LIMIT 1");

        }

        public void SavePlayer(WorldClient client)
        {
            System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ".";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;

            uint charID = NumericalUtils.ByteArrayToUint32(client.playerInstance.CharacterID.getValue(), 1);
            string handle = StringUtils.charBytesToString_NZ(client.playerInstance.CharacterName.getValue());

            int[] rsiValues = client.playerData.getRsiValues();

            double x = 0; double y = 0; double z = 0;
            byte[] Ltvector3d = client.playerInstance.Position.getValue();
            NumericalUtils.LtVector3dToDoubles(Ltvector3d, ref x, ref y, ref z);

            int rotation = client.playerInstance.YawInterval.getValue()[0];

            string positionQuery = "update characters set x =" + x + " ,y=" + y + " ,z=" + z + " , rotation=" +
                                   rotation + ", districtId=" + client.playerData.getDistrictId() + " where handle='" +
                                   handle + "' ";
            ExecuteNonResultQuery(positionQuery);
            Output.WriteLine(StringUtils.bytesToString(StringUtils.stringToBytes(positionQuery)));
            Output.writeToLogForConsole("[WORLD DB ACCESS ]" + positionQuery);

            string rsiQuery = "update rsivalues set sex='" + rsiValues[0] + "',body='" + rsiValues[1] + "',hat='" +
                              rsiValues[2] + "',face='" + rsiValues[3] + "',shirt='" + rsiValues[4] + "',coat='" +
                              rsiValues[5] + "',pants='" + rsiValues[6] + "',shoes='" + rsiValues[7] +
                              "',gloves='" + rsiValues[8] + "',glasses='" + rsiValues[9] + "',hair='" +
                              rsiValues[10] + "',facialdetail='" + rsiValues[11] + "',shirtcolor='" +
                              rsiValues[12] + "',pantscolor='" + rsiValues[13] + "',coatcolor='" + rsiValues[14] +
                              "',shoecolor='" + rsiValues[15] + "',glassescolor='" + rsiValues[16] +
                              "',haircolor='" + rsiValues[17] + "',skintone='" + rsiValues[18] + "',tattoo='" +
                              rsiValues[19] + "',facialdetailcolor='" + rsiValues[20] + "',leggins='" +
                              rsiValues[21] + "' where charId='" + charID + "';";
            ExecuteNonResultQuery(rsiQuery);

            Output.writeToLogForConsole("[WORLD DB ACCESS ]" + rsiQuery);


        }


        public void UpdateInventorySlot(ushort sourceSlot, ushort destSlot, uint charId)
        {
            ExecuteNonResultQuery("UPDATE inventory SET slot = '" + destSlot.ToString() + "' WHERE slot = '" + sourceSlot.ToString() + "' AND charID = '" + charId + "' LIMIT 1");
        }

        public bool IsSlotinUseByItem(ushort slotId)
        {
            OpenConnection();
            bool isSlotInUse = false;
            uint charID = Store.currentClient.playerData.getCharID();
            string sqlQuery = "SELECT slot FROM inventory WHERE slot = '" + slotId.ToString() + "' AND charID = '" +
                              charID.ToString() + "' LIMIT 1";

            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            ushort freeSlot = 0;

            if (dr.Read())
            {
                isSlotInUse = true;
            }

            dr.Close();
            CloseConnection();
            return isSlotInUse;

        }

        public ushort GetCrewMemberCountByCrewName(string crewName)
        {
            OpenConnection();
            string sqlQuery = "SELECT COUNT(cm.id) as count_members FROM crew_members cm LEFT JOIN crews c ON cm.crew_id=c.id WHERE c.crew_name='" + crewName + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            ushort countCrewMembers = 0;

            if (dr.Read())
            {
                countCrewMembers = (ushort)dr.GetInt16(0);

            }

            dr.Close();
            CloseConnection();
            return countCrewMembers;
        }

        public uint GetCrewIdByCrewMasterHandle(string playerHandle)
        {
            // ToDo: we need to proove if this can work this way 
            // ToDo: can only the master invite other players ? i am not sure then we need to change this
            // ToDo: make DB easier
            OpenConnection();
            string sqlQuery = "SELECT c.id, c.crew_name FROM crew_members cm LEFT JOIN crews c ON cm.crew_id=c.id WHERE c.master_player_handle='" + playerHandle + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            ushort countCrewMembers = 0;

            if (dr.Read())
            {
                countCrewMembers = (ushort)dr.GetInt16(0);

            }

            dr.Close();
            CloseConnection();
            return countCrewMembers;
        }

        public uint GetCrewIdByInviterHandle(string playerHandle)
        {
            uint crewId = 0;
            OpenConnection();
            string sqlQuery = "SELECT crewId FROM characters WHERE handle ='" + playerHandle + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            string factionName = "";

            if (dr.Read())
            {

                crewId = (uint)dr.GetInt32(0);

            }
            dr.Close();
            CloseConnection();
            return crewId;
        }

        public string GetFactionNameById(uint factionId)
        {
            OpenConnection();
            string sqlQuery = "SELECT name FROM factions WHERE id =" + factionId + " LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            string factionName = "";

            if (dr.Read())
            {

                factionName = dr.GetString(0);

            }
            dr.Close();
            CloseConnection();
            return factionName;
        }

        public bool IsCrewNameAvailable(string crewName)
        {
            OpenConnection();
            bool isCrewNameAvailable = true;
            crewName = crewName.Replace("'", @"\'");

            string sqlQuery = "SELECT id,created_at,deleted_at FROM crews WHERE crew_name='" + crewName + "' AND deleted_at > NOW() LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            if (dr.Read())
            {
                ushort memberCount = GetCrewMemberCountByCrewName(crewName);

                uint crewId = (uint)dr.GetInt32(0);
                DateTime createdAt = dr.GetDateTime(1);
                DateTime deletedAt = dr.GetDateTime(2);

                isCrewNameAvailable = false;

                // Lets check if more than one player is in the crew and if created_at is greater than one day
                if ((DateTime.Now - deletedAt).TotalHours > 24 && memberCount < 2)
                {
                    DeleteCrew(crewId);
                    isCrewNameAvailable = true;
                }


            }

            dr.Close();
            CloseConnection();
            return isCrewNameAvailable;
        }

        public void DeleteCrew(uint crewId)
        {
            // Delete the Crew
            ExecuteNonResultQuery("UPDATE crews SET deleted_at = NOW() WHERE id=" + crewId + " LIMIT 1");

            // Delete the Player Crew ID
            ExecuteNonResultQuery("UPDATE characters SET crewId = 0 WHERE crewId = " + crewId);

            // Delete the Player Crew ID
            ExecuteNonResultQuery("DELETE FROM crew_members WHERE crew_id = " + crewId);
        }

        public void AddCrew(string crewName, string masterHandle)
        {
            crewName = crewName.Replace("'", @"\'");
            ExecuteNonResultQuery("INSERT INTO crews SET crew_name= '" + crewName + "', created_at =NOW() ");
        }

        public ushort GetFirstNewSlot()
        {
            OpenConnection();
            uint charID = Store.currentClient.playerData.getCharID();

            // We want the next free slot which is not in the "wearing" range (which starts with 97)
            string sqlQuery = "SELECT inv.slot +1 as freeSlot FROM inventory inv WHERE NOT EXISTS (SELECT * FROM inventory inv2 WHERE inv2.slot = inv.slot + 1 AND charID = '" + charID.ToString() + "') AND charID = '" + charID.ToString() + "' AND slot<97 LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            ushort freeSlot = 0;
            while (dr.Read())
            {
                freeSlot = (ushort)dr.GetInt16(0);
            }
            dr.Close();
            CloseConnection();
            return freeSlot;
        }

        public void AddItemToInventory(ushort slotId, uint itemGoID)
        {
            uint charID = Store.currentClient.playerData.getCharID();
            ExecuteNonResultQuery("INSERT INTO inventory SET charId = '" + charID.ToString() + "' , goid = '" +
                                  itemGoID + "', slot = '" + slotId.ToString() + "', created = NOW() ");
        }

        public uint GetItemGOIDAtInventorySlot(ushort slotId)
        {
            OpenConnection();
            uint charID = Store.currentClient.playerData.getCharID();

            string sqlQuery = "SELECT goid FROM inventory WHERE charID = '" + charID + "' AND slot = '" + slotId + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            uint GoID = 0;
            while (dr.Read())
            {
                GoID = (uint)dr.GetInt32(0);
            }
            dr.Close();
            CloseConnection();
            return GoID;
        }


        public void SaveInfo(WorldClient client, long cash)
        {
            ExecuteNonResultQuery("UPDATE characters SET cash =" + cash + " WHERE charId= " +
                                  client.playerData.getCharID().ToString() + " LIMIT 1");

        }

        public void UpdateAbilityLoadOut(List<ushort> abilitySlots, uint loaded)
        {
            uint charID = Store.currentClient.playerData.getCharID();
            string sqlQuery = "";
            foreach (ushort slot in abilitySlots)
            {
                sqlQuery += "UPDATE char_abilities SET is_loaded = " + loaded.ToString() + " WHERE char_id = " + charID.ToString() + " AND slot = " + slot.ToString() + ";";
            }

            ExecuteNonResultQuery(sqlQuery);
        }

        public ushort GetCurrentAbilityLevel(int abilityGoId, uint charId)
        {
            OpenConnection();

            string sqlQuery = "SELECT level FROM char_abilities WHERE char_id = '" + charId + "' AND ability_id = '" + abilityGoId + "' LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            ushort level = 0;
            while (dr.Read())
            {
                level = (ushort)dr.GetInt16(0);
            }
            dr.Close();
            CloseConnection();
            return level;
        }

        public ushort UpgradeAbilityLevel(int abilityGoID, uint level)
        {
            uint charID = Store.currentClient.playerData.getCharID();
            ushort currentLevel = GetCurrentAbilityLevel(abilityGoID, charID);
            ExecuteNonResultQuery("UPDATE char_abilities SET level = " + currentLevel + level + " WHERE char_id=" + charID + " AND ability_id=" + abilityGoID + " LIMIT 1");
            return (ushort)(currentLevel + level);
        }

        public void SaveExperience(WorldClient client, long exp)
        {
            ExecuteNonResultQuery("UPDATE characters SET exp =" + exp + " WHERE charId= " + client.playerData.getCharID().ToString() + " LIMIT 1");
        }

        public void UpdateSourceHlForObjectTracking(ushort sourceDistrict, ushort sourceHl, uint lastObjectId)
        {
            throw new NotImplementedException();
        }
    }
}

