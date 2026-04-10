using System;
using Avalonia.Controls;
using DynamicFileExplorer.Infrastructures;

namespace DynamicFileExplorer.UI.Components;
public partial class HeaderContainer : UserControl
{
    readonly Button backButton, forwardButton,backwardButton;
    readonly FileManager fileManager;

    public HeaderContainer()
    {
        InitializeComponent();
        fileManager = App.Current.fileManager;
        backButton = this.FindControl<Button>("BackButton")
            ?? throw new Exception("No se encontró BackButton");

        forwardButton = this.FindControl<Button>("ForwardButton")
            ?? throw new Exception("No se encontró ForwardButton");
        backwardButton = this.FindControl<Button>("BackwardButton")
            ?? throw new Exception("No se encontró ForwardButton");
        forwardButton.Click += (_, _) => fileManager.GoForward();
        backwardButton.Click += (_, _) => fileManager.GoBackward();
        backButton.Click += (_, _) => fileManager.GoBack();

    }
}