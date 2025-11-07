namespace Common
{
    public class BasePlayerData
    {
        public string ID { get; set; }

        public string AuthToken { get; set; }

        public string UserName { get; set; }


        public override string ToString()
        {
            return "BasePlayerData:\n" +
                   $"ID: {ID}\n" +
                   $"UserName: {UserName}\n";
        }
    }
}