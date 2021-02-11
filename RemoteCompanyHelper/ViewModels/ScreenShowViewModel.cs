using Caliburn.Micro;
using RemoteCompanyHelper.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace RemoteCompanyHelper.ViewModels
{
    public class ScreenShowViewModel : Screen
    {
        public List<Screenshot> Screens;
        private int _position;
        private double _percentSize;
        private string _sizeText;
        private string _curPosText;
        private BitmapSource _curSource;

        public string CurPosText
        {
            get => _curPosText;
            set
            {
                _curPosText = value;
                NotifyOfPropertyChange(() => CurPosText);
            }
        }

        public string SizeText
        {
            get => _sizeText;
            set
            {
                _sizeText = value;
                NotifyOfPropertyChange(() => SizeText);
            }
        }

        public double PercentSize
        {
            get => _percentSize;
            set
            {
                _percentSize = value;
                SizeText = Math.Round(_percentSize*100).ToString() + "%";
                NotifyOfPropertyChange(() => PercentSize);
                NotifyOfPropertyChange(() => CanIncreaseImageSize);
                NotifyOfPropertyChange(() => CanDecreaseImageSize);
            }
        }

        public int Position
        {
            get => _position;
            set
            {
                _position = value;
                CurSource = Screens[_position].Source;
                CurPosText = (_position + 1).ToString() + "/" + Screens.Count;
                NotifyOfPropertyChange(() => CanNextScr);
                NotifyOfPropertyChange(() => CanPrevScr);
            }
        }
        public BitmapSource CurSource
        {
            get => _curSource;
            set
            {
                _curSource = value;
                NotifyOfPropertyChange(() => CurSource);
            }
        }

        public ScreenShowViewModel(List<Screenshot> screenList, int currentItem)
        {
            Screens = screenList;
            Position = currentItem;
            PercentSize = 0.7;
        }
        public bool CanIncreaseImageSize
        {
            get { return PercentSize < 2.0; }
        }
        public bool CanDecreaseImageSize
        {
            get { return PercentSize > 0.05; }
        }
        public void IncreaseImageSize()
        {
            PercentSize += 0.05;
        }

        public void DecreaseImageSize()
        {
            PercentSize -= 0.05;
        }

        public bool CanPrevScr
        {
            get { return Position > 0; }
        }
        public bool CanNextScr
        {
            get { return Screens.Count - 1 > Position; }
        }

        public void PrevScr()
        {
            Position--;
        }
        public void NextScr()
        {
            Position++;
        }

        public void KeyDownCommand(KeyEventArgs a)
        {
            if (a.Key == Key.Right && CanNextScr)
            {
                NextScr();
            }
            else if (a.Key == Key.Left && CanPrevScr)
            {
                PrevScr();
            }
            else
            {
                TryClose();
            }
        }
        public void MouseWheelScroll(MouseWheelEventArgs a)
        {
            if (a.Delta > 0 && CanIncreaseImageSize)
            {
                IncreaseImageSize();
            }
            else if (a.Delta < 0 && CanDecreaseImageSize)
            {
                DecreaseImageSize();
            }
        }
    }
}
