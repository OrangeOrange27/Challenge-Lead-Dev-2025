using System.Collections.Generic;
using Common.ConfigSystem;
using Common.Minigames.Models;

namespace Common.Minigames
{
    public class MinigamesConfig : BaseConfig
    {
        public List<MinigameModel> Minigames { get; set; }
    }
}