using System.Windows;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Web.WebView2.Core;
using System.Windows;

namespace CourseWorkCompiller.View;

public partial class HelpWindow : Window
{
    public HelpWindow(string path)
    {
        InitializeComponent();
        InitializeWebView(path);
    }
    
    private async void InitializeWebView(string path)
    {
        var env = await CoreWebView2Environment.CreateAsync(null, "C:/temp/WebView2Data");
        await webView.EnsureCoreWebView2Async(env);
        webView.Source = new System.Uri(path);
    }
}