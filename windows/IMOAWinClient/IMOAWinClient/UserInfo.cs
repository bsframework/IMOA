using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMOAWinClient
{
    class UserInfo
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public int parentID { get; set; }

        public List<UserInfo> UserInfoList ()
        {
            List<UserInfo> UserInfos = new List<UserInfo>();
            UserInfo u1 = new UserInfo();
            u1.ID = 0;
            u1.Name = "军工事业部";
            u1.parentID = 999;
            UserInfo u2 = new UserInfo();
            u2.ID = 1;
            u2.Name = "张三";
            u2.parentID = 0;
            UserInfo u3 = new UserInfo();
            u3.ID = 3;
            u3.Name = "张四";
            u3.parentID = 0;

            UserInfo u11 = new UserInfo();
            u11.ID = 4;
            u11.Name = "其他事业部";
            u11.parentID = 999;
            UserInfo u5 = new UserInfo();
            u5.ID = 5;
            u5.Name = "李四";
            u5.parentID = 4;
            UserInfos.Add(u1);
            UserInfos.Add(u11);
            UserInfos.Add(u2);
            UserInfos.Add(u3);
            UserInfos.Add(u5);
            return UserInfos;
           
        }

    }
}
