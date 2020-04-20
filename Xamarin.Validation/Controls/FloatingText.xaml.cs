using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.AttributeValidation.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FloatingText : ContentView
    {
        public FloatingText(Grid grid)
        {
            InitializeComponent();
            LayoutChanged += delegate
            {
                MessageGrid.TranslateTo(MessageGrid.TranslationX - MessageGrid.Width, MessageGrid.TranslationY, 250, Easing.CubicInOut);
            };

            MessagingCenter.Subscribe<View>(this, "Clear_FloatingText", (sender) => grid.Children.Remove(this));

            //var gr = new TapGestureRecognizer();
            //grid.GestureRecognizers.Add(gr);
            //gr.Tapped += delegate
            //{
            //    this.IsVisible = false;
            //};
        }
    }
}