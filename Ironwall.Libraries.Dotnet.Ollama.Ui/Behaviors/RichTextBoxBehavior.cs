using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;

namespace Ironwall.Libraries.Dotnet.Ollama.Ui.Behaviors;
/****************************************************************************
   Purpose      :                                                          
   Created By   : GHLee                                                
   Created On   : 1/13/2025 11:43:51 AM                                                    
   Department   : SW Team                                                   
   Company      : Sensorway Co., Ltd.                                       
   Email        : lsirikh@naver.com                                         
****************************************************************************/
public static class RichTextBoxBehavior
{
    public static readonly DependencyProperty BindableDocumentProperty =
        DependencyProperty.RegisterAttached(
            "BindableDocument",
            typeof(FlowDocument),
            typeof(RichTextBoxBehavior),
            new PropertyMetadata(null, OnBindableDocumentChanged));

    public static FlowDocument GetBindableDocument(DependencyObject obj)
    {
        return (FlowDocument)obj.GetValue(BindableDocumentProperty);
    }

    public static void SetBindableDocument(DependencyObject obj, FlowDocument value)
    {
        obj.SetValue(BindableDocumentProperty, value);
    }

    private static void OnBindableDocumentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is RichTextBox richTextBox)
        {
            richTextBox.Document = e.NewValue as FlowDocument;
        }
    }
}