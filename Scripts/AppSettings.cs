namespace WYUN
{
    public class AppSettings
    {
        public string ip { get; set; }
        public int port { get; set; }
        public long appID { get; set; }
        public string userName { get; set; }

        public AppSettings(string ip_port, long id, string name)
        {
            ip = ip_port.Split(':')[0];
            port = int.Parse(ip_port.Split(':')[1]);
            appID = id;
            userName = name;
        }
        public AppSettings(string ip, int p, long id, string name)
        {
            ip = ip;
            port = p;
            appID = id;
            userName = name;
        }
    }
}
