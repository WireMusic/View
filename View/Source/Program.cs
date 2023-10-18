using View;
using Stage.Core;
using View.Properties;

Thread.CurrentThread.SetApartmentState(ApartmentState.Unknown);
Thread.CurrentThread.SetApartmentState(ApartmentState.STA);

ApplicationSpecification spec = new ApplicationSpecification();
spec.Name = "View";
spec.CommandLineArgs = args;
spec.MaximiseWindow = true;

spec.DefaultFontSize = 18.0f;
spec.DefaultFontData = Resources.OpenSans_Regular;
spec.BoldFontData = Resources.OpenSans_Bold;

spec.DefaultFontSizes = new[] {
    22.0f
};

spec.BoldFontSizes = new[] {
    22.0f,
    33.0f,
    45.0f
};

spec.LoadFontOnNewSize = false;

spec.GLFW = Resources.glfw3;
spec.cimgui = Resources.cimgui;
spec.OtherImplementations = Resources.mview;
spec.OtherImplementationsName = "mview";

Application app = new Application(spec);
EditorLayer editor = new EditorLayer();

app.AddLayer(editor);

app.Run();

app.Shutdown();
