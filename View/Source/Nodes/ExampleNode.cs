using Stage.Core;

namespace View.Nodes
{
    public class DoubleNode : Node
    {
        public float InFloatValue;
        public float OutFloatValue;
        
        public DoubleNode()
            : base(new Vector2(200.0f, 300.0f), new Vector2(400.0f))
        {
            RegisterInput(nameof(InFloatValue), InFloatValue);
            RegisterOutput(nameof(OutFloatValue), OutFloatValue);
        }

        public DoubleNode(Vector2 pos)
            : base(new Vector2(200.0f, 300.0f), pos)
        {
            RegisterInput(nameof(InFloatValue), InFloatValue);
            RegisterOutput(nameof(OutFloatValue), OutFloatValue);
        }

        public override void Process()
        {
            ModifyOutput(nameof(OutFloatValue), InFloatValue * 2.0f);
        }

        public override string ToString()
        {
            return "Double Node";
        }
    }
}
