using UnityEditor.IMGUI.Controls;
namespace Editor
{
    public class UIWindowTreeViewItem : TreeViewItem<int>
    {
        public int Rank;
        public string WindowName;
        public int WidgetNum;
        public string ParentNodeName;
        public long UsedCount;
        public long UsedTimestamp;
        public bool IsActive;
        public bool IsPermanent;

        public UIWindowTreeViewItem()
        {
        
        }
    }
}