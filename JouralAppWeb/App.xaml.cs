using JouralAppWeb.Database;

namespace JouralAppWeb
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Database = new AppDbContext();
        }
        public static AppDbContext Database { get; private set; } = null!;
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "JouralAppWeb" };
        }
    }
}
