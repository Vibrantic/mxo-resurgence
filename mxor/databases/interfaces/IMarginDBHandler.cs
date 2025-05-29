using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using mxor.databases.Entities;
using mxor.margin.Entities;

namespace mxor.databases.interfaces{
    public interface IMarginDBHandler{

        Character GetCharInfo (int charId);
        List<MarginInventoryItem> LoadInventory(int charId);
        List<MarginAbilityItem> LoadAbilities(int charId);
        string LoadAllHardlines();
        UInt32 GetNewCharnameID(string handle, UInt32 userId);
        UInt32 CreateNewCharacter(string handle, UInt32 userid, UInt32 worldId);
        void UpdateRSIValue(string field, string value, UInt32 charID);
        void UpdateCharacter(string firstName, string lastName, string background, UInt32 charID);

        // NEW
        void AddAbility(Int32 abilityID, UInt16 slotID, UInt32 charID, UInt16 level, UInt16 is_loaded);

        void AddItemToSlot(UInt32 itemId, UInt16 slotID, UInt32 charID);
        void DeleteCharacter(UInt64 charId);
        void updateRSIValue(string v1, string v2, uint newCharID);
    }
}