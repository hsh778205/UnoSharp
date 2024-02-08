using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

using System.Threading;
using System.ComponentModel;


namespace UnoSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {




        ComputerCalc computer = new ComputerCalc();
        Rank rank = new Rank();

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyChanged)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyChanged));
        }



        private string TurnVertical(string str)
        {
            string ret = "";
            for (int i = 0; i < str.Length; i++)
            {
                ret += str.Substring(i, 1) + "\n";
            }
            return ret;
        }

        string GetCompileVersion()
        {
            string OriginVersion = "" + System.IO.File.GetLastWriteTime(this.GetType().Assembly.Location);
            int MsgCnt = 0;
            string year = "";
            string month = "";
            string day = "";
            string hour = "";
            string min = "";
            string sec = "";
            for (int i = 0; i < OriginVersion.Length && MsgCnt < 6; i++)
            {
                char ch = OriginVersion[i];
                if (ch >= '0' && ch <= '9')
                {
                    switch (MsgCnt)
                    {
                        case 0: year += ch; break;
                        case 1: month += ch; break;
                        case 2: day += ch; break;
                        case 3: hour += ch; break;
                        case 4: min += ch; break;
                        case 5: sec += ch; break;
                    }
                }
                else
                {
                    MsgCnt++;
                }
            }
            while (year.Length < 4) year = "0" + year;
            while (month.Length < 2) month = "0" + month;
            while (day.Length < 2) day = "0" + day;
            while (hour.Length < 2) hour = "0" + hour;
            while (min.Length < 2) min = "0" + min;
            while (sec.Length < 2) sec = "0" + sec;
            return year + "/" + month + "/" + day + " " + hour + ":" + min + ":" + sec;
        }

        List<String> CARD = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "+2", "X", "R", "O", "+4"];
        private Dictionary<Color, int> colorPriority = new Dictionary<Color, int>
        {
            { Colors.Red, 1 },
            { Colors.Gold, 2 },
            { Colors.Blue, 3 },
            { Colors.Green, 4 }
        };

        private string _NowTurn = "您";

        public string NowTurn
        {
            get { return _NowTurn; }
            set
            {
                _NowTurn = value;
                RaisePropertyChanged(nameof(NowTurn));
            }
        }


        void Update_Window()
        {

            YourCardArea.Children.Clear();
            for (int i = 0; i < YourCards.Count; i++)
            {
                var button = YourCards[i];
                button.Tag = i;  // 将按钮的 Tag 设置为牌的索引
                YourCardArea.Children.Add(button);
            }

            if (YourCards.Count > 2 || YourCards.Count == 0) UnoColor = Brushes.Black;

            YouRemain.Text = YourCardArea.Children.Count.ToString();
            ComputerRemain.Text = ComputerCardList.Count.ToString();
            if (ComputerCardList.Count == 1) ComputerUno.Foreground = Brushes.Red;
            else ComputerUno.Foreground = Brushes.Black;

        }

        private string _PreCardContent;

        public string PreCardContent
        {
            get { return _PreCardContent; }
            set
            {
                _PreCardContent = value;
                RaisePropertyChanged(nameof(PreCardContent));
            }
        }

        private string _PreCardText = "无";

        public string PreCardText
        {
            get { return _PreCardText; }
            set
            {
                _PreCardText = value;
                RaisePropertyChanged(nameof(PreCardText));
            }
        }


        private Brush _PreCardForeground;

        public Brush PreCardForeground
        {
            get { return _PreCardForeground; }
            set
            {
                _PreCardForeground = value;
                RaisePropertyChanged(nameof(PreCardForeground));
            }
        }

        private string _ComputerMessage;

        public string ComputerMessage
        {
            get { return _ComputerMessage; }
            set
            {
                _ComputerMessage = value;
                RaisePropertyChanged(nameof(ComputerMessage));
            }
        }

        private string _DrawContent="抽牌";

        public string DrawContent
        {
            get { return _DrawContent; }
            set
            {
                _DrawContent = value;
                RaisePropertyChanged(nameof(DrawContent));
            }
        }



        public string Bout
        {
            get { return rank._Bout; }
            set
            {
                rank._Bout = value;
                RaisePropertyChanged(nameof(Bout));
            }
        }

        void init()
        {
            //设置上一张牌为空白
            PreviousCard.Style = (Style)System.Windows.Application.Current.FindResource("RedCardStyle");
            PreviousCard.Content = "";
            PreviousCard.Background = Brushes.White;

            PreCardText = "无";
            PreCardForeground = Brushes.Black;//这个很重要，只有通过更改PreCardForeground属性才能确保TextBlock和Button的前景色同步更新
            
            Bout = "0";

            DrawContent = "抽牌";


            //设置手牌
            //YourCardArea.Visibility = Visibility.Hidden;
            YourCards.Clear();
            ComputerCardList.Clear();
            Update_Window();
        }


        void ChangePreCard(Button target)
        {
            //着色
            PreCardForeground = target.Foreground;//
            PreviousCard.Background = target.Background;
            PreviousCard.Style = target.Style;
            PreviousCard.Content = target.Content;

            PreCardText = "";
            if (PreCardForeground == Brushes.Black) PreCardText = "黑";
            if (PreCardForeground == Brushes.Red) PreCardText = "红";
            if (PreCardForeground == Brushes.Gold) PreCardText = "黄";
            if (PreCardForeground == Brushes.Blue) PreCardText = "蓝";
            if (PreCardForeground == Brushes.Green) PreCardText = "绿";
            PreCardText += " ";
            if (CARD.IndexOf(target.Content.ToString()) <= 9)
            {
                PreCardText += target.Content.ToString();
            }
            else
            {
                if (target.Content.ToString() == "R") PreCardText += "反转";
                if (target.Content.ToString() == "+2") PreCardText += "+2";
                if (target.Content.ToString() == "+4") PreCardText += "+4";
                if (target.Content.ToString() == "O") PreCardText += "万能";
                if (target.Content.ToString() == "X") PreCardText += "禁用";
            }
        }

        //Computer

        List<Button> ComputerCardList = new List<Button>();
        void Add_Computer_Card()
        {
            Random rand = new Random();
            Button newCard = new Button();

            int rnd = rand.Next(108);
            if (rnd >= 100)
            {
                newCard.Style = (Style)System.Windows.Application.Current.FindResource("BlackCardStyle");
                if (rnd < 104)
                {
                    newCard.Content = CARD[13];
                }
                else
                {
                    newCard.Content = CARD[14];
                }
            }
            else
            {
                int color = rand.Next(0, 4);
                rnd += 4;
                rnd /= 8;

                newCard.Content = CARD[rnd];
                newCard.Content = CARD[rnd];
                {
                    if (color == 0) newCard.Style = (Style)System.Windows.Application.Current.FindResource("RedCardStyle");
                    if (color == 1) newCard.Style = (Style)System.Windows.Application.Current.FindResource("YellowCardStyle");
                    if (color == 2) newCard.Style = (Style)System.Windows.Application.Current.FindResource("BlueCardStyle");
                    if (color == 3) newCard.Style = (Style)System.Windows.Application.Current.FindResource("GreenCardStyle");
                }
            }
            ComputerCardList.Add(newCard);
            ComputerMessage = "加牌了";
            Update_Window();
        }





        async void ComputerTurn() // 注意这里使用 async void，但在实际应用中最好避免使用 async void，除非在事件处理器中
        {
            NowTurn = "电脑";
            Bout = (int.Parse(Bout) + 1).ToString();

            int choose = computer.NormalCalc(ref ComputerCardList, PreviousCard, YourCards);

            await Task.Delay(1000); // 异步等待，不会阻塞UI线程


            if (choose == -1)
            {
                Add_Computer_Card();
                ComputerMessage = "不出牌";
                if (ComputerCardList[ComputerCardList.Count - 1].Content == PreCardContent || ComputerCardList[ComputerCardList.Count - 1].Foreground == PreCardForeground)
                {
                    choose = ComputerCardList.Count - 1;
                    ComputerMessage = "刚好抓到一张能出的！";
                }
            }


            if (choose != -1)
            {
                if (CARD.IndexOf(ComputerCardList[choose].Content.ToString()) >= 10)
                {
                    Button button = ComputerCardList[choose];
                    ChangePreCard(ComputerCardList[choose]);
                    ComputerCardList.RemoveAt(choose);
                    Update_Window();

                    if (button.Content == "+2")
                    {
                        Add_Card();
                        Add_Card();
                    }
                    if (button.Content == "+4")
                    {
                        Add_Card();
                        Add_Card();
                        Add_Card();
                        Add_Card();
                    }
                    ComputerMessage = "还是我的回合";
                    ComputerTurn();


                }
                else
                {
                    ChangePreCard(ComputerCardList[choose]);
                    ComputerCardList.RemoveAt(choose);
                    ComputerMessage = "";
                }
            }


            if (ComputerCardList.Count == 0)
            {
                rank.Finish();
                MessageBox.Show("我赢了！\n进行了"+ Bout + "回合\n用时"+rank.minute.ToString()+"分钟"+rank.second+"秒");
                ComputerMessage = "我赢了";

                init();
                Playing = "开始";
                return;
            }

            NowTurn = "您";
            Bout = (int.Parse(Bout) + 1).ToString();
            Update_Window();
        }


        //Human

        List<Button> YourCards = new List<Button>();


        private void Card_Click(object sender, EventArgs e)
        {


            if (NowTurn == "电脑") return;
            Button ClickedButtom = sender as Button;
            if (ClickedButtom == null) return;
            int tag = (int)ClickedButtom.Tag;


            bool jump = false;
            if (CARD.IndexOf(ClickedButtom.Content.ToString()) >= 10) jump = true;


            //判断能不能出牌
            bool canPlay = false;

            if (PreCardContent == "" || PreCardForeground == Brushes.Black || ClickedButtom.Foreground == Brushes.Black)
            {
                canPlay = true;
            }
            if (ClickedButtom.Foreground == PreviousCard.Foreground || ClickedButtom.Content == PreviousCard.Content)
            {
                canPlay = true;
            }
            if (!canPlay)
            {
                MessageBox.Show("这张牌出不了");
                return;
            }

            //效果
            if (ClickedButtom.Content == "+2")
            {
                Add_Computer_Card();
                Add_Computer_Card();
            }
            if (ClickedButtom.Content == "+4")
            {
                Add_Computer_Card();
                Add_Computer_Card();
                Add_Computer_Card();
                Add_Computer_Card();
            }


            if (YourCardArea.Children.Count == 2 && UnoColor != Brushes.Red)
            {
                MessageBox.Show("Uno？ohno，你没说UNO！");
                Add_Card();
                Add_Card();
            }


            ChangePreCard(ClickedButtom);


            YourCards.RemoveAt(tag);

            if (YourCards.Count == 0)
            {
                rank.Finish();
                //string winMessage=
                MessageBox.Show("你赢了！\n进行了"+ Bout + "回合\n用时"+rank.minute.ToString()+"分钟"+rank.second+"秒");


                init();
                Playing = "开始";
                return;
            }

            if (jump == false)
            {
                Update_Window();
                ComputerTurn();
            }
            else
            {
                NowTurn = "您";
                Bout = (int.Parse(Bout) + 1).ToString();
                Update_Window();
            }

            DrawContent = "抽牌";
            
        }


        void Add_Card()
        {
            Random rand = new Random();
            Button newCard = new Button();

            int rnd = rand.Next(108);
            if (rnd >= 100)
            {
                newCard.Style = (Style)System.Windows.Application.Current.FindResource("BlackCardStyle");
                if (rnd < 104)
                {
                    newCard.Content = CARD[13];
                }
                else
                {
                    newCard.Content = CARD[14];
                }
            }
            else
            {
                int color = rand.Next(0, 4);
                rnd += 4;
                rnd /= 8;

                newCard.Content = CARD[rnd];
                {
                    if (color == 0) newCard.Style = (Style)System.Windows.Application.Current.FindResource("RedCardStyle");
                    if (color == 1) newCard.Style = (Style)System.Windows.Application.Current.FindResource("YellowCardStyle");
                    if (color == 2) newCard.Style = (Style)System.Windows.Application.Current.FindResource("BlueCardStyle");
                    if (color == 3) newCard.Style = (Style)System.Windows.Application.Current.FindResource("GreenCardStyle");
                }
            }

            newCard.Click += Card_Click;
            YourCards.Add(newCard);
            SortCard();

            Update_Window();

        }




        void SortCard()
        {
            if (YourCards.Count == 0) return;
            YourCards.Sort((b1, b2) =>
            {
                Color color1 = ((SolidColorBrush)b1.Foreground).Color; // 从 Brush 提取 Color
                Color color2 = ((SolidColorBrush)b2.Foreground).Color;

                int colorCompare = GetColorPriority(color1).CompareTo(GetColorPriority(color2));
                if (colorCompare != 0) return colorCompare;

                return CARD.IndexOf(b1.Content.ToString()).CompareTo(CARD.IndexOf(b2.Content.ToString()));
            });

            Update_Window();
        }

        private int GetColorPriority(Color color)
        {
            if (colorPriority.TryGetValue(color, out int priority))
            {
                return priority;
            }
            else
            {
                // 处理颜色未找到的情况，可能返回一个默认值或抛出异常
                return int.MaxValue; // 作为示例，这里我们给予未找到的颜色最低的优先级
            }
        }



        public MainWindow()
        {
            InitializeComponent();
            syzp.Text = TurnVertical(syzp.Text);
            CompileTime.Text = "CompileTime:" + GetCompileVersion();
            this.DataContext = this;
            init();
        }


        private string _Playing = "开始";

        public string Playing
        {
            get { return _Playing; }
            set
            {
                _Playing = value;
                RaisePropertyChanged(nameof(Playing));
            }
        }





        private void StartButtom_Click(object sender, RoutedEventArgs e)
        {
            if (Playing.ToString() == "重来")
            {
                Playing = "开始";
                NowTurn = "";
                init();
            }
            else
            {
                rank.Start();
                Playing = "重来";
                NowTurn = "您";
                Bout = "1";
                for (int i = 0; i < 5; i++)
                {
                    Add_Computer_Card();
                    //Thread.Sleep(300);
                }


                for (int i = 0; i < 5; i++)
                {
                    Add_Card();
                    //Thread.Sleep(300);
                }

            }
            ComputerMessage = "";
        }

        private void DrawButtom_Click(object sender, RoutedEventArgs e)
        {
            if (NowTurn == "电脑" || Playing == "开始") return;
            if(DrawContent=="抽牌")
            {
                DrawContent = "不出";
                Add_Card();
            }
            else
            {
                DrawContent = "抽牌";
                ComputerTurn();
            }
            

        }

        private void SortButtom_Click(object sender, RoutedEventArgs e)
        {
            SortCard();
        }

        private void SubmissionButtom_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("直面弱小，永不言败！");
        }

        private Brush _UnoColor = Brushes.Black;
        public Brush UnoColor
        {
            get { return _UnoColor; }
            set
            {
                _UnoColor = value;
                RaisePropertyChanged(nameof(UnoColor));
            }
        }


        private void UnoButtom_Click(object sender, RoutedEventArgs e)
        {
            if (YourCards.Count != 2)
            {
                if (YourCards.Count > 2) MessageBox.Show("你现在还有很多牌呢");
                return;
            }
            UnoColor = Brushes.Red;
        }
    }
}