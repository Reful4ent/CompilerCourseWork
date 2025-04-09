using System.IO;
using System.Windows;
using System.Windows.Controls;
using CourseWorkCompiller.ViewModel;
using Microsoft.Win32;

namespace CourseWorkCompiller.View;

public partial class MainWindow : Window
{
    private SaveFileDialog saveFileDialog;
    private OpenFileDialog openFileDialog;
    private string fileName = string.Empty;
    
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new MainVM();
        
        saveFileDialog = new SaveFileDialog();
        saveFileDialog.DefaultExt = ".txt";
        saveFileDialog.Filter = "Text documents (.txt)|*.txt";
        
        openFileDialog = new OpenFileDialog();
        openFileDialog.DefaultExt = ".txt";
        openFileDialog.Filter = "Text documents (.txt)|*.txt";

        Closing += MenuExit_OnClickMenu;
    }

    private void TextBlockScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (e.VerticalChange != 0)
        {
            textBoxScrollViewer.ScrollToVerticalOffset(textBlockScrollViewer.VerticalOffset);
        }
    }

    private void TextBoxScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
    {
        if (e.VerticalChange != 0)
        {
            textBlockScrollViewer.ScrollToVerticalOffset(textBoxScrollViewer.VerticalOffset);
        }
    }

    private void ButtonCopy_OnClick(object sender, RoutedEventArgs e)
    {
        codeBox.Copy();
    }

    private void ButtonCut_OnClick(object sender, RoutedEventArgs e)
    {
        codeBox.Cut();
    }

    private void ButtonInsert_OnClick(object sender, RoutedEventArgs e)
    {
        codeBox.Paste();
    }

    private void MenuHelp_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "Help.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }

    private void ButtonSave_OnClick(object sender, RoutedEventArgs e)
    {
       SaveFile(sender, e);
    }

    private void ButtonOpen_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(fileName) || !string.IsNullOrEmpty(codeBox.Text))
        {
            MessageBoxResult result = MessageBox.Show(
                "Хотите сохранить файл перед открытием нового файла?",
                "Подтверждение",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveFile(sender, e);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return;
            }
        }

        OpenFileDialog openFileDialog = new OpenFileDialog();
        if (openFileDialog.ShowDialog() == true)
        {
            codeBox.Text = File.ReadAllText(openFileDialog.FileName);
            fileName = openFileDialog.FileName;
        }
    }
    
    private void SaveFile(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            SaveFileAs(sender, e);
        }
        else
        {
            File.WriteAllText(fileName, codeBox.Text);
        }
    }
    
    private void SaveFileAs(object sender, RoutedEventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt",
            DefaultExt = ".txt"
        };
        if (saveFileDialog.ShowDialog() == true)
        {
            File.WriteAllText(saveFileDialog.FileName, codeBox.Text);
            fileName = saveFileDialog.FileName;
        }
    }

    private void ButtonCreate_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(fileName) || !string.IsNullOrEmpty(codeBox.Text))
        {
            MessageBoxResult result = MessageBox.Show(
                "Хотите сохранить файл перед созданием нового файла?",
                "Подтверждение",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveFile(sender, e);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return;
            }
        }

        codeBox.Clear();
        fileName = string.Empty;
    }

    private void MenuSelect_OnClick(object sender, RoutedEventArgs e)
    {
        codeBox.SelectAll();
    }

    private void MenuDelete_OnClick(object sender, RoutedEventArgs e)
    {
        codeBox.Clear();
    }

    private void MenuExit_OnClick(object sender, RoutedEventArgs e)
    {
        if (!string.IsNullOrEmpty(fileName) || !string.IsNullOrEmpty(codeBox.Text))
        {


            MessageBoxResult result = MessageBox.Show(
                "У вас есть несохраненные изменения. Хотите сохранить файл перед выходом?",
                "Подтверждение выхода",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveFileAs(sender, e);
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return;
            }
        }


        Application.Current.Shutdown();
    }

    private void ButtonSaveAs_OnClick(object sender, RoutedEventArgs e)
    {
        SaveFileAs(sender, e);
    }

    private void MenuAboutProgram_OnClick(object sender, RoutedEventArgs e)
    {
        AboutProgramWindow aboutProgramWindow = new AboutProgramWindow();
        aboutProgramWindow.ShowDialog();
    }
    
    private void MenuExit_OnClickMenu(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!string.IsNullOrEmpty(fileName) || !string.IsNullOrEmpty(codeBox.Text))
        {
            MessageBoxResult result = MessageBox.Show(
                "У вас есть несохраненные изменения. Хотите сохранить файл перед выходом?",
                "Подтверждение выхода",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                SaveFileAs(sender, new RoutedEventArgs());
            }
            else if (result == MessageBoxResult.Cancel)
            {
                return;
            }
        }


        Application.Current.Shutdown();
    }

    private void ButtonUndo_OnClick(object sender, RoutedEventArgs e)
    {
        codeBox.Undo();
    }

    private void ButtonRedo_OnClick(object sender, RoutedEventArgs e)
    {
        codeBox.Redo();
    }

    private void Task_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "Task.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }

    private void Grammar_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "Grammar.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }

    private void Homsk_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "Homsk.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }

    private void Analysis_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "Analysis.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }

    private void Diagnosis_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "Diagnosis.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }

    private void Test_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "Test.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }

    private void Literature_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "Literature.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }

    private void Code_OnClick(object sender, RoutedEventArgs e)
    {
        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        string fullPath = Path.Combine(projectDirectory, "View", "CodeProgram.html");


        if (File.Exists(fullPath))
        {
            HelpWindow helpWindow = new HelpWindow(fullPath);
            helpWindow.ShowDialog();
        }
        else
        {
            MessageBox.Show($"Файл не найден: {fullPath}");
        }
    }
}