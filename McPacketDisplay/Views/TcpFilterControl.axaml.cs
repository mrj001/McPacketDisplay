using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace McPacketDisplay.Views
{
    public partial class TcpFilterControl : UserControl
    {
        public TcpFilterControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}