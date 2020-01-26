using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotoIto.App.WatchDogsVRC
{
    public partial class Configurator : Form
    {
        List<string> joinedPlayerList = new List<string>();
        List<string> friendList = new List<string>();
        List<DateTime> joinedDateTimeList = new List<DateTime>();
        VRCLogStream vrcLog = new VRCLogStream();
        VRCWebAPI.UserFriendList userFriendList;
        Config.VRCUserModel vrcUser;
        public Configurator()
        {
            InitializeComponent();
            NotoIto.Utility.ClassSerializer.ReadXML<Config.VRCUserModel>(Path.Combine(vrcLog.FolderPath, "VRCUser.xml")).Match(
                (x) => {
                    vrcUser = x;
                    userFriendList = new VRCWebAPI.UserFriendList(x.ID, x.Password);
                    signinedPanel.Visible = true;
                },
                () => vrcUser = new Config.VRCUserModel());
            VRCLogParser.OnPlayerJoined += OnPlayerJoined;
            VRCLogParser.OnFriendRequestReceived += OnFriendRequestReceived;
        }

        private void OnFriendRequestReceived(object sender, EventArgs e)
        {
            VRNotify.BalloonTipTitle = "VRC　フレンドリクエスト";
            VRNotify.BalloonTipText = sender as string;
            VRNotify.ShowBalloonTip(5000);
        }


        private void OnPlayerJoined(object sender, EventArgs e)
        {
            if (!(bool)friendList?.Contains(sender as string))
                return;
            for (int i = 0; i < joinedDateTimeList.Count; i++)
            {
                if (joinedDateTimeList[i].AddSeconds(10) < DateTime.Now)
                {
                    joinedPlayerList.RemoveAt(i);
                    joinedDateTimeList.RemoveAt(i);
                    i--;
                }
            }
            joinedPlayerList.Add(sender as string);
            joinedDateTimeList.Add(DateTime.Now);
            VRNotify.BalloonTipTitle = "VRC　参加通知";
            if (joinedPlayerList.Count == 1)
                VRNotify.BalloonTipText = joinedPlayerList[0] + " さんがこのセッションに参加しています";
            else
            {
                VRNotify.BalloonTipText = "";
                int i;
                for (i = 0; i < joinedPlayerList.Count; i++)
                {
                    VRNotify.BalloonTipText += joinedPlayerList[i] + ",";
                    if (VRNotify.BalloonTipText.Length > 32)
                        break;
                }
                if (i != joinedPlayerList.Count)
                    VRNotify.BalloonTipText += "他" + (joinedPlayerList.Count - i) + "名がこのセッションに参加しています";
                else
                    VRNotify.BalloonTipText += "さんらがこのセッションに参加しています";
            }
            VRNotify.ShowBalloonTip(5000);
        }

        private void Configurator_Load(object sender, EventArgs e)
        {
            VRNotify.ShowBalloonTip(3000);
        }

        private void signinButton_Click(object sender, EventArgs e)
        {
            vrcUser.ID = iDTextBox.Text;
            vrcUser.Password = passwordTextBox.Text;

            userFriendList = new VRCWebAPI.UserFriendList(vrcUser.ID, vrcUser.Password);
            NotoIto.Utility.ClassSerializer.WriteXML<Config.VRCUserModel>(vrcUser,Path.Combine(vrcLog.FolderPath,"VRCUser.xml"));
            signinedPanel.Visible = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            friendList = userFriendList?.Get();
        }
    }
}
