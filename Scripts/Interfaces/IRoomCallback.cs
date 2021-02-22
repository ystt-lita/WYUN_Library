namespace WYUN
{
    public interface IRoomCallback
    {
        void ServerError(string message);
        void MessageReceived(string message);
        void JoinedRoom();
        void LeftRoom();
        void UpdatedRoomMember(string mList);
        void UpdatedRoomOption(string option);
    }
}