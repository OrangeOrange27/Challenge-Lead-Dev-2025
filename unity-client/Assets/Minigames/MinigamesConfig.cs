using System.Collections.Generic;
using Common.ConfigSystem;
using Common.Models;

namespace Minigames
{
    public class MinigamesConfig : BaseConfig
    {
        public List<MinigameModel> Minigames { get; set; }
    }
}