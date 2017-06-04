using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace kartiserv1
{
    class Program
    {

        static List<karta> vse1 = new List<karta>();
        static List<karta> centr = new List<karta>();
        static List<karta> otboi = new List<karta>();
        static List<karta> player1 = new List<karta>();
        static List<karta> player2= new List<karta>();
        static StreamReader reader;
        static StreamWriter writer;
        static StreamReader reader1;
        static StreamWriter writer1;
        static int port = 12345;
        static string lastzapros="";
        static Random rand = new Random();
        static string lastzapros1 = "";
        static bool game = true;
        static int KOZIR = 0;
        static karta KOZIR1;
        static void Main(string[] args)
        {
            
            bool hodPlayer1 = true;
            bool attack = true;

            
            Console.WriteLine(@"1ip("")   2port("")");

            string ip1 = Console.ReadLine();
            if (ip1 == "")
                ip1 = "192.168.1.64";
            string portTemp;
            portTemp = Console.ReadLine();
            port = portTemp == "" ? 12345 : Convert.ToInt32(portTemp);

            TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse(ip1), port));
            listener.Start();


            TcpClient server = listener.AcceptTcpClient();

            server.ReceiveTimeout = 50;
            reader = new StreamReader(server.GetStream());
            writer = new StreamWriter(server.GetStream());
            Console.WriteLine("1 подключился");
            // reader.ReadLine();
            writer.AutoFlush = true;
            writer.WriteLine("имя#1");

            TcpClient server1 = listener.AcceptTcpClient();

            server1.ReceiveTimeout = 50;
            reader1 = new StreamReader(server1.GetStream());
            writer1 = new StreamWriter(server1.GetStream());
            // reader.ReadLine();
            writer1.AutoFlush = true;
            Console.WriteLine("2 подключился");
            writer1.WriteLine("имя#2");
            



            for (int i=0;i<4;++i)
            {
                for (int o = 0;  o < 9;++o)
                {
                    vse1.Add(new karta(o,i));

                }

            }
            int kozirInt = rand.Next(0, 36);
            KOZIR = vse1[kozirInt].mast;
            writer1.WriteLine("козырь" + "#" + vse1[kozirInt].dost + "#" + KOZIR);
            writer.WriteLine("козырь" + "#" + vse1[kozirInt].dost + "#" + KOZIR);
            KOZIR1 = new karta(vse1[kozirInt].dost, KOZIR);
            vse1.Remove(vse1[kozirInt]);

            dobor_kart();



            while (game)//досчитать кто победил отстановка цикла кнопки типо взять карты и тд
            {
                //writer.WriteLine("111111#111");
                string tekkarta = "";
                if (hodPlayer1)
                {
                    tekkarta = read();
                }
                else
                {


                    tekkarta = read1();
                }
                
                   

                    string[] tempmass = tekkarta.Split('#');
                    switch (tempmass[0])
                    {

                   // case "имя":

                       // break;

                        case "закончить":

                        //сейчас просто перемещает все в отбой и добирает до 6 карт добавить взятие в руки если не отбито до того что есть
                        
                        otboi.AddRange(centr);
                        centr.Clear();
                     
                        dobor_kart();
                        attack = attack == true ? false : true;

                        // writer.WriteLine("закончить");
                        // writer1.WriteLine("закончить");

                        hodPlayer1 = hodPlayer1 == true ? false : true;
                        break;


                        case "карта_в_центр":

                        //сравнивает только с верхней картой без нескольких и отправляет закинута ли карта в центр или нет
                        karta tekkarta1 = new karta(Convert.ToInt32(tempmass[1]), Convert.ToInt32(tempmass[2]));
                        if (!centr.Contains(tekkarta1))
                        {
                            bool zakid = false;

                            if (centr.Count == 0)
                                zakid = true;
                            else
                            {
                                if (centr.Count%2==0)
                                {
                                    for (int i = 0; i < centr.Count; ++i)
                                    {
                                        if (centr[i].dost == tekkarta1.dost)
                                            zakid = true;

                                    }
                                }
                                else
                                {
                                    if ((centr[centr.Count - 1].dost < tekkarta1.dost&& centr[centr.Count - 1].mast== tekkarta1.mast)||(tekkarta1.mast==KOZIR&& centr[centr.Count - 1].mast!=KOZIR))
                                        zakid = true;
                                }
                            }

                            if (zakid)
                            {
                                //attack = attack == true ? false : true;/////////////////////////
                                
                                centr.Add(tekkarta1);
                                if (hodPlayer1)
                                {
                                    player1.Remove(tekkarta1);
                                    int a = player1.Count;
                                    writer.WriteLine("помещено в центр" + "#" + tekkarta1.dost + "#" + tekkarta1.mast);
                                    writer1.WriteLine("помещено в центр" + "#" + tekkarta1.dost + "#" + tekkarta1.mast);
                                }
                                else
                                {

                                    player2.Remove(tekkarta1);

                                    int a = player2.Count;
                                    writer1.WriteLine("помещено в центр" + "#" + tekkarta1.dost + "#" + tekkarta1.mast);
                                    writer.WriteLine("помещено в центр" + "#" + tekkarta1.dost + "#" + tekkarta1.mast);
                                }
                                hodPlayer1 = hodPlayer1 == true ? false : true;
                            }
                            else
                             {
                            // if (hodPlayer1)
                            //{
                            //player1.Remove(tekkarta1);
                             writer.WriteLine("не помещено в центр");
                            // }
                            //else
                            //{

                            //player2.Remove(tekkarta1);
                             writer1.WriteLine("не помещено в центр");
                                //}
                                
                            }
                            
                        }


                        break;
                    case "взять":

                        string temp = "взять";
                        foreach (karta i in centr)
                        {

                            temp += "#" + i.dost + "#" + i.mast;

                        }
                        if (hodPlayer1)
                        {
                            player1.AddRange(centr);
                            writer1.WriteLine("взять");
                            writer.WriteLine(temp);

                            
                        }
                        else
                        {
                            player2.AddRange(centr);
                            writer.WriteLine("взять");
                            writer1.WriteLine(temp);
                        }
                        dobor_kart();
                        centr.Clear();

                        break;
                    default:
                        writer.WriteLine("");
                        writer1.WriteLine("");
                        break;


                    }
                   
            
            }


        }

////////////////////////??????????????????
        private static void dobor_kart()
        {
            
            string temp = "добор";
            string temp1 = "добор";
            if(vse1.Count==0&&KOZIR1!=null)
            {
                vse1.Add(KOZIR1);
                KOZIR1 = null;

            }

            if(vse1.Count>0)
            for (int i = player1.Count; i < 6; ++i)
            {
                int nomerkarti = rand.Next(0, vse1.Count);//??? может быть надо count-1
                player1.Add(vse1[nomerkarti]);
                vse1.Remove(vse1[nomerkarti]);
                temp += "#" + player1[i].dost + "#" + player1[i].mast;

            }


            if (vse1.Count > 0)
                for (int i = player2.Count; i < 6; ++i)
            {
                int nomerkarti = rand.Next(0, vse1.Count);
                player2.Add(vse1[nomerkarti]);
                vse1.Remove(vse1[nomerkarti]);
                temp1 += "#" + player2[i].dost + "#" + player2[i].mast;
            }

            if(vse1.Count==0)
            {
                if(player1.Count==0)
                {
                    writer1.WriteLine("выйграл игрок 1");
                    
                    writer.WriteLine("выйграл игрок 1");
                    game = false;
                }
                if(player2.Count == 0)
                {
                    writer1.WriteLine("выйграл игрок 2");

                    writer.WriteLine("выйграл игрок 2");
                    game = false;
                }
            }
            if (game)
            {
                //if(temp1.IndexOf("#")!=-1)
                writer1.WriteLine(temp1);
                //if (temp.IndexOf("#") != -1)
                writer.WriteLine(temp);
            }
        }


        private static string read()
        {
            try
            {
                string text;
                text = reader.ReadLine();
                text = text == lastzapros ? "" : text;
                lastzapros = text;
                return text;
            }
            catch
            {
                return "";
            }
        }

        private static string read1()
        {
            try
            {
                string text;
                text = reader1.ReadLine();
                text = text == lastzapros1 ? "" : text;
                lastzapros1 = text;
                return text;
            }
            catch
            {
                return "";
            }
        }
    }
    class karta:IEquatable<karta>
    {
        public int dost;
        public int mast;


        public karta()
        {
            dost = 0;
            mast = 0;
        }
        public karta(int a,int b)
        {
            dost = a;
            mast = b;
        }
        /*
        public bool IEquatable<karta>.Equals(karta a)
        {
            if(dost==a.dost&&mast==a.mast)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        */
        public bool Equals(karta a)
        {

            if (a== null)
                return false;

            if (this.dost == a.dost && this.mast == a.mast)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public override bool Equals(Object obj)
        {
            //karta a = (karta)obj;
            if (obj == null)
                return false;
            var a = obj as karta;
            if (a == null)
                return false;

            
            if (this.dost == a.dost && this.mast == a.mast)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        }

}
