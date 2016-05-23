//C# Example
using UnityEditor;
using UnityEngine;

public class PixelCanvas : EditorWindow
{
    // Add menu item named "Pixel Canvas" to the Window menu
    [MenuItem("Window/Pixel Canvas")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(PixelCanvas));
    }
    
    void OnGUI()
    {
        
    }
}
