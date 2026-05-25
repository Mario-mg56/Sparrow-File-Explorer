using System;
using Avalonia.Controls;
using DynamicFileExplorer.Helpers;

namespace DynamicFileExplorer.UI.Components;
public partial class HeaderContainer : UserControl
{
    readonly Button backButton, forwardButton, backwardButton;
    readonly CheckBox hideItemsCheckBox;
    private static FileManager? FM {get => App.Current.FocusedTab?.fileManager; }

    public HeaderContainer()
    {
        InitializeComponent();
        backButton = this.FindControl<Button>("BackButton")
            ?? throw new Exception("No se encontró BackButton");

        forwardButton = this.FindControl<Button>("ForwardButton")
            ?? throw new Exception("No se encontró ForwardButton");
        backwardButton = this.FindControl<Button>("BackwardButton")
            ?? throw new Exception("No se encontró ForwardButton");
        hideItemsCheckBox = this.FindControl<CheckBox>("UnHideItems")
            ?? throw new Exception("No se encontró ForwardButton");
        hideItemsCheckBox.IsChecked = App.Current.Cache.Config.DefaultIsHideItems;
        hideItemsCheckBox.IsCheckedChanged += (_,_) => FM?.ChangeHideItems(hideItemsCheckBox.IsChecked);
        forwardButton.Click += (_, _) => FM?.GoForward();
        backwardButton.Click += (_, _) => FM?.GoBackward();
        backButton.Click += (_, _) => FM?.GoBack();

        SettingsIcon.PointerPressed += (_, e) => {
            if(e.GetCurrentPoint(SettingsIcon).Properties.IsLeftButtonPressed)
                App.Current.tabManager.CreateSettingsTab();
        };

    }

}