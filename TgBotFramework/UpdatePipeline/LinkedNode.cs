using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgBotFramework.UpdatePipeline
{
    public class LinkedNode<TContext> where TContext : IUpdateContext
    {
        private UpdateDelegate<TContext> _data;
        private LinkedNode<TContext> _next;
        private LinkedNode<TContext> _previous;

        public LinkedNode() : this(default(UpdateDelegate<TContext>)) { }
        public LinkedNode(UpdateDelegate<TContext> dataItem) : this(dataItem, null, null) { }
        public LinkedNode(UpdateDelegate<TContext> dataItem, LinkedNode<TContext> next, LinkedNode<TContext> previous)
        {
            Data = dataItem;
            Next = next;
            Previous = previous;
        }

        public virtual UpdateButtonDelegate<TContext> CallbackButtonHandler { get; set; }
        public virtual UpdateButtonDelegate<TContext> ReplyKeyboardButtonHandler { get; set; }
        public virtual UpdateDelegate<TContext> Handler { get; set; }
        public virtual UpdateDelegate<TContext> Step { get; set; }


        public virtual UpdateDelegate<TContext> Data
        {
            get { return this._data; }
            set { this._data = value; }
        }

        public virtual LinkedNode<TContext> Next
        {
            get { return this._next; }
            set { this._next = value; }
        }

        public virtual LinkedNode<TContext> Previous
        {
            get { return this._previous; }
            set { this._previous = value; }
        }
    }
}
