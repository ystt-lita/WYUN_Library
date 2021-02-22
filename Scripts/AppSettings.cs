namespace WYUN
{
    public class AppSettings
    {
        public string ip { get; private set; }
        public int port { get; private set; }
        public long appID { get; private set; }
        public string userName { get; private set; }

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
