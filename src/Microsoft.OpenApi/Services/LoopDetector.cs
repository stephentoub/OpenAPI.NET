using System;
using System.Collections.Generic;

namespace Microsoft.OpenApi
{
    internal class LoopDetector
    {
        private Dictionary<Type, Stack<object>>? _loopStacks;

        /// <summary>
        /// Maintain history of traversals to avoid stack overflows from cycles
        /// </summary>
        /// <param name="key">Identifier used for current context.</param>
        /// <returns>If method returns false a loop was detected and the key is not added.</returns>
        public bool PushLoop<T>(T key)
        {
            _loopStacks ??= [];

            if (!_loopStacks.TryGetValue(typeof(T), out var stack))
            {
                stack = new();
                _loopStacks.Add(typeof(T), stack);
            }

            if (key is not null && !stack.Contains(key))
            {
                stack.Push(key);
                return true;
            }
            else
            {
                return false;  // Loop detected
            }
        }

        /// <summary>
        /// Exit from the context in cycle detection
        /// </summary>
        public void PopLoop<T>()
        {
            if (_loopStacks?[typeof(T)].Count > 0)
            {
                _loopStacks[typeof(T)].Pop();
            }
        }

        public void SaveLoop<T>(T loop)
        {
            if (!Loops.ContainsKey(typeof(T)))
            {
                Loops[typeof(T)] = new();
            }
            if (loop is not null)
            {
                Loops[typeof(T)].Add(loop);
            }
        }

        private Dictionary<Type, List<object>>? _loops;

        /// <summary>
        /// List of Loops detected
        /// </summary>
        public Dictionary<Type, List<object>> Loops => _loops ??= [];

        /// <summary>
        /// Reset loop tracking stack
        /// </summary>
        internal void ClearLoop<T>()
        {
            _loopStacks?[typeof(T)].Clear();
        }
    }
}
