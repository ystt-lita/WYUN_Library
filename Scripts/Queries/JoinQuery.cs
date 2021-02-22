namespace WYUN.Queries
{
    [System.Serializable]
    public class JoinQuery
    {
        public string query;
        public string name;
        public JoinQuery(string n, bool withQuery = false)
        {
            name = n;
            if (withQuery)
            {
                query = "join";
            }
            else
            {
                query = null;
            }
        }
    }
}