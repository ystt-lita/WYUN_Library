using System.Collections.Generic;

namespace WYUN.Deserialization
{
    [System.Serializable]
    public class RoomList
    {
        public List<Queries.CreateQuery> rooms;
        public RoomList()
        {
            rooms = new List<Queries.CreateQuery>();
        }
        public void Add(Queries.CreateQuery item)
        {
            rooms.Add(item);
        }
        public void Remove(Queries.CreateQuery item)
        {
            rooms.Remove(item);
        }
    }
}