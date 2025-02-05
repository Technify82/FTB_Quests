using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTB_Quests
{

    public class QuestTreeViewManager
    {
        private TreeView questTreeView;
        public QuestTreeViewManager(TreeView treeView)
        {
            questTreeView = treeView;
        }

        public void InitializeTreeView(EventHandler doubleClickHandler)
        {
            questTreeView.DoubleClick += doubleClickHandler;
        }

        public void LoadFileTree(string rootPath)
        {
            DirectoryInfo rootDirectory = new DirectoryInfo(rootPath);
            TreeNode rootNode = new TreeNode(rootDirectory.Name)
            {
                Tag = rootDirectory
            };
            GetFiles(rootDirectory, rootNode);
            questTreeView.Nodes.Add(rootNode);
        }

        private void GetFiles(DirectoryInfo directoryInfo, TreeNode parentNode)
        {
            foreach (var directory in directoryInfo.GetDirectories())
            {
                TreeNode directoryNode = new TreeNode(directory.Name)
                {
                    Tag = directory
                };
                parentNode.Nodes.Add(directoryNode);

                GetFiles(directory, directoryNode);
            }

            foreach (var file in directoryInfo.GetFiles("*.snbt"))
            {
                TreeNode fileNode = new TreeNode(file.Name)
                {
                    Tag = file
                };
                parentNode.Nodes.Add(fileNode);
            }
        }

        public void FilterNodes(TreeNodeCollection nodes, Action<TreeNode> action)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.EndsWith(".snbt") && !node.Text.Contains("index"))
                {
                    action(node);
                }

                if (node.Nodes.Count > 0)
                {
                    FilterNodes(node.Nodes, action);
                }
            }
        }

        public async Task FilterNodesAsync(TreeNodeCollection nodes, Func<TreeNode, Task> action)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.Text.EndsWith(".snbt") && !node.Text.Contains("index"))
                {
                    await action(node);
                }

                if (node.Nodes.Count > 0)
                {
                    await FilterNodesAsync(node.Nodes, action);
                }
            }
        }
    }
}
