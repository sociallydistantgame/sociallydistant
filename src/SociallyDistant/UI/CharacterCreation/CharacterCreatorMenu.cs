using System.Runtime.InteropServices;
using AcidicGUI.Layout;
using AcidicGUI.TextRendering;
using AcidicGUI.Widgets;
using Microsoft.Xna.Framework;
using SociallyDistant.Architecture;
using SociallyDistant.Core;
using SociallyDistant.Core.Core;
using SociallyDistant.Core.Modules;
using SociallyDistant.Core.Shell;
using SociallyDistant.Core.Shell.Windowing;
using SociallyDistant.Core.UI.Common;
using SociallyDistant.Core.UI.Effects;
using SociallyDistant.Core.UI.VisualStyles;
using SociallyDistant.GamePlatform;
using SociallyDistant.UI.Common;

namespace SociallyDistant.UI.CharacterCreation;

public class CharacterCreatorMenu : Widget
{
    private readonly SociallyDistantGame game;
    private readonly TextWidget          title                  = new();
    private readonly StackPanel          root                   = new();
    private readonly Box                 decoration             = new();
    private readonly Box                 decorationInner        = new();
    private readonly InfoBox             mainBox                = new();
    private readonly StackPanel          tutorialPrompt         = new();
    private readonly TextWidget          tutorialPromptText     = new();
    private readonly WrapPanel           tutorialPromptButtons  = new();
    private readonly TextButton          enableTutorialsButton  = new();
    private readonly TextButton          disableTutorialsButton = new();
    private readonly StackPanel          lifepathSection        = new();
    private readonly TextWidget          lifepathPrompt         = new();
    private readonly LifepathList        lifepaths              = new();
    private readonly SimpleField         nameField              = new();
    private readonly SimpleField         computerNameField      = new();
    private readonly SimpleField         genderField            = new();
    private readonly InputField          nameEntry              = new();
    private readonly InputField          computerNameEntry      = new();
    private readonly StringDropdown      genderDropdown         = new();
    private readonly TextWidget          userPrompt             = new();
    private readonly StackPanel          userInfoArea           = new();
    private readonly TextButton          createAccountButton    = new();
    private readonly TextWidget          errorText              = new();
    private          LifepathAsset?      chosenLifepath;
    private          State               state;
    private          bool                useTutorialsInWorld = false;
    
    internal CharacterCreatorMenu(SociallyDistantGame game)
    {
        this.game = game;

        errorText.SetCustomProperty(WidgetForegrounds.Common);
        errorText.SetCustomProperty(CommonColor.Red);
        
        nameField.TExt = "Your name:";
        computerNameField.TExt = "Computer name:";
        genderField.TExt = "Pronoun preference:";
        userPrompt.WordWrapping = true;
        userPrompt.Text = @"Please enter the following information to create a new user account.";
        createAccountButton.Text = "Continue";
        createAccountButton.HorizontalAlignment = HorizontalAlignment.Left;
        userInfoArea.Spacing = 3;
        userInfoArea.Direction = Direction.Vertical;
        lifepathPrompt.WordWrapping = true;
        lifepathPrompt.Text = @"Your Lifepath determines your character’s morals and ethics. Each Lifepath also has a set of unique missions and dialog choices. Once you choose your Lifepath, there’s no going back.";
        lifepathSection.Spacing = 3;
        tutorialPrompt.Spacing = 3;
        tutorialPromptText.WordWrapping = true;
        tutorialPromptButtons.Direction = Direction.Horizontal;
        tutorialPromptButtons.SpacingX = 3;
        root.HorizontalAlignment = HorizontalAlignment.Left;
        root.MinimumSize = new Point(480, 0);
        root.MaximumSize = new Point(480, 0);
        root.VerticalAlignment = VerticalAlignment.Middle;
        enableTutorialsButton.Text = "Yes";
        disableTutorialsButton.Text = "No, thanks";
        
        title.Text = "Create New User";
        title.FontWeight = FontWeight.Medium;
        title.FontSize = 36;

        decoration.Margin = 1;
        decoration.SetCustomProperty(WidgetBackgrounds.WindowBorder);
        decorationInner.Margin = 3;
        decorationInner.SetCustomProperty(WidgetBackgrounds.Overlay);
        
        Children.Add(root);
        root.ChildWidgets.Add(title);
        root.ChildWidgets.Add(decoration);
        decoration.Content = decorationInner;
        decorationInner.Content = mainBox;

        tutorialPromptText.Text = @"If this is your first time playing the game, you may want to enable in-game tutorials. These tutorials will help walk you through the game’s interface and mechanics as you play through the prologue mission.

Would you like to enable tutorials for this account?";
        
        tutorialPrompt.ChildWidgets.Add(tutorialPromptText);
        tutorialPrompt.ChildWidgets.Add(tutorialPromptButtons);
        tutorialPromptButtons.ChildWidgets.Add(enableTutorialsButton);
        tutorialPromptButtons.ChildWidgets.Add(disableTutorialsButton);
        
        lifepathSection.ChildWidgets.Add(lifepathPrompt);
        lifepathSection.ChildWidgets.Add(lifepaths);

        nameField.Content = nameEntry;
        computerNameField.Content = computerNameEntry;
        genderField.Content = genderDropdown;
        
        userInfoArea.ChildWidgets.Add(userPrompt);
        userInfoArea.ChildWidgets.Add(nameField);
        userInfoArea.ChildWidgets.Add(computerNameField);
        userInfoArea.ChildWidgets.Add(genderField);
        userInfoArea.ChildWidgets.Add(errorText);
        userInfoArea.ChildWidgets.Add(createAccountButton);
        
        enableTutorialsButton.ClickCallback = EnableTutorials;
        disableTutorialsButton.ClickCallback = DisableTutorials;
        lifepaths.OnSelected += SelectLifepath;
        
        genderDropdown.Items.Add("They / Them");
        genderDropdown.Items.Add("He / Him");
        genderDropdown.Items.Add("She / Her");

        genderDropdown.SelectedIndex = 0;

        createAccountButton.ClickCallback = CreateAccount;
    }

    private async void CreateAccount()
    {
        if (chosenLifepath == null)
        {
            errorText.Text = "You need to choose a Lifepath first.";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(nameEntry.Value))
        {
            errorText.Text = "You must have a name. Please try again.";
            return;
        }
        
        if (string.IsNullOrWhiteSpace(computerNameEntry.Value))
        {
            errorText.Text = "Your computer must have a name. Please try again.";
            return;
        }

        var dialog = game.Shell.CreateMessageDialog("Please wait...");
        dialog.Message = "Creating your account...";

        var playerInfo = new PlayerInfo();
        playerInfo.Name = nameEntry.Value;
        playerInfo.HostName = computerNameEntry.Value;
        playerInfo.Comment = "Prologue";
        playerInfo.UserName = nameEntry.Value.ToUnix();
        playerInfo.PlayerGender = (Gender)genderDropdown.SelectedIndex;

        WorldManager.Instance.WipeWorld();

        var world = WorldManager.Instance.World;
        world.ChangePlayerLifepath(chosenLifepath);

        if (useTutorialsInWorld)
            world.WorldFlags.Add("TUTORIALS_ENABLED");
        
        var gameData = await LocalGameData.CreateNewGame(playerInfo, WorldManager.Instance.World);

        dialog.Close();

        await game.ContentManager.RefreshContentDatabaseAsync();

        await game.StartGame(gameData);
    }
    
    private Task<bool> ConfirmLifepath()
    {
        var source = new TaskCompletionSource<bool>();
        var dialog = game.Shell.CreateMessageDialog("Confirm Lifepath Selection");
        dialog.Message = "Are you sure you want to choose this Lifepath? Once you've chosen a Lifepath, you cannot change it again for this character.";
        dialog.MessageType = MessageBoxType.Warning;
        dialog.Buttons.Add(new MessageBoxButtonData("Yes", MessageDialogResult.Yes));
        dialog.Buttons.Add(new MessageBoxButtonData("No",  MessageDialogResult.No));

        dialog.DismissCallback = (result) => source.SetResult(result == MessageDialogResult.Yes);
        
        return source.Task;
    }
    
    private async void SelectLifepath(LifepathAsset asset)
    {
        var confirmed = await ConfirmLifepath();
        if (!confirmed)
            return;
        
        chosenLifepath = asset;
        state = State.UserInformation;
        UpdateView();
    }

    private void DisableTutorials()
    {
        useTutorialsInWorld = false;
        state = State.LifepathChoice;
        UpdateView();
    }

    private void EnableTutorials()
    {
        useTutorialsInWorld = true;
        state = State.LifepathChoice;
        UpdateView();
    }

    public void Start()
    {
        this.RenderEffect = BackgroundBlurWidgetEffect.GetEffect(game);
        decorationInner.RenderEffect = BackgroundBlurWidgetEffect.GetEffect(game);
        state = State.Tutorials;
        UpdateView();
    }

    private void UpdateView()
    {
        switch (state)
        {
            case State.Tutorials:
            {
                mainBox.TitleText = "Welcome to Socially Distant";
                mainBox.Content = tutorialPrompt;
                break;
            }
            case State.LifepathChoice:
            {
                mainBox.TitleText = "Choose your LIfepath";
                mainBox.Content = lifepathSection;
                lifepaths.SetItems(game.ContentManager.GetContentOfType<LifepathAsset>());
                break;
            }
            case State.UserInformation:
            {
                mainBox.TitleText = "Enter User Information";
                mainBox.Content = userInfoArea;
                break;
            }
        }
    }

    private enum State
    {
        Tutorials,
        LifepathChoice,
        UserInformation,
        
    }
}