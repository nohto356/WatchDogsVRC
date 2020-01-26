using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NotoIto.App.WatchDogsVRC
{
    public static class VRCLogParser
    {
        public static event EventHandler<EventArgs> OnPlayerJoined;
        public static event EventHandler<EventArgs> OnFriendRequestReceived;
        public enum MessageType { Unknown }
        private static Func<string, string, string, Optional.Option<(MessageType, string[])>>[] checkFuncList
                = new Func<string, string, string, Optional.Option<(MessageType, string[])>>[]
                {
                    //OnPlayerJoined
                    (level, sender, message) =>
                    {
                        if(level != "Log" || sender != "NetworkManager"|| !message.Contains("OnPlayerJoined"))
                            return Optional.Option.None<(MessageType, string[])>();
                        (MessageType, string[]) rt = default;
                        List<string> messages = new List<string>();

                        OnPlayerJoined?.Invoke(message.Replace("OnPlayerJoined ","").Trim(),EventArgs.Empty);

                        rt.Item1 = MessageType.Unknown;
                        rt.Item2 = messages.ToArray();
                        return Optional.Option.None<(MessageType, string[])>();
                    },

                    //OnFriendRequestReceived
                    (level, sender, message) =>
                    {
                        if(level != "Log" || sender != "" || !message.Contains("Received Notification:") || !message.Contains("friendRequest"))
                            return Optional.Option.None<(MessageType, string[])>();
                        (MessageType, string[]) rt = default;
                        List<string> messages = new List<string>();

                        OnFriendRequestReceived?.Invoke("フレンドリクエストが届きました．",EventArgs.Empty);

                        rt.Item1 = MessageType.Unknown;
                        rt.Item2 = messages.ToArray();
                        return Optional.Option.None<(MessageType, string[])>();
                    },

                };

        public static (string,string,MessageType,string[]) ParseText(string text)
        {
            (string, string, MessageType, string[]) ret = ("Unknown", "Unknown", MessageType.Unknown, new string[] { });
            var splittedText = StringSplitter(text);
            ret.Item1 = splittedText[0];
            ret.Item2 = splittedText[1];
            var message = CheckMessageType(ret.Item1, ret.Item2, splittedText[2]);
            ret.Item3 = message.Item1;
            ret.Item4 = message.Item2;
            return ret;
        }
        private static string[] StringSplitter(string text)
        {
            if (text == null)
                return null;
            List<string> strList = new List<string>();
            var regPattern = @"\[.*?\]";
            var strs1 = text.Split('-');
            if (strs1.Length == 0)
                return null;
            var strs2 = strs1[0].Split(' ');
            if (strs2.Length > 2)
                strList.Add(strs2[2]);
            else
                strList.Add("");
            var splitter = new Regex(regPattern);
            if (splitter.IsMatch(strs1.Last()))
            {
                var split = splitter.Match(strs1.Last());
                strList.Add(split.Value.Trim('[', ']'));
                strList.Add(strs1.Last().Replace(split.Value, ""));
            }
            else
            {
                strList.Add("");
                strList.Add(strs1.Last());
            }
            return strList.ToArray();
        }

        private static (MessageType, string[]) CheckMessageType(string level, string sender, string text)
        {
            (MessageType,string[]) ret = default;
            foreach(var f in checkFuncList)
            {
                if (ret != default)
                    break;
                f(level, sender, text).MatchSome((x) =>
                {
                    ret = (x.Item1, x.Item2);
                });
            }
            if (ret == default)
                return (MessageType.Unknown, new string[] { });
            else
                return ret;
        }
    }
}
