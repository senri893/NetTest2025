using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace JankenRoom
{
    class S
    {
        public static void Main()
        {
            Console.WriteLine("JankenRoom");
            SocketServer();
            Console.ReadKey();
        }

        public static void SocketServer()
        {
            // IPアドレスやポートの設定
            byte[] bytes = new byte[1024];
            string hostName = Dns.GetHostName();
            IPHostEntry ipHostInfo = Dns.GetHostEntry(hostName);
            IPAddress ipAddress = ipHostInfo.AddressList[1];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 11000);

            // ソケットの作成
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(localEndPoint);
            listener.Listen(10);

            Console.WriteLine("クライアントの接続を待っています...");

            // クライアント2つの接続を待つ
            //一つ目のクライアント
            Socket client1 = listener.Accept();
            Console.WriteLine("クライアント1が接続しました。");
            int bytesRec1 = client1.Receive(bytes);
            string playerName1 = Encoding.UTF8.GetString(bytes, 0, bytesRec1);
            Console.WriteLine($"{Encoding.UTF8.GetString(bytes, 0, bytesRec1)}");
            //数字だけを取り出す
            string charCount1 = Regex.Replace(playerName1, @"[^0-9]", "");

            //二つ目のクライアント
            Socket client2 = listener.Accept();
            Console.WriteLine("クライアント2が接続しました。");
            int bytesRec2 = client2.Receive(bytes);
            string playerName2 = Encoding.UTF8.GetString(bytes, 0, bytesRec2);
            Console.WriteLine($"{Encoding.UTF8.GetString(bytes, 0, bytesRec2)}");

            string charCount2 = Regex.Replace(playerName2, @"[^0-9]", "");

            //三つ目のクライアント
            //Socket client3 = listener.Accept();
            //Console.WriteLine("クライアント3が接続しました。");
            //int bytesRec3 = client3.Receive(bytes);
            //string playerName3 = Encoding.UTF8.GetString(bytes, 0, bytesRec3);
            //Console.WriteLine($"{Encoding.UTF8.GetString(bytes, 0, bytesRec3)}");
            ////数字だけを取り出す
            //string charCount3 = Regex.Replace(playerName3, @"[^0-9]", "");



            //Sendで送信している。
            //プレイヤーの名前数をもらってきてint型に変換
            byte[] comment1 = Encoding.UTF8.GetBytes(playerName2.Substring(7, int.Parse(charCount2)) + "　が勝負を挑んできた！");
            client1.Send(comment1);
            byte[] comment2 = Encoding.UTF8.GetBytes(playerName1.Substring(7, int.Parse(charCount1)) + "　が勝負を挑んできた！");
            client2.Send(comment2);
            //string waitSend = "あなたはシードです";
            //byte[] waitMsg = Encoding.UTF8.GetBytes(waitSend);
            //client3.Send(waitMsg);

            // クライアントにじゃんけんのメッセージを送信
            string sendData = "勝負！！じゃんけんゲーム！\r\n0:ぐう　1:ちょき　2:ぱあ\r\n";
            byte[] msg = Encoding.UTF8.GetBytes(sendData);
            client1.Send(msg);
            client2.Send(msg);
            //client3.Send(waitMsg);

            // クライアント1の手を受信
            bytesRec1 = client1.Receive(bytes);
            string client1HandStr = Encoding.UTF8.GetString(bytes, 0, bytesRec1);
            Console.WriteLine($"クライアント1の手: {client1HandStr}");

            // クライアント2の手を受信
            bytesRec2 = client2.Receive(bytes);
            string client2HandStr = Encoding.UTF8.GetString(bytes, 0, bytesRec2);
            Console.WriteLine($"クライアント2の手: {client2HandStr}");

            bool client1Win = false;
            bool client2Win = false;

            // 勝敗の判定
            string result1, result2;
            if (int.TryParse(client1HandStr.Substring(0, 1), out int client1Hand) &&
                int.TryParse(client2HandStr.Substring(0, 1), out int client2Hand))
            {
                if (client1Hand == client2Hand)
                {
                    result1 = result2 = "あいこ";
                }
                else if ((client1Hand + 1) % 3 == client2Hand)
                {
                    //あなたの勝ち＆あなたの負けと表示する
                    result1 = "あなたの勝ち！";
                    result2 = "あなたの負け";
                    client1Win = true;
                }
                else
                {
                    //あなたの勝ち＆あなたの負けと表示する
                    result1 = "あなたの負け";
                    result2 = "あなたの勝ち！";
                    client2Win = true;
                }
            }
            else
            {
                result1 = result2 = "無効な手が入力されました。";
            }

            // 結果をクライアントに送信
            client1.Send(Encoding.UTF8.GetBytes($"結果: {result1}\r\n"));
            client2.Send(Encoding.UTF8.GetBytes($"結果: {result2}\r\n"));

            //２回目の勝負の処理
            //if (client1Win && !client2Win)
            //{
            //    // クライアントにじゃんけんのメッセージを送信
            //    string sendData2 = "勝負！！じゃんけんゲーム！\r\n0:ぐう　1:ちょき　2:ぱあ\r\n";
            //    byte[] msg2 = Encoding.UTF8.GetBytes(sendData2);
            //    client1.Send(msg2);
            //    client3.Send(msg2);

            //    // クライアント1の手を受信
            //    bytesRec1 = client1.Receive(bytes);
            //    string client1HandStr2 = Encoding.UTF8.GetString(bytes, 0, bytesRec1);
            //    Console.WriteLine($"クライアント1の手: {client1HandStr}");

            //    // クライアント2の手を受信
            //    bytesRec3 = client3.Receive(bytes);
            //    string client2HandStr4 = Encoding.UTF8.GetString(bytes, 0, bytesRec2);
            //    Console.WriteLine($"クライアント2の手: {client2HandStr}");


            //    // 勝敗の判定
            //    string result3, result4;
            //    if (int.TryParse(client1HandStr.Substring(0, 1), out int client1Hand2) &&
            //        int.TryParse(client2HandStr.Substring(0, 1), out int client3Hand))
            //    {
            //        if (client1Hand2 == client3Hand)
            //        {
            //            result3 = result4 = "あいこ";
            //        }
            //        else if ((client1Hand2 + 1) % 3 == client3Hand)
            //        {
            //            //あなたの勝ち＆あなたの負けと表示する
            //            result3 = "あなたの勝ち！";
            //            result4 = "あなたの負け";

            //        }
            //        else
            //        {
            //            //あなたの勝ち＆あなたの負けと表示する
            //            result3 = "あなたの負け";
            //            result4 = "あなたの勝ち！";

            //        }
            //    }
            //    else
            //    {
            //        result3 = result4 = "無効な手が入力されました。";
            //    }
            //}
            //else
            //{
            //    // クライアントにじゃんけんのメッセージを送信
            //    string sendData2 = "勝負！！じゃんけんゲーム！\r\n0:ぐう　1:ちょき　2:ぱあ\r\n";
            //    byte[] msg2 = Encoding.UTF8.GetBytes(sendData2);
            //    client2.Send(msg2);
            //    client3.Send(msg2);

            //    // クライアント1の手を受信
            //    bytesRec2 = client2.Receive(bytes);
            //    string client2HandStr2 = Encoding.UTF8.GetString(bytes, 0, bytesRec2);
            //    Console.WriteLine($"クライアント1の手: {client2HandStr2}");

            //    // クライアント2の手を受信
            //    bytesRec3 = client3.Receive(bytes);
            //    string client3HandStr = Encoding.UTF8.GetString(bytes, 0, bytesRec3);
            //    Console.WriteLine($"クライアント2の手: {client3HandStr}");


            //    // 勝敗の判定
            //    string result3, result4;
            //    if (int.TryParse(client2HandStr2.Substring(0, 1), out int client2Hand2) &&
            //        int.TryParse(client3HandStr.Substring(0, 1), out int client3Hand))
            //    {
            //        if (client2Hand2 == client3Hand)
            //        {
            //            result3 = result4 = "あいこ";
            //        }
            //        else if ((client2Hand2 + 1) % 3 == client3Hand)
            //        {
            //            //あなたの勝ち＆あなたの負けと表示する
            //            result3 = "あなたの勝ち！";
            //            result4 = "あなたの負け";

            //        }
            //        else
            //        {
            //            //あなたの勝ち＆あなたの負けと表示する
            //            result3 = "あなたの負け";
            //            result4 = "あなたの勝ち！";

            //        }
            //    }
            //    else
            //    {
            //        result3 = result4 = "無効な手が入力されました。";
            //    }
            //}


            // ソケットの終了
            client1.Shutdown(SocketShutdown.Both);
            client1.Close();
            client2.Shutdown(SocketShutdown.Both);
            client2.Close();
            listener.Close();
        }
    }
}
