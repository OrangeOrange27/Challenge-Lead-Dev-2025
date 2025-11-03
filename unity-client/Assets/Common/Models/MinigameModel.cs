using System.Collections.Generic;

namespace Common.Models
{
    public class MinigameModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string IconId { get; set; }
        public List<MinigameModeModel> Modes { get; set; }
    }
}