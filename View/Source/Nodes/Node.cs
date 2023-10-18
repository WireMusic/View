using Stage.Core;
using System.Reflection;

namespace View.Nodes
{
    public interface INode
    {
        void Process();

        Vector2 GetPosition();
        Vector2 GetSize();
        void MoveBy(Vector2 delta);

        void SetInput(string name, object value);
        object GetOutput(string name);

        string ToString();
    }

    public abstract class Node : INode
    {
        public string Name { get; set; }

        private Vector2 Size;
        private Vector2 Position;

        private Dictionary<string, object> Inputs;
        private Dictionary<string, object> Outputs;

        public Node(Vector2 size, Vector2 position = new Vector2())
        {
            Size = size;
            Position = position;
            Name = string.Empty;

            Inputs = new Dictionary<string, object>();
            Outputs = new Dictionary<string, object>();
        }

        public abstract void Process();

        public Vector2 GetPosition() => Position;

        public Vector2 GetSize() => Size;

        public void MoveBy(Vector2 delta)
        {
            Position += delta;
        }

        public void SetInput(string name, object value)
        {
            Inputs[name] = value;
        }

        public object GetOutput(string name)
        {
            return Outputs[name];
        }

        protected void RegisterInput(string name, object input)
        {
            Inputs.Add(name, input);
        }

        protected void RegisterOutput(string name, object output)
        {
            Outputs.Add(name, output);
        }

        protected void ModifyOutput(string name, object value)
        {
            try
            {
                Outputs[name] = value;
            }
            catch
            {
                throw;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class VarNode<T> : INode where T : notnull
    {
        public string Name { get; set; }

        private Vector2 Size;
        private Vector2 Position;

        private T _value;

        public VarNode(string name, T value)
        {
            Name = name;
            _value = value;

            Size = new Vector2(50.0f, 50.0f);
            Position = new Vector2();
        }

        public void Process()
        {
            // Variables don't need to be processed.
        }

        public Vector2 GetPosition() => Position;
        public Vector2 GetSize() => Size;

        public void MoveBy(Vector2 vector)
        {
            Position += vector;
        }

        public void SetInput(string name, object value)
        {
            // Cannot set input, as there is none.
            throw new InvalidOperationException(); // Change?
        }

        public object GetOutput(string name)
        {
            return _value;
        }

        public T GetValue()
        {
            return _value;
        }

        public void SetValue(T value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

/*

Node's inputs are externally set
Node.Process() sets the outputs
Node's outputs are used in another node
 
*/
