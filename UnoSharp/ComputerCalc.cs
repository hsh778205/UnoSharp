using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace UnoSharp
{
    class ComputerCalc
    {
        public ComputerCalc() { }

        public int NormalCalc(ref List<Button> cards, Button precard, List<Button> yourcards)
        {
            if (yourcards.Count <= 2 || cards.Count == 2)
            {
                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].Foreground == Brushes.Black) return i;
                }
            }
            Random rand = new Random();
            if (precard.Foreground == Brushes.Black)
            {
                return rand.Next(0, cards.Count);
            }


            for (int i = 0; i < cards.Count; i++)
            {
                if (cards[i].Foreground == precard.Foreground)
                {
                    return i;
                }
                if (cards[i].Content == precard.Content)
                {
                    return i;
                }
            }


            return -1;
        }
    }
}
