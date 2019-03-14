using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace HomeBudget
{
    public abstract class Command
    {
        protected Command(CommandEntryList parent) { this.parent = parent; }

        public abstract void Redo();
        public abstract void Undo();

        protected CommandEntryList parent;
    }

    [Serializable]
    public class CommandEntryList : ObservableCollection<Post>
    {
        public CommandEntryList() : base()
        {
            redoList = new LinkedList<Command>();
            undoList = new LinkedList<Command>();
        }

        public new void Add(Post item)
        {
            redoList.Clear();
            base.Add(item);
            undoList.AddLast(new AddCommand(this, item));          
        }

        public void AddSet(Post[] items)
        {
            redoList.Clear();

            foreach (Post e in items)
                base.Add(e);
            undoList.AddLast(new AddCommand(this, items));
        }

        public new void Clear()
        {
            redoList.Clear();
            undoList.Clear();
            base.Clear();
        }

        public new void RemoveAt(int index)
        {
            Post item = this[index];
            base.RemoveAt(index);
            undoList.AddLast(new RemoveAtCommand(this, item));
        }

        public new Post this[int index]
        {
            get { return base[index]; }
            set
            {
                Post prev = base[index];
                base[index] = value;
                undoList.AddLast(new IndexerCommand(this, new Post[] {prev, value}, index));
            }
        }

        public Post[] ToArray()
        {
            Post[] entries = new Post[base.Count];

            for (int i = 0; i < entries.Length; i++)
            {
                Post entry = base[i];

                if (entry is Expense)
                    entries[i] = new Expense((Expense)entry);
                else if (entry is Income)
                    entries[i] = new Income((Income)entry);
            }

            return entries;
        }

        public bool IsRedoPossible
        { 
            get 
            {
                if (redoList == null)
                    redoList = new LinkedList<Command>();
                return redoList.Count != 0; 
            } 
        }

        public bool IsUndoPossible
        { 
            get 
            {
                if (undoList == null)
                    undoList = new LinkedList<Command>();
                return undoList.Count != 0; 
            } 
        }

        public void Redo()
        {
            command = redoList.Last<Command>();
            redoList.RemoveLast();
            undoList.AddLast(command);
            command.Redo();
        }

        public void Undo()
        {
            command = undoList.Last<Command>();
            undoList.RemoveLast();
            redoList.AddLast(command);
            command.Undo();
        }

        [NonSerialized]
        private Command command;
        [NonSerialized]
        private LinkedList<Command> redoList;
        [NonSerialized]
        private LinkedList<Command> undoList;     
    }

    #region List Commands

    public class AddCommand : Command
    {
        public AddCommand(CommandEntryList parent, Post item)
            : base(parent)
        {
            this.items = new Post[] { item };
        }

        public AddCommand(CommandEntryList parent, Post[] items)
            : base(parent)
        {
            this.items = items;
        }

        public override void Redo()
        {
            foreach(Post e in items)
                ((ObservableCollection<Post>)parent).Add(e); 
        }
        public override void Undo()
        {
            foreach(Post e in items)
                ((ObservableCollection<Post>)parent).Remove(e);
        }

        private Post[] items;
    }

    public class RemoveAtCommand : Command
    {
        public RemoveAtCommand(CommandEntryList parent, Post item)
            : base(parent)
        {
            this.item = item;
        }

        public override void Redo() { ((ObservableCollection<Post>)parent).Remove(item); }
        public override void Undo() { ((ObservableCollection<Post>)parent).Add(item); }

        private Post item;
    }

    public class IndexerCommand : Command
    {
        public IndexerCommand(CommandEntryList parent, Post[] items, int index)
            : base(parent)
        {
            this.items = items;
            this.index = index;
        }

        public override void Redo() { ((ObservableCollection<Post>)parent)[index] = items[1]; }
        public override void Undo() { ((ObservableCollection<Post>)parent)[index] = items[0]; }

        private Post[] items;
        private int index;
    }

    #endregion
}
