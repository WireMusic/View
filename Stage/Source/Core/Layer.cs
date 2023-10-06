namespace Stage.Core
{
    public abstract class Layer : ILayer
    {
        private string m_Name;
        public string Name => m_Name;

        

        public Layer(string name)
        {
            m_Name = name;
        }

        public virtual void OnAttach()
        {
        }

        public virtual void OnDetach()
        {
        }

        public virtual void OnUI()
        {
        }

        public virtual void OnUpdate()
        {
        }
    }
}
