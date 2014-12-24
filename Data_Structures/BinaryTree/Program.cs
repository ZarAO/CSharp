namespace BinaryTreeTest
{
    class Program
    {
        static void Main()
        {
            var tree = new BinaryTree();
            tree.Insert(20);
            tree.Insert(40);
            tree.Insert(10);
            tree.Insert(30);
            tree.Insert(80);
            tree.Insert(29);
            tree.Insert(31);
            tree.Insert(32);
            tree.Insert(70);
            BinaryTreeExtensions.Print(tree);
            tree.Remove(40);
            BinaryTreeExtensions.Print(tree);
            tree.Remove(20);
            BinaryTreeExtensions.Print(tree);
        }
    }
}
