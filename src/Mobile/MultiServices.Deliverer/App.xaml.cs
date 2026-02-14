namespace MultiServices.Deliverer;
public partial class App : Application
{
    private readonly IServiceProvider _sp;
    public App(IServiceProvider sp) { InitializeComponent(); _sp = sp; }
    protected override Window CreateWindow(IActivationState? a) => new Window(new AppShell(_sp));
}
