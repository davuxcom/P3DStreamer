﻿using System.Windows;
using System.Windows.Media;

namespace P3DStreamer.Extensions
{
    static class TreeHelper
    {
        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                var childAsT = child as T;
                if (childAsT != null)
                    return childAsT;
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static T FindAncestorOrSelf<T>(this DependencyObject obj)
        where T : DependencyObject
        {
            while (obj != null)
            {
                T objTest = obj as T;
                if (objTest != null)
                    return objTest;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

        public static T FindAncestorOrSelf<T>(DependencyObject obj, string name)
        where T : FrameworkElement
        {
            while (obj != null)
            {
                if (obj is T objTest && objTest.Name == name)
                    return objTest;
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }
    }
}
