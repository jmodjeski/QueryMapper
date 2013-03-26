using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QueryMapper
{
    public class ScopeStack<T>
        : IEnumerable<T>
    {
        private Stack<T> m_stack;

        public ScopeStack()
        {
            m_stack = new Stack<T>();
        }

        public IDisposable Push(T item)
        {
            m_stack.Push(item);

            return new DisposablePop<T>(m_stack, m_stack.Peek());
        }

        public T Peek()
        {
            return m_stack.Peek();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_stack.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return m_stack.GetEnumerator();
        }

        private class DisposablePop<TItem>
            : IDisposable
        {
            private Stack<TItem> m_stack;
            private TItem m_current;

            public DisposablePop(Stack<TItem> stack, TItem current)
            {
                m_stack = stack;
                m_current = current;
            }

            public void Dispose()
            {
                if (m_current.Equals(m_stack.Peek()))
                    m_stack.Pop();
            }
        }
    }
}
