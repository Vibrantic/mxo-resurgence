using System;

namespace mxor.auth
{
    public class WorldList
    {
        bool playerExist;

        CharacterPack cp;
        WorldsPack wp;

        string username;
        string password;
        int userID;

        int timeCreated;
        byte[] publicModulus;
        byte[] privateExponent;

        public WorldList()
        {
            cp = new CharacterPack();
            wp = new WorldsPack();
            playerExist = false;
            privateExponent = new byte[96];
            publicModulus = new byte[96];
        }

        public CharacterPack GetCharPack()
        {
            return cp;
        }

        public WorldsPack GetWorldPack()
        {
            return wp;
        }

        public void SetUsername(string username)
        {
            this.username = username;
        }

        public void SetPassword(string password)
        {
            this.password = password;
        }

        public string GetUsername()
        {
            return username;
        }

        public string GetPassword()
        {
            return password;
        }

        public bool GetExistance()
        {
            return playerExist;
        }

        public void SetExistance(bool param)
        {
            playerExist = param;
        }

        public void SetUserID(int param)
        {
            userID = param;
        }

        public int GetUserID()
        {
            return userID;
        }

        public void SetPublicModulus(byte[] param)
        {
            publicModulus = param;
        }

        public byte[] GetPublicModulus()
        {
            return publicModulus;
        }


        public void SetTimeCreated(int param)
        {
            timeCreated = param;
        }

        public int GetTimeCreated()
        {
            return timeCreated;
        }

        public void SetPrivateExponent(byte[] param)
        {
            privateExponent = param;
        }

        public byte[] GetPrivateExponent()
        {
            return privateExponent;
        }
    }
}