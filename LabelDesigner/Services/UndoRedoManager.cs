using System;
using System.Collections.Generic;

namespace LabelDesigner.Services
{
    /// <summary>
    /// 泛用的 Undo/Redo 管理器
    /// </summary>
    public class UndoRedoManager<T>
    {
        private readonly Stack<T> _undoStack = new();
        private readonly Stack<T> _redoStack = new();

        private readonly Func<T, T> _cloner;

        public UndoRedoManager(Func<T, T> cloner)
        {
            _cloner = cloner ?? throw new ArgumentNullException(nameof(cloner));
        }

        /// <summary>
        /// 記錄一個狀態快照
        /// </summary>
        public void PushState(T state)
        {
            _undoStack.Push(_cloner(state));
            _redoStack.Clear();
        }

        /// <summary>
        /// 是否可 Undo
        /// </summary>
        public bool CanUndo => _undoStack.Count > 1; // 保留至少一個初始狀態

        /// <summary>
        /// 是否可 Redo
        /// </summary>
        public bool CanRedo => _redoStack.Count > 0;

        /// <summary>
        /// 執行 Undo，回傳新的狀態
        /// </summary>
        public T? Undo(T current)
        {
            if (!CanUndo) return current;

            _redoStack.Push(_cloner(current));
            var prev = _undoStack.Pop();
            return _cloner(prev);
        }

        /// <summary>
        /// 執行 Redo，回傳新的狀態
        /// </summary>
        public T? Redo(T current)
        {
            if (!CanRedo) return current;

            _undoStack.Push(_cloner(current));
            var next = _redoStack.Pop();
            return _cloner(next);
        }

        /// <summary>
        /// 清除所有紀錄
        /// </summary>
        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();
        }
    }
}
