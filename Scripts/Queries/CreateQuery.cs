namespace WYUN.Queries
{
    [System.Serializable]
    public class CreateQuery
    {
        public CreateQuery(string n, long l, string o, bool withQuery = false)
        {
            name = n;
            limit = l;
            owner = o;
            if (withQuery)
            {
                query = "create";
            }
            else
            {
                query = null;
            }
        }
        public CreateQuery(){}
        public string query;
        public string name;
        public long limit;
        public string owner;
    }
}
