using GalaSoft.MvvmLight;
using System.Collections.Generic;
using SharpDX;

namespace IvyGenerator.Utilities
{
    public class UndoRedoManager<T>
    {
        private static int DEFAULT_CAPACITY = 10;

        protected T subject;

        private RoundStack<IMemento<T>> undoStack;
        private RoundStack<IMemento<T>> redoStack;

        public UndoRedoManager(T subject)
            : this(subject, DEFAULT_CAPACITY) {}

        public UndoRedoManager(T subject, int capacity)
        {
            this.subject = subject;
            undoStack = new RoundStack<IMemento<T>>(capacity);
            redoStack = new RoundStack<IMemento<T>>(capacity);
        }

        public bool CanUndo
        {
            get { return undoStack.Count != 0; }
        }

        public bool CanRedo
        {
            get { return redoStack.Count != 0; }
        }

        public void Do(IMemento<T> m)
        {
            redoStack.Clear();
            undoStack.Push(m);
        }

        public void Undo()
        {
            IMemento<T> top = undoStack.Pop();
            redoStack.Push(top.Restore(subject));
        }

        public void Redo()
        {
            IMemento<T> top = redoStack.Pop();
            undoStack.Push(top.Restore(subject));
        }
    }
}