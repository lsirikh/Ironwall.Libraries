using System;
using System.Windows.Controls;
using System.Windows;

namespace Ironwall.Libraries.Dotnet.Ollama.Ui.Behaviors;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 1/14/2025 10:45:01 AM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public static class RichTextBoxScrollBehavior
{
    public static readonly DependencyProperty AutoScrollProperty =
        DependencyProperty.RegisterAttached(
            "AutoScroll",
            typeof(bool),
            typeof(RichTextBoxScrollBehavior),
            new PropertyMetadata(false, OnAutoScrollChanged));

    public static bool GetAutoScroll(DependencyObject obj) =>
        (bool)obj.GetValue(AutoScrollProperty);

    public static void SetAutoScroll(DependencyObject obj, bool value) =>
        obj.SetValue(AutoScrollProperty, value);

    private static void OnAutoScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RichTextBox richTextBox && e.NewValue is true)
        {
            richTextBox.TextChanged += (s, args) =>
            {
                richTextBox.ScrollToEnd();
            };
        }
    }
}