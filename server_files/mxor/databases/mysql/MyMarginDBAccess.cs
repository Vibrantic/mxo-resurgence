using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using mxor.databases.Entities;
using MySql.Data.MySqlClient;

using mxor.databases.interfaces;
using mxor.shared;
using mxor.margin.Entities;

namespace mxor.databases
{

    public class MyMarginDBAccess : IMarginDBHandler
    {

        private IDbConnection conn;
        private IDbCommand queryExecuter;
        private IDataReader dr;
        private XmlParser xmlParser;

        private MatrixDbContext dbContext;

        public MyMarginDBAccess()
        {

            var config = Store.config;
            dbContext = new MatrixDbContext(config.DBParams);
            /* Params: Host, port, database, user, password */
            conn = new MySqlConnection("Server=" + config.DBParams.Host + ";" + "Database=" + config.DBParams.DatabaseName + ";" + "User ID=" + config.DBParams.Username + ";" + "Password=" + config.DBParams.Password + ";" + "Pooling=false;");
        }

        public Character GetCharInfo(int charId)
        {
            return dbContext.Characters.SingleOrDefault(c => c.CharId == (ulong)charId);
        }

        public List<MarginInventoryItem> LoadInventory(int charId)
        {
            var items = dbContext.Inventories.Where(i => i.CharId == (ulong)charId);
            List<MarginInventoryItem> Inventory = new List<MarginInventoryItem>();
            foreach (var inventoryItem in items)
            {
                MarginInventoryItem item = new MarginInventoryItem();
                item.SetItemID((uint)inventoryItem.Goid);
                item.SetPurity((ushort)inventoryItem.Purity);
                item.SetAmount((ushort)inventoryItem.Count);
                item.SetSlot(inventoryItem.Slot);
                Inventory.Add(item);
            }

            return Inventory;
        }

        public string LoadAllHardlines()
        {
            var hardLines = dbContext.DataHardlines.ToList();

            string hexpacket = "";
            foreach (var hardline in hardLines)
            {
                string districtHex = StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArrayShort(hardline.DistrictId));
                string hardlineHex = StringUtils.bytesToString_NS(NumericalUtils.uint16ToByteArray(hardline.HardLineId, 1));
                hexpacket = hexpacket + districtHex + hardlineHex + "0000";
            }

            return hexpacket;
        }

        public List<MarginAbilityItem> LoadAbilities(int charId)
        {
            List<CharAbility> charAbilities =
                dbContext.CharAbilities.Where(ca => ca.CharId == charId).OrderBy(ca => ca.Slot).ToList();

            List<MarginAbilityItem> abilities = new List<MarginAbilityItem>();
            foreach (var charAbility in charAbilities)
            {
                MarginAbilityItem ability = new MarginAbilityItem();
                ability.SetSlot((ushort)charAbility.Slot);
                ability.SetAbilityID((int)charAbility.AbilityId);
                ability.SetLevel((ushort)charAbility.Level);
                ability.SetLoaded(charAbility.IsLoaded);
                abilities.Add(ability);
            }

            return abilities;
        }


        public uint GetNewCharnameID(string handle, uint userId)
        {
            var character = dbContext.Characters.FirstOrDefault(c => c.Handle == handle && c.IsDeleted == 0);

            int number = 0;
            if (character != null)
            {
                number = (int)character.CharId;
            }

            // It does exist... hence... invent a new one
            if (number != 0)
            {
                return 0;
            }

            uint charId = CreateNewCharacter(handle, userId, 1);
            return charId;

        }

        public uint CreateNewCharacter(string handle, uint userid, uint worldId)
        {


            uint charId = 0;
            //TODO: Complete with real data from a hashtable (or something to do it faster);
            //TODO: find values for uria starting place
            var character = new Character { Handle = handle, UserId = userid, WorldId = (ushort)worldId };
            dbContext.Characters.Add(character);


            //As i didnt find a solution for "last_insert_id" in C# we must fetch the last row by a normal query
            conn.Open();
            string sqlQuery = "SELECT charId FROM characters WHERE userId='" + userid.ToString() + "' AND worldId='" + worldId.ToString() + "' AND is_deleted='0' ORDER BY charId DESC LIMIT 1";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlQuery;
            dr = queryExecuter.ExecuteReader();

            while (dr.Read())
            {
                charId = (uint)dr.GetDecimal(0);
            }

            conn.Close();

            // Create RSI Entry
            conn.Open();
            string sqlRSIQuery = "INSERT INTO rsivalues SET charid='" + charId.ToString() + "' ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlRSIQuery;
            queryExecuter.ExecuteNonQuery();

            conn.Close();

            return charId;

        }

        public void UpdateCharacter(string firstName, string lastName, string background, uint charID)
        {
            string theQuery = "UPDATE characters SET firstName = '" + firstName + "', lastName='" + lastName + "', background='" + background + "' WHERE charid='" + charID.ToString() + "' ";
            conn.Open();
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = theQuery;
            queryExecuter.ExecuteNonQuery();
            conn.Close();
        }


        public void UpdateRSIValue(string field, string value, uint charID)
        {
            string theQuery = "UPDATE rsivalues SET " + field + "='" + value + "' WHERE charid='" + charID.ToString() + "' ";
            conn.Open();
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = theQuery;
            queryExecuter.ExecuteNonQuery();
            conn.Close();
        }

        public void AddAbility(int abilityID, ushort slotID, uint charID, ushort level, ushort is_loaded)
        {
            string theQuery = "INSERT INTO char_abilities SET char_id='" + charID.ToString() + "', slot='" + slotID.ToString() + "', ability_id='" + abilityID.ToString() + "', level='" + level.ToString() + "', is_loaded='" + is_loaded.ToString() + "', added=NOW() ";
            conn.Open();
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = theQuery;
            queryExecuter.ExecuteNonQuery();
            conn.Close();
        }


        public void AddItemToSlot(uint itemGoId, ushort slotId, uint charId)
        {

            // Faster way instead of checking on every item type 
            if (itemGoId != 0 && slotId != 0 && charId != 0)
            {
                conn.Open();
                string sqlQuery = "INSERT INTO inventory SET charId = '" + charId.ToString() + "' , goid = '" + itemGoId.ToString() + "', slot = '" + slotId.ToString() + "', created = NOW() ";
                queryExecuter = conn.CreateCommand();
                queryExecuter.CommandText = sqlQuery;
                queryExecuter.ExecuteNonQuery();
                conn.Close();
            }
        }

        public void DeleteCharacter(ulong charId)
        {
            // This is a "soft-delete" Method. The Data wouldnt be deleted but will be invisible to the users.
            conn.Open();
            string sqlSoftDeleteChar = "UPDATE characters SET is_deleted='1' WHERE charId = '" + charId.ToString() + "' ";
            queryExecuter = conn.CreateCommand();
            queryExecuter.CommandText = sqlSoftDeleteChar;
            queryExecuter.ExecuteNonQuery();
            conn.Close();
        }

        public void updateRSIValue(string v1, string v2, uint newCharID)
        {
            throw new NotImplementedException();
        }
    }
}
