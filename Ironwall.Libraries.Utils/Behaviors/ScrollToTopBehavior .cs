﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Ironwall.Libraries.Utils.Behaviors
{
    public static class ScrollToTopBehavior
    {
        public static readonly DependencyProperty ScrollToTopProperty =
            DependencyProperty.RegisterAttached
            (
                "ScrollToTop",
                typeof(bool),
                typeof(ScrollToTopBehavior),
                new UIPropertyMetadata(false, OnScrollToTopPropertyChanged)
            );
        public static bool GetScrollToTop(DependencyObject obj)
        {
            return (bool)obj.GetValue(ScrollToTopProperty);
        }
        public static void SetScrollToTop(DependencyObject obj, bool value)
        {
            obj.SetValue(ScrollToTopProperty, value);
        }
        private static void OnScrollToTopPropertyChanged(DependencyObject dpo,
                                                         DependencyPropertyChangedEventArgs e)
        {
            ItemsControl itemsControl = dpo as ItemsControl;
            if (itemsControl != null)
            {
                DependencyPropertyDescriptor dependencyPropertyDescriptor =
                        DependencyPropertyDescriptor.FromProperty(ItemsControl.ItemsSourceProperty, typeof(ItemsControl));
                if (dependencyPropertyDescriptor != null)
                {
                    if ((bool)e.NewValue == true)
                    {
                        dependencyPropertyDescriptor.AddValueChanged(itemsControl, ItemsSourceChanged);
                    }
                    else
                    {
                        dependencyPropertyDescriptor.RemoveValueChanged(itemsControl, ItemsSourceChanged);
                    }
                }
            }
        }
        static void ItemsSourceChanged(object sender, EventArgs e)
        {
            ItemsControl itemsControl = sender as ItemsControl;
            EventHandler eventHandler = null;
            eventHandler = new EventHandler(delegate
            {
                if (itemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
                {
                    ScrollViewer scrollViewer = GetVisualChild<ScrollViewer>(itemsControl) as ScrollViewer;
                    scrollViewer.ScrollToTop();
                    itemsControl.ItemContainerGenerator.StatusChanged -= eventHandler;
                }
            });
            itemsControl.ItemContainerGenerator.StatusChanged += eventHandler;
        }

        private static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
