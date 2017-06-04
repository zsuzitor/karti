using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace kartiform
{
    public partial class Form1 : Form
    {
        StreamReader reader;
        StreamWriter writer;
        int port = 12345;
        string ip1 = "192.168.1.64";
        public bool player1 = true;
        int posPlayer1X=0;
        int posPlayer1Y = 0;
        int posCentrX = 0;
        int posCentrY = 0;
        int tekkartaX = 0;
        int tekkartaY = 0;

        string lastzapros = "";
        object nagatayaKArtaObj;
        karti nagataya = new karti();
        karti KOZIR1 = new karti();
        bool hodPlayer1 = true;
        bool attack = true;

        PictureBox nagatiiPic=new PictureBox();
        bool nagatiiPic1 = false;

        List<karti> centr = new List<karti>();
        List<karti> ImPlayer = new List<karti>();
        List<karti> vse_karti1 = new List<karti>();


        public Form1()
        {
            InitializeComponent();


            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.Bounds = Screen.PrimaryScreen.Bounds;

            //this.Height = 900;
            //this.Width = 1600;
            ip11.Left = panel1.Right - 100;
            port11.Left = panel1.Right - 100;
            start.Left = panel1.Right - 100;
            kozir_ui.Location = new Point(panel1.Width - 177, panel1.Height / 2);
            
            vzyat.Location = new Point(panel1.Width - 200, panel1.Height / 2+130);
            otboi.Location = new Point(panel1.Width - 100, panel1.Height / 2+130);



            posPlayer1Y = panel1.Height*2/3;
            posCentrY=panel1.Height /4;



            for (int i = 0; i < 4; ++i)
            {
                for (int o = 0; o < 9; ++o)
                {
                    vse_karti1.Add(new karti(o, i,imageList1.Images[o * 4 + i]));

                }

            }


            pictureBox1.Image = imageList1.Images[36];
            pictureBox2.Image = imageList1.Images[36];


            pictureBox1.Location = new Point(panel1.Width / 2 ,50);
            pictureBox2.Location = new Point(panel1.Width - 100, panel1.Height / 2);



        }

       


        private void startClient()
        {
            TcpClient client = new TcpClient();
            client.Connect(ip1, port);
            client.ReceiveTimeout = 50;
            reader = new StreamReader(client.GetStream());
            writer = new StreamWriter(client.GetStream());
            writer.AutoFlush = true;
           // writer.WriteLine("имя");

        }

        private string read1()
        {
            try
            {
                string text;
                text = reader.ReadLine();
               // text = text == lastzapros ? "" : text;
                lastzapros = text;
                return text;
            }
            catch
            {
                return "";
            }
        }

        //возвращает image из листа картинок
///////////////////////////////////
        private Image getimage(int a,int b)
        {


            return imageList1.Images[a*4+b];
        }


        //добавляет картинку по координатам
///////////////////////////////////
        private void dobavitImage(int a,int b,  Image c)
        {

            PictureBox tekpikture = new PictureBox();
            tekpikture.Image = c;
            tekpikture.Left = a;
            tekpikture.Top = b;
            tekpikture.Width = 71;
            tekpikture.Height = 111;
            //tekpikture.DoubleClick += pictureBox_DoubleClick;
            
            tekpikture.Click+= pictureBox3_Click;
            panel1.Controls.Add(tekpikture);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
          
            string[] masGet = read1().Split('#');
            if(masGet[0].IndexOf("выйграл игрок")!=-1)
            {
                MessageBox.Show(masGet[0]);
                masGet[0] = "";
            }
            
           /* if(nagatiiPic1)
            {
                nagatiiPic.Left = Cursor.Position.X;
                nagatiiPic.Top = Cursor.Position.Y;

            }*/

            switch (masGet[0])
            {
                case "имя":
                    if (masGet[1] == "1")
                    {
                        hodPlayer1 = false;
                        attack = false;
                    }
                    else
                    {
                        hodPlayer1 = true;
                        attack = true;
                    }


                    break;
                case "козырь":
                    kozir_ui.Image = getimage(Convert.ToInt32( masGet[1]), Convert.ToInt32(masGet[2]));
                    KOZIR1.ves = Convert.ToInt32(masGet[1]);
                    KOZIR1.mast = Convert.ToInt32(masGet[2]);
                    KOZIR1.image1 = kozir_ui.Image;
                    break;

                case "не помещено в центр":
                    nagatiiPic.Left = tekkartaX;
                    nagatiiPic.Top = tekkartaY;
                    break;
                case "помещено в центр":
                    
                    hodPlayer1 = hodPlayer1 == true ? false : true;
                    //attack = attack== true ? false : true;
                    obnovkarti(1);
                    for (int i=0;i<ImPlayer.Count;++i)
                    {
                        
                        if (ImPlayer[i].ves == Convert.ToInt32(masGet[1]) && ImPlayer[i].mast == Convert.ToInt32(masGet[2]))
                        {
                           
                            
                            ImPlayer.Remove(nagataya);
                            int a = ImPlayer.Count;
                            

                        }
                    }
                    obnovkarti(2);
                    centr.Add(new karti(Convert.ToInt32(masGet[1]), Convert.ToInt32(masGet[2]), getimage(Convert.ToInt32(masGet[1]), Convert.ToInt32(masGet[2]))));

                    
                    dobavitImage(posCentrX, posCentrY, getimage(Convert.ToInt32(masGet[1]), Convert.ToInt32(masGet[2])));
                    if(posCentrX>panel1.Width-200)
                    {
                        posCentrX = 0;
                        posCentrY += 111;

                    }
                    else
                    posCentrX += 90;

                    break;
                case "взять":
                    if(masGet.Count()==1)
                    {

                        otboi_method();
                    }
                    else
                    {
                        ImPlayer.AddRange(centr);

                        obnovkarti(1);

                        otboi_method();
                        obnovkarti(2);
                    }

                    //writer.WriteLine("закончить");
                    hodPlayer1 = hodPlayer1 == true ? false : true;
                   // attack = attack == true ? false : true;
                    break;

                case "добор":
                    //hodPlayer1 = hodPlayer1 == true ? false : true;
                    posCentrX = 0;
                    posCentrY = panel1.Height / 4;
                    otboi_method();
                    obnovkarti(1);
                    for (int i = 1; i < masGet.Count(); ++i)
                    {
                     if(KOZIR1.ves== Convert.ToInt32(masGet[i])&&KOZIR1.mast== Convert.ToInt32(masGet[i + 1]))
                        {
                            //если взят козырь
                            kozir_ui.Dispose();
                        }
                        ImPlayer.Add(new karti(Convert.ToInt32(masGet[i]), Convert.ToInt32(masGet[i + 1]), getimage(Convert.ToInt32(masGet[i]), Convert.ToInt32(masGet[i + 1]))));
                        ++i;
                        
                        
                        
                    }
                    obnovkarti(2);
                    hodPlayer1 = hodPlayer1 == true ? false : true;
                    attack = attack == true ? false : true;
                    break;
              /* case "выйграл игрок":
                  
                    otboi_method();
                    hodPlayer1 = hodPlayer1 == true ? false : true;
                    break;*/

                default:
                    break;

            }

        }


        private void pictureBox_DoubleClick(object sender, EventArgs e)
        {
            nagatiiPic = sender as PictureBox;
            if (hodPlayer1 )
            {
                karti temp = new karti();
                nagatayaKArtaObj = sender;


                foreach (karti i in ImPlayer)
                {
                   
                    if (EqualityImage(i.image1, ((PictureBox)sender).Image))
                    {
                        temp = karti.equal(i);
                        nagataya = karti.equal(temp);
                    }
                }
                writer.WriteLine("карта_в_центр#" + temp.ves + "#" + temp.mast);
            }
        }

///////////////////////////////////
        private bool EqualityImage(Image Img1, Image Img2)
        {
            Bitmap Bmp1 = (Bitmap)Img1;
            Bitmap Bmp2 = (Bitmap)Img2;
            if(Img1==null||Img2==null)
                return false;
            if (Bmp1.Size == Bmp2.Size)
            {
                for (int i = 0; i < Bmp1.Width; i++)
                    for (int j = 0; j < Bmp1.Height; j++)
                        if (Bmp1.GetPixel(i, j) != Bmp2.GetPixel(i, j))
                            return false;
                return true;
            }
            else
                return false;
        }
       
        ///////////////////////

        private void obnovkarti(int a)
        {
           // int a1 = ImPlayer.Count;
            if (a == 1)
            {
                List<PictureBox> temp11 = new List<PictureBox>();


                foreach (Control c in panel1.Controls)
                {
                    var cb = c as PictureBox;

                    if (cb != null)
                    {

                        temp11.Add(cb);
                    }
                }
                for(int i=temp11.Count-1;i>=0;--i)
                {
                    foreach (karti i1 in ImPlayer)
                    {
                        if (EqualityImage(i1.image1, temp11[i].Image))
                        {
                            temp11[i].Dispose();
                        }
                    }
                }


                posPlayer1X = 0;
                posPlayer1Y = panel1.Height * 2 / 3;
            }
            if (a == 2)
            { 
            foreach (karti i in ImPlayer)
            {
                    

                    dobavitImage(posPlayer1X, posPlayer1Y, i.image1);
                    if (posPlayer1X > panel1.Width - 200)
                    {
                        posPlayer1X = 0;
                        posPlayer1Y += 111;

                    }
                    else
                        posPlayer1X += 90;
                    //posPlayer1X += 90;
            }
                
                }

            


        }


        private void otboi_Click(object sender, EventArgs e)
        {
            if (hodPlayer1&&attack)
            {
                
                //нужен флаг о том кто ходит что бы отбой кликать не все могли

                //hodPlayer1 = hodPlayer1 == true ? false : true;
                writer.WriteLine("закончить");
               
                otboi_method();
                
            }

        }
        private void otboi_method()
        {
            List<PictureBox> temp11 = new List<PictureBox>();


            foreach (Control c in panel1.Controls)
            {
                var cb = c as PictureBox;
                if (cb != null)
                {

                    temp11.Add(cb);
                }
            }

            for (int i = temp11.Count - 1; i >= 0; --i)
            {
                foreach (karti i1 in centr)
                {
                    if (EqualityImage(i1.image1, temp11[i].Image))
                    {
                        temp11[i].Dispose();
                    }
                }
            }

            centr.Clear();
        }

        private void start_Click(object sender, EventArgs e)
        {

           
                ip1 = ip11.Text;
            port = Convert.ToInt32( port11.Text);
            ip11.Visible = false;
            port11.Visible = false;
            start.Visible = false;
            startClient();


        }

        private void vzyat_Click(object sender, EventArgs e)
        {
            bool a = hodPlayer1;
            bool a1 = attack;
            if(hodPlayer1&&!attack)

            writer.WriteLine("взять");
        }

        //клик для анимации
        private void pictureBox3_Click(object sender, EventArgs e)
        {
            
            nagatiiPic = sender as PictureBox;
            //int X = 0;
            //int Y = 0;
            if (hodPlayer1)
            {
                if (nagatiiPic != null && !EqualityImage(nagatiiPic.Image, imageList1.Images[36]))
                {
                    //nagatiiPic1 = nagatiiPic1 ? false : true;
                    nagatiiPic1 = true;
                    tekkartaX = nagatiiPic.Left;
                    tekkartaY = nagatiiPic.Top;
                    Cursor.Hide();
                }

                while (nagatiiPic1)
                {

                    nagatiiPic.Left = Cursor.Position.X - 35;
                    nagatiiPic.Top = Cursor.Position.Y - 55;

                    if (nagatiiPic.Top < posCentrY + 50)
                    {
                        nagatiiPic1 = false;
                        pictureBox_DoubleClick(sender, null);
                        Cursor.Show();
                    }
                    if (nagatiiPic.Top > posPlayer1Y)
                    {

                        nagatiiPic1 = false;
                        nagatiiPic.Left = tekkartaX;
                        nagatiiPic.Top = tekkartaY;
                        Cursor.Show();
                    }


                }
            }
        }

        
    }


    class karti : IEquatable<karti>
    {
        public int ves;
        public int mast;
        public Image image1;

        public karti()
        {
            ves = 0;
            mast = 0;
        }
        public karti(int a,int b,Image c)
        {
            ves = a;
            mast = b;
            image1 = c;
        }
        public static karti equal(karti a)
        {
            return new karti(a.ves,a.mast,a.image1);
        }
        bool IEquatable<karti>.Equals(karti a)
        {
            if (ves == a.ves && mast == a.mast)
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
