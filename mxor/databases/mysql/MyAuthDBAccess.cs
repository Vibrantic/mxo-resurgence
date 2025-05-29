using System;
using System.Data;
using System.Linq;
using mxor.auth;
using mxor.databases.Entities;
using mxor.databases.interfaces;
using mxor.shared;

namespace mxor.databases
{
    public class MyAuthDBAccess : IAuthDBHandler
    {
        private MatrixDbContext dbContext;

        public MyAuthDBAccess()
        {
            var config = Store.config;
            dbContext = new MatrixDbContext(config.DBParams);
        }


        public bool FetchWorldList(ref WorldList wl)
        {
            // Doesnt exist by default
            wl.SetExistance(false);

            var worldList = wl;

            try
            {
                var player = dbContext.Users
                    .Where(u => u.Username == worldList.GetUsername())
                    .Single(u => u.Passwordmd5 == worldList.GetPassword());

                byte[] publicModulus = new byte[96];
                byte[] privateExponent = new byte[96];

                ArrayUtils.copy(player.PublicModulus, 0, publicModulus, 0, 96);
                ArrayUtils.copy(player.PrivateExponent, 0, privateExponent, 0, 96);
                var dateTimeCreated = new DateTimeOffset(player.TimeCreated).ToUniversalTime().ToUnixTimeMilliseconds();

                wl.SetPublicModulus(publicModulus);
                wl.SetPrivateExponent(privateExponent);
                wl.SetExistance(true);
                wl.SetUserID((int)player.UserId);
                wl.SetTimeCreated((int)dateTimeCreated);
            }
            catch (Exception ex)
            {
                string msg = "Player not found on DB with #" + wl.GetUsername() + "# and #" + wl.GetPassword() + "#";
                Output.WriteLine(msg);
                return false;
            }

            // Prepare to read characters
            var userId = wl.GetUserID();
            var characters = dbContext.Characters.Where(c => c.UserId == userId).Where(c => c.IsDeleted == 0)
                .ToList();

            wl.GetCharPack().SetTotalChars(characters.Count);

            foreach (var character in characters)
            {
                wl.GetCharPack().AddCharacter(character.Handle, (int)character.CharId, character.Status,
                    character.WorldId);
            }

            var worlds = dbContext.Worlds.OrderBy(w => w.WorldId).ToList();
            foreach (var world in worlds)
            {
                wl.GetWorldPack().AddWorld(world.Name, world.WorldId, world.Status, world.Type,
                    (ushort)world.NumPlayers);
            }

            return true;
        }
    }
}