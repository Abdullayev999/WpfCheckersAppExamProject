using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Checkers.Models
{
    class Checker : ViewModelBase
    {
        private bool isEnabled = true;
        private SolidColorBrush background;
        private SolidColorBrush foreground;
        private string content;
        private string text = "";

        public bool IsEnabled { get => isEnabled; set => Set(ref isEnabled, value); }
        public SolidColorBrush Background { get => background; set => Set(ref background, value); }
        public SolidColorBrush Foreground { get => foreground; set => Set(ref foreground, value); }
        public string Content { get => content; set => Set(ref content, value); }
        public string Text { get => text; set => Set(ref text, value); } 
    }
}
