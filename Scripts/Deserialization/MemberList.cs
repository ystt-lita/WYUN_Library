using System.Collections.Generic;

namespace WYUN.Deserialization
{
    [System.Serializable]
    public class MemberList
    {
        public class Member
        {
            public string name;
            // override object.Equals
            public override bool Equals(object obj)
            {
                //
                // See the full list of guidelines at
                //   http://go.microsoft.com/fwlink/?LinkID=85237
                // and also the guidance for operator== at
                //   http://go.microsoft.com/fwlink/?LinkId=85238
                //

                if (obj == null || GetType() != obj.GetType())
                {
                    return false;
                }

                if (name.Equals(((Member)obj).name))
                {
                    return true;
                }
                else { return false; }
            }
            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }
        public List<Member> members;
        public MemberList()
        {
            members = new List<Member>();
        }
    }
}