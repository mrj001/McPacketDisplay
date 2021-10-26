using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace McPacketDisplay.Views
{
    public partial class MineCraftPacketFilterControl : UserControl
    {
        public MineCraftPacketFilterControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}