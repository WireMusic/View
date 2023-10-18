using Stage.Core;

namespace View.Nodes
{
    public class NodeManager
    {
        public Dictionary<string, object> Variables;
        public List<INode> Nodes;

        public NodeManager()
        {
            Variables = new Dictionary<string, object>();
            Nodes = new List<INode>();

            SetupExamplePatch();
        }

        private void SetupExamplePatch()
        {
            Nodes.Add(new DoubleNode());
            Nodes.Add(new DoubleNode(new Vector2(100, 700)));
            //Nodes.Add(new VarNode<float>("Number1", 76.0f));
        }

        public void AddVariable(string name, object value)
        {
            Variables.Add(name, value);
        }

        public void AddNode(INode node)
        {
            Nodes.Add(node);
        }

        public void Update()
        {

        }
    }
}

/*

Update loop:
Any nodes with no inputs start first (variables, other generators);
For each input the nodes' output is connected to, the value of that input is set;
For each of the nodes that the inputs was just set, Process() is called
 (making sure no nodes are processed twice), and the output of the nodes are set;
The inputs of the next nodes are set, and this repeats.

 */
