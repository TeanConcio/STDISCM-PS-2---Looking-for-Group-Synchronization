using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STDISCM_PS_2___Looking_for_Group_Synchronization
{
    class Player
    {
        public enum PlayerClass
        {
            TANK,
            HEALER,
            DPS
        }

        public int id;
        public PlayerClass playerClass;
        public string name;

        public Player(int id, PlayerClass playerClass)
        {
            this.id = id;
            this.playerClass = playerClass;
            this.name = $"{this.playerClass.ToString()} {this.id}";
        }
    }
}
